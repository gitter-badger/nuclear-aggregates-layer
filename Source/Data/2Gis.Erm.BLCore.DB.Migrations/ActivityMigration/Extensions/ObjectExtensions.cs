using System;

namespace DoubleGis.Erm.BLCore.DB.Migrations.ActivityMigration.Extensions
{
    internal static class ObjectExtensions
    {
        public static TOut Map<TIn, TOut>(this TIn value, Func<TIn, TOut> projector)
        {
            return projector(value);
        }

        public static void DoIfNotNull<T>(this T obj, Action<T> action) where T : class
        {
            if (obj != null)
            {
                action(obj);
            }
        }
    }
}