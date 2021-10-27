﻿using MvcMusicStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MvcMusicStore.Service;


namespace MvcMusicStore.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        MusicStoreEntities storeDB = new MusicStoreEntities();
        ICatalogService catalogSvc = new SqlCatalogService();

        public ActionResult Index()
        {
            // Get most popular albums
            var albums = GetTopSellingAlbums(5);

            return View(albums);
        }

        private List<Album> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count

            // Based on placed orders, get the top selling album IDs by quantity
            var topSellingAlbums = storeDB.OrderDetails
                    .GroupBy(d => d.AlbumId, d => d.Quantity)
                    .Select(g => new { AlbumId = g.Key, Quantity = g.Sum() })
                    .OrderByDescending(s => s.Quantity)
                    .Take(count)
                    .Select(t => t.AlbumId)
                    .ToList();

            // Return the albums corresponding to the top selling Ids. By supplying
            // the "count" parameter, it is ensured that we will always have the correct
            // length list of albums on display.
            return catalogSvc.GetAlbums(topSellingAlbums, count);

        }
    }
}