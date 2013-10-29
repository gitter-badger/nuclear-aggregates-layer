using System;
using System.Transactions;

namespace DoubleGis.Erm.Platform.DAL.Transactions
{
    public static class DefaultTransactionOptions
    {
        public static TransactionOptions Default
        {
            get
            {
                return new TransactionOptions
                    {
                        IsolationLevel = IsolationLevel.Snapshot,
                        Timeout = TimeSpan.Zero
                    };
            }
        }
    }
}