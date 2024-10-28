using MetroVoip.Business.Interfaces;
using MetroVoip.Business.Services;
using MetroVoip.Presentation.Hubs;

namespace MetroVoip.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSingleton<IDriverCommunicationService, DriverCommunicationService>();
            //builder.Services.AddSingleton<IPassengerCommunicationService, PassengerCommunicationService>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<ICommendIntercomService, CommendIntercomService>();
            builder.Services.AddCors(builder =>
            {
                builder.AddPolicy("AllowAll", options =>
                {
                    options.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            }); 
            builder.Services.AddSignalR();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<VoiceChatHub>("/voiceChatHub");
            app.UseCors("AllowAll");
            app.Run();
        }
    }
}
