using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest_RoleBased.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles ="ADMIN,MANAGER")]
        public IActionResult ProductList()
        {
            string[] prList = { "Mouse", "Keyboard", "Monitor" };
            ViewBag.pr = prList;
            return View();
        }
    }
}
