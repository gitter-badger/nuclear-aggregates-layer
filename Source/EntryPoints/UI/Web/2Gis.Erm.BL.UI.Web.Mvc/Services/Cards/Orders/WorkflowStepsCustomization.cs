using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Newtonsoft.Json;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Orders
{
    public sealed class WorkflowStepsCustomization : IViewModelCustomization
    {
        private readonly IPublicService _publicService;

        public WorkflowStepsCustomization(IPublicService publicService)
        {
            _publicService = publicService;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            var entityViewModel = (ICustomizableOrderViewModel)viewModel;

            modelState.SetModelValue("WorkflowStepId",
                                     new ValueProviderResult(entityViewModel.PreviousWorkflowStepId,
                                                             entityViewModel.PreviousWorkflowStepId.ToString(CultureInfo.InvariantCulture),
                                                             null));

            entityViewModel.AvailableSteps = GetAvailableSteps(entityViewModel.Id,
                                                               entityViewModel.IsNew,
                                                               (OrderState)entityViewModel.WorkflowStepId,
                                                               entityViewModel.SourceOrganizationUnit.Key);

            if (entityViewModel.WorkflowStepId == (int)OrderState.Approved || entityViewModel.WorkflowStepId == (int)OrderState.OnTermination)
            {
                if (!entityViewModel.DestinationOrganizationUnit.Key.HasValue)
                {
                    throw new NotificationException("Destination organization unit should be specified");
                }
            }
        }

        private string GetAvailableSteps(long orderId, bool isNew, OrderState currentState, long? sourceOrganizationUnitId)
        {
            var resultList = new List<OrderState> { currentState };

            if (!isNew)
            {
                var response = (AvailableTransitionsResponse)_publicService.Handle(new AvailableTransitionsRequest
                                                                                       {
                                                                                           OrderId = orderId,
                                                                                           CurrentState = currentState,
                                                                                           SourceOrganizationUnitId = sourceOrganizationUnitId
                                                                                       });
                resultList.AddRange(response.AvailableTransitions);
            }

            return JsonConvert.SerializeObject(resultList.ConvertAll(state => new
                                                                                  {
                                                                                      Value = state.ToString("D"),
                                                                                      Text = state.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)
                                                                                  }));
        }
    }
}