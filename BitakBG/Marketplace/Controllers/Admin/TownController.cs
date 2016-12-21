using Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marketplace.Controllers.Admin
{
    public class TownController : Controller
    {
        // GET: Town
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        // GET: Town/List
        public ActionResult List()
        {
            using (var database = new MarketplaceDbContext())
            {
                var towns = database.Towns
                    .ToList();
                return View(towns);
            }
        }

       

        // GET: Town/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Town/Create
        [HttpPost]
        public ActionResult Create(Town town)
        {
            if (ModelState.IsValid)
            {
                using (var database = new MarketplaceDbContext())
                {
                    database.Towns.Add(town);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(town);
        }

        // GET: Town/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                var town = database.Towns
                    .FirstOrDefault(c => c.Id == id);

                if (town == null)
                {
                    return HttpNotFound();
                }

                return View(town);
            }
        }

        // POST: Town/Edit/5
        [HttpPost]
        public ActionResult Edit(Town town)
        {
            if (ModelState.IsValid)
            {
                using (var database = new MarketplaceDbContext())
                {
                    database.Entry(town).State = System.Data.Entity.EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(town);
        }

        // GET: Town/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                var town = database.Towns
                    .FirstOrDefault(c => c.Id == id);

                if (town == null)
                {
                    return HttpNotFound();
                }

                return View(town);
            }
        }

        // POST: Town/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var database = new MarketplaceDbContext())
            {
                var town = database.Towns
                    .FirstOrDefault(c => c.Id == id);

                var townsAds = town.Ads
                    .ToList();

                foreach (var ad in townsAds)
                {
                   
                    string fullPathPrimary = Request.MapPath("~/Content/UploadedImages/" + ad.primaryImageName);
                    if (System.IO.File.Exists(fullPathPrimary))
                    {
                        System.IO.File.Delete(fullPathPrimary);
                    }


                    var images = database.Images
                        .Where(a => a.AdId == ad.Id)
                        .ToList();



                    foreach (var image in images)
                    {
                        string fullPath = Request.MapPath("~/Content/UploadedImages/" + image.FileName);
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        database.Images.Remove(image);
                        database.SaveChanges();
                    }

                    //Delete comments 

                    var comments = database.Comments
                       .Where(a => a.AdId == ad.Id)
                       .ToList();

                    foreach (var comment in comments)
                    {

                        database.Comments.Remove(comment);
                        database.SaveChanges();
                    }

                    // Delete Ad from database
                    database.Ads.Remove(ad);
                    database.SaveChanges();
                }
                database.Towns.Remove(town);
                database.SaveChanges();
                TempData["Success"] = "Градът е премахнат успешно.";
                return RedirectToAction("Index");
            }
        }
    }
}
