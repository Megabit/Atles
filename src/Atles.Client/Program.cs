using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Atles.Client.Services;
using Atles.Data;
using Atles.Domain.Categories.Commands;
using Atles.Domain.Handlers.Categories.Commands;
using Atles.Domain.Validators.Categories;
using Atles.Models.Public;
using FluentValidation;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenCqrs.Extensions;
using Tewr.Blazor.FileReader;

namespace Atles.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("Atles.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Atles.ServerAPI"));

            builder.Services.AddHttpClient<AnonymousService>(client => 
            { 
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); 
            });

            builder.Services.AddHttpClient<AuthenticatedService>(client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            }).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped<ApiService>();

            builder.Services.AddApiAuthorization();

            builder.Services.AddOptions();

            builder.Services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("Admin", policy =>
                    policy.RequireRole("Admin"));
            });

            builder.Services.AddLocalization(options =>
            {
                options.ResourcesPath = "Resources";
            });

            builder.Services.AddFileReaderService(o => o.UseWasmSharedBuffer = true);

            builder.Services.AddDbContext<AtlesDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Atles;Trusted_Connection=True;MultipleActiveResultSets=true"));

            builder.Services.AddOpenCQRS(typeof(CreateCategoryHandler));

            builder.Services.Scan(s => s
                .FromAssembliesOf(typeof(CreateCategoryValidator))
                .AddClasses()
                .AsImplementedInterfaces());

            var host = builder.Build();

            var apiService = host.Services.GetRequiredService<ApiService>();
            var site = await apiService.GetFromJsonAsync<CurrentSiteModel>("api/public/current-site");
            var cultureName = site.Language ?? "en";
            var culture = new CultureInfo(cultureName);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            await host.RunAsync();
        }
    }
}
