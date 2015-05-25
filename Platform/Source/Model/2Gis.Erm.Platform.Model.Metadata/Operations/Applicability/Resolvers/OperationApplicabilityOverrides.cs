using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Metamodeling.Domain.Operations.Detail;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Model.Metadata.Operations.Applicability.Resolvers
{
    public static class OperationApplicabilityOverrides
    {
        private static readonly IReadOnlyDictionary<int, OperationApplicability> OverridesMap = new Dictionary<int, OperationApplicability>();

        static OperationApplicabilityOverrides()
        {
            OverridesMap = new Dictionary<int, OperationApplicability>()
                .AddOverridesFor<ActionHistoryIdentity>(EntityType.Instance.Account(),
                                                        EntityType.Instance.Client(),
                                                        EntityType.Instance.Firm(),
                                                        EntityType.Instance.Deal(),
                                                        EntityType.Instance.Order(),
                                                        EntityType.Instance.LegalPerson(),
                                                        EntityType.Instance.Limit());
        }

        private static Dictionary<int, OperationApplicability> AddOverridesFor<TOperationIdentity>(
            this Dictionary<int, OperationApplicability> dictionary, 
            params IEntityType[] entityNames)
            where TOperationIdentity : OperationIdentityBase<TOperationIdentity>, new()
        {
            var identity = new TOperationIdentity();
            dictionary.Add(identity.Id, new OperationApplicability(identity, new[] { new OperationMetadataDetailContainer{ SpecificTypes = entityNames.ToEntitySet() } }));
            return dictionary;
        }

        public static Dictionary<int, OperationApplicability> ApplyOverrides(this Dictionary<int, OperationApplicability> source)
        {
            foreach (var item in OverridesMap)
            {   // если есть перизатираем, если нет добавляем
                source[item.Key] = item.Value;
            }

            return source;
        }

        public static IReadOnlyDictionary<int, OperationApplicability> ApplicabilityOverrides
        {
            get
            {
                return OverridesMap;
            }
        }
    }
}
