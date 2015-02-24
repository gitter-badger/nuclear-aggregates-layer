using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class CzechLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer
    {
        #region Rules
        private static readonly IEnumerable<IConsistencyRule> CommonRules = new IConsistencyRule[]
            {
                ConsistencyRule.CreateNonEmptyString(entity => entity.Name, ResPlatform.RequiredFieldMessage, MetadataResources.Name),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInNominative, ResPlatform.RequiredFieldMessage, MetadataResources.ChiefNameInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PersonResponsibleForDocuments, ResPlatform.RequiredFieldMessage, MetadataResources.PersonResponsibleForDocuments),
                ConsistencyRule.CreateFormat(entity => entity.DocumentsDeliveryMethod, method => method == DocumentsDeliveryMethod.Undefined, ResPlatform.RequiredFieldMessage, MetadataResources.DocumentsDeliveryMethod),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInGenitive, ResPlatform.RequiredFieldMessage, MetadataResources.ChiefNameInGenitive),
                ConsistencyRule.CreateNonNull(entity => entity.OperatesOnTheBasisInGenitive, ResPlatform.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive),
                ConsistencyRule.CreateNonNull(entity => entity.PaymentMethod, ResPlatform.RequiredFieldMessage, MetadataResources.PaymentMethod)
            };

        private static readonly IEnumerable<ConsistencyRuleCollection<LegalPersonType>> AllowedLegalPersonDocuments =
            new List<ConsistencyRuleCollection<LegalPersonType>>
                {
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.LegalPerson)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.None),
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.Businessman)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.None),
                        },
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<LegalPersonType>> LegalPersonTypeChecks =
            new List<ConsistencyRuleCollection<LegalPersonType>>
                {
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.LegalPerson)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.Registered, ResPlatform.RequiredFieldMessage, MetadataResources.Registered)
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<OperatesOnTheBasisType>> RepresentativeAuthorityDocumentChecks =
            new List<ConsistencyRuleCollection<OperatesOnTheBasisType>>
                {
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Warranty)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyBeginDate, ResPlatform.RequiredFieldMessage, MetadataResources.WarrantyBeginDate)
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<PaymentMethod>> PaymentMethodChecks =
            new List<ConsistencyRuleCollection<PaymentMethod>>
                {
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankTransaction)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, ResPlatform.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.BankCode, ResPlatform.RequiredFieldMessage, MetadataResources.BankCode),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.BankName, ResPlatform.RequiredFieldMessage, MetadataResources.BankName),
                        }
                }; 
        #endregion

        public IEnumerable<IConsistencyRule> GetApplicableRules(LegalPerson person, LegalPersonProfile profile)
        {
            foreach (var rule in CommonRules)
            {
                yield return rule;
            }

            var legalPersonType = person.LegalPersonTypeEnum;
            foreach (var rule in GetAllowedLegalPersonDocuments(legalPersonType))
            {
                yield return rule;
            }

            foreach (var rule in GetLegalPersonTypeChecks(legalPersonType))
            {
                yield return rule;
            }

            if (profile.OperatesOnTheBasisInGenitive.HasValue)
            {
                var representativeAuthorityDocument = profile.OperatesOnTheBasisInGenitive.Value;
                foreach (var rule in GetRepresentativeAuthorityDocumentChecks(representativeAuthorityDocument))
                {
                    yield return rule;
                }
            }

            if (profile.PaymentMethod.HasValue)
            {
                var paymentMethod = profile.PaymentMethod.Value;
                foreach (var rule in GetPaymentMethodChecks(paymentMethod))
                {
                    yield return rule;
                }
            }
        }

        private static IEnumerable<IConsistencyRule> GetAllowedLegalPersonDocuments(LegalPersonType key)
        {
            IEnumerable<IConsistencyRule> result = AllowedLegalPersonDocuments.SingleOrDefault(collection => collection.Key == key);
            return result ?? new IConsistencyRule[0];
        }

        private static IEnumerable<IConsistencyRule> GetLegalPersonTypeChecks(LegalPersonType key)
        {
            IEnumerable<IConsistencyRule> result = LegalPersonTypeChecks.SingleOrDefault(collection => collection.Key == key);
            return result ?? new IConsistencyRule[0];
        }

        private static IEnumerable<IConsistencyRule> GetRepresentativeAuthorityDocumentChecks(OperatesOnTheBasisType key)
        {
            IEnumerable<IConsistencyRule> result = RepresentativeAuthorityDocumentChecks.SingleOrDefault(collection => collection.Key == key);
            return result ?? new IConsistencyRule[0];
        }

        private static IEnumerable<IConsistencyRule> GetPaymentMethodChecks(PaymentMethod key)
        {
            IEnumerable<IConsistencyRule> result = PaymentMethodChecks.SingleOrDefault(collection => collection.Key == key);
            return result ?? new IConsistencyRule[0];
        }
    }
}
