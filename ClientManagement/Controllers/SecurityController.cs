using ClientManagement.Models;
using ClientManagement.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClientManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityRepository _securityRepository;

        public SecurityController(ISecurityRepository securityRepository)
        {
            _securityRepository = securityRepository;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpModel signUpModel)
        {
            var result = await _securityRepository.SignUpAsync(signUpModel);

            if (result.Succeeded)
            {
                return Ok(result.Succeeded);
            }

            return Unauthorized();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] SignInModel signInModel)
        {
            var result = await _securityRepository.LoginAsync(signInModel);

            if(string.IsNullOrEmpty(result))
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }
    }
}
