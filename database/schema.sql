-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'WhaleUsers')
BEGIN
    CREATE DATABASE WhaleUsers;
END
GO

USE WhaleUsers;
GO

-- Create Users table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(255) NOT NULL UNIQUE,
        Password NVARCHAR(255) NOT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        LastLogin DATETIME2 NULL,
        IsActive BIT NOT NULL DEFAULT 1
    );

    -- Create indexes
    CREATE INDEX IX_Users_Email ON Users(Email);
    CREATE INDEX IX_Users_IsActive ON Users(IsActive);
    CREATE INDEX IX_Users_Email_IsActive ON Users(Email, IsActive);
END
GO

-- Insert sample data (optional)
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'admin@whale.com')
BEGIN
    INSERT INTO Users (Name, Email, Password, IsActive)
    VALUES ('Admin User', 'admin@whale.com', '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj4J/8K8K8K8', 1);
END
GO
