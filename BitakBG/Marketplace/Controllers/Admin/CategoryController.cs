using Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marketplace.Controllers.Admin
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }
        // GET: Category/List
        public ActionResult List()
        {
            using (var database = new MarketplaceDbContext())
            {
                var categories = database.Categories
                    .ToList();
                return View(categories);
            }
        }

        // GET: Category/Create
        public ActionResult create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var database = new MarketplaceDbContext())
                {
                    database.Categories.Add(category);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(category);
        }

        // GET: Category/edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            using (var database = new MarketplaceDbContext())
            {
                var category = database.Categories
                    .FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        // GET: Category/Edit
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                using (var database = new MarketplaceDbContext())
                {
                    database.Entry(category).State = System.Data.Entity.EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(category);
        }

        // GET: Category/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                var category = database.Categories
                    .FirstOrDefault(c => c.Id == id);

                if (category == null)
                {
                    return HttpNotFound();
                }

                return View(category);
            }
        }

        // POST: Category/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            using (var database = new MarketplaceDbContext())
            {
                var category = database.Categories
                    .FirstOrDefault(c => c.Id == id);

                var categoryAds = category.Ads
                    .ToList();

                foreach (var ad in categoryAds)
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

                database.Categories.Remove(category);
                database.SaveChanges();
                TempData["Success"] = "Категорията е премахната успешно.";
                return RedirectToAction("Index");

            }
             
                
            }
        }
}
