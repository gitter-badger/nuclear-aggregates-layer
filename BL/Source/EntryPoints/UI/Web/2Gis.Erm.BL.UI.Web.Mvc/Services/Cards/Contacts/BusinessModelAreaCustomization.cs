﻿using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Contacts
{
    public sealed class BusinessModelAreaCustomization : IViewModelCustomization<ICustomizableContactViewModel>
    {
        private readonly IBusinessModelSettings _businessModelSettings;

        public BusinessModelAreaCustomization(IBusinessModelSettings businessModelSettings)
        {
            _businessModelSettings = businessModelSettings;
        }

        public void Customize(ICustomizableContactViewModel viewModel, ModelStateDictionary modelState)
        {
            viewModel.BusinessModelArea = _businessModelSettings.BusinessModel.ToString();
        }
    }
}