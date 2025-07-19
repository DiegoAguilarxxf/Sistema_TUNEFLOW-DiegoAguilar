using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Modelos.Tuneflow.Media;
using Modelos.Tuneflow.Payments;
using Modelos.Tuneflow.Playlists;
using Modelos.Tuneflow.User.Administration;
using Modelos.Tuneflow.User.Consumer;
using Modelos.Tuneflow.User.Profiles;
using Modelos.Tuneflow.Models;
using Modelos.Tuneflow.User.Production;

    public class TUNEFLOWContext : DbContext
    {
        public TUNEFLOWContext (DbContextOptions<TUNEFLOWContext> options)
            : base(options)
        {
        }

        public DbSet<Modelos.Tuneflow.Media.Song> Songs { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Media.FavoriteSong> FavoritesSongs { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Media.Playback> Playbacks { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Payments.Payment> Payments { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Playlists.Album> Albums { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Playlists.SongPlaylist> MusicsPlaylists { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Playlists.Playlist> Playlists { get; set; } = default!;

public DbSet<Modelos.Tuneflow.User.Administration.Administrator> Administrators { get; set; } = default!;

public DbSet<Modelos.Tuneflow.User.Consumer.Client> Clients { get; set; } = default!;

public DbSet<Modelos.Tuneflow.User.Consumer.Subscription> Subscriptions { get; set; } = default!;

public DbSet<Modelos.Tuneflow.User.Consumer.SubscriptionType> SubscriptionsTypes { get; set; } = default!;

public DbSet<Modelos.Tuneflow.User.Profiles.Profile> Profiles { get; set; } = default!;

public DbSet<Modelos.Tuneflow.Models.Country> Countrys { get; set; } = default!;

public DbSet<Modelos.Tuneflow.User.Administration.ArtistStatistics> ArtistsStatistics { get; set; } = default!;

public DbSet<Modelos.Tuneflow.User.Production.Artist> Artists { get; set; } = default!;

public DbSet<Modelos.Tuneflow.User.Production.Follow> Follows { get; set; } = default!;
    }
