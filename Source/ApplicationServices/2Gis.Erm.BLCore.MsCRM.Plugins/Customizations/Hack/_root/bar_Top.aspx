<%@ Page language="c#" Inherits="Microsoft.Crm.Application.Pages.Root.TopBar" CodeBehind="Microsoft.Crm.Application.Pages.dll" %>
<%@ Register TagPrefix="loc" Namespace="Microsoft.Crm.Application.Controls.Localization" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="mnu" Namespace="Microsoft.Crm.Application.Menus" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="cnt" Namespace="Microsoft.Crm.Application.Controls" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="ui" Namespace="Microsoft.Crm.Application.Components.UI" Assembly="Microsoft.Crm.Application.Components.UI" %>
<%@ Import Namespace="Microsoft.Crm.Application.Pages.Common" %><html>
<head>
<cnt:AppHeader id="crmHeader" runat="server" />
<script type="text/javascript">






function DownloadClient()
{
var sUrl = "/_root/ClientInstaller.exe";
var oFrame = window.document.frames.frmDownloadOlk;
if(IsNull(oFrame))
{
oFrame = window.document.createElement("<iframe style='display:none' id='frmDownloadOlk' src='" + prependOrgName("/_root/Blank.aspx") + "'></iframe>");
oFrame = window.document.body.insertBefore(oFrame);

window.setTimeout("window.document.frames.frmDownloadOlk.location = \"" + sUrl + "\"", 10);
}
else
{
oFrame.location = sUrl;
}
}
function DisableAddinDownload() {
if (!IsNull(document.getElementById("btn_download_olk")))
{
document.getElementById("btn_download_olk").style.visibility = "hidden";
}
}

window.attachEvent("onload", function()
{
    if (!IsNull(document.getElementById('btn_new_campaignresponse')))
    {
        document.getElementById('btn_new_campaignresponse').style.display = 'none';
    }
});
</script>
<!--[if MSCRMClient]>
<script  type="text/javascript">



window.attachEvent( "onload", DisableAddinDownload);
</script>
<![endif]-->
</head>
<body>
<table id="barTopTable" cellpadding="0" cellspacing="0">
<tr>
<td colspan="2">
<table class="ms-crm-MastHead" cellpadding="0" cellspacing="0">
<tr>
<% if( IsCrmLive() ) { %>
<td class="ms-crm-MastHead-Logo-Live ms-crm-MastHead-SignIn" id="tdLogoMastHeadBar">
<% } else { %>
<td class="ms-crm-MastHead-Logo ms-crm-MastHead-SignIn" id="tdLogoMastHeadBar">
<% } %>
<%= RenderUserInfo() %>
<% if( ShouldRenderSignOutLink() ) { %>
<span class="ms-crm-MastHead-SignOut" id="tdSignOut">
<%= signOutHtml %>
</span>
<% } %>
</td>
</tr>
</table>
</td>
</tr>
<tr>
<td colspan="2">
<mnu:AppGlobalWebMenuBar id="crmMenuBar" runat="server" />
</td>
</tr>
<tr>

<td width="<loc:Text Encoding='HtmlAttribute' ResourceId='HomePage_Left_Navigation_Frame_Width' runat='server'/>" id="leftContextTD">
<table class="ms-crm-NavHeader stdTable" cellspacing="0" cellpadding="0">
<tr>
<td class="ms-crm-NavHeader-Title">
<nobr id="tdLeftContextBar"><%=Microsoft.Crm.CrmEncodeDecode.CrmHtmlEncode(leftContextBarTitle)%></nobr>
</td>

</tr>
</table>
</td>
<td>
<table class="ms-crm-ContextHeader stdTable" cellspacing="0" cellpadding="0">
<tr>
<td class="ms-crm-ContextHeader-Title">
<nobr id="tdStageContextBar"><%=Microsoft.Crm.CrmEncodeDecode.CrmHtmlEncode(stageContextBarTitle)%></nobr>
</td>
</tr>
</table>
</td>
</tr>
</table>
</body>
</html>
