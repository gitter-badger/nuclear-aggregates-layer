using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata.Features;
using DoubleGis.Erm.BLCore.API.OrderValidation.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public sealed class OrderValidationRuleProvider : IOrderValidationRuleProvider
    {
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IOrderValidationRulesSettings _orderValidationRulesSettings;
        private readonly IMetadataProvider _metadataProvider;
        private readonly IOrderValidationRuleFactory _orderValidationRuleFactory;

        private readonly IReadOnlyDictionary<ValidationType, Predicate<OrderValidationRuleMetadata>> _validationRulesFilters;
        private readonly IReadOnlyCollection<OrderValidationRuleGroup> _orderedValidationRuleGroupsSequence = new[]
            {
                OrderValidationRuleGroup.Generic,
                OrderValidationRuleGroup.AdvertisementMaterialsValidation,
                OrderValidationRuleGroup.ADPositionsValidation,
                OrderValidationRuleGroup.AdvertisementAmountValidation
            };

        private readonly Dictionary<ValidationType, IEnumerable<OrderValidationRuleGroupDescriptor>> _ruleGroupsDescriptorsCache = 
                            new Dictionary<ValidationType, IEnumerable<OrderValidationRuleGroupDescriptor>>();

        public OrderValidationRuleProvider(
            IBusinessModelSettings businessModelSettings,
            IOrderValidationRulesSettings orderValidationRulesSettings,
            IMetadataProvider metadataProvider,
            IOrderValidationRuleFactory orderValidationRuleFactory)
        {
            _businessModelSettings = businessModelSettings;
            _orderValidationRulesSettings = orderValidationRulesSettings;
            _metadataProvider = metadataProvider;
            _orderValidationRuleFactory = orderValidationRuleFactory;

            var validationRulesFilters = new Dictionary<ValidationType, Predicate<OrderValidationRuleMetadata>>();
            AttachValidationTypeRules(validationRulesFilters,
                                      ValidationType.SingleOrderOnRegistration,
                                      metadata => metadata.Uses<CommonRuleFeature>(),
                                      metadata => metadata.Uses<NonManualRuleFeature>(),
                                      metadata => metadata.Uses<SingleOrderValidationRuleFeature>());
            AttachValidationTypeRules(validationRulesFilters,
                                      ValidationType.SingleOrderOnStateChanging);
            AttachValidationTypeRules(validationRulesFilters,
                                      ValidationType.PreReleaseBeta,
                                      metadata => metadata.Uses<CommonRuleFeature>(),
                                      metadata => metadata.Uses<NonManualRuleFeature>());
            AttachValidationTypeRules(validationRulesFilters,
                                      ValidationType.PreReleaseFinal,
                                      metadata => metadata.Uses<CommonRuleFeature>(),
                                      metadata => metadata.Uses<NonManualRuleFeature>());
            AttachValidationTypeRules(validationRulesFilters,
                                      ValidationType.ManualReport,
                                      metadata => metadata.Uses<CommonRuleFeature>());
            AttachValidationTypeRules(validationRulesFilters,
                                      ValidationType.ManualReportWithAccountsCheck,
                                      metadata => metadata.Uses<CommonRuleFeature>());

            _validationRulesFilters = validationRulesFilters;
        }

        public IEnumerable<OrderValidationRulesContianer> GetAppropriateRules(ValidationType validationType)
        {
            var groupDescriptors = GetApropriateRuleDescriptors(validationType);

            return groupDescriptors.Select(gd => new OrderValidationRulesContianer(
                                                     gd.Group,
                                                     gd.RuleDescriptors
                                                            .Select(rd => new OrderValidationRuleDescritpor(
                                                                                _orderValidationRuleFactory.Create(rd.RuleType),
                                                                                rd.RuleCode,
                                                                                rd.CachingExplicitlyDisabled))
                                                            .ToArray()));
        }

        private static void AttachValidationTypeRules(
            Dictionary<ValidationType, Predicate<OrderValidationRuleMetadata>> validationTypeRegistry,
            ValidationType validationType,
            params Predicate<OrderValidationRuleMetadata>[] filters)
        {
            validationTypeRegistry.Add(validationType,
                                       metadata => filters.Any(filter => filter(metadata))
                                                    || metadata.Features<AvailableForValidationTypeFeature>().Any(f => f.ValidationType == validationType));
        }

        private IEnumerable<OrderValidationRuleGroupDescriptor> GetApropriateRuleDescriptors(ValidationType validationType)
        {
            IEnumerable<OrderValidationRuleGroupDescriptor> groupDescriptors;
            if (_ruleGroupsDescriptorsCache.TryGetValue(validationType, out groupDescriptors))
            {
                return groupDescriptors;
            }

            MetadataSet orderValidationMetadata;
            if (!_metadataProvider.TryGetMetadata<MetadataOrderValidationRulesIdentity>(out orderValidationMetadata))
            {
                throw new InvalidOperationException("Metadata for order validation rules is not configured");
            }

            Predicate<OrderValidationRuleMetadata> validationRulesFilter;
            if (!_validationRulesFilters.TryGetValue(validationType, out validationRulesFilter))
            {
                throw new InvalidOperationException("Can't get validation rule metadata filters " + validationType);
            }

            var appropriateRulesMetadata =
                    orderValidationMetadata.Metadata.Values
                                           .OfType<OrderValidationRuleMetadata>()
                                           .Where(metadata => metadata.Features<DisabledForBusinessModelFeature>().All(f => f.BusinessModel != _businessModelSettings.BusinessModel)
                                                              && validationRulesFilter(metadata))
                                           .ToArray();

            groupDescriptors = new List<OrderValidationRuleGroupDescriptor>(
                            _orderedValidationRuleGroupsSequence.Select(orderValidationRuleGroup => GetGroupDescriptor(orderValidationRuleGroup, appropriateRulesMetadata)));
            _ruleGroupsDescriptorsCache.Add(validationType, groupDescriptors);
            
            return groupDescriptors;
        }

        private OrderValidationRuleGroupDescriptor GetGroupDescriptor(OrderValidationRuleGroup ruleGroup, IEnumerable<OrderValidationRuleMetadata> rulesMetadata)
        {
            var groupDescriptor = new OrderValidationRuleGroupDescriptor { Group = ruleGroup };
            var ruleDescriptors =
                    rulesMetadata
                        .Where(m => m.RuleGroup == ruleGroup)
                        .Select(m => new OrderValidationRuleDescriptor
                        {
                            RuleType = m.RuleType,
                            RuleCode = m.RuleCode,
                            CachingExplicitlyDisabled = _orderValidationRulesSettings.RulesExplicitlyDisabledCaching.Contains(m.RuleType.Name)
                        });
            groupDescriptor.RuleDescriptors.AddRange(ruleDescriptors);

            return groupDescriptor;
        }

        private sealed class OrderValidationRuleGroupDescriptor
        {
            public OrderValidationRuleGroupDescriptor()
            {
                RuleDescriptors = new List<OrderValidationRuleDescriptor>();
            }

            public OrderValidationRuleGroup Group { get; set; }
            public List<OrderValidationRuleDescriptor> RuleDescriptors { get; private set; }
        }

        private sealed class OrderValidationRuleDescriptor
        {
            public Type RuleType { get; set; }
            public int RuleCode { get; set; }
            public bool CachingExplicitlyDisabled { get; set; }
        }
    }
}
