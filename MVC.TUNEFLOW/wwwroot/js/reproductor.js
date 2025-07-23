// --- VARIABLES Y ELEMENTOS DOM ---
const audioPlayer = document.getElementById('audioPlayer');
const cancionActual = document.getElementById('cancionActual');
const portadaActual = document.getElementById('portadaActual');
const portadaGrande = document.getElementById('portadaGrande');
const volumenControl = document.getElementById('volumenControl');
const barraProgreso = document.getElementById('barraProgreso');
const tiempoActual = document.getElementById('tiempoActual');
const duracionTotal = document.getElementById('duracionTotal');
const reproductor = document.getElementById('reproductor');
const togglePlayerBtn = document.getElementById('togglePlayerBtn');
const iconoPlayPause = document.getElementById('icono-play-pause');
const contenedorLetra = document.getElementById('contenedorLetra');
const textoLetra = document.getElementById('textoLetra');

let historialCanciones = JSON.parse(localStorage.getItem('historialCanciones') || '[]');
let indiceActual = parseInt(localStorage.getItem('indiceActual') || '-1');
let mostrandoLetra = false;
let estaReproduciendo = false;

let cancionEnReproduccion = {
    id: '',
    titulo: '',
    artista: '',
    url: '',
    portada: '',
    tiempo: 0,
    idCliente: 0
};

// --- AUXILIARES ---
function formatearTiempo(segundos) {
    if (isNaN(segundos) || !segundos) return '0:00';
    const m = Math.floor(segundos / 60);
    const s = Math.floor(segundos % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
}

function actualizarIconoPlayPause() {
    if (!iconoPlayPause) return;
    if (estaReproduciendo) {
        iconoPlayPause.src = "/img/icons/pausa.png";
        iconoPlayPause.alt = "Pause";
    } else {
        iconoPlayPause.src = "/img/icons/play.png";
        iconoPlayPause.alt = "Play";
    }
}

function guardarHistorial() {
    localStorage.setItem('historialCanciones', JSON.stringify(historialCanciones));
    localStorage.setItem('indiceActual', indiceActual.toString());
}

function mostrarNotificacion(mensaje, tipo = 'info') {
    let notif = document.getElementById('notificacion');
    if (!notif) {
        notif = document.createElement('div');
        notif.id = 'notificacion';
        notif.style.cssText = `
            position: fixed; top: 20px; right: 20px; z-index: 9999;
            padding: 15px; border-radius: 5px; color: white;
            max-width: 300px; opacity: 0; transition: opacity 0.3s;
        `;
        document.body.appendChild(notif);
    }
    notif.className = tipo === 'error' ? 'alert alert-danger' : 'alert alert-success';
    notif.textContent = mensaje;
    notif.style.opacity = '1';

    setTimeout(() => {
        notif.style.opacity = '0';
        setTimeout(() => notif.remove(), 300);
    }, 3000);
}

// --- REPRODUCCIÓN ---
async function reproducirCancion(id, titulo, url, portada, idCliente, tiempo = 0, autoPlay = true, artista = '') {
    cancionEnReproduccion = { id, titulo, url, portada, tiempo, idCliente, artista };

    if (portadaGrande) portadaGrande.src = portada;
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
                await actualizarBotonFavorito();
            } catch (err) {
                console.error('Error al reproducir:', err);
                mostrarNotificacion('Error al reproducir la canción', 'error');
            }
        }
    }

    document.documentElement.style.setProperty('--portada-fondo', `url('${portada}')`);
    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));

    if (mostrandoLetra) toggleLetra(false);
}

function agregarYReproducir(cancion) {
    if (indiceActual >= 0 && historialCanciones[indiceActual]?.id === cancion.id) {
        reproducirCancion(cancion.id, cancion.titulo, cancion.url, cancion.portada, cancion.idCliente, 0, true, cancion.artista);
        return;
    }
    if (indiceActual < historialCanciones.length - 1) {
        historialCanciones = historialCanciones.slice(0, indiceActual + 1);
    }
    historialCanciones.push(cancion);
    indiceActual = historialCanciones.length - 1;
    guardarHistorial();
    reproducirCancion(cancion.id, cancion.titulo, cancion.url, cancion.portada, cancion.idCliente, 0, true, cancion.artista);
}

