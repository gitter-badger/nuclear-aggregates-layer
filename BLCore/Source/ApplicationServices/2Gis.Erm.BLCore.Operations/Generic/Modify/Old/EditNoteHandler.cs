using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old
{
    public sealed class EditNoteHandler : RequestHandler<EditNoteRequest, EmptyResponse>
    {
        private readonly INoteService _noteService;
        private readonly IUserContext _userContext;

        public EditNoteHandler(IUserContext userContext, INoteService noteService)
        {
            _userContext = userContext;
            _noteService = noteService;
        }

        protected override EmptyResponse Handle(EditNoteRequest request)
        {
            var note = request.Entity;
            _noteService.CreateOrUpdate(note, request.ParentTypeName, _userContext.Identity.Code);

            return Response.Empty;
        }
    }
}
