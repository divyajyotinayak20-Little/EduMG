using EducationManagement.Models.Entities;

namespace EducationManagement.DataAccess.Interfaces;

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string email);

    Task<bool> CreateAsync(User user);

    Task<User?> GetByEmailAsync(string email);
}