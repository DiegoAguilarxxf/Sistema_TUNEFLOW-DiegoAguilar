using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Usuario.Administracion;
using Modelos.Tuneflow.Usuario.Consumidor;
using Modelos.Tuneflow.Usuario.Perfiles;
using Modelos.Tuneflow.Playlist;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;

namespace API.Consumer
{
    public class Crud<T>
    {
        public static string EndPoint { get; set; }

        public static async Task<List<T>> GetAllAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(EndPoint);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static async Task<T> GetByIdAsync(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }


        public static async Task<List<T>> GetByAsync(string campo, int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/{campo}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<T>>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static async Task<T> CreateAsync(T item)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    EndPoint,
                    new StringContent(
                        JsonConvert.SerializeObject(item),
                        Encoding.UTF8,
                        "application/json"
                    )
                );

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static async Task<bool> UpdateAsync(int id, T item)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PutAsync(
                    $"{EndPoint}/{id}",
                    new StringContent(
                        JsonConvert.SerializeObject(item),
                        Encoding.UTF8,
                        "application/json"
                    )
                );

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static async Task<bool> DeleteAsync(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.DeleteAsync($"{EndPoint}/{id}");
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static async Task<List<Song>> GetCancionesPorPalabrasClave(string palabraClave)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{EndPoint}/Titulo/{Uri.EscapeDataString(palabraClave)}";
                    Console.WriteLine($"Llamando a API: {url}");
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Respuesta JSON: {json.Substring(0, Math.Min(json.Length, 200))}..."); // imprime primeros 200 caracteres
                        var canciones = JsonConvert.DeserializeObject<List<Song>>(json);
                        Console.WriteLine($"Canciones deserializadas: {canciones?.Count ?? 0}");
                        return canciones;
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("No se encontraron canciones.");
                        return new List<Song>();
                    }
                    else
                    {
                        Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                        return new List<Song>();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en llamada API: {ex.Message}");
                return new List<Song>();
            }
        }

        public static async Task<Client> GetClientePorUsuarioId(string idUsuario)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/Usuario/{idUsuario}");
                Console.WriteLine($"{EndPoint}/Usuario/{idUsuario}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("JSON recibido: " + json);

                    try
                    {
                        var cliente = JsonConvert.DeserializeObject<Client>(json);
                        return cliente;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error de deserialización: " + ex.Message);
                        throw;
                    }
                }
                else
                {
                    Console.WriteLine("La API devolvió: " + response.StatusCode);
                    return null;
                }
            }
        }

        public static async Task<Profile> GetPerfilPorClienteId(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/Usuario/Obtencion/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Perfil>(json);
                }
                else
                {
                    return new Perfil();
                }
            }
        }

        public static async Task<List<Playlist>> GetPlaylistPorClienteId(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/Cliente/Playlist/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var playlists = JsonConvert.DeserializeObject<List<Playlist>>(json);
                    return playlists;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("No se encontraron playlist");
                    return new List<Playlist>();
                }
                else
                {
                    Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                    return new List<Playlist>();
                }
            }
        }

        public static async Task<List<Cancion>> GetCancionesPorPlaylist(int idPlaylist)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/CancionesPorPlaylist/{idPlaylist}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var canciones = JsonConvert.DeserializeObject<List<Cancion>>(json);
                    return canciones;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("No se encontraron canciones");
                    return new List<Cancion>();
                }
                else
                {
                    Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                    return new List<Cancion>();
                }
            }
        }

    }
}
