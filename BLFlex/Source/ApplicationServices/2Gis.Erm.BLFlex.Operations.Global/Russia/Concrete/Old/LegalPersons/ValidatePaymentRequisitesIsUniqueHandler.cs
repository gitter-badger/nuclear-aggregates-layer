using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.LegalPersons
{
    public sealed class ValidatePaymentRequisitesIsUniqueHandler : RequestHandler<ValidatePaymentRequisitesIsUniqueRequest, EmptyResponse>, IRussiaAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ILegalPersonRepository _legalPersonRepository;

        public ValidatePaymentRequisitesIsUniqueHandler(
            ILegalPersonRepository legalPersonRepository,
            ISecurityServiceFunctionalAccess functionalAccessService,
            IUserContext userContext)
        {
            _functionalAccessService = functionalAccessService;
            _userContext = userContext;
            _legalPersonRepository = legalPersonRepository;
        }

        protected override EmptyResponse Handle(ValidatePaymentRequisitesIsUniqueRequest request)
        {
            var modelLegalPersonType = request.Entity.LegalPersonTypeEnum;
            var inn = !string.IsNullOrEmpty(request.Entity.Inn) ? request.Entity.Inn.Trim() : null;
            var kpp = !string.IsNullOrEmpty(request.Entity.Kpp) ? request.Entity.Kpp.Trim() : null;

            switch (modelLegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    {
                        var innDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, inn);
                        if (!innDublicate.ActiveDublicateExists && !innDublicate.InactiveDublicateExists && !innDublicate.DeletedDublicateExists)
                        {
                            break;
                        }

                        var permission = GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code));

                        switch (permission)
                        {
                            case LegalPersonChangeRequisitesAccess.None:
                                {
                                    if (innDublicate.ActiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.ActiveLegalPersonWithSpecifiedInnExist);
                                    }

                                    if (innDublicate.InactiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.InactiveLegalPersonWithSpecifiedInnExist);
                                    }

                                    if (innDublicate.InactiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.DeletedLegalPersonWithSpecifiedInnExist);
                                    }

                                    break;
                                }

                            case LegalPersonChangeRequisitesAccess.Granted:
                                break;

                            case LegalPersonChangeRequisitesAccess.GrantedLimited:
                                {
                                    var innAndKppDublicate = _legalPersonRepository.CheckIfExistsInnAndKppDuplicate(request.Entity.Id, inn, kpp);
                                    if (innAndKppDublicate.ActiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.ActiveLegalPersonWithSpecifiedInnAndKppAlreadyExist);
                                    }

                                    if (innAndKppDublicate.InactiveDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.InactiveLegalPersonWithSpecifiedInnAndKppAlreadyExist);
                                    }

                                    if (innAndKppDublicate.DeletedDublicateExists)
                                    {
                                        throw new NotificationException(BLResources.DeletedLegalPersonWithSpecifiedInnAndKppAlreadyExist);
                                    }
                                }

                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    break;

                case LegalPersonType.Businessman:
                    {
                        var innDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, inn);

                        if (innDublicate.ActiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.ActiveLegalPersonWithSpecifiedInnExist);
                        }

                        if (innDublicate.InactiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.InactiveLegalPersonWithSpecifiedInnExist);
                        }

                        if (innDublicate.DeletedDublicateExists)
                        {
                            throw new NotificationException(BLResources.DeletedLegalPersonWithSpecifiedInnExist);
                        }  
                    }

                    break;

                case LegalPersonType.NaturalPerson:
                    {
                        var passportSeries = !string.IsNullOrEmpty(request.Entity.PassportSeries) ? request.Entity.PassportSeries.Trim() : null;
                        var passportNumber = !string.IsNullOrEmpty(request.Entity.PassportNumber) ? request.Entity.PassportNumber.Trim() : null;

                        var passportSeriesAndNumberDublicate = _legalPersonRepository.CheckIfExistsPassportDuplicate(request.Entity.Id,
                                                                                                                     passportSeries,
                                                                                                                     passportNumber);

                        if (passportSeriesAndNumberDublicate.ActiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.ActiveLegalPersonWithSpecifiedPassportSeriesAndNumberAlreadyExist);
                        }

                        if (passportSeriesAndNumberDublicate.InactiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.InactiveLegalPersonWithSpecifiedPassportSeriesAndNumberAlreadyExist);
                        }

                        if (passportSeriesAndNumberDublicate.DeletedDublicateExists)
                        {
                            throw new NotificationException(BLResources.DeletedLegalPersonWithSpecifiedPassportSeriesAndNumberAlreadyExist);
                        }
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return Response.Empty;
        }

        private static LegalPersonChangeRequisitesAccess GetMaxAccess(int[] accesses)
        {
            if (!accesses.Any())
            {
                return LegalPersonChangeRequisitesAccess.None;
            }

            var priorities = new[] { LegalPersonChangeRequisitesAccess.None, LegalPersonChangeRequisitesAccess.GrantedLimited, LegalPersonChangeRequisitesAccess.Granted };

            var maxPriority = accesses.Select(x => Array.IndexOf(priorities, (LegalPersonChangeRequisitesAccess)x)).Max();
            return priorities[maxPriority];
        }
    }
}
