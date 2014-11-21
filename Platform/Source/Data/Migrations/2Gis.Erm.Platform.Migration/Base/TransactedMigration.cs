namespace DoubleGis.Erm.Platform.Migration.Base
{
    /// <summary>
    /// Класс с поддержкой транзакций.
    /// </summary>
    public abstract class TransactedMigration : IContextedMigration<IMigrationContext>
    {
        public void Apply(IMigrationContext context)
        {
            context.Connection.BeginTransaction();

            try
            {
                ApplyOverride(context);
                context.Connection.CommitTransaction();
            }
            catch
            {
                context.Connection.RollBackTransaction();
                throw;
            }
        }

        public void Revert(IMigrationContext context)
        {
            context.Connection.BeginTransaction();
            try
            {
                RevertOverride(context);
                context.Connection.CommitTransaction();
            }
            catch
            {
                context.Connection.RollBackTransaction();
            }
        }

        protected abstract void ApplyOverride(IMigrationContext context);

        protected virtual void RevertOverride(IMigrationContext context)
        {
        }
    }
}
