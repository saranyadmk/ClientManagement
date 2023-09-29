using ClientManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ClientManagement.Repository
{
    public class SecurityRepository : ISecurityRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public SecurityRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<IdentityResult> SignUpAsync(SignUpModel signUpModel)
        {
            var user = new ApplicationUser()
            {
                FirstName = signUpModel.FirstName,
                LastName = signUpModel.LastName,
                Email = signUpModel.Email,
                UserName = signUpModel.Email,
                Password = signUpModel.Password,
                ConfirmPassword = signUpModel.ConfirmPassword,
            };
            return await _userManager.CreateAsync(user);
        }

        public async Task<string> LoginAsync(SignInModel signInModel)
        {
            var result = await _signInManager.PasswordSignInAsync(signInModel.Email, signInModel.Password, false, false);
            
            if (!result.Succeeded)
            {
                return null;
            }
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, signInModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigninKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(10),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigninKey, SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
