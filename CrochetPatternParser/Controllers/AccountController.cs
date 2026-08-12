using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CrochetPatternParser.Models;
using CrochetPatternParser.Data;

namespace CrochetPatternParser.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly SignInManager<ApplicationUserEntity> _signInManager;

        public AccountController(ApplicationDbContext db, UserManager<ApplicationUserEntity> userManager, SignInManager<ApplicationUserEntity> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUserEntity { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await SeedDefaultExamplePatternAsync(user.Id);
                await _signInManager.SignInAsync(user, isPersistent: false);
                
                // If user was redirected from a save attempt, redirect to pattern page
                if (TempData["FromSaveAttempt"]?.ToString() == "True")
                {
                    TempData["PatternTitle"] = TempData["PatternTitle"];
                    TempData["PatternRoundTexts"] = TempData["PatternRoundTexts"];
                    TempData["PatternImagePath"] = TempData["PatternImagePath"];
                    TempData["FromSaveAttempt"] = TempData["FromSaveAttempt"];
                    return RedirectToAction("Index", "Pattern");
                }
                
                return RedirectToAction("Index", "Pattern");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                // If user was redirected from a save attempt, redirect to pattern page
                if (TempData["FromSaveAttempt"]?.ToString() == "True")
                {
                    TempData["PatternTitle"] = TempData["PatternTitle"];
                    TempData["PatternRoundTexts"] = TempData["PatternRoundTexts"];
                    TempData["PatternImagePath"] = TempData["PatternImagePath"];
                    TempData["FromSaveAttempt"] = TempData["FromSaveAttempt"];
                    return RedirectToAction("Index", "Pattern");
                }
                
                return RedirectToAction("Index", "Pattern");
            }

            ModelState.AddModelError("", "Invalid login");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Pattern");
        }

        private async Task SeedDefaultExamplePatternAsync(string userId)
        {
            PatternEntity DefaultExamplePattern = CreateDefaultExamplePattern();
            var examplePattern = new PatternEntity
            {
                Title = DefaultExamplePattern.Title,
                UserId = userId,
                ImagePath = DefaultExamplePattern.ImagePath,
                Sections = DefaultExamplePattern.Sections
                    .Select((section, index) => new SectionEntity
                    {
                        SectionNumber = index + 1,
                        SectionName = section.SectionName,
                        Rounds = section.Rounds
                            .Select((round, roundIndex) => new RoundEntity
                            {
                                RoundNumber = roundIndex + 1,
                                Text = round.Text
                            })
                            .ToList()
                    })
                    .ToList()
            };

            _db.Patterns.Add(examplePattern);
            await _db.SaveChangesAsync();
        }

        private static PatternEntity CreateDefaultExamplePattern()
        {
            return new PatternEntity
            {
                Title = "Example Pattern",
                Sections = new List<SectionEntity>
                {
                    new SectionEntity
                    {
                        SectionNumber = 1,
                        SectionName = "Head",
                        Rounds = new List<RoundEntity>
                        {
                            new RoundEntity {
                                RoundNumber = 1,
                                Text = "6mr"
                            }, new RoundEntity {
                                RoundNumber = 2,
                                Text = "6inc"
                            }, new RoundEntity {
                                RoundNumber = 3,
                                Text = "(sc, inc) 6"
                            }, new RoundEntity {
                                RoundNumber = 4,
                                Text = "(2sc inc) 6"
                            }, new RoundEntity {
                                RoundNumber = 5,
                                Text = "24sc"
                            }, new RoundEntity {
                                RoundNumber = 5,
                                Text = "@red 8sc [4dc] @blue 3dec 9sc"
                            }, new RoundEntity {
                                RoundNumber = 6,
                                Text = "(2sc, dec) 6"
                            }, new RoundEntity {
                                RoundNumber = 7,
                                Text = "(sc, dec) 6"
                            }, new RoundEntity {
                                RoundNumber = 8,
                                Text = "6dec FO"
                            }
                        }
                    },
                    new SectionEntity
                    {
                        SectionNumber = 2,
                        SectionName = "Error Section",
                        Rounds = new List<RoundEntity>
                        {
                            new RoundEntity {
                                RoundNumber = 1,
                                Text = "6mr"
                            }, new RoundEntity {
                                RoundNumber = 2,
                                Text = "7inc"
                            }, new RoundEntity {
                                RoundNumber = 3,
                                Text = "(sc, inc) 8"
                            }, new RoundEntity {
                                RoundNumber = 4,
                                Text = "9sc @blue 9sc"
                            }, new RoundEntity {
                                RoundNumber = 5,
                                Text = "@red 9sc @blue 9sc"
                            }, new RoundEntity {
                                RoundNumber = 6,
                                Text = "18sc"
                            }, new RoundEntity{
                                RoundNumber = 7,
                                Text = "(sc, dec) 6 FO"
                            }
                        }
                    }
                }
            };
        }
    }
}
