using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineSalesCore.Cache;
using OnlineSalesCore.DTO;
using OnlineSalesCore.Exceptions;
using OnlineSalesCore.Service;
using OnlineSalesTool.Filter;
using OnlineSalesTool.Helper;
using System.Threading.Tasks;
using static OnlineSalesCore.ApiParameter.ListingParams;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute(nameof(UserController))]
    [Authorize]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _service;
        private readonly IRoleCache _roleCache;
        public UserController(IUserService service, IRoleCache roleCache, ILogger<UserController> logger)
        {
            _service = service;
            _roleCache = roleCache;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get(
            [FromQuery]int count = 10,
            [FromQuery]int page = 1,
            [FromQuery]string type = "",
            [FromQuery]string contain = "",
            [FromQuery]string order = "",
            [FromQuery]bool asc = true)
        {
            var paramBuilder = new ParamBuilder()
                    .SetPage(page)
                    .SetItemPerPage(count)
                    .SetType(type).SetContain(contain)
                    .SetOrderBy(order)
                    .SetAsc(asc);
            return Ok(await _service.Get(paramBuilder.Build()));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AppUserDTO user)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                return Ok(await _service.Create(user));
            }
            catch (BussinessException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] AppUserDTO user)
        {
            if (!ModelState.IsValid) return BadRequest();
            try
            {
                await _service.Update(user);
                return Ok();
            }
            catch (BussinessException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
            catch(DbUpdateException ex)
            {
                Helper.Utility.LogException(ex, _logger);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Suggest([FromQuery]string role, [FromQuery]string q = "")
        {
            if (string.IsNullOrEmpty(q) || string.IsNullOrEmpty(role))
                return NoContent();
            try
            {
                _roleCache.GetRoleId(role, out int roleId, out var appRole);
                return Ok(await _service.SearchSuggest(appRole, q));
            }
            catch (BussinessException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Check([FromQuery] string q)
        {
            if(string.IsNullOrEmpty(q)) return BadRequest();
            return Ok(await _service.CheckUsername(q.ToLower()));
        }
    }
}
