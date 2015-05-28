namespace NuClear.Storage.Specifications
{
    public interface IMapSpecification<in TInput, out TOutput>
    {
        TOutput Map(TInput input);
    }
}