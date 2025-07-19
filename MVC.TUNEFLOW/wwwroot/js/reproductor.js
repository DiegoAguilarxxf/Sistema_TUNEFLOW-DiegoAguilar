<script>
    // 🎨 TEMA (Claro / Oscuro)
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

    toggle?.addEventListener('change', () => {
        setTheme(toggle.checked ? 'dark' : 'light');
    });

    loadTheme();

    // 🎧 REPRODUCTOR
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
        id: 0,
    titulo: '',
    url: '',
    portada: '',
    tiempo: 0,
    idCliente: 0
    };

    // 🔄 Restaurar canción previa
    document.addEventListener('DOMContentLoaded', () => {
        const data = sessionStorage.getItem('ultimaCancion');
    if (data) {
            const c = JSON.parse(data);
    reproducirCancion(c.id, c.titulo, c.url, c.portada, c.idCliente, c.tiempo, false);
        }
    });

    // 🔊 Volumen y progreso
    volumenControl?.addEventListener('input', () => {
        audioPlayer.volume = volumenControl.value;
    });

    barraProgreso?.addEventListener('input', () => {
        const nuevoTiempo = (barraProgreso.value / 100) * audioPlayer.duration;
    audioPlayer.currentTime = nuevoTiempo;
    });

    audioPlayer?.addEventListener('timeupdate', () => {
        const progreso = (audioPlayer.currentTime / audioPlayer.duration) * 100;
    barraProgreso.value = progreso || 0;
    tiempoActual.textContent = formatearTiempo(audioPlayer.currentTime);
    duracionTotal.textContent = formatearTiempo(audioPlayer.duration);

    // Guardar sesión
    cancionEnReproduccion.tiempo = audioPlayer.currentTime;
    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
    });

    function formatearTiempo(segundos) {
        if (isNaN(segundos)) return '0:00';
    const m = Math.floor(segundos / 60);
    const s = Math.floor(segundos % 60).toString().padStart(2, '0');
    return `${m}:${s}`;
    }

    // ▶️ Reproducir canción
    async function reproducirCancion(id, titulo, url, portada, idCliente, tiempo = 0, autoPlay = true) {
        cancionEnReproduccion = { id, titulo, url, portada, tiempo, idCliente };
    cancionActual.textContent = titulo;
    portadaActual.src = portada;
    audioPlayer.src = url;
    audioPlayer.currentTime = tiempo;

    if (autoPlay) {
            try {
        await audioPlayer.play();
    estaReproduciendo = true;
    iconoPlayPause.src = '~/img/icons/pause.png';
            } catch (err) {
        console.error("Error al reproducir:", err);
            }
        }

    const esFavorita = await ComprobarEsCancionfavorita(id, idCliente);
    actualizarBotonFavorito(esFavorita);
    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
    }

    function togglePlay() {
        if (!audioPlayer.src) return;
    if (audioPlayer.paused) {
        audioPlayer.play().then(() => {
            estaReproduciendo = true;
            iconoPlayPause.src = '~/img/icons/pause.png';
        });
        } else {
        audioPlayer.pause();
    estaReproduciendo = false;
    iconoPlayPause.src = '~/img/icons/play.png';
        }
    }

    function siguienteCancion() {
        alert("Siguiente canción no implementada.");
    }

    function anteriorCancion() {
        alert("Anterior canción no implementada.");
    }

    async function añadirFavorito() {
        const {id, idCliente} = cancionEnReproduccion;
    if (!id || !idCliente) {
        alert("No hay canción en reproducción.");
    return;
        }

    const esFavorita = await ComprobarEsCancionfavorita(id, idCliente);
    const idPlaylistFavoritos = await ExtraerPlaylistDeFavortos(idCliente);

    if (esFavorita) {
            const idMusicaPlaylist = await ObtenerIdDeMusicaPlaylist(id, idPlaylistFavoritos);
    const idCancionesFavoritas = await ObtenerIdDeCancionFavorita(id, idCliente);

    await fetch(`https://localhost:7031/api/MusicasPlaylists/${idMusicaPlaylist}`, {method: "DELETE", credentials: "include" });
    await fetch(`https://localhost:7031/api/CancionesFavoritas/${idCancionesFavoritas}`, {method: "DELETE", credentials: "include" });

    actualizarBotonFavorito(false);
    alert("🎵 Canción eliminada de favoritos.");
        } else {
        await fetch("https://localhost:7031/api/MusicasPlaylists", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ PlaylistId: idPlaylistFavoritos, CancionId: id })
        });

    await fetch("https://localhost:7031/api/CancionesFavoritas", {
        method: "POST",
    headers: {"Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify({
        ClienteId: idCliente,
    CancionId: id,
    FechaAgregado: new Date().toISOString()
                })
            });

    actualizarBotonFavorito(true);
    alert("🎵 Añadida a favoritos correctamente.");
        }
    }

    async function ExtraerPlaylistDeFavortos(id) {
        const res = await fetch(`https://localhost:7031/api/Playlists/PlaylistFavoritos/${id}`, {credentials: "include" });
    return res.ok ? await res.json() : null;
    }

    async function ComprobarEsCancionfavorita(idCancion, idCliente) {
        const res = await fetch(`https://localhost:7031/api/CancionesFavoritas/IsFavorita/${idCancion}/${idCliente}`, {credentials: "include" });
    if (!res.ok) return false;
    const data = await res.json();
    return !!data.id;
    }

    async function ObtenerIdDeCancionFavorita(idCancion, idCliente) {
        const res = await fetch(`https://localhost:7031/api/CancionesFavoritas/IsFavorita/${idCancion}/${idCliente}`, {credentials: "include" });
    return res.ok ? (await res.json()).id : null;
    }

    async function ObtenerIdDeMusicaPlaylist(idCancion, idPlaylist) {
        const res = await fetch(`https://localhost:7031/api/MusicasPlaylists/ExistMusicaPlaylist/${idCancion}/${idPlaylist}`, { credentials: "include" });
        return res.ok ? (await res.json()).id : null;
    }

    function actualizarBotonFavorito(esFavorita) {
        const btn = document.getElementById('btnfavorito');
        if (!btn) return;

        const icono = btn.querySelector("img");
        if (!icono) return;

        if (esFavorita) {
            btn.classList.remove('btn-outline-warning');
            btn.classList.add('btn-warning');
            icono.src = '~/img/icons/me-gusta.png'; // Corazón lleno
            btn.title = "Quitar de favoritos";
        } else {
            btn.classList.remove('btn-warning');
            btn.classList.add('btn-outline-warning');
            icono.src = '~/img/icons/me-gusta.png'; // Corazón vacío (puedes usar otra img si tienes)
            btn.title = "Añadir a favoritos";
        }
    }

    // Expandir/minimizar reproductor
    togglePlayerBtn?.addEventListener('click', () => {
        reproductor.classList.toggle('reducido');
        togglePlayerBtn.textContent = reproductor.classList.contains('reducido') ? '⬆' : '⬇';

        const size = reproductor.classList.contains('reducido') ? "60px" : "200px";
        portadaActual.style.height = size;
        portadaActual.style.width = size;
    });

    // Registrar funciones globales
    window.reproducirCancion = reproducirCancion;
    window.añadirFavorito = añadirFavorito;
    window.togglePlay = togglePlay;
    window.siguienteCancion = siguienteCancion;
    window.anteriorCancion = anteriorCancion;
</script>
n = anteriorCancion;