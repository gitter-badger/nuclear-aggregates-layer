using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class CyprusLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer
    {
        #region Rules
        private static readonly IEnumerable<IConsistencyRule> CommonRules = new IConsistencyRule[]
            {
                ConsistencyRule.CreateNonEmptyString(entity => entity.Name, BLResources.RequiredFieldMessage, MetadataResources.Name),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInNominative, BLResources.RequiredFieldMessage, MetadataResources.ChiefNameInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PersonResponsibleForDocuments, BLResources.RequiredFieldMessage, MetadataResources.PersonResponsibleForDocuments),
                ConsistencyRule.CreateFormat(entity => (DocumentsDeliveryMethod)entity.DocumentsDeliveryMethod, method => method == DocumentsDeliveryMethod.Undefined, BLResources.RequiredFieldMessage, MetadataResources.DocumentsDeliveryMethod),
                ConsistencyRule.CreateNonNull(entity => entity.OperatesOnTheBasisInGenitive, BLResources.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive),
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
                                OperatesOnTheBasisType.FoundingBargain,
                                OperatesOnTheBasisType.Bargain,
                                OperatesOnTheBasisType.RegistrationCertificate,
                                OperatesOnTheBasisType.None),
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.Businessman)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => (OperatesOnTheBasisType)entity.OperatesOnTheBasisInGenitive.Value,
                                BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                                OperatesOnTheBasisType.Certificate,
                                OperatesOnTheBasisType.Warranty,
                                OperatesOnTheBasisType.RegistrationCertificate,
                                OperatesOnTheBasisType.None)
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.NaturalPerson)
                        {
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => (OperatesOnTheBasisType)entity.OperatesOnTheBasisInGenitive.Value,
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
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative, BLResources.RequiredFieldMessage, MetadataResources.PositionInNominative),
                        },
                    new ConsistencyRuleCollection<LegalPersonType>(LegalPersonType.Businessman)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative, BLResources.RequiredFieldMessage, MetadataResources.PositionInNominative)
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<OperatesOnTheBasisType>> RepresentativeAuthorityDocumentChecks =
            new List<ConsistencyRuleCollection<OperatesOnTheBasisType>>
                {
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Certificate)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.CertificateNumber, BLResources.RequiredFieldMessage, MetadataResources.CertificateNumber),
                            ConsistencyRule.CreateNonNull(entity => entity.CertificateDate, BLResources.RequiredFieldMessage, MetadataResources.CertificateDate)
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Warranty)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.WarrantyNumber, BLResources.RequiredFieldMessage, MetadataResources.WarrantyNumber),
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyBeginDate, BLResources.RequiredFieldMessage, MetadataResources.WarrantyBeginDate),
                            ConsistencyRule.CreateNonNull(entity => entity.WarrantyEndDate, BLResources.RequiredFieldMessage, MetadataResources.WarrantyEndDate),
                            ConsistencyRule.CreateFormat(entity => entity, profile => profile.WarrantyBeginDate.Value.Date > profile.WarrantyEndDate.Value.Date, BLResources.WarrantyBeginDateMustBeGreaterOrEqualThanEndDate),
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Bargain)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.BargainNumber, BLResources.RequiredFieldMessage, MetadataResources.BargainNumber),
                            ConsistencyRule.CreateNonNull(entity => entity.BargainBeginDate, BLResources.RequiredFieldMessage, MetadataResources.BargainBeginDate),
                            ConsistencyRule.CreateNonNull(entity => entity.BargainEndDate, BLResources.RequiredFieldMessage, MetadataResources.BargainEndDate),
                            ConsistencyRule.CreateFormat(entity => entity, profile => profile.BargainBeginDate.Value.Date > profile.BargainEndDate.Value.Date, BLResources.BargainBeginDateMustBeGreaterOrEqualThanEndDate),
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.RegistrationCertificate)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.RegistrationCertificateDate, BLResources.RequiredFieldMessage, MetadataResources.RegistrationCertificateDate),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.RegistrationCertificateNumber, BLResources.RequiredFieldMessage, MetadataResources.RegistrationCertificateNumber),
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
