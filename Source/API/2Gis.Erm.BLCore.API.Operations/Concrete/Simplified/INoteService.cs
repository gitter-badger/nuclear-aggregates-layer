﻿using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified
{
    public interface INoteService : ISimplifiedModelConsumer
    {
        void CreateOrUpdate(Note note, EntityName parentEntityName, long ownerCode);
        UploadFileResult UploadFile(UploadFileParams<Note> uploadFileParams);
        StreamResponse DownloadFile(DownloadFileParams<Note> downloadFileParams);
    }
}