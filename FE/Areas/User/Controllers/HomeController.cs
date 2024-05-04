namespace FE.Areas.User.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FE.Models;
using System.Text;
using Newtonsoft.Json;
using System.Data.Entity;

[Area("User")]
public class HomeController : BaseController
{
    private readonly IHubContext<OnlineUsersHub> _hubContext;
    private readonly HttpClient _httpClient;
    private readonly MyDbContext _db;

    public HomeController(IHubContext<OnlineUsersHub> hubContext,MyDbContext dbContext)
    {
        _hubContext = hubContext;
        _httpClient = new HttpClient();
        _db = dbContext;
    }

    [HttpGet("/User/Index")]
    public ActionResult Index()
    {
        if (HttpContext.Session.GetInt32("idUser") != null)
        {
            var json = GetChartData();
            ViewBag.json = json;
            return View();
        }
        else
        {
            return Redirect("/Home/Login");
        }
    }
    [HttpGet("/User/Classify")]
    public ActionResult Classify()
    {
        return View();
    }
    [HttpPost("/User/Classify")]
    public async Task<ActionResult> Classify(string url)
    {
        // Địa chỉ URL của API
        string apiUrl = "http://127.0.0.1:5001/api/detectnews";

        try
        {
            // Tạo đối tượng chứa dữ liệu JSON
            var jsonContent = new StringContent("{\"url\": \"" + url + "\"}", Encoding.UTF8, "application/json");
            Console.WriteLine(url);
            // Gửi yêu cầu POST đến API
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, jsonContent);
            // Kiểm tra xem phản hồi có thành công không
            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung của phản hồi
                string responseData = await response.Content.ReadAsStringAsync();
                // Chuyển đổi chuỗi JSON thành chuỗi JSON định dạng
                string formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseData), Formatting.Indented);
                var log = new ClassificationLog
                {
                    UserId = HttpContext.Session.GetInt32("idUser") ?? 0, // Lấy ID của người dùng từ Session
                    Time = DateTime.Now,
                    Url = url,
                    ResponseData = responseData
                };
                // Thêm log vào cơ sở dữ liệu
                _db.ClassificationLogs.Add(log);
                // Lưu thay đổi vào cơ sở dữ liệu
                await _db.SaveChangesAsync();
                // Hiển thị dữ liệu phản hồi dưới dạng JSON trong ViewBag
                ViewBag.ResponseData = formattedJson;
            }
            else
            {
                // Xử lý trường hợp phản hồi không thành công
                TempData["error"] = "Fail to detect";
            }
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            TempData["error"] =  ex.Message;
        }

        return View();
    }
    [HttpGet("/User/Summarize")]
    public ActionResult Summarize()
    {
        return View();
    }
    [HttpPost("/User/Summarize")]
    public async Task<ActionResult> Summarize(string text, int numSentences)
    {
        // Địa chỉ URL của API
        string apiUrl = "http://127.0.0.1:5001/api/summerize";

        try
        {
            var postData = new
            {
                text = text,
                num_sentences = numSentences
            };
            // Tạo đối tượng chứa dữ liệu JSON
            var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(postData), Encoding.UTF8, "application/json");
            // Gửi yêu cầu POST đến API
            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, jsonContent);
            // Kiểm tra xem phản hồi có thành công không
            if (response.IsSuccessStatusCode)
            {
                // Đọc nội dung của phản hồi
                string responseData = await response.Content.ReadAsStringAsync();
                string formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseData), Formatting.Indented);

                // Hiển thị dữ liệu phản hồi trong ViewBag
                ViewBag.ResponseData = formattedJson;
            }
            else
            {
                // Xử lý trường hợp phản hồi không thành công
                TempData["error"] = "Fail to summarize";
            }
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            TempData["error"] = ex.Message;
        }

        return View();
    }

    [HttpGet("/User/Logout")]
    public async Task<IActionResult> Logout()
    {
        HttpContext.Session.Clear();
        await _hubContext.Clients.All.SendAsync("OnDisconnectedAsync");
        return Redirect("/Home/Home");
    }


    [HttpGet("/User/Crawler")]
    public ActionResult Crawler(){
        return View();
    }
    [HttpGet("/User/CommentClassify")]
    public ActionResult CommentClassify(){
        return View();
    }
    [HttpGet("/User/DownloadVideo")]
    public ActionResult DownloadVideo(){
        return View();
    }
    [HttpGet("/User/ActivityLog")]
    public IActionResult ActivityLog()
    {
        int userId = HttpContext.Session.GetInt32("idUser") ?? 0;
        var userLogs = _db.ClassificationLogs.Where(log => log.UserId == userId).ToList();
        ViewBag.UserLogs = userLogs;
        return View();
    }
    [HttpGet("/User/Settings")]
    public ActionResult Settings(){
        return View();
    }
    public IActionResult Error()
    {
        return View();
    }
    [HttpPost("/User/Settings")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Settings(Users model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int userId = HttpContext.Session.GetInt32("idUser") ?? 0;
                    var user = _db.Users.FirstOrDefault(s => s.IdUser == userId);
                    if (user != null)
                    {
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Email = model.Email;

                        // Nếu bạn cần xử lý mật khẩu, bạn có thể thực hiện các thay đổi ở đây

                        await _db.SaveChangesAsync();
                        TempData["success"] = "User information updated successfully.";
                    }
                    else
                    {
                        TempData["error"] = "User not found.";
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"An error occurred while updating user information: {ex.Message}";
                }
            }
            else
            {
                TempData["error"] = "Invalid model data.";
            }

            return RedirectToAction("Settings"); // Chuyển hướng đến trang chính hoặc trang thông báo khác
        }



    public string GetChartData()
    {
        var responseDataList = _db.ClassificationLogs
            .OrderByDescending(log => log.Time)
            .Select(log => log.ResponseData)
            .ToList();

        // Convert responseDataList to JSON string
        var json = JsonConvert.SerializeObject(responseDataList);
        return json;
    }

}