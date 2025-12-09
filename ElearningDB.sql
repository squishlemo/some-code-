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

IF OBJECT_ID('dbo.RegistrationRequests', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RegistrationRequests (
        RequestId      INT IDENTITY(1,1) PRIMARY KEY,
        Username       NVARCHAR(50)  NOT NULL,
        PasswordHash   NVARCHAR(255) NOT NULL,
        FullName       NVARCHAR(100) NOT NULL,
        Email          NVARCHAR(100) NOT NULL,
        Role           NVARCHAR(20)  NOT NULL, -- 'SV', 'GV', 'Admin'
        Status         NVARCHAR(20)  NOT NULL DEFAULT N'PendingEmail', 
        OTP            NVARCHAR(10)  NULL,
        CreatedAt      DATETIME      NOT NULL DEFAULT GETDATE()
    );
END;
GO

USE ElearningDB;
GO

--------------------------------------------------------
-- 1) BẢNG KHOA
--------------------------------------------------------
IF OBJECT_ID('dbo.Faculties', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Faculties
    (
        FacultyId   INT IDENTITY(1,1) PRIMARY KEY,
        FacultyName NVARCHAR(100) NOT NULL UNIQUE
    );
END;
GO

--------------------------------------------------------
-- 2) BẢNG NGÀNH
--------------------------------------------------------
IF OBJECT_ID('dbo.Majors', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Majors
    (
        MajorId    INT IDENTITY(1,1) PRIMARY KEY,
        MajorName  NVARCHAR(100) NOT NULL,
        FacultyId  INT NOT NULL,
        CONSTRAINT FK_Majors_Faculties
            FOREIGN KEY (FacultyId) REFERENCES Faculties(FacultyId)
    );
END;
GO

--------------------------------------------------------
-- 3) BẢNG LỚP
--------------------------------------------------------
IF OBJECT_ID('dbo.Classes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Classes
    (
        ClassId   INT IDENTITY(1,1) PRIMARY KEY,
        ClassName NVARCHAR(100) NOT NULL,
        MajorId   INT NOT NULL,
        CONSTRAINT FK_Classes_Majors
            FOREIGN KEY (MajorId) REFERENCES Majors(MajorId)
    );
END;
GO

--------------------------------------------------------
-- 4) BẢNG HỌC VỊ GIÁO VIÊN
--------------------------------------------------------
IF OBJECT_ID('dbo.AcademicTitles', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AcademicTitles
    (
        TitleId   INT IDENTITY(1,1) PRIMARY KEY,
        TitleName NVARCHAR(50) NOT NULL UNIQUE
    );
END;
GO

--------------------------------------------------------
-- 5) DỮ LIỆU MẪU
--------------------------------------------------------

-- KHOA
IF NOT EXISTS (SELECT 1 FROM Faculties WHERE FacultyName = N'Khoa Công nghệ thông tin')
    INSERT INTO Faculties(FacultyName) VALUES (N'Khoa Công nghệ thông tin');

IF NOT EXISTS (SELECT 1 FROM Faculties WHERE FacultyName = N'Khoa Kỹ thuật điện tử')
    INSERT INTO Faculties(FacultyName) VALUES (N'Khoa Kỹ thuật điện tử');

DECLARE @FacCNTT INT = (SELECT FacultyId FROM Faculties WHERE FacultyName = N'Khoa Công nghệ thông tin');
DECLARE @FacKTDT INT = (SELECT FacultyId FROM Faculties WHERE FacultyName = N'Khoa Kỹ thuật điện tử');

-- NGÀNH
IF NOT EXISTS (SELECT 1 FROM Majors WHERE MajorName = N'Công nghệ thông tin')
    INSERT INTO Majors(MajorName, FacultyId) VALUES (N'Công nghệ thông tin', @FacCNTT);

IF NOT EXISTS (SELECT 1 FROM Majors WHERE MajorName = N'Công nghệ kỹ thuật điện, điện tử')
    INSERT INTO Majors(MajorName, FacultyId) VALUES (N'Công nghệ kỹ thuật điện, điện tử', @FacKTDT);

