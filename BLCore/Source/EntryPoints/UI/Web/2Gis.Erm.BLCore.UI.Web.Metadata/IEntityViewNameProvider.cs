using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.UI.Web.Metadata
{
    // Не совсем уверен, где это должно располагаться. Но планируется использование в Web.MVC
    public interface IEntityViewNameProvider
    {
        string GetView<TViewModel, TEntity>()
            where TViewModel : class, IEntityViewModelAbstract<TEntity> 
            where TEntity : IEntityKey;
    }
}
