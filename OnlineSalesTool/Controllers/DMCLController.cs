using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Service;
using OnlineSalesTool.Filter;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute(nameof(DMCLController))]
    public class DMCLController : Controller
    {
        private readonly ILogger<DMCLController> _logger;
        private readonly IDMCLService _service;
        private readonly IAPIAuth _apiAuth;
        public DMCLController(IDMCLService service, IAPIAuth apiAuth, ILogger<DMCLController> logger)
        {
            _logger = logger;
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
