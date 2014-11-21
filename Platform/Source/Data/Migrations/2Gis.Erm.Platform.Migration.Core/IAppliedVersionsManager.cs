namespace DoubleGis.Erm.Platform.Migration.Core
    {
    public interface IAppliedVersionsManager : IAppliedVersionsReader
    {
        void DeleteVersion(long version);
        void SaveVersionInfo(long version);
    }
}