using ClientManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace ClientManagement.Repository
{
    public interface ISecurityRepository
    {
        Task<IdentityResult> SignUpAsync(SignUpModel signUpModel);
        Task<string> LoginAsync(SignInModel signInModel);
    }
}