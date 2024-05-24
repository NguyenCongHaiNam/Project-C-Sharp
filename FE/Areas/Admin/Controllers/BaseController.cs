using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;


public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Session.GetInt32("idUser") == null)
        {
            context.Result = Redirect("/Home/Login");
        }
        base.OnActionExecuting(context);
    }
}
