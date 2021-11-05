using Amazon.CDK;
using Amazon.CDK.AWS.CodeCommit;
using Amazon.CDK.Pipelines;

namespace MvcMusicStoreInfra
{
    public class MusicStoreInfraPipeline : Stack
    {
        internal MusicStoreInfraPipeline(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {
            //Add CodeCommit Repo
            var repo = new Repository(this, "MusicStoreRepo", new RepositoryProps
            {
                RepositoryName = "MusicStoreRepo"
            });

            //Add CodePipeline 
            var pipeline = new CodePipeline(this, "pipeline", new CodePipelineProps
            {
                PipelineName = "MusicStore-Pipeline",
                Synth = new ShellStep("Synth", new ShellStepProps
                {
                    Input = CodePipelineSource.CodeCommit(repo, "module3-recovery-point"),
                    PrimaryOutputDirectory = "MvcMusicStore.Infra/cdk.out",
                    InstallCommands = new string[]{
                        "npm install -g aws-cdk",
                    },
                    Commands = new string[] {
                            "cd MvcMusicStore.Infra",
                            "dotnet build src",
                            "cdk synth"
                        }
                }),

            });

            pipeline.AddStage(new MusicStoreInfraPipelineStage(this, "Deploy"));
        }
    }
}
