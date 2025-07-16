using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Pagos;
using Modelos.Tuneflow.Playlist;
using Modelos.Tuneflow.Usuario.Administracion;
using Modelos.Tuneflow.Usuario.Consumidor;
using Modelos.Tuneflow.Usuario.Perfiles;
using Modelos.Tuneflow.Modelos;
using Modelos.Tuneflow.Usuario.Produccion;

    public class TUNEFLOWContext : DbContext
    {
        public TUNEFLOWContext (DbContextOptions<TUNEFLOWContext> options)
            : base(options)
        {
        }

        public DbSet<Modelos.Tuneflow.Media.Song> Songs { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Media.FavoriteSong> FavoritesSongs { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Media.Playback> Playbacks { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Pagos.Payment> Payments { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Playlist.Album> Albums { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Playlist.MusicPlaylist> MusicsPlaylists { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Playlist.Playlist> Playlists { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Usuario.Administracion.Administrator> Administrators { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Usuario.Consumidor.Client> Clients { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Usuario.Consumidor.Subscription> Subscriptions { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Usuario.Consumidor.SubscriptionType> SubscriptionsTypes { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Usuario.Perfiles.Profile> Profiles { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Modelos.Country> Countrys { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Usuario.Administracion.ArtistStatistics> ArtistsStatistics { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Usuario.Produccion.Artist> Artists { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Usuario.Produccion.Follow> Follows { get; set; } = default!;
    }
