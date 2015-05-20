using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Currencies;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public class DeleteCurrencyOperationService : IDeleteGenericEntityService<DoubleGis.Erm.Platform.Model.Entities.Erm.Currency>
    {
        private readonly ICurrencyService _currencyService;

        public DeleteCurrencyOperationService(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public DeleteConfirmation Delete(long entityId)
        {
            var currencyInfo = _currencyService.GetCurrencyWithRelations(entityId);

            if (currencyInfo == null)
            {
                throw new ArgumentException(BLResources.EntityNotFound);
            }
            if (currencyInfo.Currency.IsBase)
            {
                throw new ArgumentException(BLResources.CurrencyController_BaseCurrencyCannotBeDeleted);
            }
            if (!string.IsNullOrEmpty(currencyInfo.RelatedCountryName))
            {
                throw new ArgumentException(string.Format(BLResources.CurrencyController_RelatedCountryExists,
                                                          currencyInfo.Currency.Name, currencyInfo.RelatedCountryName));
            }
            if (currencyInfo.RelatedCurrencyRateCreateDate != null)
            {
                throw new ArgumentException(string.Format(BLResources.CurrencyController_RelatedCurrencyRateExists,
                                                          currencyInfo.Currency.Name, currencyInfo.RelatedCurrencyRateCreateDate.Value));
            }

            _currencyService.Delete(currencyInfo.Currency);
            return null;
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            var currencyInfo = _currencyService.GetCurrencyWithRelations(entityId);

            if (currencyInfo == null)
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.EntityNotFound
                };

            if (currencyInfo.Currency.IsBase)
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = BLResources.CurrencyController_BaseCurrencyCannotBeDeleted
                };

            if (!string.IsNullOrEmpty(currencyInfo.RelatedCountryName))
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = string.Format(BLResources.CurrencyController_RelatedCountryExists, currencyInfo.Currency.Name, currencyInfo.RelatedCountryName)
                };

            if (currencyInfo.RelatedCurrencyRateCreateDate != null)
                return new DeleteConfirmationInfo
                {
                    IsDeleteAllowed = false,
                    DeleteDisallowedReason = string.Format(BLResources.CurrencyController_RelatedCurrencyRateExists, currencyInfo.Currency.Name, currencyInfo.RelatedCurrencyRateCreateDate.Value)
                };

            return new DeleteConfirmationInfo
            {
                EntityCode = currencyInfo.Currency.Name,
                IsDeleteAllowed = true,
                DeleteConfirmation = string.Empty
            };
        }
    }
}