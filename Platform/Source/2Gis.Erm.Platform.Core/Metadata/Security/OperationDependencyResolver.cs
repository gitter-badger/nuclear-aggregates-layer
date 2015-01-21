using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public sealed class OperationDependencyResolver
    {
        private readonly IDictionary<StrictOperationIdentity, IEnumerable<IAccessRequirement>> _cache;
        private readonly Dictionary<StrictOperationIdentity, IOperationAccessRequirement> _operations;
        private readonly bool _hasDependencyLoops;

        public OperationDependencyResolver(IEnumerable<IOperationAccessRequirement> requirements)
        {
            _cache = new Dictionary<StrictOperationIdentity, IEnumerable<IAccessRequirement>>();
            _hasDependencyLoops = HasRequirementsDependencyLoops(requirements);
            _operations = requirements.ToDictionary(requirement => requirement.StrictOperationIdentity, requirement => requirement);
        }

        public bool HasDependencyLoops 
        {
            get { return _hasDependencyLoops; }
        }

        public IEnumerable<IAccessRequirement> GetFlattedRequirements(StrictOperationIdentity operationIdentity)
        {
            IEnumerable<IAccessRequirement> requrements;
            if (_cache.TryGetValue(operationIdentity, out requrements))
            {
                return requrements;
            }

            IOperationAccessRequirement operationRequirement;
            if (!_operations.TryGetValue(operationIdentity, out operationRequirement))
            {
                throw new ArgumentException(string.Format("Operation {0} was not registered", operationIdentity));
            }

            requrements = operationRequirement.UsedOperations.SelectMany(GetFlattedRequirements).Concat(operationRequirement.Requirements).Distinct().ToArray();
            _cache.Add(operationIdentity, requrements);
            return requrements;
        }

        private bool HasRequirementsDependencyLoops(IEnumerable<IOperationAccessRequirement> requirementContainers)
        {
            var operationsUsages = GetOperationUsages(requirementContainers);

            while (operationsUsages.Count > 0)
            {
                var currentLayerOperations = operationsUsages.Where(pair => pair.Value.Count == 0).Select(pair => pair.Key).ToList();
                if (currentLayerOperations.Count == 0 && operationsUsages.Count > 0)
                {
                    return true;
                }

                foreach (var operation in currentLayerOperations)
                {
                    operationsUsages.Remove(operation);
                    foreach (var operationsUsage in operationsUsages)
                    {
                        operationsUsage.Value.Remove(operation);
                    }
                }
            }

            return false;
        }

        private IDictionary<StrictOperationIdentity, IList<StrictOperationIdentity>> GetOperationUsages(IEnumerable<IOperationAccessRequirement> requirementContainers)
        {
            var allOperationsUsings = new Dictionary<StrictOperationIdentity, IList<StrictOperationIdentity>>();
            foreach (var requirement in requirementContainers)
            {
                foreach (var operation in requirement.UsedOperations)
                {
                    IList<StrictOperationIdentity> currentOperationUsings;
                    if (!allOperationsUsings.TryGetValue(operation, out currentOperationUsings))
                    {
                        currentOperationUsings = new List<StrictOperationIdentity>();
                        allOperationsUsings.Add(operation, currentOperationUsings);
                    }

                    currentOperationUsings.Add(requirement.StrictOperationIdentity);
                }
            }

            return allOperationsUsings;
        }
    }
}
