CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Login TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FullName TEXT NOT NULL,
    Department TEXT NOT NULL,
    Role INTEGER NOT NULL
);

CREATE TABLE Documents (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    RegistrationNumber TEXT NOT NULL UNIQUE,
    Title TEXT NOT NULL,
    DocumentType TEXT NOT NULL,
    AuthorLogin TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    DueDate TEXT NULL,
    Status INTEGER NOT NULL,
    CurrentVersion TEXT NOT NULL,
    Content TEXT NOT NULL
);

CREATE TABLE ApprovalTasks (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    DocumentId INTEGER NOT NULL,
    ApproverLogin TEXT NOT NULL,
    CreatedAt TEXT NOT NULL,
    Deadline TEXT NOT NULL,
    IsCompleted INTEGER NOT NULL,
    Resolution TEXT NULL,
    Comment TEXT NULL,
    FOREIGN KEY (DocumentId) REFERENCES Documents(Id)
);

CREATE TABLE AuditLogs (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CreatedAt TEXT NOT NULL,
    UserLogin TEXT NOT NULL,
    Action TEXT NOT NULL,
    EntityName TEXT NOT NULL,
    EntityKey TEXT NOT NULL,
    Description TEXT NOT NULL
);
