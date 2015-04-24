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

using BLFlexResources = DoubleGis.Erm.BLFlex.Resources.Server.Properties.BLResources;
namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Concrete.Old.LegalPersons
{
    public sealed class CyprusValidatePaymentRequisitesIsUniqueHandler : RequestHandler<ValidatePaymentRequisitesIsUniqueRequest, EmptyResponse>, ICyprusAdapted
    {
        private readonly IUserContext _userContext;
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly ILegalPersonRepository _legalPersonRepository;

        public CyprusValidatePaymentRequisitesIsUniqueHandler(
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
            var tic = !string.IsNullOrEmpty(request.Entity.Inn) ? request.Entity.Inn.Trim() : null;

            switch (modelLegalPersonType)
            {
                case LegalPersonType.LegalPerson:
                    {
                        var ticDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, tic);
                        if (!ticDublicate.ActiveDublicateExists && !ticDublicate.InactiveDublicateExists && !ticDublicate.DeletedDublicateExists)
                        {
                            break;
                        }

                        // { fixme, v.sinitsyn } Вряд ли нижеследующая логика для этого пермишена имеет смысл в кипрской версии
                        var permission = GetMaxAccess(_functionalAccessService.GetFunctionalPrivilege(FunctionalPrivilegeName.LegalPersonChangeRequisites, _userContext.Identity.Code));

                        switch (permission)
                        {
                            case LegalPersonChangeRequisitesAccess.None:
                                {
                                    if (ticDublicate.ActiveDublicateExists)
                                    {
                                        throw new NotificationException(BLFlexResources.CyprusActiveLegalPersonWithSpecifiedTicExist);        
                                    }

                                    if (ticDublicate.InactiveDublicateExists)
                                    {
                                        throw new NotificationException(BLFlexResources.CyprusInactiveLegalPersonWithSpecifiedTicExist);
                                    }

                                    if (ticDublicate.DeletedDublicateExists)
                                    {
                                        throw new NotificationException(BLFlexResources.CyprusDeletedLegalPersonWithSpecifiedTicExist);
                                    } 
                                }

                                break;
                                
                            case LegalPersonChangeRequisitesAccess.Granted:
                                break;

                            case LegalPersonChangeRequisitesAccess.GrantedLimited:
                                {
                                    // В российской версии здесь находится проверка на дубль по инн и кпп
                                    if (ticDublicate.ActiveDublicateExists)
                                    {
                                        throw new NotificationException(BLFlexResources.CyprusActiveLegalPersonWithSpecifiedTicExist);
                                    }

                                    if (ticDublicate.InactiveDublicateExists)
                                    {
                                        throw new NotificationException(BLFlexResources.CyprusInactiveLegalPersonWithSpecifiedTicExist);
                                    }

                                    if (ticDublicate.DeletedDublicateExists)
                                    {
                                        throw new NotificationException(BLFlexResources.CyprusDeletedLegalPersonWithSpecifiedTicExist);
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
                        var innDublicate = _legalPersonRepository.CheckIfExistsInnDuplicate(request.Entity.Id, tic);

                        if (innDublicate.ActiveDublicateExists)
                        {
                            throw new NotificationException(BLFlexResources.CyprusActiveBusinessmanWithSpecifiedTicExist);
                        }

                        if (innDublicate.InactiveDublicateExists)
                        {
                            throw new NotificationException(BLFlexResources.CyprusInactiveBusinessmanWithSpecifiedTicExist);
                        }

                        if (innDublicate.DeletedDublicateExists)
                        {
                            throw new NotificationException(BLFlexResources.CyprusDeletedBusinessManWithSpecifiedTicExist);
                        }  
                    }

                    break;

                case LegalPersonType.NaturalPerson:
                    {
                        var passportSeries = !string.IsNullOrEmpty(request.Entity.PassportSeries) ? request.Entity.PassportSeries.Trim() : null;
                        var passportNumber = !string.IsNullOrEmpty(request.Entity.PassportNumber) ? request.Entity.PassportNumber.Trim() : null;

                        var passportNumberDublicate = _legalPersonRepository.CheckIfExistsPassportDuplicate(request.Entity.Id,
                                                                                                            passportSeries,
                                                                                                            passportNumber);

                        if (passportNumberDublicate.ActiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.ActiveLegalPersonWithSpecifiedPassportNumberAlreadyExist);
                        }

                        if (passportNumberDublicate.InactiveDublicateExists)
                        {
                            throw new NotificationException(BLResources.InactiveLegalPersonWithSpecifiedPassportNumberAlreadyExist);
                        }

                        if (passportNumberDublicate.DeletedDublicateExists)
                        {
                            throw new NotificationException(BLResources.DeletedLegalPersonWithSpecifiedPassportNumberAlreadyExist);
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
