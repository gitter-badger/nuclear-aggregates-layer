namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Consistency
{
    public interface IConsistencyRule
    {
        void Apply(object obj);
    }

    internal interface IConsistencyRule<in TArg> : IConsistencyRule
    {
        void Apply(TArg obj);
    }
}
