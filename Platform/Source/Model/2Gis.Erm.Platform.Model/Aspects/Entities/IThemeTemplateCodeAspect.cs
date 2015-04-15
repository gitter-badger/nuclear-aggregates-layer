using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Aspects.Entities
{
    public interface IThemeTemplateCodeAspect : IAspect
    {
        ThemeTemplateCode TemplateCode { get; set; }
    }
}