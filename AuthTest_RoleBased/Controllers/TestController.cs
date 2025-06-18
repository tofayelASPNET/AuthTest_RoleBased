using Microsoft.AspNetCore.Mvc;

namespace AuthTest_RoleBased.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            var s = HttpContext.Session.GetString("r62");
            ViewBag.msg = s;
            return View();
        }
    }
}
