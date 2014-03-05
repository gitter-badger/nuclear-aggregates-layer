using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Crosscutting;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Update.AdvertisementElements
{
    public interface IModifyingAdvertisementElementValidator : IInvariantSafeCrosscuttingService
    {
        void Validate(AdvertisementElementTemplate elementTemplate, IEnumerable<AdvertisementElement> elements, string elementPlainText, string elementFormattedText);
    }
}
