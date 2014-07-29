using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old
{
    public class EditNoteRequest : EditRequest<Note>
    {
        public EntityName ParentTypeName { get; set; }
    }
}