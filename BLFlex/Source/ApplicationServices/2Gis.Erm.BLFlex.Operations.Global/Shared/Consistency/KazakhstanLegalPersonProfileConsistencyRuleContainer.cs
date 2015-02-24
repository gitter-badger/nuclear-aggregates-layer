using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{   // FIXME {a.tukaev, 10.10.2014}: Скопипасчено с Украины
    public class KazakhstanLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer
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
                ConsistencyRule.CreateNonNull(entity => entity.PaymentMethod, ResPlatform.RequiredFieldMessage, MetadataResources.PaymentMethod),
                ConsistencyRule.CreateFormat(entity => entity.IBAN, IsIbanInvalid, ResPlatform.InvalidFieldValue, MetadataResources.IBAN)
            };
        private static readonly IEnumerable<ConsistencyRuleCollection<LegalPersonType>> AllowedLegalPersonDocuments =
            new List<ConsistencyRuleCollection<LegalPersonType>>
                {
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.LegalPerson)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(    
                                entity => entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Charter,
                                OperatesOnTheBasisType.Certificate,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.Decree,
                                OperatesOnTheBasisType.Other)
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.Businessman)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Charter,
                                OperatesOnTheBasisType.Certificate,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.Decree,
                                OperatesOnTheBasisType.Other)
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.NaturalPerson)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.Other)
                        },
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<LegalPersonType>> RequiredFieldsChecks =
            new List<ConsistencyRuleCollection<LegalPersonType>>
                {
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.LegalPerson)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative, ResPlatform.RequiredFieldMessage, MetadataResources.PositionInNominative),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInGenitive, ResPlatform.RequiredFieldMessage, MetadataResources.PositionInGenitive)
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.Businessman)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative, ResPlatform.RequiredFieldMessage, MetadataResources.PositionInNominative),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInGenitive, ResPlatform.RequiredFieldMessage, MetadataResources.PositionInGenitive)
                        },
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<OperatesOnTheBasisType>> RepresentativeAuthorityDocumentChecks =
            new List<ConsistencyRuleCollection<OperatesOnTheBasisType>>
                {
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Warranty)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyBeginDate,
                                                          ResPlatform.RequiredFieldMessage,
                                                          MetadataResources.WarrantyBeginDate),
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyEndDate,
                                                          ResPlatform.RequiredFieldMessage,
                                                          MetadataResources.WarrantyEndDate),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.WarrantyNumber, ResPlatform.RequiredFieldMessage, MetadataResources.WarrantyNumber)
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Certificate)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.CertificateDate, ResPlatform.RequiredFieldMessage, MetadataResources.CertificateDate),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.CertificateNumber, ResPlatform.RequiredFieldMessage, MetadataResources.CertificateNumber)
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Other)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(x => x.OtherAuthorityDocument), ResPlatform.RequiredFieldMessage, MetadataResources.OtherAuthorityDocument)
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Decree)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(x => x.DecreeNumber), ResPlatform.RequiredFieldMessage, MetadataResources.DecreeNumber),
                            ConsistencyRule.CreateNonNull(entity => entity.Within<KazakhstanLegalPersonProfilePart>().GetPropertyValue(x => x.DecreeDate), ResPlatform.RequiredFieldMessage, MetadataResources.DecreeDate),
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<PaymentMethod>> PaymentMethodChecks =
            new List<ConsistencyRuleCollection<PaymentMethod>>
                {
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankTransaction)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.IBAN, ResPlatform.RequiredFieldMessage, MetadataResources.IBAN),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.SWIFT, ResPlatform.RequiredFieldMessage, MetadataResources.SWIFT),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.BankName, ResPlatform.RequiredFieldMessage, MetadataResources.BankName)
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
            IEnumerable<IConsistencyRule> result = RequiredFieldsChecks.SingleOrDefault(collection => collection.Key == key);
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

        // http://en.wikipedia.org/wiki/International_Bank_Account_Number#Validating_the_IBAN
        private static bool IsIbanInvalid(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban))
            {
                return false;
            }

            var stringValue = string.Concat(iban.Substring(4), iban.Substring(0, 4))
                                  .ToLower()
                                  .Select(c => char.IsDigit(c) ? c.ToString() : (c - 'a' + 10).ToString());

            decimal decimalValue;
            if (!decimal.TryParse(string.Concat(stringValue), out decimalValue))
            {
                return true;
            }

            return decimalValue % 97 != 1;
        }
    }
}