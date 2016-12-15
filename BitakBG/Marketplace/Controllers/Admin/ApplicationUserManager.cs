using Marketplace.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Marketplace.Controllers.Admin
{
    internal class ApplicationUserManager<T>
    {
        private UserStore<ApplicationUser> userStore;

        public ApplicationUserManager(UserStore<ApplicationUser> userStore)
        {
            this.userStore = userStore;
        }
    }
}