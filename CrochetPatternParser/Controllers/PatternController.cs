using Microsoft.AspNetCore.Mvc;
using CrochetPatternParser.Core.Tokenizer;
using CrochetPatternParser.Core.Parser;
using CrochetPatternParser.Models;
using CrochetPatternParser.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CrochetPatternParser.Controllers
{
    public class PatternController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly IWebHostEnvironment _environment;

        public PatternController(ApplicationDbContext db, UserManager<ApplicationUserEntity> userManager, IWebHostEnvironment environment)
        {
            _db = db;
            _userManager = userManager;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = GetPatternFromTempData() ?? new PatternViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(PatternViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Validate(PatternViewModel model, int? patternId)
        {
            var viewModel = ValidatePattern(model.Sections ?? new List<SectionViewModel>());
            viewModel.Title = model.Title;
            viewModel.RoundTexts = model.RoundTexts ?? new List<string>();

            viewModel.ImagePath = await GetImagePathAsync(model);

            if (patternId.HasValue)
            {
                ViewBag.PatternId = patternId;
            }
            return View("Index", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateSection(SectionViewModel model, int? patternId)
        {
            var viewModel = ValidatePattern(model.RoundTexts ?? new List<string>());
            viewModel.Title = model.Title;
            viewModel.RoundTexts = model.RoundTexts ?? new List<string>();

            viewModel.ImagePath = await GetImagePathAsync(model);

            if (patternId.HasValue)
            {
                ViewBag.PatternId = patternId;
            }
            return View("Index", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(PatternViewModel model, int? patternId)
        {
            var imagePath = await GetImagePathAsync(model);

            if (!User.Identity?.IsAuthenticated ?? true)
            {
                StorePatternInTempData(model, imagePath);
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Pattern") });
            }

            var roundTexts = GetFilteredRoundTexts(model.RoundTexts);

            if (roundTexts.Count == 0)
            {
                ModelState.AddModelError("", "Cannot save pattern with no rounds. Please add at least one round.");
                model.RoundTexts = model.RoundTexts ?? new List<string>();
                return View("Index", model);
            }

            var viewModel = await SavePattern(model.Title, roundTexts, imagePath, patternId);
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
                .Include(p => p.Sections)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (pattern == null)
                return NotFound();

            // Load sections from database
            var sectionTexts = pattern.Sections
                .OrderBy(s => s.SectionNumber)
                .Select(s => new SectionViewModel
                {
                    RoundTexts = s.Rounds
                    .OrderBy(r => r.RoundNumber)
                    .Select(r => r.Text)
                    .ToList()
                })
                .ToList();

            var viewModel = new PatternViewModel
            {
                Title = pattern.Title,
                Sections = sectionTexts,
                ImagePath = pattern.ImagePath
            };

            ViewBag.PatternId = pattern.Id;
            return View("Index", viewModel);
        }

        private PatternViewModel ValidatePattern(List<SectionViewModel> sections)
        {
            var viewModel = new PatternViewModel();
            for (int i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                var roundTexts = section.RoundTexts ?? new List<string>();
                var sectionViewModel = new SectionViewModel
                {
                    SectionIndex = i,
                    RoundTexts = roundTexts
                };

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
                        section.Rounds.Add(new RoundViewModel
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
                    section.Rounds.Add(new RoundViewModel
                    {
                        RoundIndex = 0,
                        Error = ex.Message
                    });
                }
                }

            return viewModel;
        }

        private async Task<PatternViewModel> SavePattern(string title, List<SectionViewModel> sections, string? imagePath, int? patternId = null)
        {
            var viewModel = new PatternViewModel
            {
                Title = title,
                Sections = sections,
                ImagePath = imagePath
            };

            var userId = _userManager.GetUserId(User)
                ?? throw new InvalidOperationException("Authenticated user has no user ID.");

            if (patternId.HasValue)
            {
                // Update existing pattern
                var entity = await _db.Patterns
                    .Include(p => p.Sections)
                    .FirstOrDefaultAsync(p => p.Id == patternId && p.UserId == userId);

                if (entity != null)
                {
                    entity.Title = title;

                    // Update image path if new image was uploaded
                    if (imagePath != null)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(entity.ImagePath))
                        {
                            DeleteImageFile(entity.ImagePath);
                        }
                        entity.ImagePath = imagePath;
                    }

                    // Clear existing sections and add new ones
                    entity.Sections.Clear();
                    for (int i = 0; i < sections.Count; i++)
                    {
                        var sectionViewModel = sections[i];
                        var sectionEntity = new SectionEntity
                        {
                            SectionNumber = i + 1,
                            PatternId = entity.Id
                        };

                        for (int j = 0; j < sectionViewModel.RoundTexts.Count; j++)
                        {
                            sectionEntity.Rounds.Add(new RoundEntity
                            {
                                RoundNumber = j + 1,
                                Text = sectionViewModel.RoundTexts[j]
                            });
                        }

                        entity.Sections.Add(sectionEntity);
                    }

                    _db.Patterns.Update(entity);
                    viewModel.ImagePath = entity.ImagePath;
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
                    Rounds = rounds,
                    ImagePath = imagePath
                };

                _db.Patterns.Add(entity);
            }

            await _db.SaveChangesAsync();

            return viewModel;
        }

        private async Task<string> SaveImageFile(IFormFile imageFile)
        {
            // Create uploads directory if it doesn't exist
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "patterns");
            Directory.CreateDirectory(uploadsFolder);

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            // Return relative path for storing in database
            return $"/uploads/patterns/{uniqueFileName}";
        }

        private void DeleteImageFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;

            var fullPath = Path.Combine(_environment.WebRootPath, imagePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private PatternViewModel? GetPatternFromTempData()
        {
            if (TempData["PatternTitle"] == null)
            {
                return null;
            }

            var viewModel = new PatternViewModel
            {
                Title = TempData["PatternTitle"]?.ToString() ?? "Untitled Pattern",
                ImagePath = TempData["PatternImagePath"]?.ToString()
            };

            var roundTextsJson = TempData["PatternRoundTexts"]?.ToString();
            if (!string.IsNullOrEmpty(roundTextsJson))
            {
                viewModel.RoundTexts = JsonSerializer.Deserialize<List<string>>(roundTextsJson) ?? new List<string>();
            }

            TempData.Keep("PatternTitle");
            TempData.Keep("PatternRoundTexts");
            TempData.Keep("PatternImagePath");

            return viewModel;
        }

        private void StorePatternInTempData(PatternViewModel model, string? imagePath)
        {
            TempData["PatternTitle"] = model.Title;
            TempData["PatternRoundTexts"] = JsonSerializer.Serialize(model.RoundTexts ?? new List<string>());
            TempData["PatternImagePath"] = imagePath;
            TempData["FromSaveAttempt"] = true;
        }

        private static List<string> GetFilteredRoundTexts(List<string>? roundTexts)
        {
            return (roundTexts ?? new List<string>())
                .Select(r => r?.Trim() ?? string.Empty)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .ToList();
        }

        private async Task<string?> GetImagePathAsync(PatternViewModel model)
        {
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                return await SaveImageFile(model.ImageFile);
            }

            return model.ImagePath;
        }
    }
}
