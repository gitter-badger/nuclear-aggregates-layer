using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public sealed class OperationSecurityRegistryReader : IOperationSecurityRegistryReader
    {
        private readonly OperationDependencyResolver _resolver;

        public OperationSecurityRegistryReader(Type registryType)
            : this(GetReqirementContainersFromType(registryType))
        {
        }

        public OperationSecurityRegistryReader(IEnumerable<IOperationAccessRequirement> requirements)
        {
            _resolver = new OperationDependencyResolver(requirements);
        }

        public static IEnumerable<IOperationAccessRequirement> GetReqirementContainersFromType(Type registryType)
        {
            return registryType.GetFields(BindingFlags.Static | BindingFlags.Public)
                               .Where(field => typeof(IOperationAccessRequirement).IsAssignableFrom(field.FieldType))
                               .Select(field => (IOperationAccessRequirement)field.GetValue(null));
        }

        public bool TryGetOperationRequirements(StrictOperationIdentity operationIdentity, out IEnumerable<IAccessRequirement> securityRequirements)
        {
            try
            {
                securityRequirements = _resolver.GetFlattedRequirements(operationIdentity);
                return true;
            }
            catch (ArgumentException)
            {
                // Когда резолвера спрашивают о том, чего у него нет, он кидает ArgumentException
                securityRequirements = null;
                return false;
            }
        }
    }
}
