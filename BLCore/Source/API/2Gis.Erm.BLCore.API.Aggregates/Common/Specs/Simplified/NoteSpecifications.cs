using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
{
    public class NoteSpecifications
    {
        public static class Find
        {
            public static FindSpecification<Note> ById(long id)
            {
                return new FindSpecification<Note>(x => x.Id == id);
            }
        }
    }
}