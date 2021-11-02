using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon;
using MvcMusicStore.CatalogApi.Models;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace MvcMusicStore.CatalogApi
{
    /**
     * TODO - These methods are just stubs and should be re-mapped 
     **/
    public class CatalogService
    {
        readonly DynamoDBContext context;
        readonly AmazonDynamoDBClient dynamoClient;

        // TODO - The "How" of configuring the DynamoDB client should be determined. We are
        // running this on an EC2 instance with a default profile, and will deploy to EC2 with execution
        // role. We can probably just use the SDK credentials as supplied in search order - eg through
        // web.config / appsettings.json
        //
        // Note - ideally this would also be all async. We can investigate using the object context
        // async pattern if we have time.
        public CatalogService()
        {
            dynamoClient = new AmazonDynamoDBClient(region: RegionEndpoint.GetBySystemName("us-west-2"));
            context = new DynamoDBContext(dynamoClient);
        }

        public async Task<IEnumerable<GenreModel>> GenresAsync()
        {
            var queryOperation = context.QueryAsync<GenreModel>("GENRE");

            return await queryOperation.GetRemainingAsync();
        }

        public async Task<GenreModel> GenreByNameAsync(string name)
        {
            return await context.LoadAsync<GenreModel>("GENRE", name);
        }

        public async Task<AlbumModel> AlbumByIdAsync(string albumId)
        {
            var queryOperation = context.QueryAsync<AlbumModel>(albumId, new DynamoDBOperationConfig { IndexName = "album-by-id" });

            var albums = await queryOperation.GetRemainingAsync();

            return albums.FirstOrDefault();

        }

        public async Task<IEnumerable<AlbumModel>> AlbumsByGenreAsync(string genreName)
        {
            var queryOperation = context.QueryAsync<AlbumModel>(genreName, new DynamoDBOperationConfig { IndexName = "genre-albums" });

            return await queryOperation.GetRemainingAsync();
        }

        public async Task<IEnumerable<AlbumModel>> AlbumsByIdListAsync(IEnumerable<string> ids)
        {
            var albumBatch = context.CreateBatchGet<AlbumModel>();

            ids.ToList().ForEach(i => albumBatch.AddKey(i, "metadata"));

            await albumBatch.ExecuteAsync();

            return albumBatch.Results;
        }

    }
}
