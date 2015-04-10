using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public class ViewModelCustomizationsIdentity : MetadataKindIdentityBase<ViewModelCustomizationsIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "Web/ViewModel/Customizations");

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
