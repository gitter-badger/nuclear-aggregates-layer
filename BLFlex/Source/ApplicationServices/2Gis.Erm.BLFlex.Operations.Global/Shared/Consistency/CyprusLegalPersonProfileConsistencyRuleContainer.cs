using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class CyprusLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer
    {
        #region Rules
        private static readonly IEnumerable<IConsistencyRule> CommonRules = new IConsistencyRule[]
            {
                ConsistencyRule.CreateNonEmptyString(entity => entity.Name, ResPlatform.RequiredFieldMessage, MetadataResources.Name),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInNominative, ResPlatform.RequiredFieldMessage, MetadataResources.ChiefNameInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PersonResponsibleForDocuments, ResPlatform.RequiredFieldMessage, MetadataResources.PersonResponsibleForDocuments),
                ConsistencyRule.CreateFormat(entity => entity.DocumentsDeliveryMethod, method => method == DocumentsDeliveryMethod.Undefined, ResPlatform.RequiredFieldMessage, MetadataResources.DocumentsDeliveryMethod),
                ConsistencyRule.CreateNonNull(entity => entity.OperatesOnTheBasisInGenitive, ResPlatform.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive),
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
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.FoundingBargain,
                                OperatesOnTheBasisType.Bargain,
                                OperatesOnTheBasisType.RegistrationCertificate,
                                OperatesOnTheBasisType.None),
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.Businessman)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Certificate,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.RegistrationCertificate,
                                OperatesOnTheBasisType.None)
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.NaturalPerson)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Undefined,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.RegistrationCertificate,
                                OperatesOnTheBasisType.None)
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<LegalPersonType>> LegalPersonTypeChecks =
            new List<ConsistencyRuleCollection<LegalPersonType>>
                {
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.LegalPerson)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative, ResPlatform.RequiredFieldMessage, MetadataResources.PositionInNominative),
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.Businessman)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative, ResPlatform.RequiredFieldMessage, MetadataResources.PositionInNominative)
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<OperatesOnTheBasisType>> RepresentativeAuthorityDocumentChecks =
            new List<ConsistencyRuleCollection<OperatesOnTheBasisType>>
                {
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Certificate)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.CertificateNumber, ResPlatform.RequiredFieldMessage, MetadataResources.CertificateNumber),
                            ConsistencyRule.CreateNonNull(entity => entity.CertificateDate, ResPlatform.RequiredFieldMessage, MetadataResources.CertificateDate)
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Warranty)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.WarrantyNumber, ResPlatform.RequiredFieldMessage, MetadataResources.WarrantyNumber),
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyBeginDate, ResPlatform.RequiredFieldMessage, MetadataResources.WarrantyBeginDate),
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyEndDate, ResPlatform.RequiredFieldMessage, MetadataResources.WarrantyEndDate),
                            ConsistencyRule.CreateFormat(entity => entity, profile => profile.WarrantyBeginDate.Value.Date > profile.WarrantyEndDate.Value.Date, BLResources.WarrantyBeginDateMustBeGreaterOrEqualThanEndDate),
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Bargain)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.BargainNumber, ResPlatform.RequiredFieldMessage, MetadataResources.BargainNumber),
                            ConsistencyRule.CreateNonNull(entity => entity.BargainBeginDate, ResPlatform.RequiredFieldMessage, MetadataResources.BargainBeginDate),
                            ConsistencyRule.CreateNonNull(entity => entity.BargainEndDate, ResPlatform.RequiredFieldMessage, MetadataResources.BargainEndDate),
                            ConsistencyRule.CreateFormat(entity => entity, profile => profile.BargainBeginDate.Value.Date > profile.BargainEndDate.Value.Date, BLResources.BargainBeginDateMustBeGreaterOrEqualThanEndDate),
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.RegistrationCertificate)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.RegistrationCertificateDate, ResPlatform.RequiredFieldMessage, MetadataResources.RegistrationCertificateDate),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.RegistrationCertificateNumber, ResPlatform.RequiredFieldMessage, MetadataResources.RegistrationCertificateNumber),
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
    }
}
