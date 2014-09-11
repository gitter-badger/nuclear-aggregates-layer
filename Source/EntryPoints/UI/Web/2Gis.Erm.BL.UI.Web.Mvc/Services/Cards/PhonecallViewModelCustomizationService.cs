﻿using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards
{
    public class PhonecallViewModelCustomizationService : IGenericViewModelCustomizationService<Phonecall>
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            ActivityViewModelCustomizationServiceHelper.CustomizeViewModel(viewModel, modelState);
        }
    }
}