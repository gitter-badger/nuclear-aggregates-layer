namespace DoubleGis.Erm.Platform.Core.EntityProjection
{
    public interface IProjectSpecification<in TInput, out TOutput>
    {
        TOutput Project(TInput input);
    }
}