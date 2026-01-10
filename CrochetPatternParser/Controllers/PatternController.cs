using Microsoft.AspNetCore.Mvc;
using CrochetPatternParser.Core.Tokenizer;
using CrochetPatternParser.Core.Parser;
using CrochetPatternParser.Core.Ast;
using CrochetPatternParser.Models;

namespace CrochetPatternParser.Controllers
{
    public class PatternController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Empty model for first load
            return View(new PatternViewModel());
        }

        [HttpPost]
        public IActionResult Index(PatternViewModel model)
        {
            var viewModel = new PatternViewModel();

            try
            {
                // Tokenize
                var tokenizer = new Tokenizer(model.PatternText);
                var tokens = tokenizer.Tokenize();

                // Parse
                var parser = new Parser(tokens);
                var ast = parser.Parse();

                // Validate semantically
                var validator = new PatternValidator();
                var validationResult = validator.Validate(ast);

                // Map validator results to ViewModel
                foreach (var r in validationResult.Rounds)
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
                // Syntax or unexpected errors
                viewModel.Rounds.Add(new RoundViewModel
                {
                    RoundIndex = 0,
                    Error = ex.Message
                });
            }

            return View(viewModel);
        }
    }
}
