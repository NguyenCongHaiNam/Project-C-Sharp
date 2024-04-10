using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public class HomeController : Controller
{
    

    public HomeController()
    {
        
    }

    public ActionResult Index()
    {
        return View();
    }   

    public ActionResult About() 
    {
        return View();
    }
}
