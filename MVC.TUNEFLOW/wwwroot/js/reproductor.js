// --- VARIABLES Y ELEMENTOS DOM ---
const STORAGE_KEY = 'historialCanciones';
const STORAGE_INDEX = 'indiceActual';

const portadaGrande = document.getElementById('portadaGrande');
const contenedorLetra = document.getElementById('contenedorLetra'); // Debes crear este div en HTML para la letra
const textoLetra = document.getElementById('textoLetra'); // Dentro de contenedorLetra

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

let historialCanciones = [];
let indiceActual = -1;
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

// --- FUNCIONES DE HISTORIAL ---

function cargarHistorial() {
    const guardado = localStorage.getItem(STORAGE_KEY);
    if (guardado) {
        historialCanciones = JSON.parse(guardado);
    }
    const indiceGuardado = localStorage.getItem(STORAGE_INDEX);
    if (indiceGuardado !== null) {
        indiceActual = parseInt(indiceGuardado, 10);
    }
}

function guardarHistorial() {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(historialCanciones));
    localStorage.setItem(STORAGE_INDEX, indiceActual.toString());
}

function reproducirDesdeHistorial(indice) {
    if (indice < 0 || indice >= historialCanciones.length) return;
    const cancion = historialCanciones[indice];
    indiceActual = indice;
    guardarHistorial();
    reproducirCancion(cancion.id, cancion.titulo, cancion.url, cancion.portada, cancion.idCliente, 0, true, cancion.artista);
}

function agregarYReproducir(cancion) {
    // Evitar duplicar canción si ya es la actual
    if (
        indiceActual >= 0 &&
        historialCanciones[indiceActual]?.id === cancion.id
    ) {
        reproducirCancion(cancion.id, cancion.titulo, cancion.url, cancion.portada, cancion.idCliente, 0, true, cancion.artista);
        return;
    }

    // Si navegamos en medio del historial y agregamos nueva canción, cortamos adelante
    if (indiceActual < historialCanciones.length - 1) {
        historialCanciones = historialCanciones.slice(0, indiceActual + 1);
    }

    historialCanciones.push(cancion);
    indiceActual = historialCanciones.length - 1;
    guardarHistorial();

    reproducirCancion(cancion.id, cancion.titulo, cancion.url, cancion.portada, cancion.idCliente, 0, true, cancion.artista);
}

function anteriorCancion() {
    if (indiceActual > 0) {
        reproducirDesdeHistorial(indiceActual - 1);
    }
}

function siguienteCancion() {
    if (indiceActual < historialCanciones.length - 1) {
        reproducirDesdeHistorial(indiceActual + 1);
    }
}

// --- FUNCIONES DEL REPRODUCTOR ---

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
                const esFavorito = comprobarIsFavorito();
                actualizarBotonFavorito(esFavorito);
            } catch (err) {
                console.error('Error al reproducir:', err);
            }
        }
    }

    document.documentElement.style.setProperty('--portada-fondo', `url('${portada}')`);
    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));

    // Ocultar letra si estaba visible al cambiar canción
    if (mostrandoLetra) {
        toggleLetra(false);
    }
}

// --- EVENTOS DEL REPRODUCTOR ---

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
    });
}

// --- FUNCION TOGGLE PLAY/PAUSE ---

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

// --- FUNCIONALIDAD LETRA ---

async function obtenerLetra(artista, titulo) {
    try {
        const url = `https://api.lyrics.ovh/v1/${encodeURIComponent(artista)}/${encodeURIComponent(titulo)}`;
        const res = await fetch(url);
        if (!res.ok) throw new Error('No encontrada');
        const data = await res.json();
        return data.lyrics;
    } catch (error) {
        console.error('Error al obtener la letra:', error);
        return 'No se encontró la letra para esta canción.';
    }
}

function toggleLetra(estado) {
    mostrandoLetra = estado;
    if (mostrandoLetra) {
        if (portadaGrande) portadaGrande.style.display = 'none';
        if (contenedorLetra) contenedorLetra.style.display = 'block';
    } else {
        if (contenedorLetra) contenedorLetra.style.display = 'none';
        if (portadaGrande) portadaGrande.style.display = 'block';
    }
}

async function verLetra() {
    if (!mostrandoLetra) {
        toggleLetra(true);
        textoLetra.textContent = 'Cargando letra...';

        const artista = cancionEnReproduccion.artista || 'Desconocido';
        const titulo = cancionEnReproduccion.titulo || 'Desconocida';

        const letra = await obtenerLetra(artista, titulo);
        textoLetra.textContent = letra;
    } else {
        toggleLetra(false);
    }
}

// --- FUNCION DESCARGAR CANCION ---

function descargarCancion() {
    if (!cancionEnReproduccion.url) {
        alert('No hay canción para descargar.');
        return;
    }

    const enlace = document.createElement('a');
    enlace.href = cancionEnReproduccion.url;

    const extension = cancionEnReproduccion.url.split('.').pop().split('?')[0];
    const nombreArchivo = `${cancionEnReproduccion.titulo || 'cancion'}.${extension}`;

    enlace.download = nombreArchivo;
    document.body.appendChild(enlace);
    enlace.click();
    document.body.removeChild(enlace);
}

// --- EXPANDIR/MINIMIZAR REPRODUCTOR ---

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

// --- CARGAR HISTORIAL Y ÚLTIMA CANCION AL INICIO ---

