const toggle = document.getElementById('theme-toggle');
const html = document.documentElement;
const switchLabel = document.querySelector('.switch-label');

function setTheme(theme) {
    html.setAttribute('data-theme', theme);
    window.currentTheme = theme;
    if (toggle) toggle.checked = (theme === 'dark');
    if (switchLabel) switchLabel.textContent = theme === 'dark' ? 'Modo Oscuro' : 'Modo Claro';
}

function loadTheme() {
    const saved = window.currentTheme || 'dark';
    setTheme(saved);
}

if (toggle) {
    toggle.addEventListener('change', () => {
        setTheme(toggle.checked ? 'dark' : 'light');
    });
}

// 🎵 Elementos del reproductor
const audioPlayer = document.getElementById('audioPlayer');
const cancionActual = document.getElementById('cancionActual');
const portadaActual = document.getElementById('portadaActual');
const volumenControl = document.getElementById('volumenControl');
const barraProgreso = document.getElementById('barraProgreso');
const tiempoActual = document.getElementById('tiempoActual');
const duracionTotal = document.getElementById('duracionTotal');
const reproductor = document.getElementById('reproductor');
const togglePlayerBtn = document.getElementById('togglePlayerBtn');
const iconoPlayPause = document.getElementById('icono-play-pause');

let estaReproduciendo = false;
let cancionEnReproduccion = {
    id: '',
    titulo: '',
    url: '',
    portada: '',
    tiempo: 0,
    idCliente: 0
};

// ✅ Restaurar canción desde sessionStorage
document.addEventListener('DOMContentLoaded', () => {
    loadTheme();

    const guardada = sessionStorage.getItem('ultimaCancion');
    if (guardada) {
        const c = JSON.parse(guardada);
        reproducirCancion(c.id, c.titulo, c.url, c.portada, c.idCliente, c.tiempo, false);
    }
});

// 🔊 Control de volumen
if (volumenControl) {
    volumenControl.addEventListener('input', () => {
        if (audioPlayer) audioPlayer.volume = volumenControl.value;
    });
}

// ⏩ Barra de progreso
if (barraProgreso) {
    barraProgreso.addEventListener('input', () => {
        if (audioPlayer && audioPlayer.duration) {
            const nuevoTiempo = (barraProgreso.value / 100) * audioPlayer.duration;
            audioPlayer.currentTime = nuevoTiempo;
        }
    });
}

// 🔁 Eventos del reproductor
if (audioPlayer) {
    audioPlayer.addEventListener('timeupdate', () => {
        if (audioPlayer.duration) {
            const progreso = (audioPlayer.currentTime / audioPlayer.duration) * 100;
            if (barraProgreso) barraProgreso.value = progreso || 0;
            if (tiempoActual) tiempoActual.textContent = formatearTiempo(audioPlayer.currentTime);
            if (duracionTotal) duracionTotal.textContent = formatearTiempo(audioPlayer.duration);
            cancionEnReproduccion.tiempo = audioPlayer.currentTime;
            sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
        }
    });

    audioPlayer.addEventListener('play', () => {
        estaReproduciendo = true;
        actualizarIconoPlayPause();
    });

    audioPlayer.addEventListener('pause', () => {
        estaReproduciendo = false;
        actualizarIconoPlayPause();
    });

    audioPlayer.addEventListener('ended', () => {
        estaReproduciendo = false;
        actualizarIconoPlayPause();
    });
}

