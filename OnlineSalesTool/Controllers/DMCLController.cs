using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Helper;
using OnlineSalesCore.Services;
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
        public IActionResult Ping([FromQuery] string guid, [FromQuery] string sig)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(sig))
            {
                return BadRequest();
            }
            if(_apiAuth.Check(sig, guid))
                return Ok();
            return Unauthorized();
        }
        [HttpGet]
        public async Task<IActionResult> Status([FromQuery] string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return BadRequest();
            try
            {
                return Ok(await _service.GetStatus(guid));    
            }
            catch (BussinessException ex)
            {
                _logger.LogDebug(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Push([FromBody] IEnumerable<OrderDTO> orders)
        {
            if(!ModelState.IsValid) return BadRequest();
            //Check auth
            if(orders.Any(o => !_apiAuth.Check(o.Signature, o.Guid)))
                return Unauthorized();
            try
            {
                await _service.Push(orders);
                return Ok();
            }
            catch (BussinessException ex)
            {
                _logger.LogDebug(ex.Message);
                return BadRequest(ex.Message);
            }

        }
        [HttpPost]
        public async Task<IActionResult> UpdateBill([FromBody] OnlineBillDTO onlineBill)
        {
            if(!ModelState.IsValid) return BadRequest();
            //Check auth
            if(!_apiAuth.Check(onlineBill.Signature, onlineBill.Guid))
                return Unauthorized();
            try
            {
                await _service.UpdateBill(onlineBill);
                return Ok();
            }
            catch (BussinessException ex)
            {
                _logger.LogDebug(ex.Message);
                return BadRequest(ex.Message);
            }    

        }
    }
}
