using Modelos.Tuneflow.Media;

namespace MVC.TUNEFLOW.Models
{
    public class PilaAnterior
    {
        public List<Song> Canciones { get; set; } = new();

        public void Agregar(Song cancion)
        {
            Canciones.Add(cancion); 
        }

        public Song? Anterior()
        {
            if (!Canciones.Any())
                return null;

            var ultima = Canciones.Last();
            Canciones.RemoveAt(Canciones.Count - 1); 
            return ultima;
        }

        public bool EstaVacia() => !Canciones.Any();
    }

}
