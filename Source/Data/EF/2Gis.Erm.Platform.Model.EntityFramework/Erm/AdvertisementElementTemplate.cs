//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------

// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConvertNullableToShortForm

using System;
using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed partial class AdvertisementElementTemplate : 
        IEntity, 
        IEntityKey, 
        IAuditableEntity, 
        IDeletableEntity, 
        IStateTrackingEntity
    {
        public AdvertisementElementTemplate()
        {
            this.AdsTemplatesAdsElementTemplates = new HashSet<AdsTemplatesAdsElementTemplate>();
            this.AdvertisementElements = new HashSet<AdvertisementElement>();
        }
    
        public long Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> TextLengthRestriction { get; set; }
        public Nullable<byte> MaxSymbolsInWord { get; set; }
        public Nullable<int> TextLineBreaksCountRestriction { get; set; }
        public bool FormattedText { get; set; }
        public Nullable<int> FileSizeRestriction { get; set; }
        public string FileExtensionRestriction { get; set; }
        public Nullable<int> FileNameLengthRestriction { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public int RestrictionType { get; set; }
        public bool IsRequired { get; set; }
        public string ImageDimensionRestriction { get; set; }
        public bool IsAdvertisementLink { get; set; }
        public bool NeedsValidation { get; set; }
    
        public ICollection<AdsTemplatesAdsElementTemplate> AdsTemplatesAdsElementTemplates { get; set; }
        public ICollection<AdvertisementElement> AdvertisementElements { get; set; }
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm


