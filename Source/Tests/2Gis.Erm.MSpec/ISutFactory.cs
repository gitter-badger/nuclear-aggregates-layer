namespace DoubleGis.Erm.MSpec
{
    public interface ISutFactory<out TSubject> where TSubject : class
    {
        TSubject Create();
        T AddDependency<T>(T dependency);
        void RegisterDependency<TDependency, TImpl>() where TImpl : TDependency;
        T Resolve<T>();
    }
}