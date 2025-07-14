const toggle = document.getElementById('theme-toggle');
const html = document.documentElement;
const switchLabel = document.querySelector('.switch-label');

function setTheme(theme) {
    html.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
    toggle.checked = (theme === 'dark');
    switchLabel.textContent = theme === 'dark' ? 'Modo Oscuro' : 'Modo Claro';
}

function loadTheme() {
    const saved = localStorage.getItem('theme');
    setTheme(saved === 'light' ? 'light' : 'dark');
}

toggle.addEventListener('change', () => {
    setTheme(toggle.checked ? 'dark' : 'light');
});

loadTheme();

const audioPlayer = document.getElementById('audioPlayer');
const cancionActual = document.getElementById('cancionActual');
const portadaActual = document.getElementById('portadaActual');
const volumenControl = document.getElementById('volumenControl');
const barraProgreso = document.getElementById('barraProgreso');
const tiempoActual = document.getElementById('tiempoActual');
const duracionTotal = document.getElementById('duracionTotal');
const reproductor = document.getElementById('reproductor');
const togglePlayerBtn = document.getElementById('togglePlayerBtn');

let estaReproduciendo = false;
let cancionEnReproduccion = {
    titulo: '',
    url: '',
    portada: '',
    tiempo: 0,
    idCliente: 0
};

// 🔄 Restaurar canción al recargar
document.addEventListener('DOMContentLoaded', () => {
    const data = sessionStorage.getItem('ultimaCancion');
    if (data) {
        const c = JSON.parse(data);
        reproducirCancion(c.titulo, c.url, c.portada, c.tiempo, false);
    }
});

volumenControl.addEventListener('input', () => {
    audioPlayer.volume = volumenControl.value;
});

barraProgreso.addEventListener('input', () => {
    const nuevoTiempo = (barraProgreso.value / 100) * audioPlayer.duration;
    audioPlayer.currentTime = nuevoTiempo;
});

audioPlayer.addEventListener('timeupdate', () => {
    const progreso = (audioPlayer.currentTime / audioPlayer.duration) * 100;
    barraProgreso.value = progreso || 0;
    tiempoActual.textContent = formatearTiempo(audioPlayer.currentTime);
    duracionTotal.textContent = formatearTiempo(audioPlayer.duration);

    // Guardar posición
    cancionEnReproduccion.tiempo = audioPlayer.currentTime;
    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
});

function formatearTiempo(segundos) {
    if (isNaN(segundos)) return '0:00';
    const m = Math.floor(segundos / 60);
    const s = Math.floor(segundos % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
}

async function reproducirCancion(id, titulo, url, portada, idCliente, tiempo = 0, autoPlay = true) {
    cancionEnReproduccion = { id, titulo, url, portada, tiempo, idCliente };
    cancionActual.textContent = titulo;
    portadaActual.src = portada;
    audioPlayer.src = url;
    audioPlayer.currentTime = tiempo || 0;

    if (autoPlay) {
        audioPlayer.play()
            .then(() => estaReproduciendo = true)
            .catch(err => console.error(err));
    }

    const esFavorita = await ComprobarEsCancionfavorita(id, idCliente);
    actualizarBotonFavorito(esFavorita);
    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
}

function togglePlay() {
    if (!audioPlayer.src) return;
    if (audioPlayer.paused) {
        audioPlayer.play().then(() => estaReproduciendo = true);
    } else {
        audioPlayer.pause();
        estaReproduciendo = false;
    }
}

function siguienteCancion() {
    alert("Siguiente canción no implementada.");
}

function anteriorCancion() {
    alert("Anterior canción no implementada.");
}

async function añadirFavorito() {
    if (!cancionEnReproduccion || !cancionEnReproduccion.id) {
        alert("No hay canción en reproducción.");
        return;
    }

    const id = cancionEnReproduccion.id;
    const idCliente = cancionEnReproduccion.idCliente;

    const esFavorita = await ComprobarEsCancionfavorita(id, idCliente);
    const idPlaylistFavoritos = await ExtraerPlaylistDeFavortos(cancionEnReproduccion.idCliente);

    if (esFavorita) {
        const idMusicaPlaylist = await ObtenerIdDeMusicaPlaylist(id, idPlaylistFavoritos);
        const idCancionesFavoritas = await ObtenerIdDeCancionFavorita(id, idCliente);

        const urlEliminarMusicaPlaylist = `https://localhost:7031/api/MusicasPlaylists/${idMusicaPlaylist}`;

        try {
            const res = await fetch(urlEliminarMusicaPlaylist, {
                method: "DELETE",
                credentials: "include"
            });

            if (res.ok) {
                if (res.status === 204) {
                    console.log("✅ Canción eliminada correctamente.");
                    actualizarBotonFavorito(false);
                    alert("🎵 Canción eliminada de la playlist.");
                } else {
                    // Solo intentas parsear JSON si esperas contenido
                    const data = await res.json();
                    console.log("✅ Respuesta:", data);
                }
            } else {
                console.error("❌ Error al eliminar:", res.status);
                alert("Error al eliminar de la playlist.");
            }
        } catch (err) {
            console.error("❌ Error de red:", err);
            alert("Error de red.");
        }

        const urlEliminarCancionesFavoritas = `https://localhost:7031/api/CancionesFavoritas/${idCancionesFavoritas}`;

        try {
            const res = await fetch(urlEliminarCancionesFavoritas, {
                method: "DELETE",
                credentials: "include"
            });

            if (res.ok) {
                if (res.status === 204) {
                    console.log("✅ Canción eliminada correctamente.");
                    alert("🎵 Canción eliminada de tus Favoritos");
                } else {
                    // Solo intentas parsear JSON si esperas contenido
                    const data = await res.json();
                    console.log("✅ Respuesta:", data);
                }
            } else {
                console.error("❌ Error al eliminar:", res.status);
                alert("Error al eliminar de la playlist.");
            }
        } catch (err) {
            console.error("❌ Error de red:", err);
            alert("Error de red.");
        }


    } else {

        const url3 = "https://localhost:7031/api/MusicasPlaylists";

        const bodyPlaylistFavoritos = {
            PlaylistId: idPlaylistFavoritos,
            CancionId: id
        };

        try {
            const res = await fetch(url3, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify(bodyPlaylistFavoritos)
            });

            if (res.ok) {
                const data = await res.json();
                console.log("✅ Canción añadida a la playlist:", data);
                alert("🎵 Añadida a la playlist correctamente.");
            } else {
                console.error("❌ Error al añadir:", res.status);
                alert("Error al añadir a la playlist.");
            }
        } catch (err) {
            console.error("❌ Error de red:", err);
            alert("Error de red.");
        }

        const urlCancionFavorita = "https://localhost:7031/api/CancionesFavoritas";

        const bodyCancionFavorita = {
            ClienteId: idCliente,
            CancionId: id,
            FechaAgregado: new Date().toISOString()  // fecha actual en ISO
        };

        try {
            const res = await fetch(urlCancionFavorita, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify(bodyCancionFavorita)
            });

            if (res.ok) {
                const data = await res.json();
                console.log("✅ Canción añadida a favoritos:", data);
                alert("🎵 Añadida a favoritos correctamente.");
                actualizarBotonFavorito(true);
            } else {
                console.error("❌ Error al añadir:", res.status);
                alert("Error al añadir a favoritos.");
            }
        } catch (err) {
            console.error("❌ Error de red:", err);
            alert("Error de red.");
        }
    }
}

