namespace DoubleGis.Erm.Platform.Core.EntityProjection
{
    // FIXME {d.ivanov, 15.09.2014}: Move to 2Gis.Erm.Platform.Common
    public interface IAssignSpecification<in TSource, in TTarget>
    {
        void Assign(TSource source, TTarget target);
    }
}