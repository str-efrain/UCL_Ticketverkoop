using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Domains.Data;
using Domains.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NuGet.Configuration;
using Repositories;
using Repositories.Interfaces;
using Services;
using Services.Interfaces;
using System.Text;
using UCL_Ticketverkoop.Data;
using Util;
using Util.Interfaces;

var builder = WebApplication.CreateBuilder(args);

//// Key Vault settings ophalen
//string vaultUrl = builder.Configuration["KeyVault:VaultUrl"] ?? throw new InvalidOperationException("KeyVault:VaultUrl is not configured.");
//string connectionStringDB = builder.Configuration["KeyVault:ConnectionStringDB"] ?? throw new InvalidOperationException("KeyVault:ConnectionStringDB is not configured.");
//string? emailPassword = builder.Configuration["KeyVault:EmailPassword"];

//// Split the instantiation into separate variables to prevent the Visual Studio analyzer crash
//var vaultUri = new Uri(vaultUrl);
//var credentials = new DefaultAzureCredential();

//// Maak een client met default credentials
//var client = new SecretClient(vaultUri, credentials);

////Haal de connectionstring op
//KeyVaultSecret connectionStringSecret = client.GetSecret(connectionStringDB);
////De slashes er uithalen
//string connectionStringVal = connectionStringSecret.Value.Replace("\\\\", "\\");
//// Voeg toe aan configuration
//builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionStringVal;

////Haal het emailpasswoord op
//KeyVaultSecret emailPasswordSecret = client.GetSecret(emailPassword);
//string emailPasswordVal = emailPasswordSecret.Value;
//builder.Configuration["EmailSettings:Password"] = emailPasswordVal;


////Connection string ophalen
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


//temp connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//Add UCL_TicketingDbContext
builder.Services.AddDbContext<UCL_TicketingDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//Add ApplicationDbContext voor Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
 options.UseSqlServer(connectionString));

//Default identity voor EF Core, hiermee kunnen we gebruikers beheren en authenticeren
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

//Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UEAFA Champions League API",
        Version = "version 1",
        Description = "An API to perform operations on UCL matches, stadiums and users",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Efrain M. & Thibeau B.",
            Email = "efrain.mouton@student.vives.be",
            Url = new Uri("https://vives.be"),
        },
        License = new OpenApiLicense
        {
            Name = "UCL API LICX",
            Url = new Uri("https://example.com/license"),
        }
    });
});

//Add automapper
builder.Services.AddAutoMapper(typeof(Program));

//DI
builder.Services.AddScoped<IDAO<Club>, ClubDAO>();
builder.Services.AddScoped<IService<Club>, ClubService>();

builder.Services.AddScoped<IDAO<Stadium>, StadiumDAO>();
builder.Services.AddScoped<IService<Stadium>, StadiumService>();

builder.Services.AddScoped<IDAO<Match>, MatchDAO>();
builder.Services.AddScoped<IMatchDAO, MatchDAO>();

builder.Services.AddScoped<IService<Match>, MatchService>();
builder.Services.AddScoped<IMatchService, MatchService>();

builder.Services.AddScoped<IDAO<Vak>, VakDAO>();
builder.Services.AddScoped<IService<Vak>, VakService>();



//JWT Bearer tokens
builder.Services.AddAuthentication()
    .AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    //Configureer de parameters voor het valideren van het token
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["JwtConfig:JwtIssuer"], // uitgever van het token
        ValidAudience = builder.Configuration["JwtConfig:JwtIssuer"],
        //de sleutel waarmee de token signature wordt gecontroleerd
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:JwtKey"])),
        ClockSkew = TimeSpan.Zero // remove delay of token when expire
    };
});

//CORS settings
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(myAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
});

//Policies (als nodig, dit is een voorbeeld van de EmployeeAPI oef
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminManager", policy =>
    {
        policy.RequireRole("Admin");
        policy.RequireClaim("functie", "Manager");
    });
});

//email
builder.Services.AddControllersWithViews();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddSingleton<IEmailSend, EmailSend>();

//conformation email
builder.Services.AddTransient<Microsoft.AspNetCore.Identity.UI.Services.IEmailSender, Util.EmailSend>();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "be.VIVES.Session";

    options.IdleTimeout = TimeSpan.FromMinutes(10); //Default = 20 min
});

//session 
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "EfraThibSession";

    options.IdleTimeout = TimeSpan.FromMinutes(20); //Default = 20 min
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//Swagger
var swaggerOptions = new UCL_Ticketverkoop.Options.OptionsSwagger();
builder.Configuration.GetSection(nameof(UCL_Ticketverkoop.Options.OptionsSwagger)).Bind(swaggerOptions);
app.UseSwagger(option => { option.RouteTemplate = swaggerOptions.JsonRoute; });
app.UseSwaggerUI(option =>
{
    option.SwaggerEndpoint(swaggerOptions.UiEndpoint, swaggerOptions.Description);
});
app.UseSwagger();

//Session
app.UseSession();

//User CORS
app.UseCors(myAllowSpecificOrigins);

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
