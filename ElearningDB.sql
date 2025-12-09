--------------------------------------------------------
-- 1. TẠO DATABASE
--------------------------------------------------------
IF DB_ID('ElearningDB') IS NULL
BEGIN
    CREATE DATABASE ElearningDB;
END
GO

USE ElearningDB;
GO

--------------------------------------------------------
-- 2. CÁC BẢNG CỐT LÕI (KHUNG)
--------------------------------------------------------

-- Tài khoản chung: dùng cho SV, GV, Admin
IF OBJECT_ID('dbo.Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users (
        UserId       INT IDENTITY(1,1) PRIMARY KEY,
        Username     NVARCHAR(50)  NOT NULL UNIQUE,
        PasswordHash NVARCHAR(255) NOT NULL,
        FullName     NVARCHAR(100) NOT NULL,
        Email        NVARCHAR(100) NOT NULL UNIQUE,
        Role         NVARCHAR(20)  NOT NULL,   -- 'SV','GV','Admin'
        IsActive     BIT           NOT NULL DEFAULT 1
        -- TODO: sau này thêm FailedLoginCount, LockedUntil...
    );
END
GO

-- Thông tin sinh viên
IF OBJECT_ID('dbo.Students', 'U') IS NULL
BEGIN
    CREATE TABLE Students (
        StudentId INT IDENTITY(1,1) PRIMARY KEY,
        UserId    INT NOT NULL UNIQUE,
        ClassName NVARCHAR(50),
        Faculty   NVARCHAR(100),
        CONSTRAINT FK_Students_Users 
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

-- Thông tin giảng viên
IF OBJECT_ID('dbo.Teachers', 'U') IS NULL
BEGIN
    CREATE TABLE Teachers (
        TeacherId     INT IDENTITY(1,1) PRIMARY KEY,
        UserId        INT NOT NULL UNIQUE,
        Department    NVARCHAR(100),
        AcademicTitle NVARCHAR(50),
        CONSTRAINT FK_Teachers_Users 
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

-- (Admin nếu muốn tách riêng, có thể bỏ qua)
IF OBJECT_ID('dbo.Admins', 'U') IS NULL
BEGIN
    CREATE TABLE Admins (
        AdminId         INT IDENTITY(1,1) PRIMARY KEY,
        UserId          INT NOT NULL UNIQUE,
        PermissionLevel INT NOT NULL DEFAULT 1,
        CONSTRAINT FK_Admins_Users 
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

--------------------------------------------------------
-- KHÓA HỌC & LỚP HỌC PHẦN (UC-02)
--------------------------------------------------------

IF OBJECT_ID('dbo.Courses', 'U') IS NULL
BEGIN
    CREATE TABLE Courses (
        CourseId    INT IDENTITY(1,1) PRIMARY KEY,
        CourseCode  NVARCHAR(20) NOT NULL UNIQUE,
        CourseName  NVARCHAR(100) NOT NULL,
        Description NVARCHAR(MAX),
        Credit      INT,
        Status      NVARCHAR(20) NOT NULL DEFAULT N'Active'
    );
END
GO

IF OBJECT_ID('dbo.CourseSections', 'U') IS NULL
BEGIN
    CREATE TABLE CourseSections (
        SectionId         INT IDENTITY(1,1) PRIMARY KEY,
        CourseId          INT NOT NULL,
        Semester          NVARCHAR(20) NOT NULL,
        [Year]            INT NOT NULL,
        Room              NVARCHAR(50),
        MaxEnrollment     INT NOT NULL,
        CurrentEnrollment INT NOT NULL DEFAULT 0,
        Status            NVARCHAR(20) NOT NULL DEFAULT N'Open',
        TeacherId         INT NULL, -- sau này liên kết GV
        CONSTRAINT FK_Sections_Courses 
            FOREIGN KEY (CourseId) REFERENCES Courses(CourseId),
        CONSTRAINT FK_Sections_Teachers 
            FOREIGN KEY (TeacherId) REFERENCES Teachers(TeacherId)
    );
END
GO

IF OBJECT_ID('dbo.Enrollments', 'U') IS NULL
BEGIN
    CREATE TABLE Enrollments (
        EnrollmentId   INT IDENTITY(1,1) PRIMARY KEY,
        StudentId      INT NOT NULL,
        SectionId      INT NOT NULL,
        EnrollmentDate DATETIME NOT NULL DEFAULT GETDATE(),
        Status         NVARCHAR(20) NOT NULL DEFAULT N'Active',
        CONSTRAINT FK_Enrollments_Students 
            FOREIGN KEY (StudentId) REFERENCES Students(StudentId),
        CONSTRAINT FK_Enrollments_Sections 
            FOREIGN KEY (SectionId) REFERENCES CourseSections(SectionId),
        CONSTRAINT UQ_Enrollment UNIQUE (StudentId, SectionId)
    );
END
GO

--------------------------------------------------------
-- BÀI TẬP, BÀI NỘP, ĐIỂM (UC-03,04,05) - KHUNG
--------------------------------------------------------

IF OBJECT_ID('dbo.Assignments', 'U') IS NULL
BEGIN
    CREATE TABLE Assignments (
        AssignmentId   INT IDENTITY(1,1) PRIMARY KEY,
        SectionId      INT NOT NULL,
        Title          NVARCHAR(200) NOT NULL,
        Description    NVARCHAR(MAX),
        AttachmentPath NVARCHAR(255),
        CreatedTime    DATETIME NOT NULL DEFAULT GETDATE(),
        Deadline       DATETIME NOT NULL,
        Status         NVARCHAR(20) NOT NULL DEFAULT N'Open',
        CONSTRAINT FK_Assignments_Sections 
            FOREIGN KEY (SectionId) REFERENCES CourseSections(SectionId)
    );
END
GO

IF OBJECT_ID('dbo.Submissions', 'U') IS NULL
BEGIN
    CREATE TABLE Submissions (
        SubmissionId  INT IDENTITY(1,1) PRIMARY KEY,
        AssignmentId  INT NOT NULL,
        StudentId     INT NOT NULL,
        FilePath      NVARCHAR(255) NOT NULL,
        SubmittedTime DATETIME NOT NULL DEFAULT GETDATE(),
        IsLate        BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_Submissions_Assignments 
            FOREIGN KEY (AssignmentId) REFERENCES Assignments(AssignmentId),
        CONSTRAINT FK_Submissions_Students 
            FOREIGN KEY (StudentId) REFERENCES Students(StudentId)
    );
END
GO

IF OBJECT_ID('dbo.Grades', 'U') IS NULL
BEGIN
    CREATE TABLE Grades (
        GradeId      INT IDENTITY(1,1) PRIMARY KEY,
        SubmissionId INT NOT NULL,
        Score        DECIMAL(5,2) NOT NULL,
        Comment      NVARCHAR(MAX),
        GradedBy     INT NULL,   -- TeacherId
        GradedTime   DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Grades_Submissions 
            FOREIGN KEY (SubmissionId) REFERENCES Submissions(SubmissionId),
        CONSTRAINT FK_Grades_Teachers 
            FOREIGN KEY (GradedBy) REFERENCES Teachers(TeacherId)
    );
END
GO

--------------------------------------------------------
-- THÔNG BÁO (UC-06)
--------------------------------------------------------

IF OBJECT_ID('dbo.Notifications', 'U') IS NULL
BEGIN
    CREATE TABLE Notifications (
        NotificationId INT IDENTITY(1,1) PRIMARY KEY,
        SenderId       INT NOT NULL,  -- Users.UserId
        Content        NVARCHAR(MAX) NOT NULL,
        TargetType     NVARCHAR(20) NOT NULL, -- 'USER','SECTION','COURSE','GLOBAL'
        TargetRefId    INT NULL,
        CreatedTime    DATETIME NOT NULL DEFAULT GETDATE(),
        Status         NVARCHAR(20) NOT NULL DEFAULT N'Sent',
        CONSTRAINT FK_Notifications_Users 
            FOREIGN KEY (SenderId) REFERENCES Users(UserId)
    );
END
GO

IF OBJECT_ID('dbo.NotificationRecipients', 'U') IS NULL
BEGIN
    CREATE TABLE NotificationRecipients (
        NotificationId INT NOT NULL,
        UserId         INT NOT NULL,
        IsRead         BIT NOT NULL DEFAULT 0,
        ReadTime       DATETIME NULL,
        PRIMARY KEY (NotificationId, UserId),
        CONSTRAINT FK_NotiRec_Notifications 
            FOREIGN KEY (NotificationId) REFERENCES Notifications(NotificationId),
        CONSTRAINT FK_NotiRec_Users 
            FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

--------------------------------------------------------
-- (TÙY CHỌN) THÊM 1 SV DEMO ĐỂ TEST SAU NÀY
--------------------------------------------------------

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = N'sv01')
BEGIN
    INSERT INTO Users (Username, PasswordHash, FullName, Email, Role)
    VALUES (N'sv01', N'123456', N'Nguyễn Văn A', N'sv01@example.com', N'SV');

    DECLARE @uid INT = SCOPE_IDENTITY();

    INSERT INTO Students (UserId, ClassName, Faculty)
    VALUES (@uid, N'D18QTANM', N'Công nghệ thông tin');
END
GO
