namespace DoubleGis.Erm.Platform.Migration.Core
{
    /// <summary>
    /// Класс для создания экземпляра миграции.
    /// </summary>
    public sealed class AssemblyMigrationsProvider : IMigrationsProvider
    {
        public IMigration GetMigrationImplementation(MigrationDescriptor descriptor)
        {
            return descriptor.Type.Assembly.CreateInstance(descriptor.Type.FullName) as IMigration;
        }
    }
}