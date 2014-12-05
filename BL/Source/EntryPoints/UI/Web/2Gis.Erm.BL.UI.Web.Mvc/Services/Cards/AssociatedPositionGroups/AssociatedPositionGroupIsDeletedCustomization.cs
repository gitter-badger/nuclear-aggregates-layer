﻿using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.AssociatedPositionGroups
{
    public sealed class AssociatedPositionGroupIsDeletedCustomization : IViewModelCustomization<AssociatedPositionsGroupViewModel>
    {
        public void Customize(AssociatedPositionsGroupViewModel viewModel, ModelStateDictionary modelState)
        {            
            if (!viewModel.IsDeleted)
            {
                return;
            }

            viewModel.ViewConfig.ReadOnly = true;
            viewModel.SetInfo(BLResources.CantEditDeactivatedAssociatedPositionsGroup);
        }
    }
}