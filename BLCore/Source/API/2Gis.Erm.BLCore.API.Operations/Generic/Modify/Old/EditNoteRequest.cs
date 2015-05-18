using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old
{
    public class EditNoteRequest : EditRequest<Note>
    {
        public IEntityType ParentTypeName { get; set; }
    }
}