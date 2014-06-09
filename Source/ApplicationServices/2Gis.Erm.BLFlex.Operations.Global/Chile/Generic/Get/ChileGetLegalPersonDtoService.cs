using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.LegalPersonAggregate.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonDtoService : GetLegalPersonDtoServiceBase<ChileLegalPersonDomainEntityDto>, IChileAdapted
    {
        private readonly IChileLegalPersonReadModel _chileLegalPersonReadModel;

        public ChileGetLegalPersonDtoService(IClientReadModel clientReadModel,
            ILegalPersonReadModel legalPersonReadModel,
            IChileLegalPersonReadModel chileLegalPersonReadModel,
            IUserContext userContext)
            : base(userContext, clientReadModel, legalPersonReadModel)
        {
            _chileLegalPersonReadModel = chileLegalPersonReadModel;
        }

        protected override IProjectSpecification<LegalPerson, ChileLegalPersonDomainEntityDto> GetProjectSpecification()
        {
            return LegalPersonFlexSpecs.LegalPersons.Chile.Project.DomainEntityDto();
        }

        protected override void SetSpecificPropertyValues(ChileLegalPersonDomainEntityDto dto)
            {
            dto.CommuneRef = _chileLegalPersonReadModel.GetCommuneReference(dto.Id);
        }
    }
}