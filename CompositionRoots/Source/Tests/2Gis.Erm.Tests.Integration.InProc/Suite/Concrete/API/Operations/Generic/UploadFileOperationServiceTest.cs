using System;
using System.IO;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class UploadFileOperationServiceTest : UseModelEntityTestBase<BargainFile>
    {
        private readonly IUploadFileAggregateRepository<BargainFile> _uploadFileGenericEntityService;

        public UploadFileOperationServiceTest(
            IUploadFileAggregateRepository<BargainFile> uploadFileGenericEntityService,
            IAppropriateEntityProvider<BargainFile> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _uploadFileGenericEntityService = uploadFileGenericEntityService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(BargainFile modelEntity)
        {
            var testDescrition = GetType().Name + DateTime.Now.ToFileTimeUtc();
            var stream = new MemoryStream();
            using (var streamWriter = new StreamWriter(stream) { AutoFlush = true })
            {
                streamWriter.Write(testDescrition);

                var uploadParams = new UploadFileParams
                    {
                        FileName = testDescrition,
                        Content = stream,
                        ContentLength = stream.Length,
                        ContentType = "text/xml"
                    };

                return Result
                    .When(_uploadFileGenericEntityService.UploadFile(new UploadFileParams<BargainFile>(uploadParams)))
                    .Then(uploaded => uploaded.Should().NotBeNull());
            }
        }
    }
}