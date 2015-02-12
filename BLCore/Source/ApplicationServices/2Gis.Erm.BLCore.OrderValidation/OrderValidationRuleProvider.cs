using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata;
using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata.Features;
using DoubleGis.Erm.BLCore.API.OrderValidation.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public sealed class OrderValidationRuleProvider : IOrderValidationRuleProvider
    {
        private readonly IBusinessModelSettings _businessModelSettings;
        private readonly IOrderValidationCachingSettings _orderValidationCachingSettings;
        private readonly IMetadataProvider _metadataProvider;
        private readonly IOrderValidationRuleFactory _orderValidationRuleFactory;

        private readonly IReadOnlyDictionary<ValidationType, Predicate<OrderValidationRuleMetadata>> _validationRulesFilters;
        private readonly IReadOnlyCollection<OrderValidationRuleGroup> _orderedValidationRuleGroupsSequence = new[]
            {
                OrderValidationRuleGroup.Generic,
                OrderValidationRuleGroup.SalesModelValidation,
                OrderValidationRuleGroup.AdvertisementMaterialsValidation,
                OrderValidationRuleGroup.ADPositionsValidation,
                OrderValidationRuleGroup.AdvertisementAmountValidation
            };

        private readonly Dictionary<ValidationType, IEnumerable<OrderValidationRuleGroupDescriptor>> _ruleGroupsDescriptorsCache = 
                            new Dictionary<ValidationType, IEnumerable<OrderValidationRuleGroupDescriptor>>();

        public OrderValidationRuleProvider(
            IBusinessModelSettings businessModelSettings,
            IOrderValidationCachingSettings orderValidationCachingSettings,
            IMetadataProvider metadataProvider,
            IOrderValidationRuleFactory orderValidationRuleFactory)
        {
            _businessModelSettings = businessModelSettings;
            _orderValidationCachingSettings = orderValidationCachingSettings;
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

        public IReadOnlyCollection<OrderValidationRulesContainer> GetAppropriateRules(ValidationType validationType)
        {
            var ruleContaners = new List<OrderValidationRulesContainer>();

            foreach (var groupDescriptor in GetApropriateRuleDescriptors(validationType))
            {
                var ruleDescriptors = new List<API.OrderValidation.OrderValidationRuleDescriptor>();
                bool allRulesScheduled = true;
                foreach (var ruleDescriptor in groupDescriptor.RuleDescriptors)
                {
                    if (!ruleDescriptor.Enabled)
                    {
                        allRulesScheduled = false;
                        continue;
                    }

                    ruleDescriptors.Add(
                        new API.OrderValidation.OrderValidationRuleDescriptor(
                                _orderValidationRuleFactory.Create(ruleDescriptor.RuleType),
                                ruleDescriptor.RuleCode,
                                ruleDescriptor.UseCaching));
                }

                ruleContaners.Add(
                    new OrderValidationRulesContainer(
                            groupDescriptor.Group,
                            groupDescriptor.UseCaching,
                            allRulesScheduled,
                            ruleDescriptors));
            }

            return ruleContaners;
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

            lock (_ruleGroupsDescriptorsCache)
            {
                if (_ruleGroupsDescriptorsCache.TryGetValue(validationType, out groupDescriptors))
                {
                    return groupDescriptors;
                }

                groupDescriptors =
                    _orderedValidationRuleGroupsSequence
                        .Select(targetRulesGroup => CreateGroupDescriptor(validationType, targetRulesGroup))
                        .ToArray();

                _ruleGroupsDescriptorsCache.Add(validationType, groupDescriptors);
            }

            return groupDescriptors;
        }

        private OrderValidationRuleGroupDescriptor CreateGroupDescriptor(ValidationType validationType, OrderValidationRuleGroup targetRulesGroup)
        {
            var groupMetadataId = IdBuilder.For<MetadataOrderValidationIdentity>("Rules", targetRulesGroup.ToString());
            OrderValidationRuleGroupMetadata groupMetadata;
            if (!_metadataProvider.TryGetMetadata(groupMetadataId, out groupMetadata))
            {
                throw new InvalidOperationException("Metadata for order validation rules group " + targetRulesGroup + " is not configured");
            }

            var useCaching = groupMetadata.Uses<UseCachingFeature>()
                             && (!groupMetadata.Features<EnableCachingForValidationTypeFeature>().Any()
                                 || groupMetadata.Features<EnableCachingForValidationTypeFeature>().Any(f => f.ValidationType == validationType));

            var groupDescriptor = new OrderValidationRuleGroupDescriptor
                {
                    Group = targetRulesGroup,
                    UseCaching = useCaching
                };

            var ruleDescriptors = groupMetadata.Elements<OrderValidationRuleMetadata>().Select(m => CreateRuleDescriptor(validationType, useCaching, m));
            groupDescriptor.RuleDescriptors.AddRange(ruleDescriptors);
            return groupDescriptor;
        }

        private OrderValidationRuleDescriptor CreateRuleDescriptor(
            ValidationType validationType,
            bool groupUseCaching,
            OrderValidationRuleMetadata ruleMetadata)
        {
            Predicate<OrderValidationRuleMetadata> validationRulesFilter;
            if (!_validationRulesFilters.TryGetValue(validationType, out validationRulesFilter))
            {
                throw new InvalidOperationException("Can't get validation rule metadata filters " + validationType);
            }

            return new OrderValidationRuleDescriptor
                {
                    RuleType = ruleMetadata.RuleType,
                    RuleCode = ruleMetadata.RuleCode,
                    Enabled = ruleMetadata.Features<DisabledForBusinessModelFeature>().All(f => f.BusinessModel != _businessModelSettings.BusinessModel)
                              && validationRulesFilter(ruleMetadata),
                    UseCaching = groupUseCaching
                                 && !_orderValidationCachingSettings.RulesExplicitlyDisabledCaching.Contains(ruleMetadata.RuleType.Name)
                };
        }

        private sealed class OrderValidationRuleGroupDescriptor
        {
            public OrderValidationRuleGroupDescriptor()
            {
                RuleDescriptors = new List<OrderValidationRuleDescriptor>();
            }

            public OrderValidationRuleGroup Group { get; set; }
            public bool UseCaching { get; set; }
            public List<OrderValidationRuleDescriptor> RuleDescriptors { get; private set; }
        }

        // COMMENT {i.maslennikov, 07.10.2014}: Есть класс с таким же названием в неймспейсе DoubleGis.Erm.BLCore.API.OrderValidation и он похож по контракту. Это запутывает.
        private sealed class OrderValidationRuleDescriptor
        {
            public Type RuleType { get; set; }
            public int RuleCode { get; set; }
            public bool Enabled { get; set; }
            public bool UseCaching { get; set; }
        }
    }
}
