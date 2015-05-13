using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineLegalPersonProfileObtainer : IBusinessModelEntityObtainer<LegalPersonProfile>, IAggregateReadModel<LegalPerson>, IUkraineAdapted
    {
        private readonly IFinder _finder;

        public UkraineLegalPersonProfileObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPersonProfile ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (UkraineLegalPersonProfileDomainEntityDto)domainEntityDto;

            var legalPersonProfile = dto.IsNew()
                                  ? new LegalPersonProfile
                                      {
                                          IsActive = true,
                                          Parts = new[]
                                              {
                                                  new UkraineLegalPersonProfilePart
                                                      {
                                                          IsActive = true
                                                      }
                                              }
                                      }
                                  : _finder.FindOne(Specs.Find.ById<LegalPersonProfile>(dto.Id));

            LegalPersonFlexSpecs.LegalPersonProfiles.Ukraine.Assign.Entity().Assign(dto, legalPersonProfile);

            return legalPersonProfile;
        }
    }
}