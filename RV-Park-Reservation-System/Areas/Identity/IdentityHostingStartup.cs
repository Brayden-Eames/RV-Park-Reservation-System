using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(RV_Park_Reservation_System.Areas.Identity.IdentityHostingStartup))]
namespace RV_Park_Reservation_System.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}