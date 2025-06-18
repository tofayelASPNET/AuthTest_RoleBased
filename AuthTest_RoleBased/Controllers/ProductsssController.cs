using AuthTest_RoleBased.Data;
using AuthTest_RoleBased.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthTest_RoleBased.Controllers
{
    public class ProductsssController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsssController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string userText, string sortOrder, int page = 1)
        {
            ViewBag.sWord = userText;
            ViewBag.sortParam = string.IsNullOrEmpty(sortOrder) ? "desc_name" : "";
            ViewBag.sortSalary = sortOrder == "sal_asc" ? "sal_desc" : "sal_asc";
            ViewBag.currentSort = sortOrder; // Add this line

            IQueryable<Product> products = _context.Products;

            if (!string.IsNullOrEmpty(userText))
            {
                string lowerUserText = userText.ToLower();
                products = products.Where(p =>
                    p.Name.ToLower().Contains(lowerUserText) ||
                    p.Unit.ToLower().Contains(lowerUserText));
            }

            switch (sortOrder)
            {
                case "desc_name":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                case "sal_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "sal_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.Name);
                    break;
            }

            int pageSize = 5;
            if (page <= 0) page = 1;

            var paginatedList = await PaginatedList<Product>.CreateAsync(products, page, pageSize);

            ViewBag.cp = paginatedList.PageIndex;
            ViewBag.tp = paginatedList.TotalPages;
            ViewBag.count = await products.CountAsync();
            ViewBag.pSize = pageSize;

            return View(paginatedList);
        }



        // GET: Productsss/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Productsss/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productsss/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Save the image
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.Photo = "/Images/" + uniqueFileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);

            
        }

        // GET: Productsss/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Productsss/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Upload new image
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // Delete old image
                        if (!string.IsNullOrEmpty(existingProduct.Photo))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingProduct.Photo.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        product.Photo = "/Images/" + uniqueFileName;
                    }
                    else
                    {
                        // Keep the existing image
                        product.Photo = existingProduct.Photo;
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // POST: Productsss/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
