namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum FasComment
    {
        Alcohol = 0,
        Supplements = 1,
        Drugs = 3,
        DrugsAndService = 4,
        NewFasComment = 6,

        // TODO {d.ivanov, a.tukaev, 10.03.2014}: Либо реализовать возможность расширения enum-ов на уровне компонентов (Core и BLFlex), либо отказаться от enum-а и сделать для комментария ФАС полноценный справочник
        // czech
        AlcoholAdvertising = 200,
        MedsMultiple = 201,
        MedsSingle = 202,
        DietarySupplement = 203,
        SpecialNutrition = 204,
        ChildNutrition = 205,
        FinancilaServices = 206,
        MedsTraditional = 207,
        Biocides = 208,

        ChileAlcohol = 301,
        ChileDrugsAndService = 302,
        ChileMedicalReceiptDrugs = 303,

        UkraineDrugsAndService = 401,
        UkraineAlcohol = 402,
        UkraineTobaccoGoods = 403,
    }
}
