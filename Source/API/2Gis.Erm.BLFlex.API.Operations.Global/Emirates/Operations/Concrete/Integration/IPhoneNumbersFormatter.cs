using System.Collections.Generic;

using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration
{
    // COMMENT {all, 27.05.2014}: В случае отказа от хранимки импорта карточек на остальных инсталяциях, этот интерфейс можно будет перенести в BLCore
    public interface IPhoneNumbersFormatter
    {
        void FormatPhoneNumbers(IEnumerable<ContactDto> phones);
    }
}
