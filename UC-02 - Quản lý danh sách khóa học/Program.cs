using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseManagementDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            // Seed dữ liệu
            Data.Seed();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("===== Course Management Demo =====");
                Console.WriteLine("Chọn vai trò:");
                Console.WriteLine("1. Sinh viên (Student)");
                Console.WriteLine("2. Giảng viên (Teacher)");
                Console.WriteLine("3. Admin");
                Console.WriteLine("0. Thoát");
                Console.Write("Lựa chọn: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        UserInterface.StudentMenu();
                        break;
                    case "2":
                        UserInterface.TeacherMenu();
                        break;
                    case "3":
                        UserInterface.AdminMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    // ----- Models -----
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }
        public string Role { get; set; } // Student, Teacher, Admin

        public User(string username, string role)
        {
            Username = username;
            Role = role;
        }
    }

    public class CourseSchedule
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public CourseSchedule(DayOfWeek day, string start, string end)
        {
            Day = day;
            StartTime = TimeSpan.Parse(start);
            EndTime = TimeSpan.Parse(end);
        }

        public override string ToString()
        {
            return $"{Day} {StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
        }
    }

    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Code { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public User Teacher { get; set; }
        public int Capacity { get; set; }
        public List<CourseSchedule> Schedules { get; set; } = new List<CourseSchedule>();
        public List<User> EnrolledStudents { get; set; } = new List<User>();

        public int RemainingSeats => Capacity - EnrolledStudents.Count;

        public override string ToString()
        {
            return $"{Code} - {Title} | GV: {Teacher.Username} | {EnrolledStudents.Count}/{Capacity} SV | Lịch: {string.Join(", ", Schedules)}";
        }
    }

    // ----- In-memory Data -----
    public static class Data
    {
        public static List<User> Users = new List<User>();
        public static List<Course> Courses = new List<Course>();

        public static void Seed()
        {
            // Users
            var teacher1 = new User("gv_nguyen", "Teacher");
            var teacher2 = new User("gv_tran", "Teacher");
            var student1 = new User("sv_1", "Student");
            var student2 = new User("sv_2", "Student");
            var admin = new User("admin1", "Admin");

            Users.AddRange(new[] { teacher1, teacher2, student1, student2, admin });

            // Courses
            var c1 = new Course
            {
                Code = "CS101",
                Title = "Lập trình C# cơ bản",
                Description = "Giới thiệu C# và lập trình hướng đối tượng",
                Teacher = teacher1,
                Capacity = 2,
                Schedules = new List<CourseSchedule> { new CourseSchedule(DayOfWeek.Monday, "09:00", "11:00") }
            };

            var c2 = new Course
            {
                Code = "CS201",
                Title = "Cấu trúc dữ liệu",
                Description = "Danh sách, ngăn xếp, hàng đợi, cây, đồ thị",
                Teacher = teacher1,
                Capacity = 30,
                Schedules = new List<CourseSchedule> { new CourseSchedule(DayOfWeek.Monday, "10:30", "12:00") }
            };

            Courses.AddRange(new[] { c1, c2 });
        }
    }

    // ----- User Interface -----
    public static class UserInterface
    {
        public static void StudentMenu()
        {
            Console.Clear();
            var student = Login("Student");
            if (student == null) return;

            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Sinh viên: {student.Username} ===");
                Console.WriteLine("1. Xem tất cả khóa học");
                Console.WriteLine("2. Xem khóa đã đăng ký");
                Console.WriteLine("3. Đăng ký khóa học");
                Console.WriteLine("4. Hủy đăng ký khóa học");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        foreach (var c in Data.Courses)
                        {
                            var status = c.EnrolledStudents.Contains(student) ? "(Đã đăng ký)" : "(Chưa đăng ký)";
                            Console.WriteLine($"{c} {status}");
                        }
                        Console.ReadKey();
                        break;
                    case "2":
                        var enrolled = Data.Courses.Where(c => c.EnrolledStudents.Contains(student)).ToList();
                        if (!enrolled.Any()) Console.WriteLine("Chưa đăng ký khóa nào.");
                        else enrolled.ForEach(c => Console.WriteLine(c));
                        Console.ReadKey();
                        break;
                    case "3":
                        RegisterCourse(student);
                        break;
                    case "4":
                        UnregisterCourse(student);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public static void TeacherMenu()
        {
            Console.Clear();
            var teacher = Login("Teacher");
            if (teacher == null) return;

            var myCourses = Data.Courses.Where(c => c.Teacher == teacher).ToList();
            Console.WriteLine($"=== Giảng viên: {teacher.Username} ===");
            if (!myCourses.Any()) Console.WriteLine("Bạn chưa dạy khóa nào.");
            else
            {
                foreach (var c in myCourses)
                {
                    Console.WriteLine(c);
                    if (c.EnrolledStudents.Any())
                        Console.WriteLine($"SV đăng ký: {string.Join(", ", c.EnrolledStudents.Select(s => s.Username))}");
                    else
                        Console.WriteLine("Chưa có sinh viên nào đăng ký.");
                }
            }
            Console.WriteLine("Nhấn phím bất kỳ để quay lại...");
            Console.ReadKey();
        }

        public static void AdminMenu()
        {
            Console.Clear();
            var admin = Login("Admin");
            if (admin == null) return;
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Admin: {admin.Username} ===");
                Console.WriteLine("1. Xem tất cả khóa học");
                Console.WriteLine("2. Thêm khóa học");
                Console.WriteLine("3. Xóa khóa học");
                Console.WriteLine("0. Quay lại");
                Console.Write("Chọn: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        foreach (var c in Data.Courses)
                            Console.WriteLine(c);
                        Console.ReadKey();
                        break;
                    case "2":
                        AddCourse();
                        break;
                    case "3":
                        DeleteCourse();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static User Login(string role)
        {
            var users = Data.Users.Where(u => u.Role == role).ToList();
            Console.WriteLine($"Danh sách {role}:");
            for (int i = 0; i < users.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {users[i].Username}");
            }
            Console.Write("Chọn người dùng: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= users.Count)
                return users[idx - 1];
            Console.WriteLine("Lựa chọn không hợp lệ.");
            Console.ReadKey();
            return null;
        }

        private static void RegisterCourse(User student)
        {
            Console.WriteLine("Nhập mã khóa học muốn đăng ký:");
            var code = Console.ReadLine();
            var course = Data.Courses.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (course == null) { Console.WriteLine("Khóa học không tồn tại."); Console.ReadKey(); return; }

            if (course.EnrolledStudents.Contains(student))
            {
                Console.WriteLine("Bạn đã đăng ký khóa học này.");
                Console.ReadKey();
                return;
            }

            if (course.RemainingSeats <= 0)
            {
                Console.WriteLine("Lớp học đã đầy.");
                Console.ReadKey();
                return;
            }

            // Check schedule conflict
            var hasConflict = Data.Courses
                .Where(c => c.EnrolledStudents.Contains(student))
                .Any(c => c.Schedules.Any(s1 => course.Schedules.Any(s2 =>
                    s1.Day == s2.Day && s1.StartTime < s2.EndTime && s2.StartTime < s1.EndTime)));

            if (hasConflict)
            {
                Console.WriteLine("Trùng lịch với khóa đã đăng ký.");
                Console.ReadKey();
                return;
            }

            course.EnrolledStudents.Add(student);
            Console.WriteLine("Đăng ký thành công!");
            Console.ReadKey();
        }

        private static void UnregisterCourse(User student)
        {
            Console.WriteLine("Nhập mã khóa học muốn hủy đăng ký:");
            var code = Console.ReadLine();
            var course = Data.Courses.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (course == null || !course.EnrolledStudents.Contains(student))
            {
                Console.WriteLine("Bạn chưa đăng ký khóa học này.");
                Console.ReadKey();
                return;
            }

            course.EnrolledStudents.Remove(student);
            Console.WriteLine("Hủy đăng ký thành công!");
            Console.ReadKey();
        }

        private static void AddCourse()
        {
            Console.Write("Mã khóa học: ");
            var code = Console.ReadLine();
            Console.Write("Tên khóa học: ");
            var title = Console.ReadLine();
            Console.Write("Mô tả: ");
            var desc = Console.ReadLine();

            Console.WriteLine("Chọn giảng viên:");
            var teachers = Data.Users.Where(u => u.Role == "Teacher").ToList();
            for (int i = 0; i < teachers.Count; i++)
                Console.WriteLine($"{i + 1}. {teachers[i].Username}");
            if (!int.TryParse(Console.ReadLine(), out int tIdx) || tIdx < 1 || tIdx > teachers.Count)
            {
                Console.WriteLine("Giảng viên không hợp lệ."); Console.ReadKey(); return;
            }
            var teacher = teachers[tIdx - 1];

            Console.Write("Số lượng tối đa: ");
            if (!int.TryParse(Console.ReadLine(), out int cap)) cap = 30;

            var course = new Course
            {
                Code = code,
                Title = title,
                Description = desc,
                Teacher = teacher,
                Capacity = cap
            };

            Console.WriteLine("Nhập lịch học (ví dụ: Monday 09:00 11:00), nhập 'done' để kết thúc:");
            while (true)
            {
                Console.Write("Lịch: ");
                var line = Console.ReadLine();
                if (line.Trim().ToLower() == "done") break;
                var parts = line.Split(' ');
                if (parts.Length != 3) { Console.WriteLine("Sai định dạng."); continue; }
                try
                {
                    var day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), parts[0], true);
                    var schedule = new CourseSchedule(day, parts[1], parts[2]);
                    course.Schedules.Add(schedule);
                }
                catch { Console.WriteLine("Sai định dạng."); }
            }

            Data.Courses.Add(course);
            Console.WriteLine("Thêm khóa học thành công!");
            Console.ReadKey();
        }

        private static void DeleteCourse()
        {
            Console.Write("Nhập mã khóa học muốn xóa: ");
            var code = Console.ReadLine();
            var course = Data.Courses.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (course == null) { Console.WriteLine("Khóa học không tồn tại."); Console.ReadKey(); return; }

            Data.Courses.Remove(course);
            Console.WriteLine("Xóa khóa học thành công!");
            Console.ReadKey();
        }
    }
}
