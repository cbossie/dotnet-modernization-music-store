using Amazon.CDK;
using Amazon.CDK.AWS.CodeCommit;
using Amazon.CDK.Pipelines;

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
                    Input = CodePipelineSource.CodeCommit(repo, "ci-cd-modernization-cdk-from-strangle-3"),
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


            // pipeline.AddStage(new PipelineAppStage(this, "App-Infra-Stage", new Amazon.CDK.StageProps
            // {
            //     Env = new Environment
            //     {
            //         Account = this.Account,
            //         Region = this.Region
            //     }
            // }));
            pipeline.AddStage(new PipelineAppStage(this, "App-Infra-Stage"));
        }

    }
}
