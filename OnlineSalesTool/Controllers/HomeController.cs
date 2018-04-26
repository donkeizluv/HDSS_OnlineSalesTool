using OnlineSalesTool.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using OnlineSalesTool.ViewModels;

namespace OnlineSalesTool.Controllers
{
    //[Authorize]
    [CustomExceptionFilterAttribute]
    public class HomeController : Controller
    {
        private IConfiguration _config;
        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        //[AllowAnonymous]
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
