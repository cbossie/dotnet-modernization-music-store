using Amazon.CDK;

namespace Infra
{
    public class PipelineAppStage : Stage
    {
        public PipelineAppStage(Construct scope, string id, StageProps props=null) : base(scope, id, props)
        {
            // Stack lambdaStack = new InfraStack(this, "AppInfraStack");
            Stack testInfra = new InfraTestStack(this, "AppInfraStack");
        }

    }
}