using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class DownloadFileOperationServiceTest : UseModelEntityTestBase<BargainFile>
    {
        private readonly IDownloadFileAggregateRepository<BargainFile> _downloadFileGenericEntityService;

        public DownloadFileOperationServiceTest(
            IDownloadFileAggregateRepository<BargainFile> downloadFileGenericEntityService,
            IAppropriateEntityProvider<BargainFile> appropriateEntityProvider)
            : base(appropriateEntityProvider)
        {
            _downloadFileGenericEntityService = downloadFileGenericEntityService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(BargainFile modelEntity)
        {
            return Result
                .When(_downloadFileGenericEntityService.DownloadFile(new DownloadFileParams<BargainFile>(modelEntity.FileId)))
                .Then(downloaded => downloaded.Should().NotBeNull());
        }
    }
}