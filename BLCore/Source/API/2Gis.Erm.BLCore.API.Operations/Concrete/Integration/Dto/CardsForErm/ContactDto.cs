using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm
{
    public sealed class ContactDto
    {
        public ContactType ContactType { get; set; }
        public string Contact { get; set; }
        public int SortingPosition { get; set; }
    }
}