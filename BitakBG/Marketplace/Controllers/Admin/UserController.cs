using Marketplace.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marketplace.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        // GET: User/List
        public ActionResult List()
        {
            using (var database = new MarketplaceDbContext())
            {
                var users = database.Users
                    .ToList();

                var admins = GetAdminUserNames(users, database);
                ViewBag.admins = admins;

                return View(users);
            }
        }

        // GET: User/Edit
        public ActionResult Edit(string id)
        {
            // Validate Id 
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                // Get user from database
                var user = database.Users
                    .Where(u => u.Id == id)
                    .First();

                // Check if user exists
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Create a view model
                var viewModel = new EditUserViewModel();
                viewModel.User = user;
                viewModel.Roles = GetUserRoles(user, database);

                // Pass the model to the view
                return View(viewModel);
            }


        }

        // POST: User/Edit
        [HttpPost]
        public ActionResult Edit(string id, EditUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                using (var database = new MarketplaceDbContext())
                {
                    // Get user from database
                    var user = database.Users.FirstOrDefault(u => u.Id == id);

                    // Check if user axist
                    if (user == null)
                    {
                        return HttpNotFound();
                    }

                    // If password field is not empty, change password
                    if (!string.IsNullOrEmpty(viewModel.Password))
                    {
                        var hasher = new PasswordHasher();
                        var passwordHash = hasher.HashPassword(viewModel.Password);
                        user.PasswordHash = passwordHash;
                    }

                    // Set user properties
                    user.Email = viewModel.User.Email;
                    user.FullName = viewModel.User.FullName;
                    user.UserName = viewModel.User.Email;
                    this.SetUserRoles(viewModel, user, database);

                    // Save changes
                    database.Entry(user).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("List");
                }
            }
            return View(viewModel);
        }

        // GET: User/Delete
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new MarketplaceDbContext())
            {
                // Get user from database
                var user = database.Users
                    .Where(u => u.Id.Equals(id))
                    .First();

                // Check if user exist
                if (user == null)
                {
                    return HttpNotFound();
                }

                return View(user);
            }
        }

        // POST: User/Delete
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
                // Get user from database
                var user = database.Users
                    .Where(u => u.Id.Equals(id))
                    .First();

                // Get user Ads from database
                var userAds = database.Ads
                    .Where(a => a.Author.Id == user.Id)
                    .ToList();

                // Delete user Ads
                foreach (var ad in userAds)
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
                
                database.Users.Remove(user);
                database.SaveChanges();
                TempData["Success"] = "Потребителят е премахнат успешно.";
                return RedirectToAction("List");
            }
           
         }
       

        private void SetUserRoles(EditUserViewModel model, ApplicationUser user, MarketplaceDbContext db)
        {
            var userManager = Request
               .GetOwinContext()
               .GetUserManager<ApplicationUserManager>();

            foreach (var role in model.Roles)
            {
                if (role.IsSelected)
                {
                    userManager.AddToRole(user.Id, role.Name);
                }
                else if (!role.IsSelected)
                {
                    userManager.RemoveFromRole(user.Id, role.Name);
                }
            }
        }

        private List<Role> GetUserRoles(ApplicationUser user, MarketplaceDbContext db)
        {
            // Create user manager
            var userManager = Request
                .GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            // Get all applicatio roles
            var roles = db.Roles
                .Select(r => r.Name)
                .OrderBy(r => r)
                .ToList();

            // For each application role, check if the user has it
            var userRoles = new List<Role>();

            foreach (var roleName in roles)
            {
                var role = new Role { Name = roleName };

                if (userManager.IsInRole(user.Id, roleName))
                {
                    role.IsSelected = true;
                }

                userRoles.Add(role);
            }

            // Return a list with all roles
            return userRoles;
            
        }

        private HashSet<string> GetAdminUserNames(List<ApplicationUser> users, MarketplaceDbContext context)
        {
            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            var admins = new HashSet<string>();

            foreach (var user in users)
            {
                if (userManager.IsInRole(user.Id, "Admin"))
                {
                    admins.Add(user.UserName);
                }
            }

            return admins;
        }
    }
}