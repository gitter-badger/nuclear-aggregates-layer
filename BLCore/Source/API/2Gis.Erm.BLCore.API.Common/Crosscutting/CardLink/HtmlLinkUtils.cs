using System;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.CardLink
{
    public static class HtmlLinkUtils
    {
        public static string AsHtmlLinkWithTitle(this Uri link, string linkTitle)
        {
            return string.Format("<a href='{0}'>{1}</a>", link.AbsoluteUri, linkTitle);
        }
    }
}