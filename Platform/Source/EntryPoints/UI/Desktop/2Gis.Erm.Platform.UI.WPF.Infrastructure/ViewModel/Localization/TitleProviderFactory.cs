﻿using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources.Titles;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.UserInfo;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization
{
    public class TitleProviderFactory : ITitleProviderFactory
    {
        private readonly IUserInfo _userInfo;

        public TitleProviderFactory(IUserInfo userInfo)
        {
            _userInfo = userInfo;
        }

        public ITitleProvider Create(ITitleDescriptor descriptor)
        {
            if (descriptor is ResourceTitleDescriptor)
            {
                return new ResourceTitleProvider((ResourceTitleDescriptor)descriptor, _userInfo);
            }

            if (descriptor is StaticTitleDescriptor)
            {
                return new StaticTitleProvider((StaticTitleDescriptor)descriptor);
            }

            throw new NotSupportedException("Specified title desciptor is not supported. " + descriptor.GetType());
        }
    }
}