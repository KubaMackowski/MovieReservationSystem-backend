using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MovieReservationSystem.Services;
using MovieReservationSystem.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieReservationSystem.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// Jeśli nie masz jeszcze klasy JwtService, usuń poniższą linię, żeby nie było błędu
builder.Services.AddScoped<JwtService>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders(); // <--- TU MA BYĆ ŚREDNIK! Koniec konfiguracji Identity.

// BLOK 2: Konfiguracja JWT (zaczynamy od nowa na builder.Services)
builder.Services.AddAuthentication(options => 
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = false,   // Dla uproszczenia na start false
            ValidateAudience = false, // Dla uproszczenia na start false
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            // Upewnij się, że masz te wpisy w appsettings.json!
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Uruchomienie Seedera
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        // Czekamy na wykonanie
        await DataSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Błąd podczas tworzenia ról.");
    }
}

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- WAŻNE: Dodano te dwie linie w tej kolejności ---
app.UseAuthentication(); // 1. Najpierw sprawdź kim jest
app.UseAuthorization();  // 2. Potem sprawdź czy ma dostęp
// ---------------------------------------------------

app.MapControllers();

app.Run();