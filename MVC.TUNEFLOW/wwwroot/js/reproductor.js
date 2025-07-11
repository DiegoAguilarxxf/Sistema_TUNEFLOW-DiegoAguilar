document.addEventListener('DOMContentLoaded', () => {
    // Referencias a elementos del DOM
    const audioPlayer = document.getElementById('audioPlayer');
    const playPauseBtn = document.getElementById('playPauseBtn');
    const playPauseIcon = document.getElementById('playPauseIcon'); // Este es el <img> dentro del botón de play/pause
    const prevBtn = document.getElementById('prevBtn');
    const nextBtn = document.getElementById('nextBtn');
    const barraProgreso = document.getElementById('barraProgreso');
    const tiempoActualSpan = document.getElementById('tiempoActual');
    const duracionTotalSpan = document.getElementById('duracionTotal');
    const volumenControl = document.getElementById('volumenControl');
    const portadaActual = document.getElementById('portadaActual');
    const cancionActual = document.getElementById('cancionActual');
    const artistaActual = document.getElementById('artistaActual'); // Asegúrate de que este elemento exista en tu HTML
    const reproductorExpandidoContenido = document.getElementById('reproductorExpandidoContenido');
    const portadaGrande = document.getElementById('portadaGrande');
    const letraGrande = document.getElementById('letraGrande');
    const lyricsBtn = document.getElementById('lyricsBtn');
    const favBtn = document.getElementById('favBtn');

    const clienteReproductor = document.getElementById('reproductor');
    const togglePlayerBtn = document.getElementById('togglePlayerBtn');

    // Variables de estado del reproductor
    let currentSongIndex = -1;
    let playlist = [];
    let cancionEnReproduccion = null; // Objeto para almacenar la canción actual y sus datos

    // --- Funciones de Utilidad ---
    function formatTime(seconds) {
        if (isNaN(seconds) || seconds < 0) return "0:00";
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = Math.floor(seconds % 60);
        return `${minutes}:${remainingSeconds < 10 ? '0' : ''}${remainingSeconds}`;
    }

    // --- Funciones de Carga y Reproducción ---

    // Función global para cargar una canción en el reproductor
    window.loadSong = (song, autoPlay = true) => {
        // Validación estricta de la canción y su fuente de audio
        if (!song || !song.src || song.src.trim() === '') {
            console.error('Canción inválida o sin URL de audio. Resetando UI del reproductor.');
            resetReproductorUI(); // Importante: Oculta el reproductor si la canción no es válida
            alert("No se pudo cargar la canción: URL de audio no válida."); // Mensaje al usuario
            return;
        }

        // Actualizar la variable global de la canción en reproducción
        cancionEnReproduccion = {
            id: song.id || null, // Añadir ID si está disponible
            title: song.title || 'Título Desconocido',
            artist: song.artist || 'Artista Desconocido',
            cover: song.cover || '/img/placeholder.png',
            src: song.src,
            lyrics: song.lyrics || '',
            currentTime: audioPlayer.currentTime || 0 // Guardar el tiempo actual si se cambia de canción
        };

        // Guardar en sessionStorage
        try {
            sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
        } catch (e) {
            console.error("Error al guardar en sessionStorage:", e);
        }

        // Mostrar el reproductor si no está activo
        if (!clienteReproductor.classList.contains('active')) {
            clienteReproductor.classList.add('active');
            // Nota: El padding-bottom del main-wrapper se ajustará vía CSS debido a esta clase
        }

        audioPlayer.src = cancionEnReproduccion.src;
        cancionActual.textContent = cancionEnReproduccion.title;
        artistaActual.textContent = cancionEnReproduccion.artist;

        portadaActual.src = cancionEnReproduccion.cover;
        portadaGrande.src = cancionEnReproduccion.cover;

        if (cancionEnReproduccion.lyrics && cancionEnReproduccion.lyrics.trim() !== '') {
            letraGrande.textContent = cancionEnReproduccion.lyrics;
            lyricsBtn.style.display = 'block';
        } else {
            letraGrande.textContent = '';
            lyricsBtn.style.display = 'none';
            letraGrande.style.display = 'none'; // Asegura que la letra esté oculta
        }

        audioPlayer.load(); // Carga la nueva fuente de audio

        // Restaurar tiempo si se proporciona (útil para reanudar)
        if (song.currentTime && song.currentTime > 0) {
            audioPlayer.currentTime = song.currentTime;
        }

        if (autoPlay) {
            playSong(); // Inicia la reproducción automáticamente
        } else {
            pauseSong(); // Asegura que el icono sea de Play si no es autoPlay
            audioPlayer.currentTime = cancionEnReproduccion.currentTime; // Restaurar el tiempo si no se reproduce automáticamente
        }
    };

    // Resetea la UI del reproductor a su estado inicial vacío y lo oculta
    function resetReproductorUI() {
        audioPlayer.pause(); // Pausa cualquier reproducción en curso
        audioPlayer.src = ''; // Elimina la fuente del audio
        audioPlayer.currentTime = 0; // Reinicia el tiempo

        cancionActual.textContent = 'Ninguna canción en reproducción';
        artistaActual.textContent = ''; // Limpiar artista
        portadaActual.src = '/img/placeholder.png';
        portadaGrande.src = '/img/placeholder.png';
        letraGrande.textContent = '';
        tiempoActualSpan.textContent = '0:00';
        duracionTotalSpan.textContent = '0:00';
        barraProgreso.value = 0;
        playPauseIcon.src = '/img/play.png'; // Cambia el icono a "play"
        lyricsBtn.style.display = 'none';

        // Asegurarse de que el reproductor está minimizado y completamente oculto
        clienteReproductor.classList.remove('expanded');
        clienteReproductor.classList.remove('active'); // ESTO ES CLAVE PARA OCULTARLO
        togglePlayerBtn.textContent = '⬆'; // Restablece el icono del toggle

        reproductorExpandidoContenido.style.display = 'none';
        portadaGrande.style.display = 'none';
        letraGrande.style.display = 'none';

        // Limpiar sessionStorage cuando el reproductor se resetea por completo
        sessionStorage.removeItem('ultimaCancion');
        cancionEnReproduccion = null; // Limpiar la variable de estado
    }

    function playSong() {
        // Solo intenta reproducir si hay una src válida y no es la URL actual de la página (indicando src vacía)
        if (audioPlayer.src && audioPlayer.src !== window.location.href) {
            audioPlayer.play()
                .then(() => {
                    playPauseIcon.src = '/img/pause.png'; // Cambia el icono a "pause"
                })
                .catch(error => {
                    console.error("Error al intentar reproducir el audio:", error);
                    // Informa al usuario que la reproducción automática pudo ser bloqueada
                    alert("El navegador bloqueó la reproducción automática o hubo un problema con el archivo de audio. Haz clic en el botón de Play para iniciar.");
                    playPauseIcon.src = '/img/play.png'; // Vuelve al icono de "play"
                });
        } else {
            console.log("No hay una URL de audio válida para reproducir.");
            // Si no hay src válida, asegura que el reproductor esté en estado de pausa
            pauseSong(); // Asegura el icono de play
        }
    }

    function pauseSong() {
        audioPlayer.pause();
        playPauseIcon.src = '/img/play.png'; // Cambia el icono a "play"
    }

    function togglePlay() {
        // Solo permitir toggle si hay una canción cargada y una src válida
        if (audioPlayer.src && audioPlayer.src !== window.location.href) {
            if (audioPlayer.paused) {
                playSong();
            } else {
                pauseSong();
            }
        } else {
            alert("No hay ninguna canción cargada para reproducir.");
            console.warn("Intento de togglePlay sin canción cargada.");
        }
    }

    // Funciones de control de playlist
    window.setPlaylist = (newPlaylist, startIndex = 0) => {
        playlist = newPlaylist;
        if (playlist.length > 0) {
            currentSongIndex = startIndex;
            loadSong(playlist[currentSongIndex]);
        } else {
            currentSongIndex = -1;
            resetReproductorUI(); // Si la playlist está vacía, resetea el UI
        }
    };

    window.anteriorCancion = () => {
        if (playlist.length === 0) {
            alert("No hay más canciones en la playlist.");
            return;
        }
        currentSongIndex = (currentSongIndex - 1 + playlist.length) % playlist.length;
        loadSong(playlist[currentSongIndex]);
    };

    window.siguienteCancion = () => {
        if (playlist.length === 0) {
            alert("No hay más canciones en la playlist.");
            return;
        }
        currentSongIndex = (currentSongIndex + 1) % playlist.length;
        loadSong(playlist[currentSongIndex]);
    };

    window.añadirFavorito = () => {
        if (!cancionEnReproduccion || !cancionEnReproduccion.id) {
            alert('No hay una canción seleccionada para añadir a favoritos.');
            return;
        }
        // Aquí iría tu lógica de fetch al backend para añadir a favoritos
        console.log(`Intentando añadir a favoritos la canción ID: ${cancionEnReproduccion.id}`);
        alert(`Funcionalidad de añadir a favoritos para "${cancionEnReproduccion.title}" (implementar en el backend)`);
    };

    // --- Manejo de Eventos del Audio Player ---
    audioPlayer.addEventListener('timeupdate', () => {
        tiempoActualSpan.textContent = formatTime(audioPlayer.currentTime);
        if (!isNaN(audioPlayer.duration) && audioPlayer.duration > 0) {
            const progress = (audioPlayer.currentTime / audioPlayer.duration) * 100;
            barraProgreso.value = progress;
            // Actualizar el tiempo en el objeto de la canción y sessionStorage
            if (cancionEnReproduccion) {
                cancionEnReproduccion.currentTime = audioPlayer.currentTime;
                try {
                    sessionStorage.setItem('ultimaCancion', JSON.stringify(cancionEnReproduccion));
                } catch (e) {
                    console.error("Error al guardar el tiempo en sessionStorage:", e);
                }
            }
        }
    });

    audioPlayer.addEventListener('loadedmetadata', () => {
        duracionTotalSpan.textContent = formatTime(audioPlayer.duration);
        barraProgreso.max = 100;
        barraProgreso.value = 0; // Reinicia el progreso al cargar nueva metadata

        // Si hay una canción en reproducción guardada y un tiempo, restaurarlo
        if (cancionEnReproduccion && cancionEnReproduccion.currentTime > 0) {
            audioPlayer.currentTime = cancionEnReproduccion.currentTime;
        }
    });

    audioPlayer.addEventListener('ended', () => {
        if (playlist.length > 0 && currentSongIndex !== -1) {
            siguienteCancion();
        } else {
            // Si no hay playlist o la última canción terminó
            pauseSong();
            barraProgreso.value = 0;
            tiempoActualSpan.textContent = '0:00';
            // Opcional: podrías llamar a resetReproductorUI() aquí si quieres que el reproductor se oculte
            // después de que la última canción en una playlist vacía termine.
        }
    });

    audioPlayer.addEventListener('error', (e) => {
        console.error("Error de audio:", e);
        if (audioPlayer.error) {
            console.error("Código de error:", audioPlayer.error.code);
            console.error("Mensaje de error:", audioPlayer.error.message);
        }
        alert('No se pudo reproducir la canción. El archivo de audio podría estar dañado o no ser accesible.'); // Mensaje de error
        playPauseIcon.src = '/img/play.png'; // Muestra el icono de play
        // No llamamos a resetReproductorUI aquí para que el usuario pueda ver el reproductor
        // e intentar de nuevo, o para ver qué canción intentó cargar.
    });

    // --- Adjuntar Eventos a los Botones del Reproductor ---
    playPauseBtn.addEventListener('click', togglePlay); // Asegúrate de que este botón existe en tu HTML y tiene el ID 'playPauseBtn'
    if (prevBtn) prevBtn.addEventListener('click', anteriorCancion);
    if (nextBtn) nextBtn.addEventListener('click', siguienteCancion);
    if (favBtn) favBtn.addEventListener('click', añadirFavorito);

    barraProgreso.addEventListener('input', () => {
        if (!isNaN(audioPlayer.duration) && audioPlayer.duration > 0) {
            audioPlayer.currentTime = (barraProgreso.value / 100) * audioPlayer.duration;
        }
    });

    volumenControl.addEventListener('input', () => {
        audioPlayer.volume = volumenControl.value;
    });

    // --- Funcionalidad del Botón de Toggle Reproductor (Expandir/Minimizar) ---
    togglePlayerBtn.addEventListener('click', () => {
        // Solo permite expandir/minimizar si el reproductor está "activo" (es decir, tiene una canción potencialmente cargada)
        if (!clienteReproductor.classList.contains('active') && !clienteReproductor.classList.contains('expanded')) {
            alert("No hay ninguna canción reproduciéndose para expandir el reproductor.");
            return;
        }

        clienteReproductor.classList.toggle('expanded');
        const isExpanded = clienteReproductor.classList.contains('expanded');
        togglePlayerBtn.textContent = isExpanded ? '⬇' : '⬆';

        if (isExpanded) {
            reproductorExpandidoContenido.style.display = 'flex';
            portadaGrande.style.display = 'block';
            if (letraGrande.textContent.trim() !== '') {
                letraGrande.style.display = 'block';
            } else {
                letraGrande.style.display = 'none';
            }
        } else {
            reproductorExpandidoContenido.style.display = 'none';
            portadaGrande.style.display = 'none';
            letraGrande.style.display = 'none';
        }
    });

    lyricsBtn.addEventListener('click', () => {
        if (clienteReproductor.classList.contains('expanded') && letraGrande.textContent.trim() !== '') {
            letraGrande.style.display = letraGrande.style.display === 'none' ? 'block' : 'none';
        } else if (!clienteReproductor.classList.contains('expanded')) {
            alert('Expande el reproductor para ver la letra.');
        } else {
            alert('Esta canción no tiene letra disponible.');
        }
    });

    // --- Inicialización del Reproductor ---
    // Establece el volumen inicial del reproductor de audio al valor del control deslizante
    audioPlayer.volume = volumenControl.value;

    // **RESTURACIÓN DE LA ÚLTIMA CANCIÓN AL CARGAR LA PÁGINA**
    const storedSongData = sessionStorage.getItem('ultimaCancion');
    if (storedSongData) {
        try {
            const song = JSON.parse(storedSongData);
            // Cargar la canción, pero no reproducirla automáticamente si no queremos
            // o si el navegador bloquea el autoplay sin interacción del usuario.
            // Le pasamos `false` al autoPlay para que el usuario tenga que dar play si lo desea.
            // El `currentTime` se restaurará en el `loadedmetadata` event listener.
            window.loadSong(song, false);
            // Si la canción estaba en pausa, asegúrate de que el icono de play sea visible.
            playPauseIcon.src = '/img/play.png';
            console.log("Última canción restaurada:", song.title, "en tiempo:", song.currentTime);
        } catch (e) {
            console.error("Error al parsear la última canción de sessionStorage:", e);
            resetReproductorUI(); // Limpiar si hay datos corruptos
        }
    } else {
        // Si no hay canción guardada, asegúrate de que el reproductor esté oculto al inicio
        resetReproductorUI();
    }

    // Evento para manejar el cambio de tema para actualizar filtros de iconos si es necesario
    document.addEventListener('themeChanged', (e) => {
        const currentTheme = e.detail;
        // La lógica de filtro de iconos ya está en CSS, esto es solo un placeholder
    });
});