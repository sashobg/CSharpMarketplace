using Marketplace.Migrations;
using Marketplace.Models;
using Microsoft.Owin;
using Owin;
using System.Data.Entity;

[assembly: OwinStartupAttribute(typeof(Marketplace.Startup))]
namespace Marketplace
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<MarketplaceDbContext, Configuration>());
            ConfigureAuth(app);
        }
    }
}
