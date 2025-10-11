CREATE TABLE [Users] (
  [UserId] bigint PRIMARY KEY IDENTITY(1, 1),
  [Username] varchar(50) UNIQUE NOT NULL,
  [Gender] char(1),
  [PasswordHash] varchar(256) NOT NULL,
  [Email] varchar(100) UNIQUE NOT NULL,
  [IsActive] BIT NOT NULL DEFAULT (1),
  [CreatedAt] datetime NOT NULL DEFAULT (GETDATE()),
  [UpdatedAt] datetime
)
GO