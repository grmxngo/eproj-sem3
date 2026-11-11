using Microsoft.AspNetCore.Mvc;

namespace HeritageTrail.Web.Controllers;

public sealed class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => View();
}
