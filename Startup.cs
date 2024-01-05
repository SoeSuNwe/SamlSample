namespace SamlSample
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.AzureAD.UI;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Identity.Web;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Other service configurations...
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<SwaggerIgnoreFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Microsoft OpenAI ", Version = "v1" });
            }); 

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // Ensure OpenIdConnectDefaults.AuthenticationScheme is set as the default challenge scheme
            })
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                var tenantId = Configuration["AzureAd:TenantId"];
                var instance = Configuration["AzureAd:Instance"];

                options.Authority = $"{instance}{tenantId}";
                options.ClientId = Configuration["AzureAd:ClientId"];
                options.CallbackPath = "/signin-oidc"; // Modify as needed
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Other app configurations...

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name V1");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        public class SwaggerIgnoreFilter : IDocumentFilter
        {
            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                var ignoredRoutes = swaggerDoc.Paths
                    .Where(p => p.Key.Contains("/AzureAD/Account/SignIn") || p.Key.Contains("/AzureAD/Account/SignOut/{scheme}"))
                    .ToList();

                foreach (var ignoredRoute in ignoredRoutes)
                {
                    swaggerDoc.Paths.Remove(ignoredRoute.Key);
                }
            }
        }
    }
}
