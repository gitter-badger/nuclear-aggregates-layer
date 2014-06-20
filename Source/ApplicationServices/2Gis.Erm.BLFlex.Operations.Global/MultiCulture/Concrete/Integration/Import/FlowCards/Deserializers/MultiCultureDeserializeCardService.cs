using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Integration.Import.FlowCards.Deserializers
{
    public sealed class MultiCultureDeserializeCardService : IDeserializeServiceBusObjectService<CardServiceBusDto>, IRussiaAdapted, IChileAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted
    {
        public IServiceBusDto Deserialize(XElement xml)
        {
            return new MultiCultureCardServiceBusDto { Content = xml };
        }

        public bool Validate(XElement xml, out string errorsMessage)
        {
            errorsMessage = null;
            return true;
        }

        public bool CanDeserialize(XElement xml)
        {
            return true;
        }
    }
}