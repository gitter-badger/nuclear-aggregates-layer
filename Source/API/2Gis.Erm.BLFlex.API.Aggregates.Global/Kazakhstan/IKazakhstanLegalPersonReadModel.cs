using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Aggregates.Global.Kazakhstan
{
    public interface IKazakhstanLegalPersonReadModel : IAggregateReadModel<LegalPerson>, IKazakhstanAdapted
    {
        CheckForDublicatesResultDto CheckIfExistsIdentityCardDuplicate(long legalPersonId, string identityCardNumber);
    }
}