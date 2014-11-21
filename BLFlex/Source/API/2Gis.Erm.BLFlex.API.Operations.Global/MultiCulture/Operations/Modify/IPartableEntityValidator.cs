using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify
{
    public interface IPartableEntityValidator<in T> where T : IPartable
    {
        void Check(T entity);
    }
}
