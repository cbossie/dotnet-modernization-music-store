﻿using MvcMusicStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvcMusicStore.Service
{
    public class SqlCatalogService : ICatalogService
    {
        private MusicStoreEntities storeDb;

        public SqlCatalogService()
        {
            storeDb = new MusicStoreEntities();
        }

        // Retrieves a particular album by its Id
        public Album GetAlbumById(Guid id)
        {
            return storeDb.Albums.Find(id);
        }

        // Given a list of album Ids, this retrieves the corresponding album data.
        public List<Album> GetAlbums(List<Guid> ids)
        {
            return storeDb.Albums.Where(a => ids.Contains(a.AlbumId)).ToList();

        }

        // Retrieves all the albums for a particular genre
        public List<Album> GetAlbumsByGenre(Guid id)
        {
            return storeDb.Albums.Where(a => a.GenreId == id).ToList();
        }

        // Retrieves a genre by its name
        public Genre GetGenreById(Guid id)
        {
            return storeDb.Genres.Include("Albums").Single(g => g.GenreId == id);
        }

        // Retrieves a list of all available genres
        public List<Genre> GetGenres()
        {
            return storeDb.Genres.ToList();
        }
    }
}
