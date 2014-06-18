using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Integration.Dto.Cards
{
    public sealed class MultiCultureCardServiceBusDto : CardServiceBusDto, IRussiaAdapted, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted
    {
        public XElement Content { get; set; }
    }
}