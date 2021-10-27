﻿using MvcMusicStore.Models;
using System;
using System.Collections.Generic;

namespace MvcMusicStore.Service
{
    interface ICatalogService
    {
        Album GetAlbumById(Guid id);
        List<Album> GetAlbums(List<Guid> ids, int minAlbums = 0);
        List<Album> GetAlbumsByGenre(Guid id);
        Genre GetGenreById(Guid genreId);
        List<Genre> GetGenres();
    }
}
