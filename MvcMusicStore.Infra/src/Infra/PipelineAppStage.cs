using Amazon.CDK;

namespace Infra
{
    public class PipelineAppStage : Stage
    {
        public PipelineAppStage(Construct scope, string id, StageProps props = null) : base(scope, id, props)
        {
            Stack infraStack = new InfraStack(this, "AppStack", new StackProps
            {
                Env = new Environment
                {
                    Account = props.Env.Account,
                    Region = props.Env.Region
                }
            });
            // Stack infraStack = new InfraStack(this, "AppStack");
            // Stack testInfra = new InfraTestStack(this, "AppInfraStack");
        }

    }
}