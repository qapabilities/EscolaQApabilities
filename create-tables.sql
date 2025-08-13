-- Criar tabelas no SQLite
CREATE TABLE IF NOT EXISTS "Users" (
    "Id" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "PasswordHash" TEXT NOT NULL,
    "Role" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "LastLoginAt" TEXT NULL,
    "LoginAttempts" INTEGER NOT NULL,
    "LockedUntil" TEXT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "Students" (
    "Id" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "BirthDate" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "Address" TEXT NOT NULL,
    "City" TEXT NOT NULL,
    "State" TEXT NOT NULL,
    "ZipCode" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "EnrollmentDate" TEXT NOT NULL,
    "ParentName" TEXT NULL,
    "ParentPhone" TEXT NULL,
    "ParentEmail" TEXT NULL,
    "EmergencyContact" TEXT NULL,
    "EmergencyPhone" TEXT NULL,
    "MedicalInformation" TEXT NULL,
    "Notes" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    CONSTRAINT "PK_Students" PRIMARY KEY ("Id")
);

-- Índices
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email" ON "Users" ("Email");
CREATE INDEX IF NOT EXISTS "IX_Users_IsActive" ON "Users" ("IsActive");
CREATE INDEX IF NOT EXISTS "IX_Users_Role" ON "Users" ("Role");

CREATE UNIQUE INDEX IF NOT EXISTS "IX_Students_Email" ON "Students" ("Email");
CREATE INDEX IF NOT EXISTS "IX_Students_Name" ON "Students" ("Name");
CREATE INDEX IF NOT EXISTS "IX_Students_Status" ON "Students" ("Status");

-- Inserir usuários padrão
INSERT OR IGNORE INTO "Users" ("Id", "Email", "PasswordHash", "Role", "Name", "IsActive", "CreatedAt", "LoginAttempts") 
VALUES 
('550e8400-e29b-41d4-a716-446655440000', 'admin@qapabilities.com', '$2a$11$8ZS4a9X3qGqvGvbOYqsQXeO1YrCMFjmKQc8qPbGhE5sBz2YwF3VtK', 'Administrator', 'Administrador do Sistema', 1, datetime('now'), 0),
('550e8400-e29b-41d4-a716-446655440001', 'teacher@qapabilities.com', '$2a$11$8ZS4a9X3qGqvGvbOYqsQXeO1YrCMFjmKQc8qPbGhE5sBz2YwF3VtK', 'Teacher', 'Professor', 1, datetime('now'), 0);

-- Atualizar histórico de migrations
INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") 
VALUES ('20250811120000_InitialCreateSQLite', '8.0.0');

