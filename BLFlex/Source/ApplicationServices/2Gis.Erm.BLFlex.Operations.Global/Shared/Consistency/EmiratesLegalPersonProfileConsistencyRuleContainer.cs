using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Resources.Server;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class EmiratesLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer
    {
        #region Rules

        private static readonly IEnumerable<IConsistencyRule> CommonRules = new IConsistencyRule[]
            {
                ConsistencyRule.CreateNonEmptyString(entity => entity.Name, ResPlatform.RequiredFieldMessage, MetadataResources.Name),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInNominative,
                                                     ResPlatform.RequiredFieldMessage,
                                                     MetadataResources.ChiefNameInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative,
                                                     ResPlatform.RequiredFieldMessage,
                                                     MetadataResources.PositionInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PersonResponsibleForDocuments,
                                                     ResPlatform.RequiredFieldMessage,
                                                     MetadataResources.PersonResponsibleForDocuments),
                ConsistencyRule.CreateFormat(entity => entity.DocumentsDeliveryMethod,
                                             method => method == DocumentsDeliveryMethod.Undefined,
                                             ResPlatform.RequiredFieldMessage,
                                             MetadataResources.DocumentsDeliveryMethod),
                ConsistencyRule.CreateFormat(entity => (PaymentMethod)entity.PaymentMethod,
                                             x => x == PaymentMethod.Undefined,
                                             ResPlatform.RequiredFieldMessage,
                                             MetadataResources.PaymentMethod),
            };

        private static readonly IEnumerable<ConsistencyRuleCollection<PaymentMethod>> RequiredByPaymentMethodFields =
            new List<ConsistencyRuleCollection<PaymentMethod>>
                {
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankTransaction)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.IBAN, ResPlatform.RequiredFieldMessage, MetadataResources.IBAN),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.SWIFT, ResPlatform.RequiredFieldMessage, MetadataResources.SWIFT),
                        },
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankChequePayment)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.IBAN, ResPlatform.RequiredFieldMessage, MetadataResources.IBAN),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.SWIFT, ResPlatform.RequiredFieldMessage, MetadataResources.SWIFT),
                        },
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<DocumentsDeliveryMethod>> RequiredByDocumentsDeliveryMethodFields =
            new List<ConsistencyRuleCollection<DocumentsDeliveryMethod>>
                {
                    new ConsistencyRuleCollection<DocumentsDeliveryMethod>(DocumentsDeliveryMethod.PostOnly)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PostAddress, ResPlatform.RequiredFieldMessage, MetadataResources.PostAddress),
                        },
                    new ConsistencyRuleCollection<DocumentsDeliveryMethod>(DocumentsDeliveryMethod.ByEmail)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.EmailForAccountingDocuments,
                                                                 ResPlatform.RequiredFieldMessage,
                                                                 MetadataResources.EmailForAccountingDocuments),
                        },
                    new ConsistencyRuleCollection<DocumentsDeliveryMethod>(DocumentsDeliveryMethod.ByCourier)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.DocumentsDeliveryAddress,
                                                                 ResPlatform.RequiredFieldMessage,
                                                                 MetadataResources.DocumentsDeliveryAddress),
                        },
                    new ConsistencyRuleCollection<DocumentsDeliveryMethod>(DocumentsDeliveryMethod.DeliveryByManager)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.DocumentsDeliveryAddress,
                                                                 ResPlatform.RequiredFieldMessage,
                                                                 MetadataResources.DocumentsDeliveryAddress),
                        }
                };

        #endregion

        public IEnumerable<IConsistencyRule> GetApplicableRules(LegalPerson person, LegalPersonProfile profile)
        {
            foreach (var rule in CommonRules)
            {
                yield return rule;
            }

            if (profile.PaymentMethod.HasValue)
            {
                var paymentMethod = profile.PaymentMethod.Value;
                foreach (var rule in GetPaymentMethodChecks(paymentMethod))
                {
                    yield return rule;
                }
            }


            var documentsDeliveryMethod = profile.DocumentsDeliveryMethod;
            foreach (var rule in GetDocumentsDeliveryMethodChecks(documentsDeliveryMethod))
            {
                yield return rule;
            }

        }

        private static IEnumerable<IConsistencyRule> GetPaymentMethodChecks(PaymentMethod key)
        {
            IEnumerable<IConsistencyRule> result = RequiredByPaymentMethodFields.SingleOrDefault(collection => collection.Key == key);
            return result ?? new IConsistencyRule[0];
        }

        private static IEnumerable<IConsistencyRule> GetDocumentsDeliveryMethodChecks(DocumentsDeliveryMethod key)
        {
            IEnumerable<IConsistencyRule> result = RequiredByDocumentsDeliveryMethodFields.SingleOrDefault(collection => collection.Key == key);
            return result ?? new IConsistencyRule[0];
        }
    }
}