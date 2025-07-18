using API.Consumer; // Para tu clase Crud que interactúa con la API
using API.TUNEFLOW.Controllers; // Posiblemente se refiere a controladores de tu API interna o compartida
using Microsoft.AspNetCore.Identity; // Para la gestión de usuarios y autenticación
using Microsoft.EntityFrameworkCore; // Para interactuar con la base de datos a través de Entity Framework Core
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Modelos;
using Modelos.Tuneflow.Pagos;
using Modelos.Tuneflow.Playlists;
using Modelos.Tuneflow.Usuario.Administracion;
using Modelos.Tuneflow.Usuario.Consumidor;
using Modelos.Tuneflow.Usuario.Perfiles;
using Modelos.Tuneflow.Usuario.Produccion;
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

            // --- Configuración de EndPoints para el cliente API ---
            // Es crucial que estos EndPoints apunten a la URL correcta de tu API de backend.
            // Si tu API (API.TUNEFLOW) no se ejecuta en 'https://localhost:7031',
            // debes ajustar estos EndPoints a la URL real donde está alojada.
            Crud<Administrator>.EndPoint = "https://localhost:7031/api/Administrators";
            Crud<Album>.EndPoint = "https://localhost:7031/api/Albums";
            Crud<Song>.EndPoint = "https://localhost:7031/api/Songs";
            Crud<FavoriteSong>.EndPoint = "https://localhost:7031/api/FavoriteSongs";
            Crud<Artist>.EndPoint = "https://localhost:7031/api/Artists";
            Crud<Client>.EndPoint = "https://localhost:7031/api/Clients"; // Aquí tenías una duplicación, se mantiene una.
            Crud<ArtistStatistics>.EndPoint = "https://localhost:7031/api/StatisticsArtists";
            Crud<SongPlaylist>.EndPoint = "https://localhost:7031/api/MusicsPlaylists";
            Crud<Payment>.EndPoint = "https://localhost:7031/api/Payments";
            Crud<Profile>.EndPoint = "https://localhost:7031/api/Profiles";
            Crud<Playback>.EndPoint = "https://localhost:7031/api/Playbacks";
            Crud<Follow>.EndPoint = "https://localhost:7031/api/Follows";
            Crud<Subscription>.EndPoint = "https://localhost:7031/api/Subscriptions";
            Crud<SubscriptionType>.EndPoint = "https://localhost:7031/api/SubscriptionsTypes";
            Crud<Playlist>.EndPoint = "https://localhost:7031/api/Playlists";
            Crud<Country>.EndPoint = "https://localhost:7031/api/Countries";
            // Crud<Modelos.Tuneflow.Usuario.Consumidor.Cliente>.EndPoint = "https://localhost:7031/api/Clientes"; // Esta línea estaba duplicada y es redundante. Se eliminó o se consolidó con la anterior.

            builder.Services.AddTransient<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));


            // --- Configuración de DbContext para la Autenticación (Identity) ---
            // Usa ApplicationDbContext para gestionar los usuarios de Identity.
            // Es CRUCIAL que esta cadena de conexión apunte a la misma base de datos de Supabase.
            var identityConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(identityConnectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' para Identity no se encontró.");
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

            // --- Configuración de DbContext para TUNEFLOW (modelos de tu aplicación) ---
            // Usas un segundo DbContext llamado TUNEFLOWContext.
            // Si este contexto también se conecta a la base de datos de Supabase,
            // la estrategia de reintentos también es fundamental aquí.
            // Asegúrate de que "DefaultConnection" sea la cadena correcta si ambos contextos usan la misma DB.
            var tuneflowConnectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // Asumo que es la misma que la de Identity.
            if (string.IsNullOrEmpty(tuneflowConnectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' para TUNEFLOWContext no se encontró.");
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

            // --- Configuración de ASP.NET Core Identity ---
            // Añade los servicios de Identity, configurando el usuario predeterminado (IdentityUser)
            // y añadiendo soporte para roles. Los datos de Identity se almacenarán en ApplicationDbContext.
            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() // Permite el uso de roles de usuario
                .AddEntityFrameworkStores<ApplicationDbContext>(); // Conecta Identity con tu DbContext

            // Añade soporte para controladores MVC con vistas
            builder.Services.AddControllersWithViews();

            // Añade soporte para Razor Pages (comúnmente usado con Identity UI)
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // --- Configuración del Pipeline de Solicitudes HTTP ---
            // Configurar el pipeline de solicitudes HTTP.
            if (app.Environment.IsDevelopment())
            {
                // En desarrollo, usa el endpoint de migraciones para aplicar cambios automáticamente (si está configurado)
                app.UseMigrationsEndPoint();
            }
            else
            {
                // En producción, configura el manejo de errores global
                app.UseExceptionHandler("/Home/Error");
                // Configura HSTS para forzar conexiones HTTPS, recomendado para producción
                app.UseHsts();
            }

            // Forzar el uso de HTTPS
            app.UseHttpsRedirection();
            // Habilitar el servicio de archivos estáticos (CSS, JS, imágenes, etc.)
            app.UseStaticFiles();

            // Habilitar el enrutamiento
            app.UseRouting();

            // Habilitar la autenticación (middleware para procesar credenciales de usuario)
            app.UseAuthentication();
            // Habilitar la autorización (middleware para aplicar políticas de acceso)
            app.UseAuthorization();

            // --- Mapeo de Rutas para Controladores y Áreas ---
            // Mapeo de rutas para áreas (ej: /Admin/Panel)
            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Panel}/{action=Panel}/{id?}");

            // Mapeo de ruta predeterminada (ej: /Home/Index)
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Mapeo de Razor Pages (usado por Identity UI)
            app.MapRazorPages();

            // Iniciar la aplicación
            app.Run();
        }
    }
}