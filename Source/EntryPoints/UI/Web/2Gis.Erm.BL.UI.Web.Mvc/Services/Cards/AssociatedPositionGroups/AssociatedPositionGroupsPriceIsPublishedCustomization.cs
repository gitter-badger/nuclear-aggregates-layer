﻿using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AssociatedPositionGroups
{
    public sealed class AssociatedPositionGroupsPriceIsPublishedCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (AssociatedPositionsGroupViewModel)viewModel;
            if (!entityViewModel.PriceIsPublished)
            {
                return;
            }

            entityViewModel.ViewConfig.ReadOnly = true;
            entityViewModel.SetInfo(entityViewModel.IsNew
                                        ? BLResources.CantAddNewGroupWhenPriceIsPublished
                                        : BLResources.CantEditGroupWhenPriceIsPublished);
        }
    }
}
