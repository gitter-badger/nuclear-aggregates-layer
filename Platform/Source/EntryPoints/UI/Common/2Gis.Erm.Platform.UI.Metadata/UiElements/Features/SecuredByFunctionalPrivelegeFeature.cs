﻿using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;

namespace DoubleGis.Erm.Platform.UI.Metadata.UiElements.Features
{
    public sealed class SecuredByFunctionalPrivelegeFeature : ISecuredElementFeature
    {
        public SecuredByFunctionalPrivelegeFeature(FunctionalPrivilegeName privilege)
        {
            Privilege = privilege;
        }

        public FunctionalPrivilegeName Privilege { get; private set; }
    }
}