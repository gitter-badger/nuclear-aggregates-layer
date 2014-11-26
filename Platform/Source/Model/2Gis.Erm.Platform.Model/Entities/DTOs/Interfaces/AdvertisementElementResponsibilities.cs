using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    public interface IAdvertisementElementRestrictions
    {
        AdvertisementElementRestrictionType TemplateRestrictionType { get; set; }
        bool TemplateFormattedText { get; set; }
        bool TemplateAdvertisementLink { get; set; }
    }

    public interface ITextAdvertisementElementDomainEntityDto
    {
        string PlainText { get; set; }
        string FormattedText { get; set; }
        int? TemplateTextLengthRestriction { get; set; }
        byte? TemplateMaxSymbolsInWord { get; set; }
        int? TemplateTextLineBreaksRestriction { get; set; }
    }

    public interface IFileAdvertisementElementDomainEntityDto
    {
        long? FileId { get; set; }
        string FileName { get; set; }
        long FileContentLength { get; set; }
        string FileContentType { get; set; }
        string TemplateFileExtensionRestriction { get; set; }
        string TemplateImageDimensionRestriction { get; set; }
    }

    public interface IPeriodAdvertisementElementDomainEntityDto
    {
        DateTime? BeginDate { get; set; }
        DateTime? EndDate { get; set; }
    }

    public interface IFasCommentAdvertisementElementDomainEntityDto
    {
        string PlainText { get; set; }
        FasComment? FasCommentType { get; set; }
        int? TemplateTextLengthRestriction { get; set; }
    }

    public interface ILinkAdvertisementElementDomainEntityDto
    {
        string PlainText { get; set; }
        int? TemplateTextLengthRestriction { get; set; }
    }

    public interface IAdvertisementElementTimestampDomainEntityDto
    {
        byte[] Timestamp { get; set; }
    }

    public static class AdvertisementElementResponsibilities
    {
        public static void TransferRestrictionValuesTo(this IAdvertisementElementRestrictions source,
                                                       IAdvertisementElementRestrictions target)
        {
            target.TemplateRestrictionType = source.TemplateRestrictionType;
            target.TemplateFormattedText = source.TemplateFormattedText;
            target.TemplateAdvertisementLink = source.TemplateAdvertisementLink;
        }

        public static void TransferTextValuesTo(this ITextAdvertisementElementDomainEntityDto source,
                                                ITextAdvertisementElementDomainEntityDto target)
        {
            target.PlainText = source.PlainText;
            target.FormattedText = source.FormattedText;
            target.TemplateTextLengthRestriction = source.TemplateTextLengthRestriction;
            target.TemplateMaxSymbolsInWord = source.TemplateMaxSymbolsInWord;
            target.TemplateTextLineBreaksRestriction = source.TemplateTextLineBreaksRestriction;
        }

        public static void TransferFileValuesTo(this IFileAdvertisementElementDomainEntityDto source,
                                                IFileAdvertisementElementDomainEntityDto target)
        {
            target.FileId = source.FileId;
            target.FileName = source.FileName;
            target.FileContentLength = source.FileContentLength;
            target.FileContentType = source.FileContentType;
            target.TemplateFileExtensionRestriction = source.TemplateFileExtensionRestriction;
            target.TemplateImageDimensionRestriction = source.TemplateImageDimensionRestriction;
        }

        public static void TransferPeriodValuesTo(this IPeriodAdvertisementElementDomainEntityDto source,
                                                  IPeriodAdvertisementElementDomainEntityDto target)
        {
            target.BeginDate = source.BeginDate;
            target.EndDate = source.EndDate;
        }

        public static void TransferFasCommentValuesTo(this IFasCommentAdvertisementElementDomainEntityDto source,
                                                      IFasCommentAdvertisementElementDomainEntityDto target)
        {
            target.FasCommentType = source.FasCommentType;
            target.PlainText = source.PlainText;
            target.TemplateTextLengthRestriction = source.TemplateTextLengthRestriction;
        }

        public static void TransferLinkValuesTo(this ILinkAdvertisementElementDomainEntityDto source,
                                                ILinkAdvertisementElementDomainEntityDto target)
        {
            target.PlainText = source.PlainText;
            target.TemplateTextLengthRestriction = source.TemplateTextLengthRestriction;
        }

        public static void TransferTimestampTo(this IAdvertisementElementTimestampDomainEntityDto source,
                                               IAdvertisementElementTimestampDomainEntityDto target)
        {
            target.Timestamp = source.Timestamp;
        }
    }
}