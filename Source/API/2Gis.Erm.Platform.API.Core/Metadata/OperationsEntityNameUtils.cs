using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.Platform.API.Core.Metadata
{
    public static class OperationsEntityNameUtils
    {
        // TODO {all, 01.08.2013}: изменить индикатор open generic реализации для entity specific операции EntityName.None -> EntityName.All + проверить чтобы не развалились метаданные, масспроцессоры и т.п.
        public const EntityName EntitySpecificGenericImplementationIndicator = EntityName.None;

        public static bool IsOpenGenericOperation(this EntitySet entitySet)
        {
            return entitySet.Entities.Length == 1 && entitySet.Entities[0] == EntitySpecificGenericImplementationIndicator;
        }
    }
}