IF NOT EXISTS (SELECT 1 FROM Majors WHERE MajorName = N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa')
    INSERT INTO Majors(MajorName, FacultyId) VALUES (N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa', @FacKTDT);

DECLARE @MajorCNTT INT = (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ thông tin');
DECLARE @MajorDien INT = (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ kỹ thuật điện, điện tử');
DECLARE @MajorTĐH  INT = (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa');

-- LỚP CNTT
IF NOT EXISTS (SELECT 1 FROM Classes WHERE ClassName = N'D18 Quản trị an ninh mạng')
    INSERT INTO Classes(ClassName, MajorId) VALUES (N'D18 Quản trị an ninh mạng', @MajorCNTT);

IF NOT EXISTS (SELECT 1 FROM Classes WHERE ClassName = N'D18 Công nghệ phần mềm')
    INSERT INTO Classes(ClassName, MajorId) VALUES (N'D18 Công nghệ phần mềm', @MajorCNTT);

IF NOT EXISTS (SELECT 1 FROM Classes WHERE ClassName = N'D18 Thương mại điện tử')
    INSERT INTO Classes(ClassName, MajorId) VALUES (N'D18 Thương mại điện tử', @MajorCNTT);

-- LỚP CNKT ĐIỆN, ĐIỆN TỬ
IF NOT EXISTS (SELECT 1 FROM Classes WHERE ClassName = N'D18 Công nghệ kỹ thuật điện, điện tử 1')
    INSERT INTO Classes(ClassName, MajorId) VALUES (N'D18 Công nghệ kỹ thuật điện, điện tử 1', @MajorDien);

IF NOT EXISTS (SELECT 1 FROM Classes WHERE ClassName = N'D18 Công nghệ kỹ thuật điện, điện tử 2')
    INSERT INTO Classes(ClassName, MajorId) VALUES (N'D18 Công nghệ kỹ thuật điện, điện tử 2', @MajorDien);

-- LỚP TỰ ĐỘNG HÓA
IF NOT EXISTS (SELECT 1 FROM Classes WHERE ClassName = N'D18 Công nghệ Kỹ thuật Điều khiển Tự động hóa')
    INSERT INTO Classes(ClassName, MajorId) VALUES (N'D18 Công nghệ Kỹ thuật Điều khiển Tự động hóa', @MajorTĐH);

IF NOT EXISTS (SELECT 1 FROM Classes WHERE ClassName = N'D18 Điều khiển thiết bị điện công nghiệp')
    INSERT INTO Classes(ClassName, MajorId) VALUES (N'D18 Điều khiển thiết bị điện công nghiệp', @MajorTĐH);

IF NOT EXISTS (SELECT 1 FROM Classes WHERE ClassName = N'D18 Tin học cho Điều khiển và Tự động hóa')
    INSERT INTO Classes(ClassName, MajorId) VALUES (N'D18 Tin học cho Điều khiển và Tự động hóa', @MajorTĐH);

-- HỌC VỊ
IF NOT EXISTS (SELECT 1 FROM AcademicTitles WHERE TitleName = N'Thạc sĩ')
    INSERT INTO AcademicTitles(TitleName) VALUES (N'Thạc sĩ');

IF NOT EXISTS (SELECT 1 FROM AcademicTitles WHERE TitleName = N'Tiến sĩ')
    INSERT INTO AcademicTitles(TitleName) VALUES (N'Tiến sĩ');
GO


ALTER TABLE RegistrationRequests
ADD ClassName     NVARCHAR(50)   NULL,
    Faculty       NVARCHAR(100)  NULL,
    Department    NVARCHAR(100)  NULL,
    AcademicTitle NVARCHAR(50)   NULL;


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


--------------------------------------------------------
-- ADMIN DEMO
--------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = N'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, FullName, Email, Role, IsActive)
    VALUES (N'admin', N'123456', N'Quản trị viên hệ thống', N'admin@example.com', N'Admin', 1);

    DECLARE @uid_admin INT = SCOPE_IDENTITY();

    INSERT INTO Admins (UserId, PermissionLevel)
    VALUES (@uid_admin, 10); -- PermissionLevel tự quy ước, 10 = cao nhất
END
GO

--------------------------------------------------------
-- GIẢNG VIÊN 01
--------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = N'gv01')
BEGIN
    INSERT INTO Users (Username, PasswordHash, FullName, Email, Role, IsActive)
    VALUES (N'gv01', N'123456', N'Nguyễn Thị B', N'gv01@example.com', N'GV', 1);

    DECLARE @uid_gv01 INT = SCOPE_IDENTITY();

    INSERT INTO Teachers (UserId, Department, AcademicTitle)
    VALUES (@uid_gv01, N'Khoa Công nghệ thông tin', N'ThS.');
END
GO

USE ElearningDB;
GO

-- Thêm cột MajorId nếu chưa có
IF COL_LENGTH('dbo.Courses', 'MajorId') IS NULL
BEGIN
    ALTER TABLE dbo.Courses
        ADD MajorId INT NULL;
END;
GO

-- Thêm khóa ngoại Courses.MajorId -> Majors.MajorId (nếu chưa có)
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Courses_Majors')
BEGIN
    ALTER TABLE dbo.Courses
        ADD CONSTRAINT FK_Courses_Majors
        FOREIGN KEY (MajorId) REFERENCES dbo.Majors(MajorId);
END;
GO




USE ElearningDB;
GO

DECLARE @MajorCNTT INT  = (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ thông tin');
DECLARE @MajorDien INT  = (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ kỹ thuật điện, điện tử');
DECLARE @MajorTĐH  INT  = (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa');




USE ElearningDB;
GO

INSERT INTO Courses (CourseCode, CourseName, Description, Credit, Status, MajorId)
VALUES
(N'IT101', N'Nhập môn Công nghệ thông tin',
 N'Giới thiệu ngành, môi trường làm việc và các hướng chuyên sâu CNTT.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ thông tin')),

(N'IT102', N'Lập trình C/C++',
 N'Cấu trúc dữ liệu cơ bản, con trỏ, mảng, hàm, struct, file.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ thông tin')),

(N'IT201', N'Cơ sở dữ liệu',
 N'Mô hình quan hệ, SQL, thiết kế lược đồ, chuẩn hóa.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ thông tin')),

(N'IT202', N'Mạng máy tính',
 N'Kiến trúc mạng, mô hình OSI, TCP/IP, địa chỉ IP, định tuyến.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ thông tin')),

(N'IT203', N'An toàn mạng',
 N'Các khái niệm tấn công, phòng thủ, mã hóa cơ bản, firewall.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ thông tin'));


 INSERT INTO Courses (CourseCode, CourseName, Description, Credit, Status, MajorId)
VALUES
(N'EE101', N'Mạch điện',
 N'Định luật mạch điện, mạch một chiều, xoay chiều, phân tích mạch.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ kỹ thuật điện, điện tử')),

(N'EE102', N'Kỹ thuật điện',
 N'Máy điện, máy biến áp, các thiết bị điện cơ bản.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ kỹ thuật điện, điện tử')),

