using Amazon.CDK;
using System;
using System.Collections.Generic;
using System.Text;

namespace MvcMusicStoreInfra
{
    public class MusicStoreInfraPipelineStage : Stage
    {
        public MusicStoreInfraPipelineStage(Construct scope, string id, StageProps props = null) : base(scope, id, props)
        {
            //The Resources Stack will be Instantiated here
        }

    }
}
