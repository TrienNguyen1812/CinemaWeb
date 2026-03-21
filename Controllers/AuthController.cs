using CinemaWeb.Models;
using CinemaWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CinemaWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AuthService _authService;

        public AuthController(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _authService = AuthService.GetInstance(_scopeFactory);
        }

        // ===== LOGIN =====
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _authService.Login(email, password);

            if (user == null)
            {
                ViewBag.Error = "Sai email hoặc mật khẩu";
                return View();
            }

            HttpContext.Session.SetString("UserId", user.IdUser.ToString());
            HttpContext.Session.SetString("UserName", user.FullName);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("UserId", user.IdUser);

            return RedirectToAction("Index", "Home");
        }

        // ===== REGISTER =====
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string ConfirmPassword)
        {
            // gán role mặc định
            user.Role = "Customer";

            // xóa lỗi validation của Role
            ModelState.Remove("Role");

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // kiểm tra mật khẩu nhập lại
            if (user.Password != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu nhập lại không khớp";
                return View(user);
            }

            // kiểm tra email đã tồn tại chưa
            if (_authService.EmailExists(user.Email))
            {
                ViewBag.Error = "Email đã tồn tại";
                return View(user);
            }

            // gọi Singleton service
            _authService.Register(user);

            return RedirectToAction("Login");
        }

        // ===== LOGOUT =====
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
