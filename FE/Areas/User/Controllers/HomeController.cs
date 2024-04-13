using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
namespace FE.Areas.User.Controllers;
[Area("User")]
public class HomeController : Controller
{
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
    public IActionResult About()
    {
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