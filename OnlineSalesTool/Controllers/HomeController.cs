using Microsoft.AspNetCore.Mvc;
using OnlineSalesTool.Filter;
using OnlineSalesTool.Models;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute(nameof(HomeController))]
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        [HttpGet]
        //SPA entry
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
