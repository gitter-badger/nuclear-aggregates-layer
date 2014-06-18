using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public sealed class EmiratesLegalPersonProfileConsistencyRuleContainer : ILegalPersonProfileConsistencyRuleContainer
    {
        #region Rules

        private static readonly IEnumerable<IConsistencyRule> CommonRules = new IConsistencyRule[]
            {
                ConsistencyRule.CreateNonEmptyString(entity => entity.Name, BLResources.RequiredFieldMessage, MetadataResources.Name),
                ConsistencyRule.CreateNonEmptyString(entity => entity.ChiefNameInNominative,
                                                     BLResources.RequiredFieldMessage,
                                                     MetadataResources.ChiefNameInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PositionInNominative,
                                                     BLResources.RequiredFieldMessage,
                                                     MetadataResources.PositionInNominative),
                ConsistencyRule.CreateNonEmptyString(entity => entity.PersonResponsibleForDocuments,
                                                     BLResources.RequiredFieldMessage,
                                                     MetadataResources.PersonResponsibleForDocuments),
                ConsistencyRule.CreateFormat(entity => (DocumentsDeliveryMethod)entity.DocumentsDeliveryMethod,
                                             method => method == DocumentsDeliveryMethod.Undefined,
                                             BLResources.RequiredFieldMessage,
                                             MetadataResources.DocumentsDeliveryMethod),
                ConsistencyRule.CreateFormat(entity => (PaymentMethod)entity.PaymentMethod,
                                             x => x == PaymentMethod.Undefined,
                                             BLResources.RequiredFieldMessage,
                                             MetadataResources.PaymentMethod),
            };

        private static readonly IEnumerable<ConsistencyRuleCollection<PaymentMethod>> RequiredByPaymentMethodFields =
            new List<ConsistencyRuleCollection<PaymentMethod>>
                {
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankTransaction)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.IBAN, BLResources.RequiredFieldMessage, MetadataResources.IBAN),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.SWIFT, BLResources.RequiredFieldMessage, MetadataResources.SWIFT),
                        },
                    new ConsistencyRuleCollection<PaymentMethod>(PaymentMethod.BankChequePayment)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.IBAN, BLResources.RequiredFieldMessage, MetadataResources.IBAN),
                            ConsistencyRule.CreateNonEmptyString(entity => entity.SWIFT, BLResources.RequiredFieldMessage, MetadataResources.SWIFT),
                        },
                };

        private static readonly IEnumerable<ConsistencyRuleCollection<DocumentsDeliveryMethod>> RequiredByDocumentsDeliveryMethodFields =
            new List<ConsistencyRuleCollection<DocumentsDeliveryMethod>>
                {
                    new ConsistencyRuleCollection<DocumentsDeliveryMethod>(DocumentsDeliveryMethod.PostOnly)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.PostAddress, BLResources.RequiredFieldMessage, MetadataResources.PostAddress),
                        },
                    new ConsistencyRuleCollection<DocumentsDeliveryMethod>(DocumentsDeliveryMethod.ByEmail)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.EmailForAccountingDocuments,
                                                                 BLResources.RequiredFieldMessage,
                                                                 MetadataResources.EmailForAccountingDocuments),
                        },
                    new ConsistencyRuleCollection<DocumentsDeliveryMethod>(DocumentsDeliveryMethod.ByCourier)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.DocumentsDeliveryAddress,
                                                                 BLResources.RequiredFieldMessage,
                                                                 MetadataResources.DocumentsDeliveryAddress),
                        },
                    new ConsistencyRuleCollection<DocumentsDeliveryMethod>(DocumentsDeliveryMethod.DeliveryByManager)
                        {
                            ConsistencyRule.CreateNonEmptyString(entity => entity.DocumentsDeliveryAddress,
                                                                 BLResources.RequiredFieldMessage,
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
                var paymentMethod = (PaymentMethod)profile.PaymentMethod.Value;
                foreach (var rule in GetPaymentMethodChecks(paymentMethod))
                {
                    yield return rule;
                }
            }


            var documentsDeliveryMethod = (DocumentsDeliveryMethod)profile.DocumentsDeliveryMethod;
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