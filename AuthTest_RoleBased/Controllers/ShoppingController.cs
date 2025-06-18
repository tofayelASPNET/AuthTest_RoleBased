using AuthTest_RoleBased.Data;
using AuthTest_RoleBased.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuthTest_RoleBased.Controllers
{
    public class ShoppingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ShoppingController(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        public IActionResult Index()
        {
            ViewBag.msg = TempData["msg"];
            return View(_context.Products.ToList());
        }
        public IActionResult AddToCart(int pId, double qty)
        {
            bool itemFound = false;
            string msg = "";
            if (qty > 0 && pId > 0)
            {
                var prod = _context.Products.FirstOrDefault(p => p.Id == pId);
                if (prod != null)
                {
                    List<Product> items = new List<Product>();
                    var xItems = HttpContext.Session.GetObject<List<Product>>("cart");
                    if (xItems != null)
                    {
                        foreach (var item in xItems)
                        {
                            if (pId == item.Id)
                            {
                                item.Quantity += qty;
                                itemFound = true;
                                msg = "Item already added, new qty is added!!";
                            }
                            items.Add(item);
                        }
                        if (!itemFound)
                        {
                            prod.Quantity = qty;
                            items.Add(prod);
                            msg = "New item is added with existing items!!!!";
                        }
                        HttpContext.Session.SetObject<List<Product>>("cart", items);
                    }
                    else
                    {
                        prod.Quantity = qty;
                        items.Add(prod);
                        HttpContext.Session.SetObject<List<Product>>("cart", items);
                        msg = "New item is added to empty cart!!";
                    }

                }
                else
                {
                    msg = "No data found!!";
                }
            }
            else
            {
                msg = "Item id or qty could not be zero!!";
            }
            TempData["msg"] = msg;
            return RedirectToAction("Index");
        }

        public IActionResult ShowCart()
        {
            List<Product> items = HttpContext.Session.GetObject<List<Product>>("cart");
            if (items != null && items.Count != 0)
            {
                return View(items.ToList());
            }
            else
            {
                items = new List<Product>();
                return View();
            }
        }
        public IActionResult RemoveFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<Product> productList = HttpContext.Session.GetObject<List<Product>>("cart");
            var removeItem = productList.FirstOrDefault(x => x.Id == id);
            productList.Remove(removeItem);
            HttpContext.Session.SetObject("cart", productList);
            return RedirectToAction(nameof(ShowCart));
        }
        public IActionResult CheckOut()
        {
            var us = HttpContext.Session.GetString("un");
            var id = HttpContext.Session.GetString("id");
            if (us != null)
            {
                return RedirectToAction(nameof(Login));
            }
            else
            {
                return View(nameof(ConfirmOrder));
            }
        }
        public IActionResult ConfirmOrder()
        {
            return View();
        }
        public IActionResult Login(AppUser appUser)
        {
            var userName = _context.AppUsers.FirstOrDefault(x => x.UserName == appUser.UserName);
            var password = _context.AppUsers.FirstOrDefault(x => x.Password == appUser.Password);

            if (userName != null && password != null)
            {
                HttpContext.Session.SetString("un", appUser.UserName);
                HttpContext.Session.SetString("id", appUser.Password);

                return RedirectToAction(nameof(ConfirmOrder));
            }
            else
            {
                TempData["wrongInfo"] = "Wrong Information!!!";
                return View(appUser);
            }
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("UserName,Password")] AppUser appUser)
        {
            string msg = "";
            if (ModelState.IsValid)
            {
                appUser.Role = 1;
                appUser.IsActive = true;

                var checkUserName = _context.AppUsers.FirstOrDefault(x => x.UserName == appUser.UserName.ToLower());
                if (checkUserName != null)
                {
                    TempData["signUp"] = "Username already exists!!";
                    return View(appUser);
                }
                _context.AppUsers.Add(appUser);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("un", appUser.UserName);
                HttpContext.Session.SetString("id", appUser.Password);

                appUser.UserName = null;
                appUser.Password = null;
                TempData["msg"] = "Successfully create a account!!";
                return RedirectToAction(nameof(Login));
            }
            else
            {
                msg = "Invalid Username or password!!";
                return View(appUser);
            }
        }
    }
}
