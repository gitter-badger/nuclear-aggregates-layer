using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class ViewModelCustomizationsIdentity : MetadataKindIdentityBase<ViewModelCustomizationsIdentity>
    {
        private readonly Uri _id = IdBuilder.For("Web/ViewModel/Customizations");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "MVC viewmodel customizations"; }
        }
    }
}
