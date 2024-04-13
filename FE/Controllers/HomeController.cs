using Microsoft.AspNetCore.Mvc;
using FE.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Drawing.Printing;
using System.Data.Entity.Validation;

namespace FE.Controllers
{
    public class HomeController : Controller
    {
    private readonly HttpClient _httpClient;
    public HomeController(){
         _httpClient = new HttpClient();
    }
       
        private readonly MyDbContext _db = new MyDbContext();

        public ActionResult Index()
        {
            return View();
        }
        // GET: User/Home
        public ActionResult Home()
        {
            if (HttpContext.Session.GetInt32("idUser") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: User/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Users _user)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine(_user.Email + " " + _user.Password);
                var check = _db.Users.FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    try
                    {
                        _user.Password = GetMD5(_user.Password);
                        _db.Users.Add(_user);
                        _db.SaveChanges();
                        return RedirectToAction("Login");
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Thực thể của kiểu \"{0}\" trong trạng thái \"{1}\" có các lỗi kiểm tra tính hợp lệ sau:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Thuộc tính: \"{0}\", Lỗi: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }

                        throw;
                    }
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View();
                }
            }
            return View();
        }

        // GET: User/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var f_password = GetMD5(password);
                var data = _db.Users.Where(s => s.Email.Equals(email) && s.Password.Equals(f_password)).ToList();
                Console.WriteLine(data.Count());
                if (data.Count() > 0)
                {
                    HttpContext.Session.SetString("FullName", data.FirstOrDefault().FirstName + " " + data.FirstOrDefault().LastName);
                    HttpContext.Session.SetString("Email", data.FirstOrDefault().Email);
                    HttpContext.Session.SetInt32("idUser",data.FirstOrDefault().IdUser);
                    return View("Home");             
                }
                else
                {
                    TempData["error"] = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            TempData["error"] = "Input invalid";
            return View("Login");
        }

        // GET: User/Logout
        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Create MD5 hash
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            StringBuilder byte2String = new StringBuilder();
            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String.Append(targetData[i].ToString("x2"));
            }
            return byte2String.ToString();
        }
        public ActionResult About() 
        {
            return View();
        }
    }
}
