using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Positions
{
    public static class PositionTools
    {
        // ДгппИд элемента номенклатуры "пакет "Дополнительный"" нужен для костыля-исключения на 2+2 месяца (до Июля)
        // Этому комментарию скоро исполнится 2 года.
        public const int AdditionalPackageDgppId = 11572;

        public static bool KeepCategoriesSynced(this Position position)
        {
            return position.IsComposite && position.DgppId != AdditionalPackageDgppId;
        }

        public static bool AllSubpositionsMustBePicked(this Position position)
        {
            return position.IsComposite && position.DgppId != AdditionalPackageDgppId;
        }

        public static bool AutoCheckSubpositionsWithFirmBindingType(this Position position)
        {
            return position.IsComposite && position.DgppId != AdditionalPackageDgppId;
        }
    }
}
