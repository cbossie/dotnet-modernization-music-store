using MvcMusicStore.CatalogApi;
using MvcMusicStore.CatalogApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MvcMusicStore.CatalogApi.Controllers
{
    /* Added by CTA: RoutePrefix attribute is no longer supported */
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        readonly CatalogService client = new CatalogService();
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Genres(string genreId = null)
        {
            if (string.IsNullOrEmpty(genreId))
            {
                return Ok(client.Genres());
            }
            else
            {
                return Ok(new List<GenreModel>{ await client.GenreById(genreId.ToUpper())});
            }
        }

        // Method expects one or more AlbumIds, comma separated
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> AlbumsAsync(string idlist = null, string genreid = null)
        {
            List<AlbumModel> albums = new List<AlbumModel>();
            if (!string.IsNullOrEmpty(idlist))
            {
                var idArray = idlist.ToUpper().Split(',');
                albums.AddRange( await client.AlbumsByIdListAsync(idArray));
            }
            else if (!string.IsNullOrEmpty(genreid))
            {
                albums.AddRange(await client.AlbumsByGenreAsync(genreid.ToUpper()));
            }

            return Ok(albums);
        }
    }
}