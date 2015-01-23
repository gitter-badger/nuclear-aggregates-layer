using System.Web.Mvc;


using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class NullViewModelCustomizationService<TEntity> : IGenericViewModelCustomizationService<TEntity> where TEntity : class, IEntityKey
    {
        public void CustomizeViewModel(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            // do nothing
        }
    }
}