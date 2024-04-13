namespace FE.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
[Area("Admin")]
public class HomeController : BaseController
{
    private readonly HttpClient _httpClient;
    public HomeController(){
         _httpClient = new HttpClient();
    }
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
    public ActionResult Classify()
    {
        return View();
    }
    [HttpPost("/Admin/Classify")]
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
    public IActionResult Contact()
    {
        return View();
    }
    public IActionResult Error()
    {
        return View();
    }
}