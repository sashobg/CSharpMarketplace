using Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;


namespace Marketplace.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var database = new MarketplaceDbContext())
            {
                var ads = database.Ads
                   .Where(a => a.Approved == 1)
                   .OrderByDescending(x => x.Id)
                   .Take(6)                   
                   .Include(a => a.Town)
                   .Include(a => a.Category)
                   .ToList();

               

                var categories = database.Categories
                   .Include(c => c.Ads)
                   .OrderBy(c => c.Name)
                   .ToList();
                
                var model = new AdCategoryModel { ads = ads, categories = categories };

                return View(model);
            }
        }

        public ActionResult ListCategories()
        {
            using (var database = new MarketplaceDbContext())
            {   
                var categories = database.Categories
                    .Include(c => c.Ads)
                    .OrderBy(c => c.Name)
                    .ToList();
                return View(categories); 
            }
        }

       

    }
}