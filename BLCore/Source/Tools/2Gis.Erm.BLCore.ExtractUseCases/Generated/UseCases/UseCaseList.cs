using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Advertisements;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.Olap;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersonProfiles;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.UserProfiles;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.API.Releasing.Releases.Old;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Advertisements;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.BranchOfficeOrganizationUnits;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Clients;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Deals;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.Olap;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.LegalPersonProfiles;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Limits;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.LocalMessages;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Prices;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Printing;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Territories;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.UserProfiles;
using DoubleGis.Erm.BLCore.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Operations.Generic.Old;
using DoubleGis.Erm.BLCore.OrderValidation;
using DoubleGis.Erm.BLCore.Releasing.Release.Old;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Russia.Operations.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.AccountDetails;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Bills;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.LegalPersons;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Generated.UseCases
{
    public static class UseCaseList
    {
        public static readonly IEnumerable<UseCase> UseCases = GetUseCases();

        private static IEnumerable<UseCase> GetSampleUseCases()
        {
            return new[]
            {
                new UseCase
                {
                    Description = "Description",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(RequestHandler<,>),
                        Request = typeof(Request),
                        ChildNodes = new[]
                        {
                                new UseCaseNode(0)
                                {
                                    ContainingClass = typeof(RequestHandler<,>),
                                    Request = typeof(Request)
                                },
                                new UseCaseNode(0)
                                {
                                    ContainingClass = typeof(RequestHandler<,>),
                                    Request = typeof(Request)
                                }
                        }
                    }
                },
                new UseCase
                {
                    Description = "Description",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(RequestHandler<,>),
                        Request = typeof(Request),
                        ChildNodes = new[]
                        {
                                new UseCaseNode(0)
                                {
                                    ContainingClass = typeof(RequestHandler<,>),
                                    Request = typeof(Request)
                                },
                                new UseCaseNode(0)
                                {
                                    ContainingClass = typeof(RequestHandler<,>),
                                    Request = typeof(Request)
                                }
                        }
                    }
                }
            };
        }

        private static IEnumerable<UseCase> GetUseCases()
        {
            return new[]
            {
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrintJointBill",
                    MaxUseCaseDepth = 2,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintOrderJointBillsHandler),
                        Request = typeof(PrintOrderJointBillRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(PrintJointBillHandler),
            Request = typeof(PrintJointBillRequest),
            ChildNodes = new[]
            {
                new UseCaseNode(2)
                {
                    ContainingClass = typeof(PrintDocumentHandler),
                    Request = typeof(PrintDocumentRequest)
                }
            }
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.AdvertisementController\SelectWhiteListedAd",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(SelectAdvertisementToWhiteListHandler),
                        Request = typeof(SelectAdvertisementToWhiteListRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Assign.AssignFirmService\Assign",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateOwnerIsNotReserve<>),
                        Request = typeof(ValidateOwnerIsNotReserveRequest<>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\CheckOrdersReadinessForReleaseDialog",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CheckOrdersReadinessForReleaseHandler),
                        Request = typeof(CheckOrdersReadinessForReleaseRequest),
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Assign.AssignAccountDetailService\Assign",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateOwnerIsNotReserve<>),
                        Request = typeof(ValidateOwnerIsNotReserveRequest<>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Assign.AssignAccountService\Assign",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateOwnerIsNotReserve<>),
                        Request = typeof(ValidateOwnerIsNotReserveRequest<>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.ReplicableEntityController<TEntity, TModel>\EditAccessSharings",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditAccessSharingHandler),
                        Request = typeof(EditAccessSharingRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.TaskService.Jobs.LocalMessages.ReprocessLocalMessages\ExecuteInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ReprocessLocalMessagesHandler),
                        Request = typeof(ReprocessLocalMessagesRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrintOrder",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintOrderHandler),
                        Request = typeof(PrintOrderRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(PrintDocumentHandler),
            Request = typeof(PrintDocumentRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrintRegionalOrder",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintOrderHandler),
                        Request = typeof(PrintOrderRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(PrintDocumentHandler),
            Request = typeof(PrintDocumentRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Handlers.OrderPositions.EditOrderPositionHandler\Handle",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ActualizeOrderReleaseWithdrawalsHandler),
                        Request = typeof(ActualizeOrderReleaseWithdrawalsRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Delete.DeleteOrderPositionService\Delete",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ActualizeOrderReleaseWithdrawalsHandler),
                        Request = typeof(ActualizeOrderReleaseWithdrawalsRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Handlers.Orders.Discounts.UpdateOrderDiscountHandler\Handle",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(RecalculateOrderDiscountHandler),
                        Request = typeof(RecalculateOrderDiscountRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\DiscountRecalc",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(RecalculateOrderDiscountHandler),
                        Request = typeof(RecalculateOrderDiscountRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.BranchOfficeOrganizationUnitController\SetAsPrimaryForRegSales",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSales),
                        Request = typeof(SetBranchOfficeOrganizationUnitAsPrimaryForRegionalSalesRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.BillController\Create",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(GetRelatedOrdersForCreateBillHandler),
                        Request = typeof(GetRelatedOrdersForCreateBillRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.BillController\GetRelatedOrdersInfoForCreateBill",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(GetRelatedOrdersForCreateBillHandler),
                        Request = typeof(GetRelatedOrdersForCreateBillRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditPriceHandler),
                        Request = typeof(EditRequest<Price>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditDeniedPositionHandler),
                        Request = typeof(EditRequest<DeniedPosition>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditPricePositionHandler),
                        Request = typeof(EditRequest<PricePosition>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditAdvertisementElementTemplateHandler),
                        Request = typeof(EditRequest<AdvertisementElementTemplate>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditPositionHandler),
                        Request = typeof(EditRequest<Position>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditAdsTemplatesAdsElementTemplateHandler),
                        Request = typeof(EditRequest<AdsTemplatesAdsElementTemplate>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditPositionChildrenHandler),
                        Request = typeof(EditRequest<PositionChildren>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditUserHandler),
                        Request = typeof(EditRequest<User>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditAdvertisementTemplateHandler),
                        Request = typeof(EditRequest<AdvertisementTemplate>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditLegalPersonHandler),
                        Request = typeof(EditRequest<LegalPerson>),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ValidatePaymentRequisitesIsUniqueHandler),
            Request = typeof(ValidatePaymentRequisitesIsUniqueRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditOrganizationUnitHandler),
                        Request = typeof(EditRequest<OrganizationUnit>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditClientHandler),
                        Request = typeof(EditRequest<Client>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditAdvertisementHandler),
                        Request = typeof(EditRequest<Advertisement>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditTerritoryHandler),
                        Request = typeof(EditRequest<Territory>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditContactHandler),
                        Request = typeof(EditRequest<Contact>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditCurrencyRateHandler),
                        Request = typeof(EditRequest<CurrencyRate>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditPrintFormTemplateHandler),
                        Request = typeof(EditRequest<PrintFormTemplate>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditAssociatedPositionHandler),
                        Request = typeof(EditRequest<AssociatedPosition>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditDepartmentHandler),
                        Request = typeof(EditRequest<Department>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditAccountDetailHandler),
                        Request = typeof(EditRequest<AccountDetail>),
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditRoleHandler),
                        Request = typeof(EditRequest<Role>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditPlatformHandler),
                        Request = typeof(EditRequest<Platform.Model.Entities.Erm.Platform>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditCurrencyHandler),
                        Request = typeof(EditRequest<Currency>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditAccountHandler),
                        Request = typeof(EditAccountRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditNoteHandler),
                        Request = typeof(EditNoteRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ChangeLegalPersonRequisitesHandler),
                        Request = typeof(ChangeLegalPersonRequisitesRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ValidatePaymentRequisitesIsUniqueHandler),
            Request = typeof(ValidatePaymentRequisitesIsUniqueRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidatePaymentRequisitesIsUniqueHandler),
                        Request = typeof(ValidatePaymentRequisitesIsUniqueRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CreateLocalMessageHandler),
                        Request = typeof(CreateLocalMessageRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditOrderPositionHandler),
                        Request = typeof(EditOrderPositionRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.Base.EntityControllerBase<TEntity, TModel>\EditInternal",
                    MaxUseCaseDepth = 2,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(EditOrderHandler),
                        Request = typeof(EditOrderRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(SetADPositionsValidationAsInvalidHandler),
            Request = typeof(SetADPositionsValidationAsInvalidRequest)
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ProcessOrderOnApprovedToOnRegistrationHandler),
            Request = typeof(ProcessOrderOnApprovedToOnRegistrationRequest)
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ProcessOrderOnApprovedToOnTerminationHandler),
            Request = typeof(ProcessOrderOnApprovedToOnTerminationRequest),
            ChildNodes = new[]
            {
                new UseCaseNode(2)
                {
                    ContainingClass = typeof(ActualizeOrderReleaseWithdrawalsHandler),
                    Request = typeof(ActualizeOrderReleaseWithdrawalsRequest)
                }
            }
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ProcessOrderOnArchiveToApprovedHandler),
            Request = typeof(ProcessOrderOnArchiveToApprovedRequest),
            ChildNodes = new[]
            {
                new UseCaseNode(2)
                {
                    ContainingClass = typeof(ActualizeOrderReleaseWithdrawalsHandler),
                    Request = typeof(ActualizeOrderReleaseWithdrawalsRequest)
                }
            }
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ProcessOrderOnTerminationToOnApprovedHandler),
            Request = typeof(ProcessOrderOnTerminationToOnApprovedRequest),
            ChildNodes = new[]
            {
                new UseCaseNode(2)
                {
                    ContainingClass = typeof(ActualizeOrderReleaseWithdrawalsHandler),
                    Request = typeof(ActualizeOrderReleaseWithdrawalsRequest)
                },
            }
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ProcessOrderOnApprovalToApprovedHandler),
            Request = typeof(ProcessOrderOnApprovalToApprovedRequest),
            ChildNodes = new[]
            {
                new UseCaseNode(2)
                {
                    ContainingClass = typeof(CheckOrderReleasePeriodHandler),
                    Request = typeof(CheckOrderReleasePeriodRequest)
                }
            }
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ProcessOrderOnApprovalToRejectedHandler),
            Request = typeof(ProcessOrderOnApprovalToRejectedRequest)
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(CheckDealHandler),
            Request = typeof(CheckDealRequest)
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(UpdateOrderDiscountHandler),
            Request = typeof(UpdateOrderDiscountRequest)
        },
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ActualizeOrderReleaseWithdrawalsHandler),
            Request = typeof(ActualizeOrderReleaseWithdrawalsRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.BillController\SavePayments",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CreateBillInfoForOrdersByTemplateHandler),
                        Request = typeof(CreateBillInfoForOrdersByTemplateRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.BillController\GetDistributedPaymentsInfo",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(GetDistributedPaymentsInfoHandler),
                        Request = typeof(GetDistributedPaymentsInfoRequest),
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.DealController\CheckIsWarmClient",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CheckIsWarmClientHandler),
                        Request = typeof(CheckIsWarmClientRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.LocalMessageController\Export",
                    MaxUseCaseDepth = 2,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ExportLocalMessageHandler),
                        Request = typeof(ExportLocalMessageRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(CreateLocalMessageHandler),
            Request = typeof(CreateLocalMessageRequest)
        },
       
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ExportLegalPersonsHandler),
            Request = typeof(ExportLegalPersonsRequest),
        },
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.TaskService.Jobs.LocalMessages.ExportLocalMessages\ExecuteInternal",
                    MaxUseCaseDepth = 2,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ExportLocalMessageHandler),
                        Request = typeof(ExportLocalMessageRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(CreateLocalMessageHandler),
            Request = typeof(CreateLocalMessageRequest)
        },
        
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ExportLegalPersonsHandler),
            Request = typeof(ExportLegalPersonsRequest),
        },
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.LocalMessageController\SaveAs",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(SaveLocalMessageHandler),
                        Request = typeof(SaveLocalMessageRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Assign.AssignLegalPersonService\Assign",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateOwnerIsNotReserve<>),
                        Request = typeof(ValidateOwnerIsNotReserveRequest<>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Assign.AssignAccountDetailService\Assign",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateOwnerIsNotReserve<>),
                        Request = typeof(ValidateOwnerIsNotReserveRequest<>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.ReleaseInfoController\DownloadResults",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(DownloadReleaseInfoResultsHandler),
                        Request = typeof(DownloadReleaseInfoResultsRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(PrepareValidationReportHandler),
            Request = typeof(PrepareValidationReportRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\CheckBeginDistributionDate",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CheckOrderBeginDistributionDateHandler),
                        Request = typeof(CheckOrderBeginDistributionDateRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.LegalPersonController\Merge",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(MergeLegalPersonsHandler),
                        Request = typeof(MergeLegalPersonsRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrintBargain",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintOrderBargainHandler),
                        Request = typeof(PrintOrderBargainRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(PrintDocumentHandler),
            Request = typeof(PrintDocumentRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.BillController\PrintBill",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        //ContainingClass = typeof(PrintBillHandler),
                        Request = typeof(PrintBillRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(PrintDocumentHandler),
            Request = typeof(PrintDocumentRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.TaskService.Jobs.Olap.ImportFirmPromising\ExecuteInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ImportFirmPromisingHandler),
                        Request = typeof(ImportFirmPromisingRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\GetAvailableSteps",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(AvailableTransitionsHandler),
                        Request = typeof(AvailableTransitionsRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Assign.AssignDealService\Assign",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateOwnerIsNotReserve<>),
                        Request = typeof(ValidateOwnerIsNotReserveRequest<>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.BillController\GetRelatedOrdersInfoForPrintJointBill",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(GetRelatedOrdersForPrintJointBillHandler),
                        Request = typeof(GetRelatedOrdersForPrintJointBillRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\GetRelatedOrdersInfoForPrintJointBill",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(GetRelatedOrdersForPrintJointBillHandler),
                        Request = typeof(GetRelatedOrdersForPrintJointBillRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrepareJointBill",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(GetRelatedOrdersForPrintJointBillHandler),
                        Request = typeof(GetRelatedOrdersForPrintJointBillRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrintLetterOfGuarantee",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintLetterOfGuaranteeHandler),
                        Request = typeof(PrintLetterOfGuaranteeRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(PrintDocumentHandler),
            Request = typeof(PrintDocumentRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.ChangeTerritory.ChangeFirmTerritoryService\ChangeTerritory",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateTerritoryAvailabilityHandler),
                        Request = typeof(ValidateTerritoryAvailabilityRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(SelectCurrentUserTerritoriesExpressionHandler),
            Request = typeof(SelectCurrentUserTerritoriesExpressionRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.LocalMessageController\ImportFromFile",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CreateLocalMessageHandler),
                        Request = typeof(CreateLocalMessageRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.TaskService.Jobs.LocalMessages.ProcessLocalMessages\ExecuteInternal",
                    MaxUseCaseDepth = 3,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ProcessLocalMessagesHandler),
                        Request = typeof(ProcessLocalMessagesRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ImportAccountDetailsFrom1CHandler),
            Request = typeof(ImportAccountDetailsFrom1CRequest),

        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.LocalMessageController\ImportFromFile",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateAccountDetailsFrom1CHandler),
                        Request = typeof(ValidateAccountDetailsFrom1CRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Handlers.Orders.Discounts.UpdateOrderDiscountHandler\Handle",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(UpdateOrderFinancialPerformanceHandler),
                        Request = typeof(UpdateOrderFinancialPerformanceRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Handlers.OrderPositions.EditOrderPositionHandler\Handle",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(UpdateOrderFinancialPerformanceHandler),
                        Request = typeof(UpdateOrderFinancialPerformanceRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Delete.DeleteOrderPositionService\Delete",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(UpdateOrderFinancialPerformanceHandler),
                        Request = typeof(UpdateOrderFinancialPerformanceRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderPositionController\GetViewModel",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CheckOrderReleasePeriodHandler),
                        Request = typeof(CheckOrderReleasePeriodRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.List.ListTerritoryService\GetListData",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(SelectCurrentUserTerritoriesHandler),
                        Request = typeof(SelectCurrentUserTerritoriesRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(SelectCurrentUserTerritoriesExpressionHandler),
            Request = typeof(SelectCurrentUserTerritoriesExpressionRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.LegalPersonController\ChangeLegalPersonRequisites",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ChangeLegalPersonRequisitesHandler),
                        Request = typeof(ChangeLegalPersonRequisitesRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ValidatePaymentRequisitesIsUniqueHandler),
            Request = typeof(ValidatePaymentRequisitesIsUniqueRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.DealController\Close",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CloseDealHandler),
                        Request = typeof(CloseDealRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.PriceController\Publish",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PublishPriceHandler),
                        Request = typeof(PublishPriceRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderPositionController\DiscountRecalc",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(RecalculateOrderPositionDiscountHandler),
                        Request = typeof(RecalculateOrderPositionDiscountRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Assign.AssignOrderService\Assign",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateOwnerIsNotReserve<>),
                        Request = typeof(ValidateOwnerIsNotReserveRequest<>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.PriceController\Unpublish",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(UnpublishPriceHandler),
                        Request = typeof(UnpublishPriceRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.Assign.AssignClientService\Assign",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateOwnerIsNotReserve<>),
                        Request = typeof(ValidateOwnerIsNotReserveRequest<>)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrintBill",
                    MaxUseCaseDepth = 2,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintOrderBillsHandler),
                        Request = typeof(PrintOrderBillsRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            //ContainingClass = typeof(PrintBillHandler),
            Request = typeof(PrintBillRequest),
            ChildNodes = new[]
            {
                new UseCaseNode(2)
                {
                    ContainingClass = typeof(PrintDocumentHandler),
                    Request = typeof(PrintDocumentRequest)
                }
            }
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\ChangeOrderDeal",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ChangeOrderDealHandler),
                        Request = typeof(ChangeOrderDealRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrintTerminationNotice",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintOrderTerminationNoticeHandler),
                        Request = typeof(PrintOrderTerminationNoticeRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(PrintDocumentHandler),
            Request = typeof(PrintDocumentRequest)
        }
    }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\PrintTerminationNoticeWithoutReason",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintOrderTerminationNoticeHandler),
                        Request = typeof(PrintOrderTerminationNoticeRequest),
                        ChildNodes = new[]
                        {
                            new UseCaseNode(1)
                            {
                                ContainingClass = typeof(PrintDocumentHandler),
                                Request = typeof(PrintDocumentRequest)
                            }
                        }
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.BranchOfficeOrganizationUnitController\SetAsPrimary",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(SetBranchOfficeOrganizationUnitAsPrimary),
                        Request = typeof(SetBranchOfficeOrganizationUnitAsPrimaryRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.BLCore.Services.Operations.List.ListTerritoryService\List",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(SelectOrganizationUnitTerritoriesHandler),
                        Request = typeof(SelectOrganizationUnitTerritoriesRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.LimitController\PrintLimits",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(PrintLimitsHandler),
                        Request = typeof(PrintLimitsRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.Services.UI.Grid.AccountDetailGridViewService\SecureViewsToolbarsInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ValidateCreateAccountDetailHandler),
                        Request = typeof(ValidateCreateAccountDetailRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.DealController\ReopenDeal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(ReopenDealHandler),
                        Request = typeof(ReopenDealRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.LegalPersonProfileController\MakeProfileMain",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(MakeLegalPersonProfileMainHandler),
                        Request = typeof(MakeLegalPersonProfileMainRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.TaskService.Jobs.ADSync.SyncUserProfiles\ExecuteInternal",
                    MaxUseCaseDepth = 0,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(SyncUserProfilesHandler),
                        Request = typeof(SyncUserProfilesRequest)
                    }
                },
                new UseCase
                {
                    Description = @"PublicService_Explicitly. DoubleGis.Erm.UI.Web.Mvc.Controllers.OrderController\CloseWithDenial",
                    MaxUseCaseDepth = 1,
                    Root = new UseCaseNode(0)
                    {
                        ContainingClass = typeof(CloseOrderHandler),
                        Request = typeof(CloseOrderRequest),
                        ChildNodes = new[]
    {
        new UseCaseNode(1)
        {
            ContainingClass = typeof(ActualizeOrderReleaseWithdrawalsHandler),
            Request = typeof(ActualizeOrderReleaseWithdrawalsRequest)
        },
    }
                    }
                }
            };
        }
    }
}