(N'EE201', N'Hệ thống cung cấp điện',
 N'Cấu trúc hệ thống điện, truyền tải – phân phối – sử dụng.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ kỹ thuật điện, điện tử')),

(N'EE202', N'Điện tử công suất',
 N'Linh kiện bán dẫn công suất, chỉnh lưu, nghịch lưu, điều khiển động cơ.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ kỹ thuật điện, điện tử'));


 INSERT INTO Courses (CourseCode, CourseName, Description, Credit, Status, MajorId)
VALUES
(N'AT101', N'Cơ sở điều khiển tự động',
 N'Mô hình hóa hệ thống, hàm truyền, ổn định, đáp ứng quá độ.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa')),

(N'AT102', N'Cảm biến và đo lường',
 N'Nguyên lý và ứng dụng của cảm biến, hệ đo lường trong tự động hóa.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa')),

(N'AT201', N'PLC và hệ thống điều khiển',
 N'Lập trình PLC, điều khiển dây chuyền, hệ thống tự động.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa')),

(N'AT202', N'Hệ thống SCADA',
 N'Giám sát, điều khiển và thu thập dữ liệu trong công nghiệp.',
 3, N'Active',
 (SELECT MajorId FROM Majors WHERE MajorName = N'Công nghệ Kỹ thuật Điều khiển và Tự động hóa'));






