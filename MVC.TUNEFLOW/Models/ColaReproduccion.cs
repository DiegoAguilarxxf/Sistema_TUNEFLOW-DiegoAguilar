using Modelos.Tuneflow.Media;

namespace MVC.TUNEFLOW.Models
{
    public class ColaReproduccion
    {
        public List<Song> Canciones { get; set; } = new();

        public void Agregar(Song cancion)
        {
            if (!Canciones.Any(c => c.Id == cancion.Id))
                Canciones.Add(cancion);
        }

        public Song? Siguiente()
        {
            if (!Canciones.Any())
                return null;
            var siguiente = Canciones[0];
            Canciones.RemoveAt(0);
            return siguiente;
        }

        public List<Song> ObtenerTodas() => Canciones;

        public bool EstaVacia() => !Canciones.Any();
    }
}