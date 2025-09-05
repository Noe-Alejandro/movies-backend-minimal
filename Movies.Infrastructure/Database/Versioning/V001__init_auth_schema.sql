-----------------------------------------------------
-- V001__init_auth_schema.sql
-- Inicialización del esquema de autenticación
-----------------------------------------------------

-- =========================================
-- TENANTS
-- =========================================
CREATE TABLE dbo.Tenants (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL,
    Domain NVARCHAR(200) NULL UNIQUE, -- opcional (para subdominios o login)
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2(0) NOT NULL DEFAULT sysutcdatetime()
);

-- AUTH SCHEMA
IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'auth')
BEGIN
    EXEC('CREATE SCHEMA auth');
END
GO

-- =========================================
-- USUARIOS DE AUTENTICACIÓN
-- =========================================

CREATE TABLE auth.UserAuth (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(256) NULL UNIQUE,
    DisplayName NVARCHAR(200) NULL,
    IsActive BIT NOT NULL DEFAULT (1),
    EmailVerified BIT NOT NULL DEFAULT (0),
    CreatedAt DATETIME2(0) NOT NULL DEFAULT sysutcdatetime(),
    LastLoginAt DATETIME2(0) NULL,
    PasswordHash VARBINARY(512) NULL -- usado solo en login interno
);

-- Relación N:M entre usuarios y tenants
CREATE TABLE auth.UserTenants (
    UserId INT NOT NULL,
    TenantId INT NOT NULL,
    PRIMARY KEY (UserId, TenantId),
    FOREIGN KEY (UserId) REFERENCES auth.UserAuth(Id) ON DELETE CASCADE,
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE CASCADE
);

-- =========================================
-- ROLES Y PERMISOS
-- =========================================
CREATE TABLE auth.Roles (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(300) NULL,
    IsActive BIT NOT NULL DEFAULT (1),
    CONSTRAINT UX_Roles_Tenant_Name UNIQUE (TenantId, Name),
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE CASCADE
);

CREATE TABLE auth.Permissions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Key] NVARCHAR(150) NOT NULL UNIQUE,
    [Name] NVARCHAR(150) NOT NULL,
    [Description] NVARCHAR(300) NULL,
    [GroupName] NVARCHAR(100) NULL
);

CREATE TABLE auth.RolePermissions (
    RoleId INT NOT NULL,
    PermissionId INT NOT NULL,
    PRIMARY KEY (RoleId, PermissionId),
    FOREIGN KEY (RoleId) REFERENCES auth.Roles(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES auth.Permissions(Id) ON DELETE CASCADE
);

CREATE TABLE auth.UserRoles (
    UserId INT NOT NULL,
    RoleId INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES auth.UserAuth(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES auth.Roles(Id) ON DELETE CASCADE
);

CREATE TABLE auth.UserPermissions (
    UserId INT NOT NULL,
    PermissionId INT NOT NULL,
    IsGranted BIT NOT NULL,
    PRIMARY KEY (UserId, PermissionId),
    FOREIGN KEY (UserId) REFERENCES auth.UserAuth(Id) ON DELETE CASCADE,
    FOREIGN KEY (PermissionId) REFERENCES auth.Permissions(Id) ON DELETE CASCADE
);

-- =========================================
-- LOGIN AUDIT
-- =========================================
CREATE TABLE auth.AuditLogins (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NULL,
    ProviderId INT NULL,
    OccurredAt DATETIME2(0) NOT NULL DEFAULT sysutcdatetime(),
    Ip NVARCHAR(64) NULL,
    UserAgent NVARCHAR(300) NULL,
    Result NVARCHAR(50) NOT NULL
);

-- =========================================
-- IDENTITY PROVIDERS (futuro)
-- =========================================
CREATE TABLE auth.IdentityProviders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    IssuerUri NVARCHAR(400) NOT NULL UNIQUE
);

CREATE TABLE auth.UserIdentities (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ProviderId INT NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    EmailAtAuth NVARCHAR(256) NULL,
    CreatedAt DATETIME2(0) NOT NULL DEFAULT sysutcdatetime(),
    LastSeenAt DATETIME2(0) NULL,
    CONSTRAINT UX_UserIdentities_Provider_Subject UNIQUE (ProviderId, Subject),
    FOREIGN KEY (UserId) REFERENCES auth.UserAuth(Id) ON DELETE CASCADE,
    FOREIGN KEY (ProviderId) REFERENCES auth.IdentityProviders(Id) ON DELETE CASCADE
);

-- =========================================
-- USER SESSIONS (multi-sesión)
-- =========================================
CREATE TABLE auth.UserSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
    UserId INT NOT NULL,
    RefreshTokenHash VARBINARY(512) NOT NULL,
    Device NVARCHAR(200) NULL,
    Ip NVARCHAR(64) NULL,
    UserAgent NVARCHAR(300) NULL,
    CreatedAt DATETIME2(0) NOT NULL DEFAULT sysutcdatetime(),
    ExpiresAt DATETIME2(0) NOT NULL,
    RevokedAt DATETIME2(0) NULL,
    FOREIGN KEY (UserId) REFERENCES auth.UserAuth(Id) ON DELETE CASCADE
);

-- =========================================
-- ALLOWED DOMAINS (por tenant, con soporte '*')
-- =========================================
CREATE TABLE auth.AllowedEmailDomains (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TenantId INT NOT NULL,
    Domain NVARCHAR(200) NOT NULL, -- puede ser 'gmail.com', 'midominio.com' o '*'
    IsActive BIT NOT NULL DEFAULT (1),
    CONSTRAINT UX_Tenant_Domain UNIQUE (TenantId, Domain),
    FOREIGN KEY (TenantId) REFERENCES dbo.Tenants(Id) ON DELETE CASCADE
);
