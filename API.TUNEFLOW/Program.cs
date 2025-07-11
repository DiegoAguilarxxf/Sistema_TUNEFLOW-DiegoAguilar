using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System; // Agrega este 'using' para poder usar TimeSpan

namespace API.TUNEFLOW
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Obtener la cadena de conexión de forma segura desde la configuración
            var connectionString = builder.Configuration.GetConnectionString("TUNEFLOWContext");

            // Configurar el DbContext (TUNEFLOWContext) con Npgsql y la estrategia de reintentos
            builder.Services.AddDbContext<TUNEFLOWContext>(options =>
            {
                // Verificar si la cadena de conexión se encontró; si no, lanzar una excepción
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("La cadena de conexión 'TUNEFLOWContext' no se encontró.");
                }

                // Usar Npgsql para PostgreSQL y configurar opciones específicas
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    // Habilitar la estrategia de reintentos para manejar fallos transitorios de red
                    // Esto ayuda a que la aplicación sea más robusta ante problemas de conexión intermitentes (como timeouts)
                    npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5, // Número máximo de veces que intentará reintentar la operación
                        maxRetryDelay: TimeSpan.FromSeconds(30), // El retraso máximo entre reintentos (con backoff exponencial)
                        errorCodesToAdd: null // Usa los códigos de error predeterminados que Npgsql considera transitorios
                    );

                    // Opcional: Puedes establecer un tiempo de espera global para los comandos de la base de datos.
                    // Si ya tienes un 'Timeout' en tu cadena de conexión, no es estrictamente necesario aquí.
                    // npgsqlOptions.CommandTimeout(60); // Establece un tiempo de espera de 60 segundos para la ejecución de comandos
                });
            });

            // Añadir servicios al contenedor de inyección de dependencias.

            // Configurar los controladores y añadir soporte para Newtonsoft.Json
            // Esto es útil para manejar bucles de referencia en la serialización JSON.
            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            // Configurar Swagger/OpenAPI para la documentación de la API
            // Más información: https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Construir la aplicación web
            var app = builder.Build();

            // Configurar el pipeline de solicitudes HTTP.
            // Si la aplicación está en entorno de desarrollo, habilitar Swagger UI.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(); // Habilita el middleware para generar la especificación Swagger
                app.UseSwaggerUI(); // Habilita el middleware para servir la UI de Swagger (Swagger-UI)
            }

            // Redireccionar solicitudes HTTP a HTTPS
            app.UseHttpsRedirection();

            // Habilitar la autorización (middleware de autorización)
            app.UseAuthorization();

            // Mapear los controladores a sus rutas
            app.MapControllers();

            // Iniciar la aplicación
            app.Run();
        }
    }
}