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
    public class InfraTestStack : Stack
    {
        internal InfraTestStack(Construct scope, string id, IStackProps props = null) : base(scope, id, props)
        {

        }
    }
}