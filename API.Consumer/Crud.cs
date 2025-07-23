using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.User.Administration;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Production;
using Modelos.Tuneflow.User.Profiles;
using Modelos.Tuneflow.Playlists;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;

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
                    var url = $"{EndPoint}/Title/{Uri.EscapeDataString(palabraClave)}";
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
        public static async Task<Artist> GetArtistaPorUsuarioId(string idUsuario)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/UsuarioArtista/{idUsuario}");
                Console.WriteLine($"{EndPoint}/UsuarioArtista/{idUsuario}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("JSON recibido: " + json);

                    try
                    {
                        var artista = JsonConvert.DeserializeObject<Artist>(json);
                        return artista;
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
                var response = await client.GetAsync($"{EndPoint}/User/ByClient/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Profile>(json);
                }
                else
                {
                    return new Profile();
                }
            }
        }
        public static async Task<Profile> GetPerfilPorArtistaId(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/User/ByArtist/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Profile>(json);
                }
                else
                {
                    return new Profile();
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

        public static async Task<List<Follow>> GetFollowsPorClienteId(int id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/FollowsByCliemnte/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var follows = JsonConvert.DeserializeObject<List<Follow>>(json);
                    return follows;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("No se encontraron playlist");
                    return new List<Follow>();
                }
                else
                {
                    Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                    return new List<Follow>();
                }
            }
        }

        public static async Task<List<Song>> GetCancionesPorPlaylist(int idPlaylist)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/SongsForPlaylist/{idPlaylist}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var canciones = JsonConvert.DeserializeObject<List<Song>>(json);
                    return canciones;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("No se encontraron canciones");
                    return new List<Song>();
                }
                else
                {
                    Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                    return new List<Song>();
                }
            }
        }

        public static async Task<List<Song>> GetCancionesPorArtistaId(int idArtista)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/PorArtista/{idArtista}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var canciones = JsonConvert.DeserializeObject<List<Song>>(json);
                    return canciones;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("No se encontraron canciones");
                    return new List<Song>();
                }
                else
                {
                    Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                    return new List<Song>();
                }
            }
        }
        public static async Task<List<Song>> GetCancionesPorgenero(string genero)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/PorGenero/{genero}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var canciones = JsonConvert.DeserializeObject<List<Song>>(json);
                    return canciones;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("No se encontraron canciones");
                    return new List<Song>();
                }
                else
                {
                    Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                    return new List<Song>();
                }
            }
        }

        public static async Task<int> GetFollowByIdClient(int idCliente, int idArtista)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/ObtenerIsFollowed/{idCliente}/{idArtista}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var follow = JsonConvert.DeserializeObject<int>(json);
                    return follow;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine("No se encontró el seguimiento");
                    return 0;
                }
                else
                {
                    Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                    return 0;
                }
            }
        }

        public static async Task<int> ObtenerIdSongPlaylist(int SongId, int PlaylistId)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{EndPoint}/ExistSongPlaylist/{SongId}/{PlaylistId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var jObj = JObject.Parse(json);
                    int id = jObj["id"]?.Value<int>() ?? 0;
                    return id;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return 0;
                }
                else
                {
                    Console.WriteLine($"Error en llamada API: {response.StatusCode}");
                    return 0;
                }
            }
        }
        public static async Task<List<Follow>> GetFollowsByClientIdAsync(int clientId)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{EndPoint}/FollowsByCliemnte/{clientId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Follow>>(json);
            }
            else
            {
                return new List<Follow>();
            }
        }
        public static async Task<List<T>> GetCustomAsync(string path)
        {
            using var client = new HttpClient();
            var response = await client.GetAsync($"{EndPoint}/{path}");

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<T>>(json);
        }




    }
}
