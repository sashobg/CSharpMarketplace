using Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marketplace.Controllers.Admin
{
    public class ApprovalController : Controller
    {
        // GET: Approval
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: Ad/List
        public ActionResult List()
        {
            using (var database = new MarketplaceDbContext())
            {

                var ads = database.Ads
                   .Include(a => a.Author)
                   .Include(a => a.Town)
                   .Include(a => a.Category)
                   .ToList();
                return View(ads);


            }

        }

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

        // POST: Approval/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        
    }
}