document.addEventListener('DOMContentLoaded', () => {
    cargarHistorial();

    if (indiceActual >= 0 && historialCanciones[indiceActual]) {
        const c = historialCanciones[indiceActual];
        reproducirCancion(c.id, c.titulo, c.url, c.portada, c.idCliente, c.tiempo, false, c.artista);
    } else {
        // Si no hay historial, cargar última canción en sessionStorage
        const guardada = sessionStorage.getItem('ultimaCancion');
        if (guardada) {
            const c = JSON.parse(guardada);
            agregarYReproducir(c);
        }
    }
});

//Funcionalidad de Favoritos

async function funcionFavorito() {
    const estaFavorito = await comprobarIsFavorito();
    console.log("entro a la funcion Favorito");

    const respuestaIdPlaylistFavoritos = await fetch(`https://localhost:7031/api/Playlists/FavoritesPlaylist/${cancionEnReproduccion.idCliente}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
    });

    let idMeGustas = 0;
    if (respuestaIdPlaylistFavoritos.ok) {
        const data = await respuestaIdPlaylistFavoritos.json();
        console.log("Respuesta de la API:", data);
        idMeGustas = data;
    }
    console.log(`Id de la playlist Me Gustas: ${idMeGustas}`);
    if (estaFavorito) {
        eliminarFavorito(idMeGustas);
    } else {
        agregarFavorito(cancionEnReproduccion.id, cancionEnReproduccion.idCliente, idMeGustas);
    }
}

function actualizarBotonFavorito(esFavorito) {
    const img = document.getElementById('imgFavorito');

    if (!img) return;

    if (esFavorito) {
        img.src = '/img/icons/me-gusta-add.png'; // imagen si es favorito
    } else {
        img.src = '/img/icons/me-gusta.png'; // imagen si no es favorito
    }
}
async function comprobarIsFavorito() {
    try {

        const response = await fetch(`https://localhost:7031/api/FavoriteSongs/IsFavorite/${cancionEnReproduccion.id}/${cancionEnReproduccion.idCliente}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const data = await response.json();
            console.log("Es favorita");
            return true;

        } else if (response.status === 404) {
            console.log("No es favorita");
            return false;
        } else {
            throw new Error('Error inesperado');
            return false;
        }

    } catch (error) {
        console.error('Error al comprobar si es favorito:', error);
        return false;
    }
}

async function agregarFavorito(idCancion, idCliente, idMeGustas) {
    try {
        if (idCancion == null || idCliente == null) {
            throw new Error('Error inesperado');
        }

        const favoriteSong = {
            id: 0,
            clientId: cancionEnReproduccion.idCliente,
            songId: cancionEnReproduccion.id,
            dateAdded: new Date().toISOString()  // fecha actual en formato ISO
        };

        const response = await fetch(`https://localhost:7031/api/FavoriteSongs`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(favoriteSong)
        });

        if (!response.ok) throw new Error('Error al agregar favorito');

        const favoritoCreado = await response.json(); 

        if (favoritoCreado && favoritoCreado.id) {
            console.log('Favorito agregado con ID:', favoritoCreado.id);
            alert('Canción agregada a Favoritos.');
            actualizarBotonFavorito(true);

            const bodySongPlaylist = {
                "id": 0,
                "songId": cancionEnReproduccion.id,
                "playlistId": idMeGustas
            };
        
            const respuestaAgregarPlaylist = await fetch(`https://localhost:7031/api/SongsPlaylists`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(bodySongPlaylist)

            });

            if (!respuestaAgregarPlaylist.ok) throw new Error('Error al agregar canción a la playlist de favoritos');

            return true;
        } else {
            console.log('No se recibió el objeto favorito correctamente');
            return false;
        }

    } catch (error) {
        return true;
    }
}

async function eliminarFavorito(idMeGustas) {
    const favoriteId = await obtenerIdFavorito();

    console.log(`Id de favoritesSongs: ${favoriteId}`);
    if (favoriteId != null && favoriteId !== false) {
        try {
            const response = await fetch(`https://localhost:7031/api/FavoriteSongs/${favoriteId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                console.log('Canción eliminada de favoritos.');
                alert('Canción eliminada de favoritos.');
                actualizarBotonFavorito(false);

                const respuestaEcontrarSongPlaylist = await fetch(`https://localhost:7031/api/SongsPlaylists/ExistSongPlaylist/${cancionEnReproduccion.id}/${idMeGustas}`)

                if (respuestaEcontrarSongPlaylist.ok) {
                    const data = await respuestaEcontrarSongPlaylist.json();

                    var responseEliminacion = await fetch(`https://localhost:7031/api/SongsPlaylists/${data}`, {
                        method: "DELETE",
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    });
                    if (!responseEliminacion) throw new Error("Error en la eliminacion ´de la musica de la playlist");
                    
                }
            } else {
                console.error('Error al eliminar favorito:', response.status);
            }
        } catch (error) {
            console.error('Error en la solicitud DELETE:', error);
        }
    } else {
        console.log("No es favorito o no se pudo obtener el ID.");
    }
}

async function obtenerIdFavorito() {
    try {

        const response = await fetch(`https://localhost:7031/api/FavoriteSongs/IsFavorite/${cancionEnReproduccion.id}/${cancionEnReproduccion.idCliente}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (response.ok) {
            const data = await response.json();
            return data.id;

        } else if (response.status === 404) {
            return null;
        } else {
            throw new Error('Error inesperado');
        }

    } catch (error) {
        console.error('Error al comprobar si es favorito:', error);
        return null;
    }
}