// --- NAVEGACIÓN ---
async function siguienteCancion() {
    if (indiceActual < historialCanciones.length - 1) {
        indiceActual++;
        const cancion = historialCanciones[indiceActual];
        guardarHistorial();
        reproducirCancion(cancion.id, cancion.titulo, cancion.url, cancion.portada, cancion.idCliente, 0, true, cancion.artista);
        return;
    }
    try {
        const response = await fetch('/Cliente/Reproductor/SiguienteCancion');
        const data = await response.json();
        if (data.success) {
            agregarYReproducir(data.cancion);
        } else {
            mostrarNotificacion('No se pudo obtener la siguiente canción', 'error');
        }
    } catch (error) {
        console.error(error);
        mostrarNotificacion('Error al cargar la siguiente canción', 'error');
    }
}

async function cancionAnterior() {
    if (indiceActual > 0) {
        indiceActual--;
        const cancion = historialCanciones[indiceActual];
        guardarHistorial();
        reproducirCancion(cancion.id, cancion.titulo, cancion.url, cancion.portada, cancion.idCliente, 0, true, cancion.artista);
    } else {
        mostrarNotificacion('No hay canciones anteriores en el historial', 'info');
    }
}

// --- FAVORITOS ---
async function actualizarBotonFavorito() {
    try {
        const response = await fetch(`/Cliente/Reproductor/ComprobarFavorito?songId=${cancionEnReproduccion.id}`);
        const data = await response.json();
        const img = document.getElementById('imgFavorito');
        if (img) {
            img.src = data.esFavorito ? '/img/icons/me-gusta-add.png' : '/img/icons/me-gusta.png';
        }
    } catch (error) {
        console.error('Error al comprobar favorito:', error);
    }
}

async function funcionFavorito() {
    try {
        const verificar = await fetch(`/Cliente/Reproductor/ComprobarFavorito?songId=${cancionEnReproduccion.id}`);
        const { esFavorito } = await verificar.json();

        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const formData = new FormData();
        formData.append('songId', cancionEnReproduccion.id);
        formData.append('__RequestVerificationToken', token);

        const url = esFavorito ? '/Cliente/Reproductor/EliminarFavorito' : '/Cliente/Reproductor/AgregarFavorito';

        const response = await fetch(url, {
            method: 'POST',
            body: formData
        });

        const resultado = await response.json();
        mostrarNotificacion(resultado.message, resultado.success ? 'success' : 'error');
        if (resultado.success) await actualizarBotonFavorito();
    } catch (error) {
        console.error(error);
        mostrarNotificacion('Error al procesar favorito', 'error');
    }
}

// --- PLAYLIST ---
async function agregarACualquierPlaylist() {
    try {
        const response = await fetch('/Cliente/Reproductor/GetPlaylistsCliente');
        const data = await response.json();

        if (data.success) {
            mostrarModalPlaylists(data.playlists);
        } else {
            mostrarNotificacion('Error al cargar playlists', 'error');
        }
    } catch (error) {
        console.error(error);
        mostrarNotificacion('Error al cargar playlists', 'error');
    }
}

function mostrarModalPlaylists(playlists) {
    const lista = document.getElementById('listaPlaylists');
    lista.innerHTML = '';
    if (playlists.length === 0) {
        lista.innerHTML = '<li style="padding:20px; text-align:center;">No tienes playlists creadas.</li>';
    } else {
        playlists.forEach(pl => {
            const item = document.createElement('li');
            item.className = 'playlist-item';
            item.style.cssText = `
                cursor:pointer; padding:15px; border-bottom:1px solid #333;
                display:flex; align-items:center; gap:10px;
                transition: background-color 0.2s;
            `;
            item.innerHTML = `
                <img src="${pl.playlistCover || '/img/icons/default-cover.png'}" alt="Portada" style="width:50px; height:50px; object-fit:cover; border-radius:5px;">
                <span style="font-size:16px;">${pl.title}</span>
            `;
            item.onmouseenter = () => item.style.backgroundColor = '#333';
            item.onmouseleave = () => item.style.backgroundColor = 'transparent';
            item.onclick = () => agregarCancionAPlaylist(pl.id);
            lista.appendChild(item);
        });
    }
    document.getElementById('overlayModal').style.display = 'block';
}

