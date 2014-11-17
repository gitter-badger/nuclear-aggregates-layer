﻿using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.DeniedPositions
{
    public sealed class DeniedPositionsPriceIsPublishedCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (DeniedPositionViewModel)viewModel;
            if (!entityViewModel.IsPricePublished)
            {
                return;
            }

            entityViewModel.ViewConfig.ReadOnly = true;
            entityViewModel.SetInfo(entityViewModel.IsNew
                                        ? BLResources.CantAddDeniedPositionWhenPriceIsPublished
                                        : BLResources.CantEditDeniedPositionWhenPriceIsPublished);
        }
    }
}