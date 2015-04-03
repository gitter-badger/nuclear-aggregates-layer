using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel.Features.Parts;

using NuClear.Metamodeling.UI.Utils.Resources;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.ViewModel
{
    public static class ViewModelParts
    {
        #region Default parts feature
        private readonly static Lazy<ViewModelPartsFeature> DefaultPartsFeature = new Lazy<ViewModelPartsFeature>(GetDefaultPartsFeature);

        private static ViewModelPartsFeature GetDefaultPartsFeature()
        {
            var defaultPartsOrder = new Expression<Func<object>>[]
                {
                        () => BLResources.TitlePlacement,
                        () => BLResources.TitleDiscount,
                        () => BLResources.TitleFinances,
                        () => BLResources.TitleCancellation,
                        () => BLResources.TitleControl,
                        () => BLResources.AdministrationTabTitle
                };

            return new ViewModelPartsFeature(defaultPartsOrder.Select(ResourceEntryKey.Create).ToArray());
        }

        public static ViewModelPartsFeature Default
        {
            get
            {
                return DefaultPartsFeature.Value;
            }
        }
        #endregion
    }
}
