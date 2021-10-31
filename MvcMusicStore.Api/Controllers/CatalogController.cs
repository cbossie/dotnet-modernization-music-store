using MvcMusicStore.Catalog;
using MvcMusicStore.Catalog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MvcMusicStore.Api.Controllers
{
    /* Added by CTA: RoutePrefix attribute is no longer supported */
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        MusicStoreDBClient client = new MusicStoreDBClient();
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Genres(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Ok(client.Genres());
            }
            else
            {
                return Ok(new List<GenreModel>{ await client.GenreByName(name)});
            }
        }

        // Method expects one or more AlbumIds, comma separated
        [HttpGet]
        [Route("[action]")]
        public IActionResult Albums(string idlist = null, string genreid = null)
        {
            if (!string.IsNullOrEmpty(idlist))
            {
                var idArray = idlist.ToUpper().Split(',');
                return Ok(client.AlbumsByIdList(idArray));
            }
            else if (!string.IsNullOrEmpty(genreid))
            {
                return Ok(client.AlbumsByGenre(genreid.ToUpper()));
            }

            return Ok(client.Albums());
        }
    }
}