using System.Data;
using EducationManagement.DataAccess.Connections;
using EducationManagement.DataAccess.Interfaces;
using EducationManagement.Models.Entities;
using Microsoft.Data.SqlClient;

namespace EducationManagement.DataAccess.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly SqlConnectionFactory _factory;

    public AuthRepository(SqlConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        using var connection = _factory.CreateConnection();

        using var command = new SqlCommand("dbo.User_EmailExists", connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add("@Email", SqlDbType.NVarChar, 256).Value = email;

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return result != null && result != DBNull.Value && Convert.ToBoolean(result);


    }

    public async Task<bool> CreateAsync(User user)
    {
        using var connection = _factory.CreateConnection();

        using var command = new SqlCommand(
            "dbo.User_Register",
            connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(
            "@FullName",
            SqlDbType.NVarChar,
            100).Value = user.FullName;

        command.Parameters.Add(
            "@Email",
            SqlDbType.NVarChar,
            256).Value = user.Email;

        command.Parameters.Add(
            "@PasswordHash",
            SqlDbType.NVarChar,
            500).Value = user.PasswordHash;

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();

        return result != null &&
               result != DBNull.Value &&
               Convert.ToInt32(result) == 1;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _factory.CreateConnection();

        using var command = new SqlCommand(
            "dbo.User_GetByEmail",
            connection);

        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(
            "@Email",
            SqlDbType.NVarChar,
            256).Value = email;

        await connection.OpenAsync();

        using var reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new User
        {
            UserId = reader.GetInt32(
                reader.GetOrdinal("UserId")),

            FullName = reader.GetString(
                reader.GetOrdinal("FullName")),

            Email = reader.GetString(
                reader.GetOrdinal("Email")),

            PasswordHash = reader.GetString(
                reader.GetOrdinal("PasswordHash")),

            IsActive = reader.GetBoolean(
                reader.GetOrdinal("IsActive"))
        };
    }
}