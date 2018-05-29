using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using OnlineSalesTool.Cache;
using OnlineSalesTool.CustomException;
using OnlineSalesTool.Filter;
using OnlineSalesTool.DTO;
using OnlineSalesTool.Service;
using System.Threading.Tasks;
using static OnlineSalesTool.ApiParameter.ListingParams;

namespace OnlineSalesTool.Controllers
{
    [LogExceptionFilterAttribute]
    [Authorize]
    public class UserController : Controller
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IUserService _service;
        private readonly IRoleCache _roleCache;
        public UserController(IUserService service, IRoleCache roleCache)
        {
            _service = service;
            _roleCache = roleCache;
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
                Utility.LogException(ex, _logger);
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
                Utility.LogException(ex, _logger);
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
    }
}
