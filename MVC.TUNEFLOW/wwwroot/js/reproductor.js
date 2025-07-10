const html = document.documentElement;
const toggle = document.getElementById('theme-toggle');
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

// 🎵 Lógica del reproductor
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
    tiempo: 0
};

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
    cancionEnReproduccion.tiempo = audioPlayer.currentTime;
    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
});

function formatearTiempo(segundos) {
    if (isNaN(segundos)) return '0:00';
    const m = Math.floor(segundos / 60);
    const s = Math.floor(segundos % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
}

function reproducirCancion(titulo, url, portada, tiempo = 0, autoPlay = true) {
    cancionEnReproduccion = { titulo, url, portada, tiempo };
    cancionActual.textContent = titulo;
    portadaActual.src = portada;
    audioPlayer.src = url;
    audioPlayer.currentTime = tiempo || 0;

    if (autoPlay) {
        audioPlayer.play()
            .then(() => estaReproduciendo = true)
            .catch(err => console.error(err));
    }

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

function añadirFavorito() {
    alert(`Añadido a favoritos: ${cancionEnReproduccion.titulo}`);
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

window.reproducirCancion = reproducirCancion;