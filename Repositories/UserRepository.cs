using Microsoft.Data.SqlClient;
using users_service.Models;
using users_service.Services;

namespace users_service.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(string email);
        Task<User> CreateAsync(User user);
        Task<User?> UpdateAsync(Guid id, UpdateUserRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateLastLoginAsync(Guid id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseService _databaseService;

        public UserRepository(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            using var command = new SqlCommand(
                "SELECT Id, Name, Email, Password, CreatedAt, UpdatedAt, LastLogin, IsActive " +
                "FROM Users WHERE Id = @Id AND IsActive = 1", 
                connection);
            
            command.Parameters.AddWithValue("@Id", id);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5),
                    LastLogin = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    IsActive = reader.GetBoolean(7)
                };
            }
            
            return null;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            using var command = new SqlCommand(
                "SELECT Id, Name, Email, Password, CreatedAt, UpdatedAt, LastLogin, IsActive " +
                "FROM Users WHERE Email = @Email AND IsActive = 1", 
                connection);
            
            command.Parameters.AddWithValue("@Email", email);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5),
                    LastLogin = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    IsActive = reader.GetBoolean(7)
                };
            }
            
            return null;
        }

        public async Task<User> CreateAsync(User user)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            using var command = new SqlCommand(
                "INSERT INTO Users (Id, Name, Email, Password, CreatedAt, UpdatedAt, IsActive) " +
                "OUTPUT INSERTED.* " +
                "VALUES (@Id, @Name, @Email, @Password, @CreatedAt, @UpdatedAt, @IsActive)", 
                connection);
            
            command.Parameters.AddWithValue("@Id", user.Id);
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@CreatedAt", user.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", user.UpdatedAt);
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5),
                    LastLogin = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    IsActive = reader.GetBoolean(7)
                };
            }
            
            throw new InvalidOperationException("Failed to create user");
        }

        public async Task<User?> UpdateAsync(Guid id, UpdateUserRequest request)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            
            var updateFields = new List<string>();
            var parameters = new List<SqlParameter>();
            
            if (!string.IsNullOrEmpty(request.Name))
            {
                updateFields.Add("Name = @Name");
                parameters.Add(new SqlParameter("@Name", request.Name));
            }
            
            if (!string.IsNullOrEmpty(request.Email))
            {
                updateFields.Add("Email = @Email");
                parameters.Add(new SqlParameter("@Email", request.Email));
            }
            
            if (updateFields.Count == 0)
                return await GetByIdAsync(id);
            
            updateFields.Add("UpdatedAt = @UpdatedAt");
            parameters.Add(new SqlParameter("@UpdatedAt", DateTime.UtcNow));
            parameters.Add(new SqlParameter("@Id", id));
            
            var sql = $"UPDATE Users SET {string.Join(", ", updateFields)} " +
                     "OUTPUT INSERTED.* WHERE Id = @Id AND IsActive = 1";
            
            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddRange(parameters.ToArray());
            
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    Password = reader.GetString(3),
                    CreatedAt = reader.GetDateTime(4),
                    UpdatedAt = reader.GetDateTime(5),
                    LastLogin = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                    IsActive = reader.GetBoolean(7)
                };
            }
            
            return null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            using var command = new SqlCommand(
                "UPDATE Users SET IsActive = 0, UpdatedAt = @UpdatedAt WHERE Id = @Id", 
                connection);
            
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.UtcNow);
            
            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task UpdateLastLoginAsync(Guid id)
        {
            using var connection = await _databaseService.GetConnectionAsync();
            using var command = new SqlCommand(
                "UPDATE Users SET LastLogin = @LastLogin WHERE Id = @Id", 
                connection);
            
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@LastLogin", DateTime.UtcNow);
            
            await command.ExecuteNonQueryAsync();
        }
    }
}