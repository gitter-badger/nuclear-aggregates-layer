using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine
{
    public interface ILegalPersonEgrpouHolder
    {
        string Egrpou { get; set; }

        string BusinessmanEgrpou { get; set; }

        LegalPersonType LegalPersonType { get; }
    }

    public static class Extensions
    {
        public static string GetEgrpou(this ILegalPersonEgrpouHolder model)
        {
            switch (model.LegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    return model.Egrpou;

                case LegalPersonType.Businessman:
                    return model.BusinessmanEgrpou;
            }

            System.Diagnostics.Debug.Assert(true, "Неизвестный тип юрлица");
            return null;
        }

        public static void SetEgrpou(this ILegalPersonEgrpouHolder model, string egrpou)
        {
            switch (model.LegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    model.Egrpou = egrpou;
                    break;

                case LegalPersonType.Businessman:
                    model.BusinessmanEgrpou = egrpou;
                    break;
            }

            System.Diagnostics.Debug.Assert(true, "Неизвестный тип юрлица");
        }
    }
}
