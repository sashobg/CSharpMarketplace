using Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marketplace.Controllers
{
    public class AdController : Controller
    {
        // GET: Ad
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: Ad/List
        public ActionResult List()
        {
            using (var database = new MarketplaceDbContext())
            {
                bool isAdmin = this.User.IsInRole("Admin");
                if (isAdmin)
                {
                    var ads = database.Ads
                        .Include(a => a.Author)
                        .Include(a => a.Town)
                        .Include(a => a.Category)
                        .ToList();
                    return View(ads);
                }
                else
                {
                    var ads = database.Ads
                        .Where(a => a.Approved == 1)
                       .Include(a => a.Author)
                       .Include(a => a.Town)
                       .Include(a => a.Category)
                       .ToList();
                    return View(ads);
                }
               
            }

        }

        // GET: Ad/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                // Get the Ad from db
                var ad = database.Ads
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .Include(a => a.Town)
                    .Include(a => a.Category)
                    .First();

                if (ad == null)
                {
                    return HttpNotFound();
                }

                return View(ad);
            }
        }

        // GET: Ad/Create
        [Authorize]
        public ActionResult Create()
        {
            using (var database = new MarketplaceDbContext())
            {
                var model = new AdViewModel();
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();

                model.Towns = database.Towns
                    .OrderBy(c => c.Name)
                    .ToList();

                return View(model);
            }
        }

        // POST: Ad/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(AdViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new MarketplaceDbContext())
                {
                    // Get author Id
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    var ad = new Ad(0 ,authorId, model.Title, model.Content, model.Price, model.CategoryId, model.TownId);

                    //Save Ad in DB
                    database.Ads.Add(ad);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        // GET: Ad/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                // Get Ad from the database
                var ad = database.Ads
                    .Where(a => a.Id == id)
                    .First();

                // Check if Ad exists
                if (ad == null)
                {
                    return HttpNotFound();
                }

                // Create the view model
                var model = new AdViewModel();
                model.Id = ad.Id;
                model.Title = ad.Title;
                model.Content = ad.Content;
                model.Price = ad.Price;
                model.CategoryId = ad.CategoryId;
                model.Categories = database.Categories
                    .OrderBy(c => c.Name)
                    .ToList();
                model.TownId = ad.TownId;
                model.Towns = database.Towns
                    .OrderBy(c => c.Name)
                    .ToList();

                // Pass the view model to view
                return View(model);
            }
        }

        // POST: Ad/Edit/5
        [HttpPost]
        public ActionResult Edit(AdViewModel model)
        {
            // Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new MarketplaceDbContext())
                {
                    // Get Ad from database
                    var ad = database.Ads
                        .FirstOrDefault(a => a.Id == model.Id);

                    if (!IsUserAuthorizedToEdit(ad))
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                    }

                    // Set Ad properties
                    ad.Title = model.Title;
                    ad.Content = model.Content;
                    ad.Price = model.Price;
                    ad.CategoryId = model.CategoryId;
                    ad.TownId = model.TownId;

                    // Save Ad state in database
                    database.Entry(ad).State = EntityState.Modified;
                    database.SaveChanges();

                    // Redirect to the index page
                    return RedirectToAction("Index");
                }
            }

            // If model state is invalid, retyrn the same view

            return View(model);
        }


        
        // GET: Ad/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                // Get Ad from database
                var ad = database.Ads
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (!IsUserAuthorizedToEdit(ad))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                // Check if Ad exist
                if (ad == null)
                {
                    return HttpNotFound();
                }

                // Pass Ad to view
                return View(ad);
            }
        }

        // POST: Ad/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                // Get Ad from database
                var ad = database.Ads
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                // Check if Ad exist
                if (ad == null)
                {
                    return HttpNotFound();
                }

                // Delete Ad from database
                database.Ads.Remove(ad);
                database.SaveChanges();

                // Redirect to index page
                return RedirectToAction("Index");
            }
        }

        private bool IsUserAuthorizedToEdit(Ad ad)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = ad.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }

        

    }
}
