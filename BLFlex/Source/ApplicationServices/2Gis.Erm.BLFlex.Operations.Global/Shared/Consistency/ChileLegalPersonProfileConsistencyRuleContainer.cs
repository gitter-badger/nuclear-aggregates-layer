using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting;
using DoubleGis.Erm.Platform.Aggregates.EAV;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile;
using DoubleGis.Erm.Platform.Resources.Server;

using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class ChileLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer
    {
        #region Rules
        private static readonly IEnumerable<IConsistencyRule> CommonRules = new IConsistencyRule[]
            {
                ConsistencyRule.CreateNonEmptyString(entity => entity.Name, ResPlatform.RequiredFieldMessage, MetadataResources.Name),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInNominative, ResPlatform.RequiredFieldMessage, MetadataResources.ChiefNameInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PersonResponsibleForDocuments, ResPlatform.RequiredFieldMessage, MetadataResources.PersonResponsibleForDocuments),
                ConsistencyRule.CreateFormat(entity => entity.DocumentsDeliveryMethod, method => method == DocumentsDeliveryMethod.Undefined, ResPlatform.RequiredFieldMessage, MetadataResources.DocumentsDeliveryMethod),
                ConsistencyRule.CreateNonNull(entity => entity.OperatesOnTheBasisInGenitive, ResPlatform.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive),
                ConsistencyRule.CreateNonNull(entity => entity.PaymentMethod, ResPlatform.RequiredFieldMessage, MetadataResources.PaymentMethod),
                ConsistencyRule.CreateFormat(entity => entity.Parts.OfType<ChileLegalPersonProfilePart>(), chileParts => chileParts.Count() != 1, BLFlexResources.ChilePartableExtensionMustBeApplied),
                ConsistencyRule.CreateNonEmptyString(entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.RepresentativeRut), ResPlatform.RequiredFieldMessage, MetadataResources.Rut),
                new InnConsistencyRule<LegalPersonProfile, ChileRutService>(entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.RepresentativeRut)),
                ConsistencyRule.CreateEnumValuesRestriction(
                    entity => entity.PaymentMethod.Value,
                    string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.PaymentMethod),
                    PaymentMethod.CashPayment,
                    PaymentMethod.BankTransaction,
                    PaymentMethod.CreditCard,
                    PaymentMethod.DebitCard,
                    PaymentMethod.BankChequePayment),
                ConsistencyRule.CreateEnumValuesRestriction(
                    entity => entity.OperatesOnTheBasisInGenitive.Value,
                    BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                    OperatesOnTheBasisType.Charter,
                    OperatesOnTheBasisType.Warranty),
            };

        private static readonly IEnumerable<ConsistencyRuleCollection<OperatesOnTheBasisType>> RepresentativeAuthorityDocumentChecks =
            new List<ConsistencyRuleCollection<OperatesOnTheBasisType>>
                {
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Warranty)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.RepresentativeAuthorityDocumentIssuedOn), ResPlatform.RequiredFieldMessage, MetadataResources.RepresentativeDocumentIssuedOn)
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Charter)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.RepresentativeAuthorityDocumentIssuedOn), ResPlatform.RequiredFieldMessage, MetadataResources.RepresentativeDocumentIssuedOn)
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<PaymentMethod>> PaymentMethodChecks =
            new List<ConsistencyRuleCollection<PaymentMethod>>
                {
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankTransaction)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.BankId), ResPlatform.RequiredFieldMessage, MetadataResources.BankName),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, ResPlatform.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.AccountType),
                                string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.BankAccountType),
                                AccountType.CurrentAccount,
                                AccountType.SavingsAccount),
                        },
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.CreditCard)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.BankId), ResPlatform.RequiredFieldMessage, MetadataResources.BankName),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, ResPlatform.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.AccountType),
                                string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.BankAccountType),
                                AccountType.CurrentAccount,
                                AccountType.SavingsAccount),
                        },
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.DebitCard)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.BankId), ResPlatform.RequiredFieldMessage, MetadataResources.BankName),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, ResPlatform.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.AccountType),
                                string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.BankAccountType),
                                AccountType.CurrentAccount,
                                AccountType.SavingsAccount),
                        },
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankChequePayment)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.BankId), ResPlatform.RequiredFieldMessage, MetadataResources.BankName),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, ResPlatform.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.Within<ChileLegalPersonProfilePart>().GetPropertyValue(part => part.AccountType),
                                string.Format(ResPlatform.RequiredFieldMessage, MetadataResources.BankAccountType),
                                AccountType.CurrentAccount,
                                AccountType.SavingsAccount),
                        },
                };
        #endregion

        public IEnumerable<IConsistencyRule> GetApplicableRules(LegalPerson person, LegalPersonProfile profile)
        {
            foreach (var rule in CommonRules)
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
