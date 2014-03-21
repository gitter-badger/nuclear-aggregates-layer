using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class ChileLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer
    {
        #region Rules
        private static readonly IEnumerable<IConsistencyRule> CommonRules = new IConsistencyRule[]
            {
                ConsistencyRule.CreateNonEmptyString(entity => entity.Name, BLResources.RequiredFieldMessage, MetadataResources.Name),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInNominative, BLResources.RequiredFieldMessage, MetadataResources.ChiefNameInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PersonResponsibleForDocuments, BLResources.RequiredFieldMessage, MetadataResources.PersonResponsibleForDocuments),
                ConsistencyRule.CreateFormat(entity => (DocumentsDeliveryMethod)entity.DocumentsDeliveryMethod, method => method == DocumentsDeliveryMethod.Undefined, BLResources.RequiredFieldMessage, MetadataResources.DocumentsDeliveryMethod),
                ConsistencyRule.CreateNonNull(entity => entity.OperatesOnTheBasisInGenitive, BLResources.RequiredFieldMessage, MetadataResources.OperatesOnTheBasisInGenitive),
                ConsistencyRule.CreateNonNull(entity => entity.PaymentMethod, BLResources.RequiredFieldMessage, MetadataResources.PaymentMethod),
                ConsistencyRule.CreateFormat(entity => entity.Parts.OfType<LegalPersonProfilePart>(), chileParts => chileParts.Count() != 1, BLFlexResources.ChilePartableExtensionMustBeApplied),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChilePart().RepresentativeRut, BLResources.RequiredFieldMessage, MetadataResources.Rut),
                new InnConsistencyRule<LegalPersonProfile, ChileRutService>(entity => entity.ChilePart().RepresentativeRut),
                ConsistencyRule.CreateEnumValuesRestriction(
                    entity => (PaymentMethod)entity.PaymentMethod.Value,
                    string.Format(BLResources.RequiredFieldMessage, MetadataResources.PaymentMethod),
                    PaymentMethod.CashPayment,
                    PaymentMethod.BankTransaction,
                    PaymentMethod.BankCard,
                    PaymentMethod.DebitCard,
                    PaymentMethod.BankChequePayment),
                ConsistencyRule.CreateEnumValuesRestriction(
                    entity => (OperatesOnTheBasisType)entity.OperatesOnTheBasisInGenitive.Value,
                    BLResources.ThisDocumentIsNotAllowedForThatTypeOfLegalPerson,
                    OperatesOnTheBasisType.Charter,
                    OperatesOnTheBasisType.Warranty),
            };

        private static readonly IEnumerable<ConsistencyRuleCollection<OperatesOnTheBasisType>> RepresentativeAuthorityDocumentChecks =
            new List<ConsistencyRuleCollection<OperatesOnTheBasisType>>
                {
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Warranty)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.ChilePart().RepresentativeAuthorityDocumentIssuedOn, BLResources.RequiredFieldMessage, MetadataResources.RepresentativeDocumentIssuedOn)
                        },
                    new ConsistencyRuleCollection<OperatesOnTheBasisType>(OperatesOnTheBasisType.Charter)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.ChilePart().RepresentativeAuthorityDocumentIssuedOn, BLResources.RequiredFieldMessage, MetadataResources.RepresentativeDocumentIssuedOn)
                        }
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<PaymentMethod>> PaymentMethodChecks =
            new List<ConsistencyRuleCollection<PaymentMethod>>
                {
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankTransaction)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.ChilePart().BankId, BLResources.RequiredFieldMessage, MetadataResources.BankName),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, BLResources.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.ChilePart().AccountType,
                                string.Format(BLResources.RequiredFieldMessage, MetadataResources.BankAccountType),
                                AccountType.CurrentAccount,
                                AccountType.SavingsAccount),
                        },
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.CreditCard)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.ChilePart().BankId, BLResources.RequiredFieldMessage, MetadataResources.BankName),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, BLResources.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.ChilePart().AccountType,
                                string.Format(BLResources.RequiredFieldMessage, MetadataResources.BankAccountType),
                                AccountType.CurrentAccount,
                                AccountType.SavingsAccount),
                        },
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.DebitCard)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.ChilePart().BankId, BLResources.RequiredFieldMessage, MetadataResources.BankName),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, BLResources.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.ChilePart().AccountType,
                                string.Format(BLResources.RequiredFieldMessage, MetadataResources.BankAccountType),
                                AccountType.CurrentAccount,
                                AccountType.SavingsAccount),
                        },
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankChequePayment)
                        {
                            ConsistencyRule.CreateNonNull(entity => entity.ChilePart().BankId, BLResources.RequiredFieldMessage, MetadataResources.BankName),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.AccountNumber, BLResources.RequiredFieldMessage, MetadataResources.AccountNumber),
                            ConsistencyRule.CreateEnumValuesRestriction(
                                entity => entity.ChilePart().AccountType,
                                string.Format(BLResources.RequiredFieldMessage, MetadataResources.BankAccountType),
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
