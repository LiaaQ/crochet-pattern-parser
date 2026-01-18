using Microsoft.AspNetCore.Mvc;
using CrochetPatternParser.Core.Tokenizer;
using CrochetPatternParser.Core.Parser;
using CrochetPatternParser.Models;
using CrochetPatternParser.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

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
        public IActionResult Validate(PatternViewModel model)
        {
            var viewModel = ValidatePattern(model.PatternText);
            return View("Index", viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Save(PatternViewModel model)
        {
            var viewModel = await SavePattern(model.PatternText);
            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MyPatterns()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account");

            var userId = _userManager.GetUserId(User);
            var patterns = _db.Patterns
                            .Where(p => p.UserId == userId)
                            .OrderByDescending(p => p.Id)
                            .ToList();

            return View(patterns);
        }


        private PatternViewModel ValidatePattern(string patternText)
        {
            var viewModel = new PatternViewModel();

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

        private async Task<PatternViewModel> SavePattern(string patternText)
        {
            var viewModel = new PatternViewModel
            {
                PatternText = patternText
            };

            var userId = _userManager.GetUserId(User)
                ?? throw new InvalidOperationException("Authenticated user has no user ID.");

            var entity = new PatternEntity
            {
                RawText = patternText,
                UserId = userId
            };

            _db.Patterns.Add(entity);
            await _db.SaveChangesAsync();

            return viewModel;
        }
    }
}
