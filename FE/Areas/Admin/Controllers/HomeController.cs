using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace FE.Areas.Admin.Controllers;
using FE.Models;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Drawing.Printing;
using System.Data.Entity.Validation;
[Area("Admin")]
public class HomeController : Controller
{
    private readonly HttpClient _httpClient;
    [HttpGet("/Admin/Index")]
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
        [HttpGet("/Admin/Classify")]
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

                // Hiển thị dữ liệu phản hồi lên màn hình
                ViewBag.ResponseData = responseData;

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
    public IActionResult Contact()
    {
        return View();
    }
    public IActionResult Error()
    {
        return View();
    }
}