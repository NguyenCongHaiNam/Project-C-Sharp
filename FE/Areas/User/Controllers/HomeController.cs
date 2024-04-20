using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace FE.Areas.User.Controllers;

using System.Text;
using Newtonsoft.Json;

[Area("User")]
public class HomeController : BaseController
{
     private readonly HttpClient _httpClient;
    public HomeController(){
         _httpClient = new HttpClient();
    }
    [HttpGet("/User/Index")]
    public ActionResult Index()
    {
        if (HttpContext.Session.GetInt32("idUser") != null)
        {
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
        string apiUrl = "http://127.0.0.1:5000/classify";

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

                // Hiển thị dữ liệu phản hồi dưới dạng JSON trong ViewBag
                ViewBag.ResponseData = formattedJson;
            }
            else
            {
                // Xử lý trường hợp phản hồi không thành công
                ViewBag.ResponseData = "Error: " + response.StatusCode;
            }
        }
        catch (Exception ex)
        {
            // Xử lý lỗi nếu có
            ViewBag.ResponseData = "Error: " + ex.Message;
        }

        return View();
    }
    [HttpGet("/User/Logout")]
    public ActionResult Logout()
    {
        HttpContext.Session.Clear();
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
    public IActionResult Error()
    {
        return View();
    }
}