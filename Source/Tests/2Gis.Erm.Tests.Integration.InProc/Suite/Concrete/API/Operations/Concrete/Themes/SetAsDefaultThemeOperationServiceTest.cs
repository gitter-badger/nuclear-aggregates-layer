using DoubleGis.Erm.BLCore.API.Operations.Concrete.Themes;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Base;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Themes
{
    public class SetAsDefaultThemeOperationServiceTest : UseModelEntityTestBase<Theme>
    {
        private readonly ISetAsDefaultThemeOperationService _setAsDefaultThemeOperationService;

        public SetAsDefaultThemeOperationServiceTest(IAppropriateEntityProvider<Theme> appropriateEntityProvider,
                                                     ISetAsDefaultThemeOperationService setAsDefaultThemeOperationService) : base(appropriateEntityProvider)
        {
            _setAsDefaultThemeOperationService = setAsDefaultThemeOperationService;
        }

        protected override FindSpecification<Theme> ModelEntitySpec
        {
            get { return Specs.Find.ActiveAndNotDeleted<Theme>() && new FindSpecification<Theme>(x => !x.IsDefault); }
        }

        protected override OrdinaryTestResult ExecuteWithModel(Theme modelEntity)
        {
            return Result.When(() => _setAsDefaultThemeOperationService.SetAsDefault(modelEntity.Id, true))
                         .Then(result => result.Status.Should().Be(TestResultStatus.Succeeded));
        }
    }
}