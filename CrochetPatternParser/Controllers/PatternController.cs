using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CrochetPatternParser.Models;
using CrochetPatternParser.Core.Tokenizer;

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
        var tokenizer = new Tokenizer(pattern);
        var tokens = tokenizer.Tokenize();
        return View(tokens);
    }
}