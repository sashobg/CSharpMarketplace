using Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;

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
        public ActionResult List(string sortBy , int? categoryId, int? townId, string userId, string currentFilter , string search, int? page)
        {
            using (var database = new MarketplaceDbContext())
            {

                ViewBag.CurrentSort = sortBy;

                

                if (search != null)
                {
                    page = 1;
                }
                else
                {
                    search = currentFilter;
                }

                ViewBag.CurrentFilter = search;

                bool isAdmin = this.User.IsInRole("Admin");
                var ads = database.Ads
                       .OrderByDescending(a => a.DateCreated)                       
                       .Include(a => a.Author)
                       .Include(a => a.Town)
                       .Include(a => a.Category)                      
                       .ToList();

                if (!isAdmin)
                {
                    ads = ads.Where(a => a.Approved == 1)
                       .ToList();                    
                }

                if (!String.IsNullOrEmpty(search))
                {
                    ads = ads.Where(s => s.Title.Contains(search) || s.Content.Contains(search))
                       .ToList();
                }

                if (categoryId != null)
                {
                    ViewBag.categoryId = categoryId;
                   ads = ads.Where(a => a.Category.Id == categoryId).ToList();
                }

                if (townId != null)
                {
                    ViewBag.townId = townId;
                    var town = database.Towns
                       .Where(a => a.Id == townId)
                       .First();
                    ViewBag.townName = town.Name;
                    ads = ads.Where(a => a.Town.Id == townId).ToList();
                }

                if (userId != null)
                {
                    ViewBag.userId = userId;
                    var user = database.Users
                        .Where(a => a.Id == userId)                       
                        .First();
                    ViewBag.userName = user.FullName;
                    ads = ads.Where(a => a.Author.Id == userId).ToList();
                }

                switch (sortBy)
                {
                    case "price":
                        ads = ads.OrderBy(s => s.Price).ToList();
                        break;
                    case "priceDesc":
                        ads = ads.OrderByDescending(s => s.Price).ToList();
                        break;
                    default:
                        ads = ads.OrderByDescending(a => a.DateCreated).ToList();
                        break;
                }

                ViewBag.categories = database.Categories
                   .Include(c => c.Ads)                   
                   .OrderBy(c => c.Name)
                   .ToList();

                ViewBag.images = database.Images                                
                   .ToList();


                ViewBag.towns = database.Towns
                   .Include(c => c.Ads)                   
                   .OrderBy(c => c.Name)
                   .ToList();
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                return View(ads.ToPagedList(pageNumber, pageSize));

            }

        }



        // GET: Ad/Details/5
        public ActionResult Details(string id)
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
                    .Include(a => a.Images)
                    .Include(a => a.Author)
                    .Include(a => a.Town)
                    .Include(a => a.Category)
                    .First();

                if (ad == null)
                {
                    return HttpNotFound();
                }

                if (ad.Approved == 1)
                {
                    ad.ViewCount = ad.ViewCount + 1;
                    database.SaveChanges();
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
        public ActionResult Create(AdViewModel model, HttpPostedFileBase primaryImage, IEnumerable<HttpPostedFileBase> uploadImages)
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

                    DateTime DateCreated = DateTime.Now;
                    int viewCount = 0;
                    string adId = Guid.NewGuid().ToString();
                    var ad = new Ad(adId,0, authorId, model.Title, model.Content, model.Price, model.CategoryId, model.TownId, viewCount, DateCreated, null);

                    //Save Ad in DB
                    database.Ads.Add(ad);
                    database.SaveChanges();

                   


                    //Upload images
                    var validImageTypes = new string[]
                       {
                            "image/gif",
                            "image/jpeg",
                            "image/pjpeg",
                            "image/png"
                       };

                    //upload Primary image


                    if (validImageTypes.Contains(primaryImage.ContentType) &&
                    primaryImage != null && primaryImage.ContentLength > 0)
                    {
                        var id = Guid.NewGuid().ToString();
                        var fileName = id + Path.GetExtension(primaryImage.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/UploadedImages"), fileName);
                        primaryImage.SaveAs(path);

                        var image = new Image(id, fileName, true, adId);
                        database.Images.Add(image);
                        database.SaveChanges();

                        ad.primaryImageName = fileName;
                        database.Entry(ad).State = EntityState.Modified;
                        database.SaveChanges();


                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }

                    //upload other images 

                    if (uploadImages != null)
                        foreach (var otherImage in uploadImages)
                        {
                           
                                if (otherImage != null && otherImage.ContentLength > 0)
                                {
                                    var id = Guid.NewGuid().ToString();
                                    var fileName = id + Path.GetExtension(otherImage.FileName);
                                    var path = Path.Combine(Server.MapPath("~/Content/UploadedImages"), fileName);
                                    otherImage.SaveAs(path);

                                    var image = new Image(id, fileName, false, adId);
                                    database.Images.Add(image);
                                    database.SaveChanges();

                                }
                        }

                  

                }

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET: Ad/Edit/5
        public ActionResult Edit(string id)
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
                
                
                model.Approved = ad.Approved;
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

                    bool isAdmin = this.User.IsInRole("Admin");
                    if (isAdmin)
                    {
                        ad.Approved = model.Approved;
                    }

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
        public ActionResult Delete(string id)
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
        public ActionResult DeleteConfirmed(string id)
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
