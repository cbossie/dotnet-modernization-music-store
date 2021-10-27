using MvcMusicStore.Models;
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
        // If the second parameter is supplied, then additional albums will be selected
        // to give a minimum length list, in order to meet the business requirements
        // of the music store
        public List<Album> GetAlbums(List<Guid> ids, int minAlbums = 0)
        {
            var selectedAlbums = storeDb.Albums.Where(a => ids.Contains(a.AlbumId)).ToList();
            if(selectedAlbums.Count < minAlbums)
            {
                selectedAlbums.AddRange(storeDb.Albums.Where(a => !ids.Contains(a.AlbumId)).Take(minAlbums - selectedAlbums.Count));
            }
            return selectedAlbums;


        }

        // Retrieves all the albums for a particular genre
        public List<Album> GetAlbumsByGenre(Guid id)
        {
            return storeDb.Albums.Where(a => a.GenreId == id).ToList();
        }

        // Retrieves a genre by its name
        public Genre GetGenreById(Guid genreId)
        {
            return storeDb.Genres.Include("Albums").Single(g => g.GenreId == genreId);
        }

        // Retrieves a list of all available genres
        public List<Genre> GetGenres()
        {
            return storeDb.Genres.ToList();
        }
    }
}
