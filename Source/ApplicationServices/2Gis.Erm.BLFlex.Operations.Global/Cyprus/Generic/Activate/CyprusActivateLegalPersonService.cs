﻿using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Activate;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Activate
{
    public sealed class CyprusActivateLegalPersonService : IActivateGenericEntityService<LegalPerson>, ICyprusAdapted
    {
        private readonly IFinder _finder;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public CyprusActivateLegalPersonService(
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

                // FIXME {all, 06.08.2013}: В кипрской версии логику ещё никто не обговаривал. - был fixme в нестандартном формате - просто актуалиация
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