// 🕐 Formato de tiempo
function formatearTiempo(segundos) {
    if (isNaN(segundos) || !segundos) return '0:00';
    const m = Math.floor(segundos / 60);
    const s = Math.floor(segundos % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
}

// ▶ Cambiar ícono de Play/Pause
function actualizarIconoPlayPause() {
    if (iconoPlayPause) {
        if (estaReproduciendo) {
            iconoPlayPause.src = "~/img/icons/paus.png";
            iconoPlayPause.alt = "Pause";
        } else {
            iconoPlayPause.src = "~/img/icons/play.png";
            iconoPlayPause.alt = "Play";
        }
    }
}

// ▶ Función para reproducir canción (con idCliente)
async function reproducirCancion(id, titulo, url, portada, idCliente, tiempo = 0, autoPlay = true) {
    cancionEnReproduccion = { id, titulo, url, portada, tiempo, idCliente };

    if (cancionActual) cancionActual.textContent = titulo;
    if (portadaActual) portadaActual.src = portada;

    if (audioPlayer) {
        if (audioPlayer.src !== url) {
            audioPlayer.src = url;
        }
        audioPlayer.currentTime = tiempo || 0;

        if (autoPlay) {
            try {
                await audioPlayer.play();
                estaReproduciendo = true;
                actualizarIconoPlayPause();
            } catch (err) {
                console.error('Error al reproducir:', err);
            }
        }
    }

    if (typeof ComprobarEsCancionfavorita === 'function') {
        try {
            const esFavorita = await ComprobarEsCancionfavorita(id, idCliente);
            actualizarBotonFavorito(esFavorita);
        } catch (err) {
            console.error('Error al comprobar favorita:', err);
        }
    }

    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
}

// 🔁 Toggle Play/Pause
function togglePlay() {
    if (!audioPlayer || !audioPlayer.src) return;

    if (audioPlayer.paused) {
        audioPlayer.play()
            .then(() => {
                estaReproduciendo = true;
                actualizarIconoPlayPause();
            })
            .catch(err => console.error('Error al reproducir:', err));
    } else {
        audioPlayer.pause();
        estaReproduciendo = false;
        actualizarIconoPlayPause();
    }
}

// Funciones placeholders (puedes implementar según necesidad)
function siguienteCancion() { }
function anteriorCancion() { }
function descargarCancion() { }
function verLetra() { }

// Añadir o quitar favorito con idCliente
async function añadirFavorito() {
    if (!cancionEnReproduccion || !cancionEnReproduccion.id) {
        alert("No hay canción en reproducción.");
        return;
    }

    const { id, idCliente } = cancionEnReproduccion;

    const esFavorita = await ComprobarEsCancionfavorita(id, idCliente);
    const idPlaylistFavoritos = await ExtraerPlaylistDeFavortos(idCliente);

    if (!idPlaylistFavoritos) {
        alert("No se encontró la playlist de favoritos.");
        return;
    }

    if (esFavorita) {
        const idMusicaPlaylist = await ObtenerIdDeMusicaPlaylist(id, idPlaylistFavoritos);
        const idCancionFavorita = await ObtenerIdDeCancionFavorita(id, idCliente);

        if (idMusicaPlaylist) {
            const urlEliminarMusicaPlaylist = `https://localhost:7031/api/MusicasPlaylists/${idMusicaPlaylist}`;
            try {
                const res = await fetch(urlEliminarMusicaPlaylist, {
                    method: "DELETE",
                    credentials: "include"
                });

                if (res.ok && res.status === 204) {
                    console.log("✅ Canción eliminada de playlist correctamente.");
                    actualizarBotonFavorito(false);
                    alert("🎵 Canción eliminada de la playlist.");
                } else {
                    const data = await res.json();
                    console.log("✅ Respuesta:", data);
                }
            } catch (err) {
                console.error("❌ Error de red al eliminar canción de playlist:", err);
                alert("Error de red.");
            }
        }

        if (idCancionFavorita) {
            const urlEliminarCancionFavorita = `https://localhost:7031/api/CancionesFavoritas/${idCancionFavorita}`;
            try {
                const res = await fetch(urlEliminarCancionFavorita, {
                    method: "DELETE",
                    credentials: "include"
                });

                if (res.ok && res.status === 204) {
                    console.log("✅ Canción eliminada de favoritos correctamente.");
                    alert("🎵 Canción eliminada de tus Favoritos");
                } else {
                    const data = await res.json();
                    console.log("✅ Respuesta:", data);
                }
            } catch (err) {
                console.error("❌ Error de red al eliminar canción favorita:", err);
                alert("Error de red.");
            }
        }

    } else {
        // Añadir a playlist favoritos
        const urlAgregarPlaylist = "https://localhost:7031/api/MusicasPlaylists";
        const bodyPlaylistFavoritos = {
            PlaylistId: idPlaylistFavoritos,
            CancionId: id
        };

        try {
            const res = await fetch(urlAgregarPlaylist, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(bodyPlaylistFavoritos)
            });

            if (res.ok) {
                const data = await res.json();
                console.log("✅ Canción añadida a la playlist:", data);
                alert("🎵 Añadida a la playlist correctamente.");
            } else {
                console.error("❌ Error al añadir a la playlist:", res.status);
                alert("Error al añadir a la playlist.");
            }
        } catch (err) {
            console.error("❌ Error de red al añadir a la playlist:", err);
            alert("Error de red.");
        }

        // Añadir a tabla CancionesFavoritas
        const urlAgregarFavorito = "https://localhost:7031/api/CancionesFavoritas";
        const bodyCancionFavorita = {
            ClienteId: idCliente,
            CancionId: id,
            FechaAgregado: new Date().toISOString()
        };

        try {
            const res = await fetch(urlAgregarFavorito, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(bodyCancionFavorita)
            });

            if (res.ok) {
                const data = await res.json();
                console.log("✅ Canción añadida a favoritos:", data);
                alert("🎵 Añadida a Favoritos correctamente.");
            } else {
                console.error("❌ Error al añadir a favoritos:", res.status);
                alert("Error al añadir a Favoritos.");
            }
        } catch (err) {
            console.error("❌ Error de red al añadir a favoritos:", err);
            alert("Error de red.");
        }
        actualizarBotonFavorito(true);
    }
}

