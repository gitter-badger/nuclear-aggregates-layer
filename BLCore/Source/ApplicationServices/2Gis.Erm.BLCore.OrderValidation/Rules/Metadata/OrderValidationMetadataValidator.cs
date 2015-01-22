using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;

using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.Validators;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.Metadata
{
    public sealed class OrderValidationMetadataValidator : MetadataValidatorBase<MetadataOrderValidationIdentity>
    {
        public OrderValidationMetadataValidator(IMetadataProvider metadataProvider) 
            : base(metadataProvider)
        {
        }

        protected override bool IsValidImpl(MetadataSet targetMetadata, out string report)
        {
            var orderValidationMetadataRegistry = new Dictionary<int, List<OrderValidationRuleMetadata>>();
            foreach (var ruleMetadata in targetMetadata.Metadata.Values.OfType<OrderValidationRuleMetadata>())
            {
                List<OrderValidationRuleMetadata> rulesContainer;
                if (!orderValidationMetadataRegistry.TryGetValue(ruleMetadata.RuleCode, out rulesContainer))
                {
                    rulesContainer = new List<OrderValidationRuleMetadata>();
                    orderValidationMetadataRegistry.Add(ruleMetadata.RuleCode, rulesContainer);
                }

                rulesContainer.Add(ruleMetadata);
            }

            var aggregatedReport = orderValidationMetadataRegistry
                .Where(pair => pair.Value.Count > 1)
                .Aggregate(new StringBuilder(),
                           (builder, pair) => builder.AppendFormat("RuleCode={0} is duplicated in several configured rules: {1}{2}",
                                                                   pair.Key,
                                                                   string.Join(", ", pair.Value.Select(x => x.RuleType.FullName))));

            report = aggregatedReport.Length > 0 ? aggregatedReport.ToString() : null;
            return report == null;
        }
    }
}
