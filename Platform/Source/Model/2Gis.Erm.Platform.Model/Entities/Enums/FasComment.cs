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

        // Были 401, 402, 403 - но теперь полностью выпилены. Использовать повторно не стоит.
        UkraineAutotherapy = 404,
        UkraineDrugs = 405,
        UkraineMedicalDevice = 406,
        UkraineAlcohol = 407,
        UkraineSoundPhonogram = 408,
        UkraineSoundLive = 409,
        UkraineEmploymentAssistance = 410,
    }
}
