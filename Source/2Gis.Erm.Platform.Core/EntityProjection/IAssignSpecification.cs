namespace DoubleGis.Erm.Platform.Core.EntityProjection
{
    public interface IAssignSpecification<in TSource, in TTarget>
    {
        void Assign(TSource source, TTarget target);
    }
}