async function ExtraerPlaylistDeFavortos(id) {
    const url = `https://localhost:7031/api/Playlists/PlaylistFavoritos/${id}`;

    try {
        const res = await fetch(url, {
            method: "GET",
            credentials: "include"
        });

        if (res.ok) {
            const data = await res.json();
            return data;
        } else {
            return null;
        }
    } catch (err) {
        console.error("Error de red:", err);
        return null;
    }
}

async function ComprobarEsCancionfavorita(idCancion, idCliente) {
    const url = `https://localhost:7031/api/CancionesFavoritas/IsFavorita/${idCancion}/${idCliente}`;

    try {
        const res = await fetch(url, {
            method: "GET",
            credentials: "include"
        });

        if (res.ok) {
            const data = await res.json();
            return data.id !== undefined && data.id !== null;
        } else {
            return false;
        }
    } catch (err) {
        console.error("Error de red:", err);
        return false;
    }
}

async function ObtenerIdDeCancionFavorita(idCancion, idCliente) {
    const urlObtenerIdCancionFavorita = `https://localhost:7031/api/CancionesFavoritas/IsFavorita/${idCancion}/${idCliente}`;

    try {
        const res = await fetch(urlObtenerIdCancionFavorita, {
            method: "GET",
            credentials: "include"
        });

        if (res.ok) {
            const data = await res.json();
            return data.id ?? null;
        } else {
            return null;
        }
    } catch (err) {
        console.error("Error de red:", err);
        return null;
    }
}

async function ObtenerIdDeMusicaPlaylist(idCancion, idPlaylist) {
    const url = `https://localhost:7031/api/MusicasPlaylists/ExistMusicaPlaylist/${idCancion}/${idPlaylist}`;

    try {
        const res = await fetch(url, {
            method: "GET",
            credentials: "include"
        });

        if (res.ok) {
            const data = await res.json();
            return data.id ?? null;
        } else {
            return null;
        }
    } catch (err) {
        console.error("Error de red:", err);
        return null;
    }
}

function actualizarBotonFavorito(esFavorita) {
    const btn = document.getElementById('btnfavorito');
    if (!btn) return;

    if (esFavorita) {
        btn.classList.remove('btn-outline-warning');
        btn.classList.add('btn-warning');  // botón activo (por ejemplo color amarillo sólido)
        btn.textContent = '💖';             // corazón lleno
        btn.title = 'Quitar de favoritos';
    } else {
        btn.classList.remove('btn-warning');
        btn.classList.add('btn-outline-warning'); // estilo normal (borde)
        btn.textContent = '❤️';                   // corazón vacío
        btn.title = 'Añadir a favoritos';
    }
}

togglePlayerBtn.addEventListener('click', () => {
    reproductor.classList.toggle('expanded');
    togglePlayerBtn.textContent = reproductor.classList.contains('expanded') ? '⬇' : '⬆';

    if (reproductor.classList.contains('expanded')) {
        portadaActual.style.height = '200px';
        portadaActual.style.width = '200px';
    } else {
        portadaActual.style.height = '60px';
        portadaActual.style.width = '60px';
    }
});

// 📢 Función global para llamar desde botones
window.reproducirCancion = reproducirCancion;
window.añadirFavorito = añadirFavorito;
window.togglePlay = togglePlay;
window.siguienteCancion = siguienteCancion;
window.anteriorCancion = anteriorCancion;