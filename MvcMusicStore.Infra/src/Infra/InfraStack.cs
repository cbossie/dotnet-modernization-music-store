using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Ecr.Assets;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;

namespace Infra
{
    public class InfraStack : Stack
    {
        internal InfraStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            // Import existing Resources
            // VPC
            var importedVpcId = "INFORM_VPC_ID";

            var vpc = Vpc.FromLookup(this, "imported-vpc", new VpcLookupOptions
            {
                VpcId = importedVpcId
            });

            // Import DynamoDb Tables
            var albumTable = Table.FromTableArn (this, "imported-album-table", "arn:aws:dynamodb:us-west-2:111122223333:table/TODO_INFORM_ALBUM_TABLE_ARN");
            var genreTable = Table.FromTableArn (this, "imported-genre-table", "arn:aws:dynamodb:us-west-2:111122223333:table/TODO_INFORM_GENRE_TABLE_ARN");
            var artistTable = Table.FromTableArn (this, "imported-artist-table", "arn:aws:dynamodb:us-west-2:111122223333:table/TODO_INFORM_ARTIST_TABLE_ARN");

            //ECR
            //Build docker image and publish on ECR Repository
            var asset = new DockerImageAsset(this, "web-api-docker-image", new DockerImageAssetProps
            {
                Directory = Path.Combine(Directory.GetCurrentDirectory(), "../MvcMusicStore.CatalogApi"),
                File = "Dockerfile"
            });
           
            // Create Cluster 
            var cluster = new Cluster(this, "demo-cluster", new ClusterProps
            {
                Vpc = vpc,
            });

            //L3 Construct for ALB + ECS + Fargate
            var loadBalancedFargateService = new ApplicationLoadBalancedFargateService(this, "demo-ecs-fargate-service", new ApplicationLoadBalancedFargateServiceProps
            {
                Cluster = cluster,
                MemoryLimitMiB = 1024,
                Cpu = 512,
                TaskImageOptions = new ApplicationLoadBalancedTaskImageOptions
                {
                    Image = ContainerImage.FromEcrRepository(asset.Repository, asset.ImageUri.Split(":").Last()),
                    Environment = new Dictionary<string, string>()
                        {
                            {"AWS_REGION", this.Region},
                            {"ASPNETCORE_ENVIRONMENT","Development"},
                            {"ASPNETCORE_URLS","http://+:80"}
                        }
                }
            });

            //Grant Read Permission
            albumTable.GrantReadData(loadBalancedFargateService.Service.TaskDefinition.TaskRole);
            genreTable.GrantReadData(loadBalancedFargateService.Service.TaskDefinition.TaskRole);
            artistTable.GrantReadData(loadBalancedFargateService.Service.TaskDefinition.TaskRole);

            new CfnOutput(this, "AddoDemoClusterArn", new CfnOutputProps { Value = cluster.ClusterArn });
            new CfnOutput(this, "AddoDemoClusterName", new CfnOutputProps { Value = cluster.ClusterName });
        }
    }
}