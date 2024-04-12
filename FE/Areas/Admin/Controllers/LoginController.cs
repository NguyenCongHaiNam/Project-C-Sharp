using System;
using System.Collections.Generic;
using System.Linq;
using FE.Areas.Admin.Models;
using FE.Models;
using Microsoft.AspNetCore.Mvc;

namespace FE.Areas.Admin.Controllers
{
    public class LoginCotroller : Controller
    {
        //GET: /Admin/Login
        public ActionResult Index(){
            return View();
        }
        public ActionResult Login(LoginModel model){
            if (ModelState.IsValid)
            {
                var dao = new Users
            }
        }
    }
}