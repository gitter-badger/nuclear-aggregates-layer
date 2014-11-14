﻿using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AssociatedPositions
{
    public class AssociatedPositionsPriceIsDeletedCustomization : IViewModelCustomization
    {
        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (AssociatedPositionViewModel)viewModel;
            if (!entityViewModel.PriceIsDeleted)
            {
                return;
            }

            entityViewModel.ViewConfig.ReadOnly = true;
        }
    }
}