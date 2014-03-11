namespace DoubleGis.Erm.Platform.Model.Entities.Enums
{
    public enum FasComment
    {
        Alcohol = 0,
        Supplements = 1,
        Smoke = 2,
        Drugs = 3,
        DrugsAndService = 4,
        Abortion = 5,
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
        Biocides = 208
    }

    // FIXME {a.tukaev, 10.03.2014}: Зачем нам FasComment и такой же по структуре FasCommentDisplayText? Можем оставить только один?
    // DONE {d.ivanov, 11.03.2014}: Сделано
}
