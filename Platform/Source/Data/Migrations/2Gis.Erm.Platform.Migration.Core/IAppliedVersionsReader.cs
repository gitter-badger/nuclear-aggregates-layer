namespace DoubleGis.Erm.Platform.Migration.Core
{
    public interface IAppliedVersionsReader
    {
        AppliedVersionsInfo AppliedVersionsInfo { get; }
        void LoadVersionInfo();
    }
}
