using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TinyStore.Data;
using TinyStore.Models;
using TinyStore.Models.ViewModels;
using TinyStore.Utility;

namespace TinyStore.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _context.Category.ToListAsync(),
                Coupon = await _context.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                var cnt = _context.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }

            return View(IndexVM);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory)
                                        .Where(m => m.Id == id).FirstOrDefaultAsync();

            ShoppingCart cartObj = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id
            };

            return View(cartObj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cart)
        {
            cart.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                cart.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = await _context.ShoppingCart
                    .Where(c => c.ApplicationUserId == cart.ApplicationUserId && c.MenuItemId == cart.MenuItemId)
                    .FirstOrDefaultAsync();

                if(cartFromDb == null)
                {
                    await _context.ShoppingCart.AddAsync(cart);
                }
                else
                {
                    cartFromDb.Count = cartFromDb.Count + cart.Count;
                }
                await _context.SaveChangesAsync();

                var count = _context.ShoppingCart.Where(c => c.ApplicationUserId == cart.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction("Index");
            }
            else
            {
                var menuItemFromDb = await _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory)
                                        .Where(m => m.Id == cart.MenuItemId).FirstOrDefaultAsync();

                ShoppingCart cartObj = new ShoppingCart()
                {
                    MenuItem = menuItemFromDb,
                    MenuItemId = menuItemFromDb.Id
                };

                return View(cart);
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
