using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon;
using MvcMusicStore.Catalog.Models;
using Amazon.DynamoDBv2.DocumentModel;

namespace MvcMusicStore.Catalog
{
    /**
     * TODO - These methods are just stubs and should be re-mapped 
     **/
    public class MusicStoreDBClient
    {
        DynamoDBContext context;

        // TODO - The "How" of configuring the DynamoDB client should be determined. We are
        // running this on an EC2 instance with a default profile, and will deploy to EC2 with execution
        // role. We can probably just use the SDK credentials as supplied in search order - eg through
        // web.config / appsettings.json
        //
        // Note - ideally this would also be all async. We can investigate using the object context
        // async pattern if we have time.
        public MusicStoreDBClient()
        {
            var dynamoClient = new AmazonDynamoDBClient();
            context = new DynamoDBContext(dynamoClient);
        }

        public Task<IEnumerable<GenreModel>> Genres()
        {
            var filter = new ScanFilter();
            filter.AddCondition("Name", ScanOperator.IsNotNull);
            var limit = 100;
            return DynamoScanAsync<GenreModel>(filter, limit);

        }

        private async Task<IEnumerable<T>> DynamoScanAsync<T>(ScanFilter filter, int limit =100)
        {
            var result = new List<T>();

            var scanConfig = new ScanOperationConfig
            {
                Limit = limit,
                Filter = filter
            };
            var queryResult = context.FromScanAsync<T>(scanConfig);

            do
            {
                result.AddRange(await queryResult.GetNextSetAsync());
            }
            while (!queryResult.IsDone && result.Count < 100);

            return result;
        }

        public async Task<GenreModel> GenreByName(string name)
        {
            // TODO: Stub - Implement with the "right" version of the single table design
            //return context.ScanAsync<GenreModel>(new ScanCondition[]{ new ScanCondition("Name", ScanOperator.Equal, name)}).FirstOrDefault();

            var filter = new ScanFilter();
            filter.AddCondition("Name", ScanOperator.Equal, name);
            var limit = 1;
            return (await DynamoScanAsync<GenreModel>(filter, limit))
                .FirstOrDefault();
        }

        public Task<IEnumerable<AlbumModel>> Albums()
        {
            // TODO: Stub - Implement with the "right" version of the single table design
            //return context.Scan<AlbumModel>();
            var filter = new ScanFilter();
            filter.AddCondition("Name", ScanOperator.IsNotNull);
            var limit = 100;
            return DynamoScanAsync<AlbumModel>(filter, limit);
        }

        public async Task<AlbumModel> AlbumById(string id)
        {
            // TODO: Stub - Implement with the "right" version of the single table design (this is only for testing)
            var album = await context.LoadAsync<AlbumModel>(id);
            if (album != null)
            {
                var artist = await context.LoadAsync<ArtistModel>(album.ArtistId);
                var genre = await context.LoadAsync<GenreModel>(album.GenreId);
                album.Artist = artist;
                album.Genre = genre;
            }
            return album;
        }
        public Task<IEnumerable<AlbumModel>> AlbumsByGenre(string genreId)
        {
            // TODO: Stub - Implement with the "right" version of the single table design
            //return context.Scan<AlbumModel>(new ScanCondition("GenreId", ScanOperator.Equal, genreId));
            var filter = new ScanFilter();
            filter.AddCondition("GenreId", ScanOperator.Equal, genreId);
            var limit = 1;
            return DynamoScanAsync<AlbumModel>(filter, limit);
        }

        public async Task<IEnumerable<AlbumModel>> AlbumsByIdList(IEnumerable<string> ids)
        {
            //// TODO: Stub - Implement with the "right" version of the single table design
            //foreach (var id in ids)
            //{
            //    yield return await AlbumById(id);
            //}

            if (ids == null || !ids.Any() )
            {
                return new List<AlbumModel>();
            }

            var batchAlbums  = context.CreateBatchGet<AlbumModel>();
            foreach (var id in ids.Distinct())
            {
                batchAlbums.AddKey(id);
            }

            await context.ExecuteBatchGetAsync(batchAlbums);

            var albums = batchAlbums.Results;
            if (albums != null && albums.Any())
            {
                var genreIds = albums.Select(s => s.GenreId).Distinct().ToArray();
                var artistIds = albums.Select(s => s.AlbumId).Distinct().ToArray();

                var batchGenres = context.CreateBatchGet<GenreModel>();
                var batchArtists = context.CreateBatchGet<ArtistModel>();

                foreach (var g in genreIds)
                {
                    batchGenres.AddKey(g);
                }

                foreach (var a in artistIds)
                {
                    batchArtists.AddKey(a);
                }

                var batchAlbumAtts = context.CreateMultiTableBatchGet(batchGenres, batchGenres);
                await batchAlbumAtts.ExecuteAsync();

                foreach (var album in albums)
                {
                    album.Genre = batchGenres.Results?.Where(w => w.GenreId == album.GenreId).FirstOrDefault();
                    album.Artist = batchArtists.Results?.Where(w => w.ArtistId == album.ArtistId).FirstOrDefault();
                }
            }

            return albums;
        }

    }
}
