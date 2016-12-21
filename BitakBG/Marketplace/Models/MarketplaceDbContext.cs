using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Marketplace.Models
{
    
    public class MarketplaceDbContext : IdentityDbContext<ApplicationUser>
    {
        public MarketplaceDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual IDbSet<Ad> Ads { get; set; }

        public virtual IDbSet<Category> Categories { get; set; }
        public virtual IDbSet<Image> Images { get; set; }
        public virtual IDbSet<Comment> Comments { get; set; }
        public virtual IDbSet<Town> Towns { get; set; }

        public static MarketplaceDbContext Create()
        {
            return new MarketplaceDbContext();
        }
    }
}