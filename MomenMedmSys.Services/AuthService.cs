using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using MomenMedmSys.Core.Entities;
using MomenMedmSys.Data;

namespace MomenMedmSys.Services
{
    public class AuthService : IAuthService
    {
        private const int BcryptWorkFactor = 12;
        private const int MaxFailedAttempts = 5;

        private readonly MedMsysDbContext _dbContext;

        public AuthService(MedMsysDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);

            if (user == null)
                return null;

            // Check if account is locked
            if (user.IsLocked)
                return null;

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                // Increment failed attempts
                user.FailedLoginAttempts++;

                if (user.FailedLoginAttempts >= MaxFailedAttempts)
                {
                    user.IsLocked = true;
                }

                await _dbContext.SaveChangesAsync();
                return null;
            }

            // Successful login - reset failed attempts and update last login
            user.FailedLoginAttempts = 0;
            user.LastLoginDate = DateTime.Now;
            await _dbContext.SaveChangesAsync();

            // Create session record
            var session = new UserSession
            {
                UserId = user.Id,
                LoginTime = DateTime.Now,
                IpAddress = "local",
                IsActive = true
            };
            _dbContext.UserSessions.Add(session);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task LogoutAsync(int userId)
        {
            var activeSessions = await _dbContext.UserSessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.IsActive = false;
                session.LogoutTime = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                return false;

            user.PasswordHash = HashPassword(newPassword);
            user.PasswordExpiryDate = DateTime.Now.AddDays(90);
            user.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.PasswordHash = HashPassword(newPassword);
            user.PasswordExpiryDate = DateTime.Now.AddDays(90);
            user.FailedLoginAttempts = 0;
            user.IsLocked = false;
            user.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task LockAccountAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return;

            user.IsLocked = true;
            user.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        public async Task UnlockAccountAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return;

            user.IsLocked = false;
            user.FailedLoginAttempts = 0;
            user.UpdatedAt = DateTime.Now;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            user.PasswordHash = HashPassword(password);
            user.PasswordExpiryDate = DateTime.Now.AddDays(90);
            user.CreatedAt = DateTime.Now;
            user.IsActive = true;

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var existing = await _dbContext.Users.FindAsync(user.Id);
            if (existing == null)
                throw new InvalidOperationException($"User with Id {user.Id} not found.");

            existing.FullName = user.FullName;
            existing.Email = user.Email;
            existing.Role = user.Role;
            existing.IsActive = user.IsActive;
            existing.UpdatedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user == null) return;

            // Soft delete
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;

            // End all active sessions
            var activeSessions = await _dbContext.UserSessions
                .Where(s => s.UserId == userId && s.IsActive)
                .ToListAsync();
            foreach (var session in activeSessions)
            {
                session.IsActive = false;
                session.LogoutTime = DateTime.Now;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetCurrentUserAsync(int? userId = null)
        {
            if (!userId.HasValue)
                return null;

            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId.Value && u.IsActive);
        }

        public bool IsPasswordValid(string password, string hash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
                return false;

            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: BcryptWorkFactor);
        }
    }
}
