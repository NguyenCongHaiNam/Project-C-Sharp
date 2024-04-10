using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    private readonly HttpClient _httpClient;

    public HomeController()
    {
        _httpClient = new HttpClient();
    }

    public ActionResult Index()
    {
        return View();
    }   

    public ActionResult About() 
    {
        return View();
    }

    public ActionResult Login() 
    {
        return View();
    }

    public ActionResult Register() 
    {
        return View();
    }

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

    return View("Classify");
}
}
