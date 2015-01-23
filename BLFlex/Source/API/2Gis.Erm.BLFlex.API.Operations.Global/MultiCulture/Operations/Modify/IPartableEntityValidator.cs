using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Modify
{
    public interface IPartableEntityValidator<in T> where T : IPartable
    {
        void Check(T entity);
    }
}
