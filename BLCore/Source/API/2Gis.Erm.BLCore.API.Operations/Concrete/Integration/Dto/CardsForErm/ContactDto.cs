using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm
{
    public class ContactDto
    {
        public ContactType ContactType { get; set; }
        public string Contact { get; set; }
        public int SortingPosition { get; set; }

        // TODO {y.baranihin, 03.06.2014}: Необычно, что в DTO есть поведение. Может быть вынести это в extension-метод (если использований много) или в вызывающий код?
        // DONE {d.ivanov, 03.06.2014}: я только за. Хотя, возможно, это стоило делать не в рамках адаптации.
    }
}