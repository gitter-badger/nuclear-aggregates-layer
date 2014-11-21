﻿using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Services;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationService : IUIService
    {
        void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState);
    }

    public interface IGenericViewModelCustomizationService<TEntity> : IEntityUIService<TEntity>, IViewModelCustomizationService where TEntity : class, IEntityKey
    {
    }
}