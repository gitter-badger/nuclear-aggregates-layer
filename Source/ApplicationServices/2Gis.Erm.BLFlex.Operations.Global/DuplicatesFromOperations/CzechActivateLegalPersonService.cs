using System.Linq;

using DoubleGis.Erm.BL.Aggregates.LegalPersons;
using DoubleGis.Erm.BL.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BL.API.Operations.Generic.Activate;
using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.DuplicatesFromOperations
{
    // FIXME {all, 06.11.2013}: вынесено из BL.Operations - уже копия в данном проекте, похоже на дублирование файлов в TFS из-за многочисленных merge - пока оставлены обе копии, при RI из 1.0 нужно обращать внимание какой целевой файл выбирается из 2ух
    // указан модификатор доступа internal, чтобы не подхватывался massprocessor
    internal sealed class CzechActivateLegalPersonService : IActivateGenericEntityService<LegalPerson>, ICzechAdapted
    {
        private readonly IFinder _finder;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public CzechActivateLegalPersonService(
            IFinder finder,
            ILegalPersonRepository legalPersonRepository,
            IOperationScopeFactory scopeFactory)
        {
            _finder = finder;
            _legalPersonRepository = legalPersonRepository;
            _scopeFactory = scopeFactory;
        }

        public int Activate(long entityId)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<ActivateIdentity, LegalPerson>())
            {
                var restoringLegalPerson = _finder.Find(Specs.Find.ById<LegalPerson>(entityId)).Single();

                if (restoringLegalPerson.IsActive)
                {
                    throw new NotificationException(string.Format(BLResources.LegalPersonToRestoreIsAlreadyActive, restoringLegalPerson.LegalName));
                }

                LegalPerson dublicateLegalPerson = null;

                switch ((LegalPersonType)restoringLegalPerson.LegalPersonTypeEnum)
                {
                    case LegalPersonType.LegalPerson:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.Inn))
                        {
                            dublicateLegalPerson = 
                                _finder
                                    .Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                            && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.LegalPerson)
                                            && LegalPersonSpecs.LegalPersons.Find.ByInn(restoringLegalPerson.Inn))
                                    .FirstOrDefault();
                        }

                        break;
                    case LegalPersonType.Businessman:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.Inn))
                        {
                            dublicateLegalPerson = 
                                _finder
                                    .Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                            && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.Businessman)
                                            && LegalPersonSpecs.LegalPersons.Find.ByInn(restoringLegalPerson.Inn))
                                    .FirstOrDefault();
                        }

                        break;
                    case LegalPersonType.NaturalPerson:
                        if (!string.IsNullOrWhiteSpace(restoringLegalPerson.PassportNumber))
                        {
                            dublicateLegalPerson = 
                                _finder
                                    .Find(Specs.Find.ActiveAndNotDeleted<LegalPerson>()
                                            && LegalPersonSpecs.LegalPersons.Find.OfType(LegalPersonType.NaturalPerson)
                                            && LegalPersonSpecs.LegalPersons.Find.ByPassport(restoringLegalPerson.PassportSeries, restoringLegalPerson.PassportNumber))
                                    .FirstOrDefault();
                        }

                        break;
                    default:
                        throw new BusinessLogicException(BLResources.UnknownLegalPersonType);
                }

                if (dublicateLegalPerson != null)
                {
                    throw new NotificationException(string.Format(BLResources.ActivateLegalPersonError, dublicateLegalPerson.LegalName));
                }

                var result = _legalPersonRepository.Activate(entityId);

                operationScope
                    .Updated<LegalPerson>(entityId)
                    .Complete();

                return result;
            }
        }
    }
}