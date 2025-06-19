using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DTO;
using Service.Interface;

namespace API.Controllers
{
    /// <summary>
    /// CRUD với role Admin (nếu User đó đã tạo bài báo thì k thể xóa)
    /// Role Admin Create a report statistic
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAccountController : Controller
    {
        private readonly IAccountService _accountService;
        public SystemAccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] AccountViewModel model)
        {
            if (model == null)
                return BadRequest("Invalid account data");
            var result = await _accountService.CreateAccountAsync(model);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] AccountViewModel model)
        {
            if (model == null)
                return BadRequest("Invalid account data");
            var result = await _accountService.UpdateAccountAsync(model);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _accountService.DeleteAccountAsync(id);
            if (!result.Success)
                return BadRequest(result.Message);
            return Ok(result);
        }
    }
}
