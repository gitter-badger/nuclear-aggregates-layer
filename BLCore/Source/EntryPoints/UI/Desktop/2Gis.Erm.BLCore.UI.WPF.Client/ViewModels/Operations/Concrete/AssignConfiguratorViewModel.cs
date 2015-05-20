using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Assign;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Operations.Concrete
{
    public sealed class AssignConfiguratorViewModel : OperationConfiguratorViewModel<AssignIdentity, AssignCommonParameter, AssignEntityParameter, AssignResult>
    {
        private readonly DataTemplateSelector _viewSelector;

        private readonly ITitleProvider _operationName;
        private readonly ITitleProvider _ownerCodeTitle;
        private readonly ITitleProvider _bypassValidationTitle;
        private readonly ITitleProvider _isPartialAssignTitle;

        private long _ownerCode;
        private bool _bypassValidation;
        private bool _isPartialAssign;

        public AssignConfiguratorViewModel(
            IEntityType entityName,
            long[] operationProcessingEntities,
            IMessageSink messageSink,
            ITitleProviderFactory titleProviderFactory,
            IUserInfo userInfo,
            DataTemplateSelector viewSelector)
            : base(entityName, operationProcessingEntities, messageSink, titleProviderFactory, userInfo)
        {
            _viewSelector = viewSelector;

            _operationName = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ErmConfigLocalization.ControlAssign));
            _ownerCodeTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => BLResources.CrmOwner));
            _bypassValidationTitle = new StaticTitleProvider(new StaticTitleDescriptor("bypassValidationTitle"));
            _isPartialAssignTitle = new StaticTitleProvider(new StaticTitleDescriptor("isPartialAssignTitle"));
        }

        public override DataTemplateSelector ConfiguratorViewSelector
        {
            get { return _viewSelector; }
        }

        public override AssignCommonParameter CommonParameter
        {
            get
            {
                return new AssignCommonParameter(Identity.Id)
                {
                    EntityName = EntityName,
                    OwnerCode = OwnerCode,
                    BypassValidation = BypassValidation,
                    IsPartialAssign = IsPartialAssign
                };
            }
        }

        public override IEnumerable<AssignEntityParameter> Parameters
        {
            get
            {
                return OperationProcessingEntities.Select(i => new AssignEntityParameter { EntityId = i });
            }
        }

        public long OwnerCode
        {
            get
            {
                return _ownerCode;
            }

            set
            {
                if (value == _ownerCode)
                {
                    return;
                }

                _ownerCode = value;
                FireCommandCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        public bool BypassValidation
        {
            get
            {
                return _bypassValidation;
            }

            set
            {
                if (value.Equals(_bypassValidation))
                {
                    return;
                }

                _bypassValidation = value;
                FireCommandCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        public bool IsPartialAssign
        {
            get
            {
                return _isPartialAssign;
            }

            set
            {
                if (value.Equals(_isPartialAssign))
                {
                    return;
                }

                _isPartialAssign = value;
                FireCommandCanExecuteChanged();
                RaisePropertyChanged();
            }
        }

        public ITitleProvider OwnerCodeTitle
        {
            get { return _ownerCodeTitle; }
        }

        public ITitleProvider BypassValidationTitle
        {
            get { return _bypassValidationTitle; }
        }

        public ITitleProvider IsPartialAssignTitle
        {
            get { return _isPartialAssignTitle; }
        }

        protected override ITitleProvider OperationName
        {
            get { return _operationName; }
        }
    }
}