using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IFinderBaseProvider
    {
        IFinderBase GetFinderBase(EntityName entityName);
    }
}