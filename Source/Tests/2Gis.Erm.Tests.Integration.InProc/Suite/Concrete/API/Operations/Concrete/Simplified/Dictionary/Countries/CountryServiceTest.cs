using DoubleGis.Erm.BL.API.Operations.Concrete.Simplified.Dictionary.Countries;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Simplified.Dictionary.Countries
{
    public class CountryServiceTest : UseModelEntityTestBase<Country>
    {
        private readonly ICountryService _countryService;
        public CountryServiceTest(IAppropriateEntityProvider<Country> appropriateEntityProvider, ICountryService countryService) : base(appropriateEntityProvider)
        {
            _countryService = countryService;
        }

        protected override OrdinaryTestResult ExecuteWithModel(Country modelEntity)
        {
            modelEntity.Name = "Test";
            _countryService.CreateOrUpdate(modelEntity);

            return OrdinaryTestResult.As.Succeeded;
        }
    }
}