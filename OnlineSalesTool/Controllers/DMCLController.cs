using OnlineSalesTool.Filter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Threading.Tasks;
using OnlineSalesTool.ViewModels;
using OnlineSalesTool.Service;
using System.Collections.Generic;
using OnlineSalesTool.DTO;
using System.Linq;
using NLog;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute]
    public class DMCLController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDMCLService _service;
        private readonly IAPIAuth _apiAuth;
        public DMCLController(IDMCLService service, IAPIAuth apiAuth)
        {
            _service = service;
            _apiAuth = apiAuth;
        }
        
        [HttpGet]
        public async Task<IActionResult> Status([FromQuery] string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return BadRequest();
            return Ok(await _service.GetStatus(guid));
        }

        [HttpPost]
        public async Task<IActionResult> Push([FromBody] IEnumerable<OrderDTO> orders)
        {
            if(!ModelState.IsValid) return BadRequest();
            //Check auth
            if(orders.Any(o => !_apiAuth.Check(o.Signature, o.Guid)))
                return Unauthorized();
            await _service.Push(orders);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBill([FromBody] OnlineBillDTO onlineBill)
        {
            if(!ModelState.IsValid) return BadRequest();
            //Check auth
            if(!_apiAuth.Check(onlineBill.Signature, onlineBill.Guid))
                return Unauthorized();
            await _service.UpdateBill(onlineBill);
            return Ok();
        }
    }
}