async function agregarCancionAPlaylist(playlistId) {
    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const formData = new FormData();
        formData.append('songId', cancionEnReproduccion.id);
        formData.append('playlistId', playlistId);
        formData.append('__RequestVerificationToken', token);

        const response = await fetch('/Cliente/Reproductor/AgregarAPlaylist', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();
        mostrarNotificacion(result.message, result.success ? 'success' : 'error');
        cerrarModal();
    } catch (error) {
        console.error(error);
        mostrarNotificacion('Error al agregar a playlist', 'error');
    }
}

function cerrarModal() {
    document.getElementById('overlayModal').style.display = 'none';
}

// --- LETRA ---
async function verLetra() {

}

function toggleLetra(estado) {
    mostrandoLetra = estado;
    console.log("Estado mostrar letra:", mostrandoLetra);

    if (mostrandoLetra) {
        if (portadaGrande) {
            portadaGrande.style.display = 'none';
            console.log("Ocultando portada grande");
        }
        if (contenedorLetra) {
            contenedorLetra.style.display = 'block';
            console.log("Mostrando contenedor de letra");
        }
    } else {
        if (contenedorLetra) {
            contenedorLetra.style.display = 'none';
            console.log("Ocultando contenedor de letra");
        }
        if (portadaGrande) {
            portadaGrande.style.display = 'block';
            console.log("Mostrando portada grande");
        }
    }
}



// --- OTROS ---
function togglePlay() {
    if (!audioPlayer || !audioPlayer.src) return;

    if (audioPlayer.paused) {
        audioPlayer.play()
            .then(() => {
                estaReproduciendo = true;
                actualizarIconoPlayPause();
            })
            .catch(err => {
                console.error('Error al reproducir:', err);
                mostrarNotificacion('Error al reproducir', 'error');
            });
    } else {
        audioPlayer.pause();
        estaReproduciendo = false;
        actualizarIconoPlayPause();
    }
}

function descargarCancion() {
    if (!cancionEnReproduccion.url) {
        mostrarNotificacion('No hay canción para descargar', 'error');
        return;
    }

    const enlace = document.createElement('a');
    enlace.href = cancionEnReproduccion.url;
    const extension = cancionEnReproduccion.url.split('.').pop().split('?')[0];
    enlace.download = `${cancionEnReproduccion.titulo || 'cancion'}.${extension}`;

    document.body.appendChild(enlace);
    enlace.click();
    document.body.removeChild(enlace);
}

// --- EVENT LISTENERS ---
if (volumenControl) {
    volumenControl.addEventListener('input', () => {
        if (audioPlayer) audioPlayer.volume = volumenControl.value;
    });
}

if (barraProgreso) {
    barraProgreso.addEventListener('input', () => {
        if (audioPlayer && audioPlayer.duration) {
            const nuevoTiempo = (barraProgreso.value / 100) * audioPlayer.duration;
            audioPlayer.currentTime = nuevoTiempo;
        }
    });
}

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
        siguienteCancion(); // Auto-avanzar
    });
}

if (togglePlayerBtn) {
    togglePlayerBtn.addEventListener('click', () => {
        reproductor.classList.toggle('expanded');
        reproductor.classList.toggle('reducido');

        if (reproductor.classList.contains('expanded')) {
            document.documentElement.style.setProperty('--portada-fondo', `url('${cancionEnReproduccion.portada}')`);
            togglePlayerBtn.textContent = '⬇';
        } else {
            document.documentElement.style.setProperty('--portada-fondo', 'none');
            togglePlayerBtn.textContent = '⬆';
        }
    });
}

// Cerrar modal al hacer clic fuera
window.addEventListener('click', function (e) {
    const overlay = document.getElementById('overlayModal');
    if (overlay && e.target === overlay) {
        cerrarModal();
    }
});

// --- INICIALIZACIÓN ---
document.addEventListener('DOMContentLoaded', () => {
    if (indiceActual >= 0 && historialCanciones[indiceActual]) {
        const c = historialCanciones[indiceActual];
        reproducirCancion(c.id, c.titulo, c.url, c.portada, c.idCliente, c.tiempo, false, c.artista);
    } else {
        const guardada = sessionStorage.getItem('ultimaCancion');
        if (guardada) {
            const c = JSON.parse(guardada);
            agregarYReproducir(c);
        }
    }
});
