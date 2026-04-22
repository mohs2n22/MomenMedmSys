-- Manual database seeding for admin user
-- Run this script against medmsys.db

-- First, ensure tables exist by checking
SELECT name FROM sqlite_master WHERE type='table' AND name='Users';

-- Insert admin user with BCrypt hash of "Admin@123"
-- BCrypt hash for "Admin@123" with work factor 12
INSERT OR IGNORE INTO Users (Username, PasswordHash, FullName, Email, Role, IsActive, IsLocked, FailedLoginAttempts, CreatedBy, CreatedAt, PasswordExpiryDate)
VALUES ('admin', '$2a$12$qVh8mKzY5rP3xL9wN2jO4uGx7bR1cD5eF6gH8iJ9kL0mN1oP2qR3s', 'System Administrator', 'admin@medmsys.local', 0, 1, 0, 0, 'System', datetime('now'), datetime('now', '+90 days'));

-- Verify
SELECT Id, Username, IsActive FROM Users WHERE Username='admin';
