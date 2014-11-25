using System;
using System.Web.Mvc;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes
{
    /// <summary>
    /// Для того, чтобы при десереализации отличать дату/время нового календаря.
    /// </summary>
    public sealed class CalendarAttribute : Attribute, IMetadataAware
    {
        public const string Name = "Calendar";

        public void OnMetadataCreated(ModelMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            metadata.AdditionalValues[Name] = true;
        }
    }
}
