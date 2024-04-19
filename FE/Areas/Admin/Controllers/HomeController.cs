namespace FE.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http;
using FE.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


[Area("Admin")]
public class HomeController : BaseController
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HomeController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    private readonly MyDbContext _db = new MyDbContext();
    [HttpGet("/Admin/Index")]
    public ActionResult Index()
        {
            var idUser = _httpContextAccessor.HttpContext.Session.GetInt32("idUser");
            if  (idUser.HasValue && idUser.Value == 1)
            {
                var users = _db.Users.ToList();
                var json = GetChartData();
                ViewBag.json = json;
                return View(users);
            }
            else
            {
                return Redirect("/Home/Error401");
            }
        }
        
    [HttpGet("/Admin/UserManager")]
    public ActionResult UserManager()
    {
        // Truy vấn danh sách người dùng từ cơ sở dữ liệu
        var users = _db.Users.ToList();

        // Trả về view với danh sách người dùng
        return View(users);
    }
    [HttpGet("/Admin/Logout")]
    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Redirect("/Home/Home");
    }
    [HttpGet("/Admin/Chart")]
    public ActionResult Chart()
    {
        var json = GetChartData();

        ViewBag.json = json;
        return View();
    }
    public string GetChartData()
    {
        var latestData = _db.VisitCount
            .OrderByDescending(vc => vc.Date)
            .Take(10)
            .Select(vc => new { Date = vc.Date.ToString(), Count = vc.Count })
            .ToList();

        // Convert latestData to JSON string
        var json = JsonConvert.SerializeObject(latestData);

        return json;
    }

    public IActionResult Error()
    {
        return View();
    }
}