// Actualizar botón de favorito
function actualizarBotonFavorito(esFavorito) {
    const btnFavorito = document.getElementById('btnFavorito');
    if (!btnFavorito) return;

    if (esFavorito) {
        btnFavorito.classList.add('favorito');
        btnFavorito.title = 'Quitar de Favoritos';
    } else {
        btnFavorito.classList.remove('favorito');
        btnFavorito.title = 'Agregar a Favoritos';
    }
}

// Comprobar si canción es favorita
async function ComprobarEsCancionfavorita(idCancion, idCliente) {
    if (!idCancion || !idCliente) return false;

    const url = `https://localhost:7031/api/CancionesFavoritas/es-favorita?idCliente=${idCliente}&idCancion=${idCancion}`;

    try {
        const res = await fetch(url, { credentials: "include" });
        if (res.ok) {
            const data = await res.json();
            return data === true;
        }
        return false;
    } catch (err) {
        console.error('Error al comprobar favorita:', err);
        return false;
    }
}

// Obtener playlist favoritos
async function ExtraerPlaylistDeFavortos(idCliente) {
    if (!idCliente) return null;

    const url = `https://localhost:7031/api/Playlists/obtener-favoritos?idCliente=${idCliente}`;

    try {
        const res = await fetch(url, { credentials: "include" });
        if (res.ok) {
            const data = await res.json();
            return data.id || null;
        }
        return null;
    } catch (err) {
        console.error('Error al obtener playlist favoritos:', err);
        return null;
    }
}

// Obtener Id de música en playlist
async function ObtenerIdDeMusicaPlaylist(idCancion, idPlaylist) {
    if (!idCancion || !idPlaylist) return null;

    const url = `https://localhost:7031/api/MusicasPlaylists/obtener-id?idCancion=${idCancion}&idPlaylist=${idPlaylist}`;

    try {
        const res = await fetch(url, { credentials: "include" });
        if (res.ok) {
            const data = await res.json();
            return data.id || null;
        }
        return null;
    } catch (err) {
        console.error('Error al obtener id música playlist:', err);
        return null;
    }
}

// Obtener Id canción favorita
async function ObtenerIdDeCancionFavorita(idCancion, idCliente) {
    if (!idCancion || !idCliente) return null;

    const url = `https://localhost:7031/api/CancionesFavoritas/obtener-id?idCliente=${idCliente}&idCancion=${idCancion}`;

    try {
        const res = await fetch(url, { credentials: "include" });
        if (res.ok) {
            const data = await res.json();
            return data.id || null;
        }
        return null;
    } catch (err) {
        console.error('Error al obtener id canción favorita:', err);
        return null;
    }
}

// Evento click en botón favorito
const btnFavorito = document.getElementById('btnFavorito');
if (btnFavorito) {
    btnFavorito.addEventListener('click', async () => {
        await añadirFavorito();
    });
}

// Evento click para Play/Pause
if (togglePlayerBtn) {
    togglePlayerBtn.addEventListener('click', togglePlay);
}
