using Amazon.CDK;

namespace Infra
{
    public class PipelineAppStage : Stage
    {
        public PipelineAppStage(Construct scope, string id, StageProps props = null) : base(scope, id, props)
        {
            Stack infraStack = new InfraStack(this, "AppStack");
        }

    }
}