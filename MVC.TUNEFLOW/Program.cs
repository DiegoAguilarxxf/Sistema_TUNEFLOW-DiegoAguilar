using API.Consumer; // Para tu clase Crud que interact�a con la API
using API.TUNEFLOW.Controllers; // Posiblemente se refiere a controladores de tu API interna o compartida
using Microsoft.AspNetCore.Identity; // Para la gesti�n de usuarios y autenticaci�n
using Microsoft.EntityFrameworkCore; // Para interactuar con la base de datos a trav�s de Entity Framework Core
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Models;
using Modelos.Tuneflow.Payments;
using Modelos.Tuneflow.Playlists;
using Modelos.Tuneflow.User.Administration;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Profiles;
using Modelos.Tuneflow.User.Production;
using MVC.TUNEFLOW.Data; // Tu contexto de base de datos para el proyecto MVC
using Npgsql;
using System.Data;
using System; // Agregado para usar TimeSpan

namespace MVC.TUNEFLOW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- Configuraci�n de EndPoints para el cliente API ---
            // Es crucial que estos EndPoints apunten a la URL correcta de tu API de backend.
            // Si tu API (API.TUNEFLOW) no se ejecuta en 'https://localhost:7031',
            // debes ajustar estos EndPoints a la URL real donde est� alojada.
            Crud<Administrator>.EndPoint = "https://localhost:7031/api/Administrators";
            Crud<Album>.EndPoint = "https://localhost:7031/api/Albums";
            Crud<Song>.EndPoint = "https://localhost:7031/api/Songs";
            Crud<FavoriteSong>.EndPoint = "https://localhost:7031/api/FavoriteSongs";
            Crud<Artist>.EndPoint = "https://localhost:7031/api/Artists";
            Crud<Client>.EndPoint = "https://localhost:7031/api/Clients"; // Aqu� ten�as una duplicaci�n, se mantiene una.
            Crud<ArtistStatistics>.EndPoint = "https://localhost:7031/api/ArtistsStatistics";
            Crud<SongPlaylist>.EndPoint = "https://localhost:7031/api/SongsPlaylists";
            Crud<Payment>.EndPoint = "https://localhost:7031/api/Payments";
            Crud<Profile>.EndPoint = "https://localhost:7031/api/Profiles"; 
            Crud<Playback>.EndPoint = "https://localhost:7031/api/Playbacks";
            Crud<Follow>.EndPoint = "https://localhost:7031/api/Follows";
            Crud<Subscription>.EndPoint = "https://localhost:7031/api/Subscriptions";
            Crud<SubscriptionType>.EndPoint = "https://localhost:7031/api/SubscriptionsTypes";
            Crud<Playlist>.EndPoint = "https://localhost:7031/api/Playlists";
            Crud<Country>.EndPoint = "https://localhost:7031/api/Countries";
            // Crud<Modelos.Tuneflow.Usuario.Consumidor.Cliente>.EndPoint = "https://localhost:7031/api/Clientes"; // Esta l�nea estaba duplicada y es redundante. Se elimin� o se consolid� con la anterior.


            builder.Services.AddTransient<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));


            // --- Configuraci�n de DbContext para la Autenticaci�n (Identity) ---
            // Usa ApplicationDbContext para gestionar los usuarios de Identity.
            // Es CRUCIAL que esta cadena de conexi�n apunte a la misma base de datos de Supabase.
            var identityConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(identityConnectionString))
            {
                throw new InvalidOperationException("La cadena de conexi�n 'DefaultConnection' para Identity no se encontr�.");
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(identityConnectionString, npgsqlOptions =>
                {
                    // Habilitar la estrategia de reintentos para Identity DB Context
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null
                    );
                    // Opcional: npgsqlOptions.CommandTimeout(60);
                });
            });

            // --- Configuraci�n de DbContext para TUNEFLOW (modelos de tu aplicaci�n) ---
            // Usas un segundo DbContext llamado TUNEFLOWContext.
            // Si este contexto tambi�n se conecta a la base de datos de Supabase,
            // la estrategia de reintentos tambi�n es fundamental aqu�.
            // Aseg�rate de que "DefaultConnection" sea la cadena correcta si ambos contextos usan la misma DB.
            var tuneflowConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Asumo que es la misma que la de Identity.
            if (string.IsNullOrEmpty(tuneflowConnectionString))
            {
                throw new InvalidOperationException("La cadena de conexi�n 'DefaultConnection' para TUNEFLOWContext no se encontr�.");
            }

            builder.Services.AddDbContext<TUNEFLOWContext>(options =>
            {
                options.UseNpgsql(tuneflowConnectionString, npgsqlOptions =>
                {
                    // Habilitar la estrategia de reintentos para TUNEFLOW DB Context
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorCodesToAdd: null
                    );
                    // Opcional: npgsqlOptions.CommandTimeout(60);
                });
            });

            // --- Configuraci�n de ASP.NET Core Identity ---
            // A�ade los servicios de Identity, configurando el usuario predeterminado (IdentityUser)
            // y a�adiendo soporte para roles. Los datos de Identity se almacenar�n en ApplicationDbContext.
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() // Permite el uso de roles de usuario
                .AddEntityFrameworkStores<ApplicationDbContext>(); // Conecta Identity con tu DbContext

            


            builder.Services.AddControllersWithViews();

            // A�ade soporte para Razor Pages (com�nmente usado con Identity UI)
            builder.Services.AddRazorPages();

            builder.Services.AddHttpClient();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // --- Configuraci�n del Pipeline de Solicitudes HTTP ---
            // Configurar el pipeline de solicitudes HTTP.
            if (app.Environment.IsDevelopment())
            {
                // En desarrollo, usa el endpoint de migraciones para aplicar cambios autom�ticamente (si est� configurado)
                app.UseMigrationsEndPoint();
            }
            else
            {
                // En producci�n, configura el manejo de errores global
                app.UseExceptionHandler("/Home/Error");
                // Configura HSTS para forzar conexiones HTTPS, recomendado para producci�n
                app.UseHsts();
            }

            // Forzar el uso de HTTPS
            app.UseHttpsRedirection();
            // Habilitar el servicio de archivos est�ticos (CSS, JS, im�genes, etc.)
            app.UseStaticFiles();

            // Habilitar el enrutamiento
            app.UseRouting();

            // Habilitar la autenticaci�n (middleware para procesar credenciales de usuario)
            app.UseAuthentication();
            // Habilitar la autorizaci�n (middleware para aplicar pol�ticas de acceso)
            app.UseAuthorization();

            // --- Mapeo de Rutas para Controladores y �reas ---
            // Mapeo de rutas para �reas (ej: /Admin/Panel)
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Panel}/{action=Panel}/{id?}");

            // Mapeo de ruta predeterminada (ej: /Home/Index)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Mapeo de Razor Pages (usado por Identity UI)
            app.MapRazorPages();

            // Iniciar la aplicaci�n
            app.Run();
        }
    }
}