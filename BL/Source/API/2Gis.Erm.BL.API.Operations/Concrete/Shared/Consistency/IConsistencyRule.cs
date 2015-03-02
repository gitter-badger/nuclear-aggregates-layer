namespace DoubleGis.Erm.BL.API.Operations.Concrete.Shared.Consistency
{
    public interface IConsistencyRule
    {
        void Apply(object obj);
    }

    public interface IConsistencyRule<in TArg> : IConsistencyRule
    {
        void Apply(TArg obj);
    }
}