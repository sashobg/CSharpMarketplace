using Marketplace.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marketplace.Controllers
{
    public class CommentController : Controller
    {
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Comment model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new MarketplaceDbContext())
                {
                   
                    DateTime DateCreated = DateTime.Now;                  
                    
                    var comment = new Comment(model.Name, model.Content, model.Stars, model.AdId, DateCreated);

                    //Save Ad in DB
                    database.Comments.Add(comment);
                    database.SaveChanges();
                    

                }
                TempData["Success"] = "Успешно добавихте коментар.";
                return RedirectToAction("Details", "Ad", new { id = model.AdId });
            }
            TempData["Danger"] = "Некоректни данни, моля опитайте отново.";
            return RedirectToAction("Details", "Ad", new { id = model.AdId });
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            bool isAdmin = this.User.IsInRole("Admin");
            if (!isAdmin)
            {
                TempData["Danger"] = "Нямате необходимите права.";
                return RedirectToAction("List", "Ad");
            }
            

            using (var database = new MarketplaceDbContext())
            {
                var comment = database.Comments
                    .FirstOrDefault(c => c.Id == id);
                var path = comment.AdId;
                if (comment == null)
                {
                    return HttpNotFound();
                }

                database.Comments.Remove(comment);
                database.SaveChanges();

                TempData["Success"] = "Успешно изтрихте коментара.";
                return RedirectToAction("Details", "Ad", new { id = path });
            }
        }
    }
}