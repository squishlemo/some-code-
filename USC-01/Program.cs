using System;
using System.Collections.Generic;
using System.Text;

public class User
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }   // SV, GV, Admin
    public bool IsLocked { get; set; }
    public int FailedAttempts { get; set; }
    public string LastDeviceId { get; set; }
}
public static class OtpService
{
    public static string GenerateOtp()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    public static void SendOtp(string email, string otp)
    {
        Console.WriteLine($"[OTP gửi tới {email}]: {otp}");
    }
}
public class AuthService
{
    private Dictionary<string, User> users = new Dictionary<string, User>();
    private Dictionary<string, string> otpStorage = new Dictionary<string, string>();

    private const int MAX_FAIL_ATTEMPT = 5;

    // Đăng ký tài khoản
    public bool Register(string email, string username, string password)
    {
        if (users.ContainsKey(username)) return false;

        string otp = OtpService.GenerateOtp();
        otpStorage[email] = otp;
        OtpService.SendOtp(email, otp);

        Console.WriteLine("Vui lòng nhập mã OTP để hoàn tất đăng ký:");
        string inputOtp = Console.ReadLine();

        if (inputOtp != otp)
        {
            Console.WriteLine("OTP sai hoặc hết hạn!");
            return false;
        }

        users[username] = new User
        {
            Username = username,
            PasswordHash = password,
            Email = email,
            Role = "SV",
            IsLocked = false,
            FailedAttempts = 0,
            LastDeviceId = null
        };

        otpStorage.Remove(email);
        Console.WriteLine("Tạo tài khoản thành công!");
        return true;
    }

    // Đăng nhập
    public bool Login(string username, string password, string deviceId)
    {
        if (!users.ContainsKey(username))
        {
            Console.WriteLine("Tài khoản không tồn tại.");
            return false;
        }

        var user = users[username];

        if (user.IsLocked)
        {
            Console.WriteLine("Tài khoản bị khóa do nhập sai nhiều lần.");
            return false;
        }

        // Kiểm tra mật khẩu
        if (user.PasswordHash != password)
        {
            user.FailedAttempts++;
            if (user.FailedAttempts >= MAX_FAIL_ATTEMPT)
            {
                user.IsLocked = true;
                Console.WriteLine("Nhập sai quá số lần — tài khoản bị khóa tạm thời.");
            }
            else
            {
                Console.WriteLine($"Sai mật khẩu. Lần {user.FailedAttempts}/{MAX_FAIL_ATTEMPT}");
            }
            return false;
        }

        // Reset số lần sai
        user.FailedAttempts = 0;

        // Kiểm tra thiết bị lạ
        if (user.LastDeviceId != deviceId)
        {
            Console.WriteLine("Phát hiện thiết bị mới — cần xác thực lại qua OTP.");
            string otp = OtpService.GenerateOtp();
            otpStorage[user.Email] = otp;
            OtpService.SendOtp(user.Email, otp);

            Console.Write("Nhập OTP: ");
            string inputOtp = Console.ReadLine();

            if (inputOtp != otp)
            {
                Console.WriteLine("OTP sai hoặc hết hạn!");
                return false;
            }

            user.LastDeviceId = deviceId;
            otpStorage.Remove(user.Email);
        }

        // Đăng nhập thành công
        Console.WriteLine($"Đăng nhập thành công! Chào {user.Username} — Vai trò: {user.Role}");
        return true;
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        AuthService auth = new AuthService();

        while (true)
        {
            Console.WriteLine("\n===== HỆ THỐNG XÁC THỰC =====");
            Console.WriteLine("1. Đăng nhập");
            Console.WriteLine("2. Đăng ký");
            Console.WriteLine("0. Thoát");
            Console.Write("Chọn chức năng: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Nhập username: ");
                    string userLogin = Console.ReadLine();

                    Console.Write("Nhập mật khẩu: ");
                    string passLogin = Console.ReadLine();

                    Console.Write("Nhập mã thiết bị (deviceId): ");
                    string deviceId = Console.ReadLine();

                    auth.Login(userLogin, passLogin, deviceId);
                    break;

                case "2":
                    Console.Write("Nhập email: ");
                    string email = Console.ReadLine();

                    Console.Write("Nhập username: ");
                    string username = Console.ReadLine();

                    Console.Write("Nhập mật khẩu: ");
                    string password = Console.ReadLine();

                    auth.Register(email, username, password);
                    break;

                case "0":
                    Console.WriteLine("Thoát chương trình...");
                    return;

                default:
                    Console.WriteLine("Lựa chọn không hợp lệ!");
                    break;
            }
        }
    }
}
