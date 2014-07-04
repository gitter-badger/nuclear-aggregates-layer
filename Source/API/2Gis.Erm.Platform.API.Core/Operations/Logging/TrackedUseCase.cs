using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public sealed class TrackedUseCase : MessageBase
    {
        public string Description { get; set; }
        public OperationScopeNode RootNode { get; set; }
        public IReadOnlyCollection<OperationScopeNode> Operations { get; set; }
        public IReadOnlyDictionary<StrictOperationIdentity, List<OperationScopeNode>> OperationsMap { get; set; }
        public IReadOnlyDictionary<Guid, HashSet<Guid>> OperationsHierarchy { get; set; }

        public override Guid Id
        {
            get { return RootNode.ScopeId; }
        }

        public override string ToString()
        {
            return RootNode.OperationIdentity.ToString();
        }
    }
}
