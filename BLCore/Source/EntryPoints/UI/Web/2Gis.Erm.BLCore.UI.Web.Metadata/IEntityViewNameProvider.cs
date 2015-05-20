using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Web.Metadata
{
    public interface IEntityViewNameProvider
    {
        string GetView<TViewModel, TEntity>()
            where TViewModel : class, IViewModel
            where TEntity : IEntityKey;
    }
}
