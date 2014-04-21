namespace DoubleGis.Erm.Platform.Model.Metadata.Globalization
{
    // FIXME {all, 07.04.2014}: тут нужно хорошо подумать о списке культур, основной его смысл пока - вывод ApplicationCulture и DefaultCulture в ILocalizationSettings
    // COMMENT {i.maslennikov, 10.04.2014}: поскольку используется русская культура, выставил параметры как в IRussiaAdapted
    [GlobalizationSpecs(BusinessModel.Ukraine, "ru-RU", "ru")]
    public interface IUkraineAdapted : IAdapted
    {
    }
}