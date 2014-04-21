﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class UkraineLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer, IUkraineAdapted
    {
        #region Rules
        private static readonly IEnumerable<IConsistencyRule> CommonRules = new IConsistencyRule[]
            {
                ConsistencyRule.CreateNonEmptyString(entity => entity.Name, BLResources.RequiredFieldMessage, MetadataResources.Name),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInNominative, BLResources.RequiredFieldMessage, MetadataResources.ChiefNameInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PersonResponsibleForDocuments, BLResources.RequiredFieldMessage, MetadataResources.PersonResponsibleForDocuments),
                ConsistencyRule.CreateFormat(entity => (DocumentsDeliveryMethod)entity.DocumentsDeliveryMethod, method => method == DocumentsDeliveryMethod.Undefined, BLResources.RequiredFieldMessage, MetadataResources.DocumentsDeliveryMethod),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInGenitive, BLResources.RequiredFieldMessage, MetadataResources.ChiefNameInGenitive),
                ConsistencyRule.CreateNonNull(entity => entity.OperatesOnTheBasisInGenitive, BLResources.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive),
                ConsistencyRule.CreateNonNull(entity => entity.PaymentMethod, BLResources.RequiredFieldMessage, MetadataResources.PaymentMethod),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative, BLResources.RequiredFieldMessage, MetadataResources.PositionInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInGenitive, BLResources.RequiredFieldMessage, MetadataResources.PositionInGenitive),
            };

        private static readonly IEnumerable<ConsistencyRuleCollection<LegalPersonType>> AllowedLegalPersonDocuments =
            new List<ConsistencyRuleCollection<LegalPersonType>>
                {
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.LegalPerson)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => (OperatesOnTheBasisType)entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Charter,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.FoundingBargain),
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.Businessman)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => (OperatesOnTheBasisType)entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.Certificate),
                        },
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<LegalPersonType>> LegalPersonTypeChecks =
            new List<ConsistencyRuleCollection<LegalPersonType>>
                {
                   
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<OperatesOnTheBasisType>> RepresentativeAuthorityDocumentChecks =
            new List<ConsistencyRuleCollection<OperatesOnTheBasisType>>
                {
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Warranty)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyBeginDate,
                                                          BLResources.RequiredFieldMessage,
                                                          MetadataResources.WarrantyBeginDate),
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyEndDate,
                                                          BLResources.RequiredFieldMessage,
                                                          MetadataResources.WarrantyEndDate),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.WarrantyNumber, BLResources.RequiredFieldMessage, MetadataResources.WarrantyNumber)
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Certificate)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.CertificateDate, BLResources.RequiredFieldMessage, MetadataResources.CertificateDate),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.CertificateNumber, BLResources.RequiredFieldMessage, MetadataResources.CertificateNumber)
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<PaymentMethod>> PaymentMethodChecks =
            new List<ConsistencyRuleCollection<PaymentMethod>>
                {
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankTransaction)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, BLResources.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.BankName, BLResources.RequiredFieldMessage, MetadataResources.BankName),
                        }
                }; 
        #endregion

        public IEnumerable<IConsistencyRule> GetApplicableRules(LegalPerson person, LegalPersonProfile profile)
        {
            foreach (var rule in CommonRules)
            {
                yield return rule;
            }

            var legalPersonType = (LegalPersonType)person.LegalPersonTypeEnum;
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
                var representativeAuthorityDocument = (OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive.Value;
                foreach (var rule in GetRepresentativeAuthorityDocumentChecks(representativeAuthorityDocument))
                {
                    yield return rule;
                }
            }

            if (profile.PaymentMethod.HasValue)
            {
                var paymentMethod = (PaymentMethod)profile.PaymentMethod.Value;
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
