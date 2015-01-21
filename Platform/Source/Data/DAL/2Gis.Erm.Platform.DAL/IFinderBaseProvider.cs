using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IFinderBaseProvider
    {
        IFinderBase GetFinderBase(IEntityType entityName);
    }
}