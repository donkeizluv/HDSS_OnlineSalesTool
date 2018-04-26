using OnlineSalesTool.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NLog;
using OnlineSalesTool.CustomException;
using OnlineSalesTool.Logic;
using OnlineSalesTool.Repository;
using OnlineSalesTool.Service;
using OnlineSalesTool.ViewModels;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace OnlineSalesTool.Controllers
{
    [CustomExceptionFilterAttribute]
    //[Authorize]
    public class ShiftController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private IScheduleRepository _repo;

        public ShiftController(IScheduleRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetVM()
        {
            using (_repo)
            {
                return Ok(await _repo.CreateAssignerVM());
            }
            
        }
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Save([FromBody]ScheduleContainer schedule)
        {
            if (!ModelState.IsValid || schedule == null) return BadRequest("Invalid post data");
            using (_repo)
            {
                try
                {
                    await _repo.SaveSchedule(schedule);
                    return Ok();
                }
                catch (BussinessException ex) //Fail bussiness check
                {
                    Utility.LogException(ex, _logger);
                    return BadRequest(ex.Message);
                }
                catch (DbUpdateException ex)
                {
                    Utility.LogException(ex, _logger);
                    return BadRequest("System error.");
                }
                catch (SqlException ex)
                {
                    Utility.LogException(ex, _logger);
                    return BadRequest("System error.");
                }
            }
           
        }
        protected override void Dispose(bool disposing)
        {
            if (_repo != null) _repo.Dispose();
            base.Dispose(disposing);
        }
    }
}
