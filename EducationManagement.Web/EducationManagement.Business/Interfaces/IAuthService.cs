using EducationManagement.Models.Entities;
using EducationManagement.Models.ViewModels;

namespace EducationManagement.Business.Interfaces;

public interface IAuthService
{
    Task<(bool Success, string Message)> RegisterAsync(
        RegisterViewModel model);

    Task<User?> LoginAsync(LoginViewModel model);
}