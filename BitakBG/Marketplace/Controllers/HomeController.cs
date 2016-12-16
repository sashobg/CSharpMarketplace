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

        public ActionResult ListAds(int? categoryId)
        {
            if (categoryId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                var ads = database.Ads
                    .Where(a => a.Approved == 1)
                    .Where(a => a.CategoryId == categoryId)
                    .Include(a => a.Author)
                     .Include(a => a.Town)
                   .Include(a => a.Category)
                    .ToList();

                return View(ads);
            }
        }

        public ActionResult ListAdsByTown(int? townId)
        {
            if (townId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                var ads = database.Ads
                    .Where(a => a.Approved == 1)
                    .Where(a => a.TownId == townId)
                    .Include(a => a.Author)
                     .Include(a => a.Town)
                   .Include(a => a.Category)
                    .ToList();

                return View(ads);
            }
        }

       

        public ActionResult ListByUser(string userId)
        {
            if (userId == "")   
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                var ads = database.Ads
                    .Where(a => a.Approved == 1)
                    .Where(a => a.Author.Id == userId)                   
                    .Include(a => a.Town)
                    .Include(a => a.Category)
                    .ToList();

                return View(ads);
            }
        }



    }
}