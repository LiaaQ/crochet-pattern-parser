using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CrochetPatternParser.Models;
using CrochetPatternParser.Core.Tokenizer;
using CrochetPatternParser.Core.Parser;

namespace CrochetPatternParser.Controllers;

public class PatternController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Index(string pattern)
    {
        try
        {
            var tokenizer = new Tokenizer(pattern);
            var tokens = tokenizer.Tokenize();

            var parser = new Parser(tokens);
            var ast = parser.Parse();

            ViewBag.Tokens = tokens;
            ViewBag.AST = ast;
            ViewBag.Pattern = pattern;
            ViewBag.Message = "Pattern parsed successfully!";

            return View();
        }
        catch (Exception ex)
        {
            ViewBag.Message = "Error: " + ex.Message;
            ViewBag.Pattern = pattern;
            return View();
        }
    }

}