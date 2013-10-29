using DocumentFormat.OpenXml.Wordprocessing;

namespace DoubleGis.Erm.Platform.Common.PrintFormEngine
{
	internal static class OpenXmlExtensions
	{
		public static string GetName(this SdtElement sdtElement)
		{
			return sdtElement.SdtProperties.GetFirstChild<SdtAlias>().Val;
		}

		public static string GetTag(this SdtElement sdtElement)
		{
			var tag = sdtElement.SdtProperties.GetFirstChild<Tag>();
			return tag == null || !tag.Val.HasValue ? string.Empty : tag.Val.Value;
		}
	}
}