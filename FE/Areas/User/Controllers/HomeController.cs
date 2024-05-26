namespace FE.Areas.User.Controllers;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using FE.Models;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

// Tạo lớp ModelViewModel
public class ModelViewModel
{
    public int Id { get; set; }
    public int IdUser { get; set; }
    public string ModelName { get; set; }
    public string ModelPath { get; set; }
    public double Accuracy { get; set; }
}

// Tạo lớp bọc để chứa mảng models
public class ModelsWrapper
{
    public List<ModelViewModel> Models { get; set; }
}

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
            Console.WriteLine("1");
            return View();
        }
        else
        {
            Console.WriteLine("2");
            return Redirect("/Home/Login");
            // return View();
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
        string blacklistJson = System.IO.File.ReadAllText("C:\\Users\\h1n4m\\OneDrive\\Desktop\\App\\ProjectC#\\FE\\blacklist.json");
        JObject jsonObject = JObject.Parse(blacklistJson);
        JArray blacklistArray = (JArray)jsonObject["included_phrases"];

        // Convert JArray thành List<string>
        List<string> blacklist = blacklistArray.ToObject<List<string>>();

        string responseData = string.Empty;
        List<string> foundWords = new List<string>();

        try
        {
            using (var connection = new SqlConnection("SERVER=h1n4m\\MSSQLSERVER01;DATABASE=NewsClassifier;UID=h1n4m;PWD=h1n4m;"))
            {
                await connection.OpenAsync();
                var commandText = "SELECT COUNT(*) FROM NewsDetected WHERE Url = @Url";
                using (var command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Url", url);
                    var result = await command.ExecuteScalarAsync();
                    int count = Convert.ToInt32(result);
                    
                    if (count > 0)
                    {
                        var newsDetected = _db.NewsDetecteds.FirstOrDefault(response => response.Url == url);
                        if (newsDetected != null)
                        {
                            responseData = newsDetected.ResponseData;
                            foundWords = JsonConvert.DeserializeObject<List<string>>(newsDetected.NegativeWords);
                            ViewBag.ResponseData = responseData;
                            ViewBag.FoundWords = foundWords;
                            Console.WriteLine(newsDetected.NegativeWords);
                        }
                        var log = new ClassificationLog
                        {
                            UserId = HttpContext.Session.GetInt32("idUser") ?? 0,
                            Time = DateTime.Now,
                            Url = url,
                            ResponseData = responseData,
                            NegativeWords = JsonConvert.SerializeObject(foundWords)
                        };
                        _db.ClassificationLogs.Add(log);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        // Tạo đối tượng chứa dữ liệu JSON
                        var jsonContent = new StringContent("{\"url\": \"" + url + "\"}", Encoding.UTF8, "application/json");
                        
                        // Gửi yêu cầu POST đến API
                        using (HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, jsonContent))
                        {
                            // Kiểm tra xem phản hồi có thành công không
                            if (response.IsSuccessStatusCode)
                            {
                                // Đọc nội dung của phản hồi
                                responseData = await response.Content.ReadAsStringAsync();
                                // Chuyển đổi chuỗi JSON thành chuỗi JSON định dạng
                                string formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseData), Formatting.Indented);
                                JObject responseDataObject = JObject.Parse(responseData);
                                string content = responseDataObject["data"]["content"].ToString();

                                foreach (string word in blacklist)
                                {
                                    string pattern = "\\b" + Regex.Escape(word) + "\\b";

                                    // Tìm kiếm các từ trong nội dung sử dụng biểu thức chính quy
                                    MatchCollection matches = Regex.Matches(content, pattern, RegexOptions.IgnoreCase);

                                    // Kiểm tra xem có từ nào được tìm thấy hay không
                                    if (matches.Count > 0)
                                    {
                                        foundWords.Add(word);
                                    }
                                }

                                var news = new NewsDetected
                                {
                                    Url = url,
                                    ResponseData = responseData,
                                    NegativeWords = JsonConvert.SerializeObject(foundWords)
                                };
                                _db.NewsDetecteds.Add(news);
                                var log = new ClassificationLog
                                {
                                    UserId = HttpContext.Session.GetInt32("idUser") ?? 0, // Lấy ID của người dùng từ Session
                                    Time = DateTime.Now,
                                    Url = url,
                                    ResponseData = responseData,
                                    NegativeWords = JsonConvert.SerializeObject(foundWords)
                                };
                                _db.ClassificationLogs.Add(log);
                                await _db.SaveChangesAsync();

                                // Hiển thị dữ liệu phản hồi dưới dạng JSON trong ViewBag
                                ViewBag.ResponseData = formattedJson;
                                ViewBag.FoundWords = foundWords;
                                Console.WriteLine(foundWords);
                            }
                            else
                            {
                                // Xử lý trường hợp phản hồi không thành công
                                TempData["error"] = "Fail to detect";
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            TempData["error"] = ex.Message;
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
    [HttpGet("/User/Train")]
    public ActionResult Train()
    {
        return View();
    }
    [HttpPost("/User/Train")]
    public async Task<IActionResult> Train(IFormFile trainData, string modelName, float alpha, bool fitPrior)
    {
        int userId = HttpContext.Session.GetInt32("idUser") ?? 0;
        Console.WriteLine(userId);
        if (trainData == null || string.IsNullOrEmpty(modelName))
        {
            ViewBag.Error = "All fields are required.";
            return View();
        }

        using (var memoryStream = new MemoryStream())
        {
            await trainData.CopyToAsync(memoryStream);
            var form = new MultipartFormDataContent();
            form.Add(new StringContent(alpha.ToString()), "alpha");
            form.Add(new StringContent(fitPrior.ToString().ToLower()), "fitPrior");
            form.Add(new StringContent(modelName), "modelName");
            form.Add(new ByteArrayContent(memoryStream.ToArray()), "trainData", trainData.FileName);
            form.Add(new StringContent(userId.ToString()), "userId");
            var response = await _httpClient.PostAsync("http://localhost:5000/train-model", form);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                ViewBag.ResponseData = responseData;
            }
            else
            {
                ViewBag.Error = "Error occurred while training the model.";
            }
        }

        return View();
    }
    [HttpGet("/User/Predict")]
    public async Task<IActionResult> Predict()
    {
        int userId = HttpContext.Session.GetInt32("idUser") ?? 0;

        var resmodel = await _httpClient.GetAsync("http://localhost:5000/models?user_id=" + userId);
        if (resmodel.IsSuccessStatusCode)
        {
            var models = await resmodel.Content.ReadAsStringAsync();
            string formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(models), Formatting.Indented);
            ViewBag.Models = formattedJson;
        }
        else
        {
            ViewBag.Error = "Error occurred while fetching models.";
        }

        return View();
    }

    [HttpPost("/User/Predict")]
    public async Task<IActionResult> Predict(string modelName, string text)
    {
        int userId = HttpContext.Session.GetInt32("idUser") ?? 0;
        if (string.IsNullOrEmpty(modelName) || string.IsNullOrEmpty(text))
        {
            ViewBag.Error = "All fields are required.";
            return View();
        }

        var postData = new { model_name = modelName, text = text , user_id = userId };
        var jsonContent = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:5000/predict", jsonContent);
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync();
            ViewBag.ResponseData = responseData;
        }
        else
        {
            ViewBag.Error = "Error occurred while predicting.";
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