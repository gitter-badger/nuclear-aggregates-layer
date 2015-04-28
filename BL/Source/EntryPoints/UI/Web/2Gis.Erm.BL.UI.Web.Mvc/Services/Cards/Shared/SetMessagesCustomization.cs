using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BLCore.UI.Metadata.Config;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Provider;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using NuClear.Security.API.UserContext;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Shared
{
    public sealed class SetMessagesCustomization : IViewModelCustomization<IEntityViewModelBase>
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IUserContext _userContext;

        public SetMessagesCustomization(IMetadataProvider metadataProvider, IUserContext userContext)
        {
            _metadataProvider = metadataProvider;
            _userContext = userContext;
        }

        public void Customize(IEntityViewModelBase viewModel, ModelStateDictionary modelState)
        {
            CardMetadata metadata;
            var metadataId = IdBuilder.For<MetadataCardsIdentity>(viewModel.ViewConfig.EntityName.ToString()).AsIdentity();
            if (!_metadataProvider.TryGetMetadata(metadataId.Id, out metadata))
            {
                throw new MetadataNotFoundException(metadataId);
            }

            var messages = new List<Tuple<MessageType, IStringResourceDescriptor>>();

            foreach (var feature in metadata.Features<IMessageExpressionFeature>())
            {
                bool expressionResult;

                if (!feature.Expression.TryExecuteAspectLambda((IAspect)viewModel, out expressionResult))
                {
                    throw new InvalidOperationException(string.Format("Unable to execute message expression for {0} card with {1} viewmodel", metadata.Entity, viewModel.GetType()));
                }

                if (expressionResult)
                {
                    messages.Add(Tuple.Create(feature.MessageType, feature.MessageDescriptor));
                }
            }

            foreach (var feature in metadata.Features<IMessageExpressionsFeature>())
            {
                bool expressionResult;

                if (!feature.Expressions.TryExecuteAspectBoolLambdas((IAspect)viewModel, feature.LogicalOperation, out expressionResult))
                {
                    throw new InvalidOperationException(string.Format("Unable to execute message expressions for {0} card with {1} viewmodel", metadata.Entity, viewModel.GetType()));
                }

                if (expressionResult)
                {
                    messages.Add(Tuple.Create(feature.MessageType, feature.MessageDescriptor));
                }
            }

            foreach (var message in messages.OrderByDescending(x => x.Item1))
            {
                var messageText = message.Item2.GetValue(_userContext.Profile.UserLocaleInfo.UserCultureInfo);
                switch (message.Item1)
                {
                    case MessageType.CriticalError:
                        viewModel.SetCriticalError(messageText);
                        break;
                    case MessageType.Warning:
                        viewModel.SetWarning(messageText);
                        break;
                    case MessageType.Info:
                        viewModel.SetInfo(messageText);
                        break;
                    default:
                        throw new ArgumentException("MessageType");
                }
            }
        }
    }
}