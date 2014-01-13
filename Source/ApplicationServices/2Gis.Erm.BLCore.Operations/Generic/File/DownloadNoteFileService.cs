using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.File
{
    public class DownloadNoteFileService : IDownloadFileGenericService<Note> 
    {
        private readonly INoteService _noteService;

        public DownloadNoteFileService(INoteService noteService)
        {
            _noteService = noteService;
        }

        public StreamResponse DownloadFile(long fileId)
        {
            return _noteService.DownloadFile(new DownloadFileParams<Note>(fileId));
        }
    }
}