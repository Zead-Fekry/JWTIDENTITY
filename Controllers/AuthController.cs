using JWT.Models;
using JWT.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsyncs([FromBody]RegisterModel model) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState); 
            }
            var result = await _authService.RegisterAsync(model);

            if (result == null)
            {
                return BadRequest(result.Message);
            }
            if (!result.IsAuthenticated) 
            {
                return BadRequest(result.Message);
            }
            return Ok(new { token = result.Token});
        }
        [HttpPost("token")]
        public async Task<IActionResult> GetTokenAsync([FromBody] GetTokenModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.GetTokenAsync(model);

            if (result == null)
            {
                return BadRequest(result.Message);
            }
            if (!result.IsAuthenticated)
            {
                return BadRequest(result.Message);
            }
            return Ok(new { token = result });
        }
    }
}
