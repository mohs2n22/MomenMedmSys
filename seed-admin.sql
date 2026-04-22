-- Create Users table and seed admin user
CREATE TABLE IF NOT EXISTS Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FullName TEXT,
    Email TEXT,
    Role INTEGER NOT NULL DEFAULT 0,
    IsActive INTEGER NOT NULL DEFAULT 1,
    IsLocked INTEGER NOT NULL DEFAULT 0,
    FailedLoginAttempts INTEGER NOT NULL DEFAULT 0,
    LastLoginDate TEXT,
    PasswordExpiryDate TEXT,
    CreatedBy TEXT,
    CreatedAt TEXT NOT NULL,
    UpdatedAt TEXT
);

-- Insert admin user with BCrypt hash of "Admin@123"
INSERT OR IGNORE INTO Users (Id, Username, PasswordHash, FullName, Email, Role, IsActive, IsLocked, FailedLoginAttempts, CreatedBy, CreatedAt, PasswordExpiryDate)
VALUES (1, 'admin', '$2a$12$LJ3m4ys3L8aMx8yZxKxGOuP7VJx8z5xKxKxKxKxKxKxKxKxKxKxK.', 'System Administrator', 'admin@medmsys.local', 0, 1, 0, 0, 'System', datetime('now'), datetime('now', '+90 days'));

-- Verify
SELECT Id, Username, IsActive FROM Users;
