using System;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiceBusDriver.Core;
using ServiceBusDriver.Core.Components.Instance;
using ServiceBusDriver.Db;
using ServiceBusDriver.Server.Middlewares;
using ServiceBusDriver.Server.PipelineBehavior;
using ServiceBusDriver.Server.Services;
using ServiceBusDriver.Server.Services.AuthContext;
using ServiceBusDriver.Server.Services.Email;
using ServiceBusDriver.Server.Services.Email.Settings;
using ServiceBusDriver.Server.Services.FirebaseAuth;
using ServiceBusDriver.Server.Services.HttpClient;
using ServiceBusDriver.Server.Services.Password;
using ServiceBusDriver.Server.Services.Validations;
using ServiceBusDriver.Server.Settings;
using ServiceBusDriver.Shared.Features.User.Validators;

namespace ServiceBusDriver.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddAzureAppConfiguration();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var firebaseAuthProjectId = Configuration.GetSection("FirebaseAuth").GetSection("ProjectName").Value;
                    var issuer = "https://securetoken.google.com/" + firebaseAuthProjectId;
                    var authority = "https://securetoken.google.com/" + firebaseAuthProjectId;

                    options.Authority = authority;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = firebaseAuthProjectId,
                        ValidateLifetime = true
                    };
                });

            services.AddHttpContextAccessor();
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddRazorPages();
        

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SbDriver API", Version = "v1"
                });
                c.CustomSchemaIds(x => x.FullName);
            });
            
            services.AddSingleton<ISettings, Settings.Settings>();
            services.AddSingleton<IFirebaseAuthSettings, FirebaseAuthSettings>();

            // Add Firestore DB
            services.AddFirestoreDb(Configuration);
            services.AddSingleton<IFirebaseAuthManager, FirebaseAuthManager>();

            // Add mediator and validation to pipeline
            services.AddMediatR(typeof(Startup));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(typeof(LoginRequestDtoValidator).Assembly);

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //Inject Library Dependency
            services.AddServiceBusDriverCore();
            services.AddScoped<ISendInBlueSettings, SendInBlueSettings>();
            services.AddSingleton<IHttpClientHelper, HttpClientHelper>();
            services.AddScoped<IEmailSenderService, EmailSenderService>();

            services.AddHttpClient(HttpClientConstants.Clients.SendInBlueClient,
                                   c => { c.BaseAddress = new Uri(HttpClientConstants.SendInBlue.BaseUrl); });
            
            services.AddHttpClient(nameof(HttpClientConstants.Clients.DisposableEmailClient), c =>
            {
                c.BaseAddress = new Uri(HttpClientConstants.DisposableEmail.BaseUrl);
                c.Timeout = TimeSpan.FromMilliseconds(2000);
            });
            
            services.AddScoped<IInstanceService, CustomDbInstanceService>();
            services.AddScoped<IAesEncryptService, AesEncryptService>();
            services.AddScoped<IClaimsManager, ClaimsManager>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IDbFetchHelper, DbFetchHelper>();
            services.AddSingleton<IDisposableEmailChecker, DisposableEmailChecker>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAzureAppConfiguration();

            app.UseMiddleware<GlobalExceptionMiddleware>();
            
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ServiceBus Driver API");
                c.RoutePrefix = "sbDriver/swagger";
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}