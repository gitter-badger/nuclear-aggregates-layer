using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified.Dictionary.Bargains;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Bargains
{
    public class BargainTypeServiceTest : UseModelEntityTestBase<BargainType>
    {
        private readonly IBargainTypeService _bargainTypeService;

        public BargainTypeServiceTest(IAppropriateEntityProvider<BargainType> appropriateEntityProvider, IBargainTypeService bargainTypeService)
            : base(appropriateEntityProvider)
        {
            _bargainTypeService = bargainTypeService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(BargainType modelEntity)
        {
            modelEntity.Name = "Test";
            _bargainTypeService.CreateOrUpdate(modelEntity);

            var newBargainType = new BargainType
                {
                    Name = "Test",
                    SyncCode1C = "000007",
                    VatRate = 7
                };



            return Result.When(() => _bargainTypeService.CreateOrUpdate(newBargainType))
                         .Then(() => newBargainType.Id.Should().NotBe(0));
        }
    }
}