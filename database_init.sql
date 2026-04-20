USE [at4370];
GO

CREATE TABLE Instructors (
    InstructorID INT IDENTITY(1,1) PRIMARY KEY,
    InstructorMail NVARCHAR(255) NOT NULL UNIQUE,
    InstructorName NVARCHAR(255) NOT NULL,
    InstructorPassword NVARCHAR(255) NOT NULL
);
GO

CREATE TABLE Classrooms (
    ClassCRN NVARCHAR(50) PRIMARY KEY,
    ClassName NVARCHAR(255) NOT NULL,
    ClassDescription NVARCHAR(MAX) NULL,
    InstructorID INT NOT NULL,
    CONSTRAINT FK_Classrooms_Instructors FOREIGN KEY (InstructorID) 
        REFERENCES Instructors(InstructorID) ON DELETE CASCADE
);
GO

CREATE TABLE Enrollments (
    StudentID NVARCHAR(50) NOT NULL,
    ClassCRN NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_Enrollments PRIMARY KEY (StudentID, ClassCRN),
    CONSTRAINT FK_Enrollments_Classrooms FOREIGN KEY (ClassCRN) 
        REFERENCES Classrooms(ClassCRN) ON DELETE CASCADE
);
GO

CREATE TABLE Sessions (
    SessionID UNIQUEIDENTIFIER PRIMARY KEY,
    ClassCRN NVARCHAR(50) NOT NULL,
    StartTime DATETIME2 NOT NULL,
    ExpiryTime DATETIME2 NULL,
    RequiresImage BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Sessions_Classrooms FOREIGN KEY (ClassCRN) 
        REFERENCES Classrooms(ClassCRN) ON DELETE CASCADE
);
GO

CREATE TABLE AttendanceRecords (
    RecordID UNIQUEIDENTIFIER PRIMARY KEY,
    SessionID UNIQUEIDENTIFIER NOT NULL,
    StudentID NVARCHAR(50) NOT NULL,
    CheckInTime DATETIME2 NOT NULL,
    ImageUrl NVARCHAR(MAX) NULL,
    IsApproved BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_AttendanceRecords_Sessions FOREIGN KEY (SessionID) 
        REFERENCES Sessions(SessionID) ON DELETE CASCADE
);
GO