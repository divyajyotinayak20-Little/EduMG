using EducationManagement.Business.Interfaces;
using EducationManagement.DataAccess.Interfaces;
using EducationManagement.Models.Entities;
using EducationManagement.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace EducationManagement.Business.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _repository;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(IAuthRepository repository)
    {
        _repository = repository;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<(bool Success, string Message)>
        RegisterAsync(RegisterViewModel model)
    {
        string email = model.Email.Trim().ToUpperInvariant();

        if (await _repository.EmailExistsAsync(email))
        {
            return (false, "An account with this email already exists.");
        }

        var user = new User
        {
            FullName = model.FullName.Trim(),
            Email = email,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user, model.Password);

        bool created = await _repository.CreateAsync(user);

        if (!created)
        {
            return (false, "An account with this email already exists.");
        }

        return (true, "Registration successful. Please log in.");
    }

    public async Task<User?> LoginAsync(LoginViewModel model)
    {
        string email = model.Email.Trim().ToUpperInvariant();

        var user = await _repository.GetByEmailAsync(email);

        if (user == null || !user.IsActive)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        return user;
    }
}