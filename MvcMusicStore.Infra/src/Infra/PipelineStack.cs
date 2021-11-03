using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amazon.CDK;
using Amazon.CDK.AWS.CodeCommit;
using Amazon.CDK.AWS.CodePipeline;
using Amazon.CDK.AWS.CodePipeline.Actions;
using Amazon.CDK.Pipelines;
using Amazon.CDK.AWS.DynamoDB;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.Ecr.Assets;
using Amazon.CDK.AWS.ECS;
using Amazon.CDK.AWS.ECS.Patterns;

namespace Infra
{
    public class PipelineStack : Stack
    {
        public PipelineStack(Construct parent, string id, IStackProps props = null) : base(parent, id, props)
        {
            var repo = new Repository(this, "WorkshopRepo", new RepositoryProps
            {
                RepositoryName = "MusicStoreRepo"
            });

            var pipeline = new CodePipeline(this, "pipeline", new CodePipelineProps
            {
                PipelineName = "MusicStore-Pipeline",
                Synth = new ShellStep("Synth", new ShellStepProps
                {
                    Input = CodePipelineSource.CodeCommit(repo, "ci-cd-modernization-cdk-from-strangle-3") ,
                    Commands = new string[] { 
                            "npm install -g aws-cdk",
                            "apt-get install -y dotnet-sdk-5.0", 
                            "cd MvcMusicStore.Infra",
                            "dotnet build src",
                            "cdk synth" 
                        }
                })
            });
        }

    }
}
