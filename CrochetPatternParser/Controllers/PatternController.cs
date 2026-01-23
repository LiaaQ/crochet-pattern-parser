using Microsoft.AspNetCore.Mvc;
using CrochetPatternParser.Core.Tokenizer;
using CrochetPatternParser.Core.Parser;
using CrochetPatternParser.Models;
using CrochetPatternParser.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CrochetPatternParser.Controllers
{
    public class PatternController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserEntity> _userManager;

        public PatternController(ApplicationDbContext db, UserManager<ApplicationUserEntity> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Empty model for first load
            return View(new PatternViewModel());
        }

        [HttpPost]
        public IActionResult Index(PatternViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Validate(PatternViewModel model, int? patternId)
        {
            var viewModel = ValidatePattern(model.RoundTexts ?? new List<string>());
            viewModel.Title = model.Title;
            viewModel.RoundTexts = model.RoundTexts ?? new List<string>();
            
            if (patternId.HasValue)
            {
                ViewBag.PatternId = patternId;
            }
            return View("Index", viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(PatternViewModel model, int? patternId)
        {
            // Filter out empty rounds
            var roundTexts = (model.RoundTexts ?? new List<string>())
                .Select(r => r?.Trim() ?? string.Empty)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .ToList();

            if (roundTexts.Count == 0)
            {
                ModelState.AddModelError("", "Cannot save pattern with no rounds. Please add at least one round.");
                model.RoundTexts = model.RoundTexts ?? new List<string>();
                return View("Index", model);
            }

            var viewModel = await SavePattern(model.Title, roundTexts, patternId);
            viewModel.RoundTexts = roundTexts;
            
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MyPatterns()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            var userId = _userManager.GetUserId(User);
            var patterns = await _db.Patterns
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.Id)
                .ToListAsync();


            return View(patterns);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);

            var pattern = await _db.Patterns
                .Include(p => p.Rounds)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (pattern == null)
                return NotFound();

            // Load rounds from database
            var roundTexts = pattern.Rounds
                .OrderBy(r => r.RoundNumber)
                .Select(r => r.Text)
                .ToList();
            
            var viewModel = new PatternViewModel
            {
                Title = pattern.Title,
                RoundTexts = roundTexts
            };

            ViewBag.PatternId = pattern.Id;
            return View("Index", viewModel);
        }

        private PatternViewModel ValidatePattern(List<string> roundTexts)
        {
            var viewModel = new PatternViewModel();
            
            // Concatenate rounds for processing
            var patternText = string.Join(";", roundTexts);

            try
            {
                // Tokenize
                var tokenizer = new Tokenizer(patternText);
                var tokens = tokenizer.Tokenize();

                // Parse
                var parser = new Parser(tokens);
                var ast = parser.Parse();

                // Validate pattern
                var validator = new PatternValidator();
                var result = validator.Validate(ast);

                // Map results to ViewModel
                foreach (var r in result.Rounds)
                {
                    viewModel.Rounds.Add(new RoundViewModel
                    {
                        RoundIndex = r.RoundIndex,
                        StitchCount = r.StitchCount,
                        ExpectedStitchConsumed = r.ExpectedStitchConsumed,
                        Error = r.Error
                    });
                }
            }
            catch (Exception ex)
            {
                viewModel.Rounds.Add(new RoundViewModel
                {
                    RoundIndex = 0,
                    Error = ex.Message
                });
            }

            return viewModel;
        }

        private async Task<PatternViewModel> SavePattern(string title, List<string> roundTexts, int? patternId = null)
        {
            var viewModel = new PatternViewModel
            {
                Title = title,
                RoundTexts = roundTexts
            };

            var userId = _userManager.GetUserId(User)
                ?? throw new InvalidOperationException("Authenticated user has no user ID.");

            if (patternId.HasValue)
            {
                // Update existing pattern
                var entity = await _db.Patterns
                    .Include(p => p.Rounds)
                    .FirstOrDefaultAsync(p => p.Id == patternId && p.UserId == userId);

                if (entity != null)
                {
                    entity.Title = title;
                    
                    // Clear existing rounds and add new ones
                    entity.Rounds.Clear();
                    for (int i = 0; i < roundTexts.Count; i++)
                    {
                        entity.Rounds.Add(new RoundEntity
                        {
                            RoundNumber = i + 1,
                            Text = roundTexts[i]
                        });
                    }
                    
                    _db.Patterns.Update(entity);
                }
            }
            else
            {
                // Create new pattern
                var rounds = new List<RoundEntity>();
                for (int i = 0; i < roundTexts.Count; i++)
                {
                    rounds.Add(new RoundEntity
                    {
                        RoundNumber = i + 1,
                        Text = roundTexts[i]
                    });
                }
                
                var entity = new PatternEntity
                {
                    Title = title,
                    UserId = userId,
                    Rounds = rounds
                };

                _db.Patterns.Add(entity);
            }

            await _db.SaveChangesAsync();

            return viewModel;
        }
    }
}
