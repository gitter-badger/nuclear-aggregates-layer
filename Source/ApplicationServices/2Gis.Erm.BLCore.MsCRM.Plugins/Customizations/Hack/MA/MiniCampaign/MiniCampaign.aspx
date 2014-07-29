<%@ Page Language="c#" Inherits="Microsoft.Crm.Application.Pages.MA.MiniCampaignPage" %>

<%@ Register TagPrefix="frm" Namespace="Microsoft.Crm.Application.Forms" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="cnt" Namespace="Microsoft.Crm.Application.Controls" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="loc" Namespace="Microsoft.Crm.Application.Controls.Localization"
    Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="cui" Namespace="Microsoft.Crm.Application.Components.UI"
    Assembly="Microsoft.Crm.Application.Components.UI" %>
<%@ Register TagPrefix="sdk" Namespace="Microsoft.Crm.Application.Components.Sdk.FormControls.Web"
    Assembly="Microsoft.Crm.Application.Components.Sdk.FormControls" %>
<%@ Import Namespace="Microsoft.Crm" %>
<%@ Import Namespace="Microsoft.Crm.Application.Types" %>
<%@ Import Namespace="Microsoft.Crm.Application.Pages.Common" %>
<html xmlns:crm>
<head>
    <title>
        <loc:Text ResourceId="Title_Create_MiniCampaign_Wizard" runat="server" /></title>
    <cnt:AppHeader id="crmHeader" runat="server" />
    <style>
        .header
        {
            background-color: #6693CF;
        }
    </style>
    <% if (Util.IsForOutlookClient())
       { %>
    <object id="_oMailMerge" classid='clsid:E19EA63A-8B6F-4aa3-9013-3DE5EBAFD7BF' style='display: none'>
    </object>
    <% } %>
    <script language="Javascript">

var _iObjectTypeCode;
var _iMCOption;
var _iMCOptionTitle;
var _iTotalRecords;
var _iSelectedRecords;
var _aIds;
var _oTemplate;
var _oGridXml;


var _totalPages = 5;
var _currentPageIndex = 1;


var _welcomePageIndex = 1;
var _miniCampaignPageIndex = 2;
var _typeOwnerPageIndex = 3;
var _activityPageIndex = 4;
var _summaryPageIndex = 5;


var mc_MiniCampaignFor_int;
var mc_EmailTemplateId_str;
var mc_TargetOption_int;
var mc_TargetIds_str;
var mc_FetchXml_str;
var mc_MiniCampaignType_str;
var mc_LeafActivitiesOwner_int;
var mc_ActivityXml_str;
var mc_MiniCampaignName_str;
var mc_UnsubscribeText_str;
var mc_MailMerge_int;
var mc_MailMergeDocType_int;
var mc_Owner_int = SystemUser;
var mc_OwnerId_str="";
var mc_SendEmail_bool;

var _done = false;
var _iLastOwnerOption = 1;


var _activityTypes = new Array( <% =Util.PhoneCall %> , <% =Util.Letter  %> , <% =Util.Fax %> , <% =Util.Email %> , <% =Util.Appointment %> );
window.attachEvent("onbeforeunload",btnCancel);

/*Crm Hack*/
var CAN_SEND_MASS_MAILS = false;
window.lib = window.lib || {};
window.lib.Ajax = {
    syncRequest: function (method, uri, cb, data, options)
    {
        var activeX = ['Msxml2.XMLHTTP.6.0',
                   'Msxml2.XMLHTTP.3.0',
                   'Msxml2.XMLHTTP'],
            createXhrObject = function ()
            {
                var http;
                try
                {
                    http = new XMLHttpRequest();
                } catch (e)
                {
                    for (var i = 0; i < activeX.length; ++i)
                    {
                        try
                        {
                            http = new ActiveXObject(activeX[i]);
                            break;
                        } catch (e) { }
                    }
                } finally
                {
                    return { conn: http};
                }
            },
            getConnectionObject = function ()
            {
                var o;
                try
                {
                    o = createXhrObject();
                } catch (e)
                {
                } finally
                {
                    return o;
                }
            },
        syncRequest = function (method, uri, callback, postData)
        {
            var o = getConnectionObject() || null;
            if (o)
            {
                o.conn.open(method, uri, false);
                o.conn.send(postData || null);
            }
            return o;
        };
        return syncRequest(method || options.method || "POST", uri, cb, data);
    }
};
window.AjaxRequest= {
    urlEncode : function(o){
            var buf = [],
                e = encodeURIComponent;
            for(var key in o) 
            {
                var item = o[key];
                buf.push('&', e(key), '=', !IsNull(item)?e(item):'');
            }
            buf.shift();
            return buf.join('');
        },
    urlAppend : function(url, s){
            if(!IsNull(s)){
                return url + (url.indexOf('?') === -1 ? '?' : '&') + s;
            }
            return url;
        },
    syncRequest: function (o)
    {
        var GET = "GET";

        var me = this;
        var p = o.params, url = o.url, method;


            p = this.urlEncode(p);

            method = o.method;

            if (method === GET && o.disableCaching !== false || o.disableCaching === true)
            {
                var dcp = '__dc';
                url = this.urlAppend(url, dcp + '=' + (new Date().getTime()));
            }
            if (method == GET && p)
            {
                url = this.urlAppend(url, p);
                p = '';
            }
            return (me.transId = lib.Ajax.syncRequest(method, url, undefined, p, o));
    }};
/*End Crm Hack*/


function window.onload()
{
    if(tblItems.all.tags("li") != null)
    {
        /*Crm Hack*/
        var serverName = ORG_UNIQUE_NAME.replace("DoubleGis", "Erm");
        var result = window.AjaxRequest.syncRequest(
            {
                method: "GET", 
                url: GetErmWebAppUrl() + "Privilege/HasFunctionalPrivilegeGranted",
                params:
                    {
                        privilegeName: "SendMassEmail"
                    }
            });
        

        if ((result.conn.status >= 200 && result.conn.status < 300) || result.conn.status == 1223)
        {
            var responseText = result.conn.responseText;
            if (responseText) {
                hasPrivilege = responseText.toLowerCase() === 'true';
                CAN_SEND_MASS_MAILS = hasPrivilege;
            }
        }
        else
        {
            alert(result.conn.responseText);
        }

        result.conn = null;
        result = null;
        /*End Crm Hack*/
        SelectItem( tblItems.all.tags("li")[0] );
    }

<% if(Util.IsForOutlookClient()) { %>
try
{
_oMailMerge.Initialize();
}
catch(e)
{
}
<% } %>


mcInit.style.display = "none";
mcInit.style.display = "none";


this.focus();


mcPageBody1.style.display = "inline";
mcPageBody1.style.display = "inline";


btn_id_Next.disabled = false;
btn_id_Back.disabled = true;
btn_id_Cancel.disabled = false;


with(window.dialogArguments)
{
_iObjectTypeCode  	= ObjectTypeCode;
_iMCOption  		= MCOption;
_iMCOptionTitle     = MCOptionTitle;
_iTotalRecords		= TotalRecords;
_iSelectedRecords	= SelectedRecords;
_aIds				= Ids;
_oGridXml			= GridXml;
}


btn_id_Back.disabled = true;

<% if(Util.IsForOutlookClient()) { %>

var queryString = "<%=Microsoft.Crm.Application.Utility.Util.PrependOrganizationName("/MA/MiniCampaign/iframes/mailmergeForm.aspx?objectTypeCode=")%>" + CrmEncodeDecode.CrmUrlEncode(_iObjectTypeCode);
if (_iObjectTypeCode == List && _aIds != null && _aIds != "" && _aIds.length > 0)
{
queryString += "&objectId=" + CrmEncodeDecode.CrmUrlEncode(_aIds[0]);
}

var oMailMergeFrame = document.getElementById("mailMergeFormFrame");
oMailMergeFrame.src = queryString;
<% } %>


overrideClose();
}

var isActivityFocus = false;
function ActivityControlFocus()
{
isActivityFocus = true;
}
function ActivityControlBlur()
{
isActivityFocus = false;
}

function On(e)
{
if(_selectedItem && e != _selectedItem)
{
e.runtimeStyle.backgroundColor = "#c6dfff";
}
}

function Off(e)
{
if(_selectedItem && e != _selectedItem)
{
e.runtimeStyle.backgroundColor = "";
}
}

function SelectItem( e )
{
if(_currentPageIndex == _typeOwnerPageIndex)
{
tblItems.focus();
}
GetSrc( e );

if( e != _selectedItem )
{
e.runtimeStyle.backgroundColor = "#a7cdf0";
if( _selectedItem ) _selectedItem.runtimeStyle.backgroundColor = "";
_selectedItem = e;



if (_onSelectFunc)
{
_onSelectFunc(e);
}
}
}

function document.onkeydown()
{
if(isActivityFocus == true)
{
try
{
switch (event.keyCode)
{
case 38:
if (_selectedItem.previousSibling.item != undefined)
{
SelectItem(_selectedItem.previousSibling);
}
break;

case 40:
if (_selectedItem.nextSibling.item != undefined)
{
SelectItem(_selectedItem.nextSibling);
}
break;
}
}
catch(e)
{
}
}
}


function closeWizard(bCompleted)
{

this.document.body.innerHTML = "<table height='100%' width='100%' style='cursor:wait'><tr><td valign='middle' align='center'><img alt='' src='/_imgs/AdvFind/progress.gif'/><br>" + LOCID_MC_PROGRESS_CLOSE + "</td></tr></table>";
window.returnValue = bCompleted;
window.close();
window.detachEvent("onbeforeunload",btnCancel);
}


function btnCancel()
{

overrideClose();


var answer = confirm(LOCID_MC_ALERT_CLOSE);


if(answer == true)
{

btn_id_Next.disabled = true;
btn_id_Back.disabled = true;
btn_id_Cancel.disabled = true;


closeWizard(false);
}
return answer;
}


function _onSelectFunc(oArg)
{
if(oArg.id.substring(0, 5) == "mm_id")
{


_iLastOwnerOption = (rad_Myself.checked == true)? 1 : _iLastOwnerOption;
_iLastOwnerOption = (rad_Record.checked == true)? 2 : _iLastOwnerOption;
_iLastOwnerOption = (rad_user_queue.checked == true)? 3 : _iLastOwnerOption;

rad_Myself.checked = true;
rad_Myself.disabled = true;
rad_Record.checked = false;
rad_Record.disabled = true;
rad_user_queue.disabled = true;
rad_user_queue.checked = false;

SendEmail.disabled = true;
SendEmail.checked = false;
}
else
{


if(rad_Myself.disabled == true)
{
rad_Myself.checked = (_iLastOwnerOption == 1);
rad_Myself.disabled = false;
rad_Record.checked = (_iLastOwnerOption == 2);
rad_Record.disabled = false;
rad_user_queue.disabled = false;
rad_user_queue.checked = (_iLastOwnerOption == 3);
}

/*CRM Hack*/
SendEmail.disabled=(_selectedItem.item == Email && CAN_SEND_MASS_MAILS===true)?false:true;
SendEmail.checked=(_selectedItem.item == Email && CAN_SEND_MASS_MAILS===true)?true:false;
/*End CRM Hack*/
}
}


function overrideClose()
{
if(phoneFormFrame && phoneFormFrame.crmForm) phoneFormFrame.crmForm._bSaving = true;
if(letterFormFrame && letterFormFrame.crmForm) letterFormFrame.crmForm._bSaving = true;
if(faxFormFrame && faxFormFrame.crmForm) faxFormFrame.crmForm._bSaving = true;
if(appointmentFormFrame && appointmentFormFrame.crmForm) appointmentFormFrame.crmForm._bSaving = true;
if(emailFormFrame && emailFormFrame.crmForm) emailFormFrame.crmForm._bSaving = true;
}


function finish()
{

btn_id_Next.disabled = true;


overrideClose();

<% if(Util.IsForOutlookClient()) { %>

if(_currentPageIndex == _activityPageIndex && _selectedItem.id.substring(0, 5) == "mm_id")
{

btn_id_Back.disabled = true;
btn_id_Cancel.disabled = true;


GetWizardData();

var objectType = mc_MiniCampaignFor_int;
var objectId = "";
if(mc_MiniCampaignFor_int == List)
{
objectId = IsNull(_aIds[0]) ? "" : _aIds[0];
}
var lookupObjectType = mailMergeFormFrame.getLookupType();
var languageId = mailMergeFormFrame.getLanguage();
var templateId = mailMergeFormFrame.getTemplateId();
var currentPage = false;
var ids = IsNull(mc_TargetIds_str)? "" : mc_TargetIds_str;
var gridXml = IsNull(mc_FetchXml_str) ? "" : mc_FetchXml_str;
var tempLayoutXml = mailMergeFormFrame.getLayoutXml(false);
var layoutXml = IsNull(tempLayoutXml) ? "" : tempLayoutXml;
var mergeType = mc_MailMergeDocType_int;
var quickCampaignName = IsNull(mc_MiniCampaignName_str)? "" : mc_MiniCampaignName_str;

try
{

_oMailMerge.DoMailMerge(objectType, objectId, lookupObjectType, languageId, templateId, currentPage, ids, gridXml, layoutXml, mergeType, quickCampaignName);
}
catch(e)
{
alert( <% =Microsoft.Crm.CrmEncodeDecode.CrmJavaScriptEncode(CurrentResourceManager.GetString("MailMerge_ErrorInAddin")) %> );
window.close();
}

}
<% } %>


closeWizard(true);
}



function updateSummary()
{
this.focus();

btn_id_Next.innerHTML = LOCID_MC_BTN_CREATE;


var ele = document.getElementById("td_MC_Name");
td_MC_Name.title = td_MC_Name.innerText = Trim(MC_Name.value);


ele = document.getElementById("td_MC_Type");
ele.innerHTML = document.getElementById("id_" + _selectedItem.item).innerHTML;


ele = document.getElementById("td_MC_Scope");
ele.innerHTML = _iMCOptionTitle;

switch(_iMCOption)
{
case 1:
ele.innerHTML += " ( " + _iSelectedRecords + " )";
break;

case 2:
ele.innerHTML += " ( " + _iTotalRecords + " )";
break;
}


ele = document.getElementById("td_MC_Owner");
if(rad_Myself.checked == true)
{
ele.innerHTML = document.getElementById("id_owner_myself").innerHTML;
}
else if(rad_Record.checked == true)
{
ele.innerHTML = document.getElementById("id_owner_record").innerHTML;
}
else if(rad_user_queue.checked == true)
{
ele.innerHTML = ActivityLookup.parentElement.previousSibling.children[0].innerHTML;
}
btn_id_Next.focus();
}


function GetWizardData()
{

mc_MiniCampaignFor_int = _iObjectTypeCode;


mc_EmailTemplateId_str = "";


mc_TargetOption_int = _iMCOption;


mc_TargetIds_str = "";
if(_aIds != null && _aIds != "" && _aIds.length > 0)
{
mc_TargetIds_str += _aIds[0];
for(var i=1; i<_aIds.length; i++)
{
mc_TargetIds_str += "," + _aIds[i];
}
}


mc_FetchXml_str = _oGridXml;


mc_MiniCampaignType_str = _selectedItem.item;


if(_selectedItem.id.substring(0, 5) == "mm_id")
{
mc_MailMerge_int = 1;
}
else
{
mc_MailMerge_int = 0;
}


mc_LeafActivitiesOwner_int = 0;

if(rad_Myself.checked == true)
{
mc_LeafActivitiesOwner_int = 2;
}
else if(rad_Record.checked == true)
{
mc_LeafActivitiesOwner_int = 1;
}
else if(rad_user_queue.checked == true)
{
mc_LeafActivitiesOwner_int = 0;
mc_Owner_int=ActivityLookup.DataValue[0].type;
mc_OwnerId_str=ActivityLookup.DataValue[0].id;
}
mc_SendEmail_bool=(SendEmail.checked);


mc_ActivityXml_str = "";


if(_selectedItem.item == Fax)
{
faxFormFrame.crmForm.BuildXml(false, false);
mc_ActivityXml_str = faxFormFrame.crmFormSubmit.crmFormSubmitXml.value;
}
else if(_selectedItem.item == PhoneCall)
{
phoneFormFrame.crmForm.BuildXml(false, false);
mc_ActivityXml_str = phoneFormFrame.crmFormSubmit.crmFormSubmitXml.value;
}
else if(_selectedItem.item == Appointment)
{
appointmentFormFrame.crmForm.BuildXml(false, false);
mc_ActivityXml_str = appointmentFormFrame.crmFormSubmit.crmFormSubmitXml.value;
}
else if(_selectedItem.item == Letter)
{
letterFormFrame.crmForm.BuildXml(false, false);
mc_ActivityXml_str = letterFormFrame.crmFormSubmit.crmFormSubmitXml.value;
}
else if(_selectedItem.item == Email)
{
emailFormFrame.crmForm.BuildXml(false, false);
mc_ActivityXml_str = emailFormFrame.crmFormSubmit.crmFormSubmitXml.value;
}


mc_MiniCampaignName_str = Trim(MC_Name.value);


mc_UnsubscribeText_str = "";


mc_MailMergeDocType_int = 0;
if(mc_MiniCampaignType_str == Letter)
{
mc_MailMergeDocType_int = 0;
}
else if(mc_MiniCampaignType_str == Email)
{
mc_MailMergeDocType_int = 4;
}
else if(mc_MiniCampaignType_str == Fax)
{
mc_MailMergeDocType_int = 5;
}
}


function btnNext()
{

if(_done == true)
{

finish();
return;
}


if(_currentPageIndex == _totalPages)
{

btn_id_Back.disabled = true;
btn_id_Cancel.disabled = true;
btn_id_Next.disabled = true;


var oldInnerHTML = mcPageBody5.innerHTML;
mcPageBody5.innerHTML = "<table height='100%' width='100%' style='cursor:wait'><tr><td valign='middle' align='center'><img alt='' src='/_imgs/AdvFind/progress.gif'/><br>" + LOCID_MC_PROGRESS_CREATE + "</td></tr></table>";


GetWizardData();


try
{

var command = new RemoteCommand("MarketingAutomation", "CreateMiniCampaign");


command.SetParameter("mc_MiniCampaignFor_int", mc_MiniCampaignFor_int);
command.SetParameter("mc_EmailTemplateId_str", mc_EmailTemplateId_str);
command.SetParameter("mc_TargetOption_int", mc_TargetOption_int);
command.SetParameter("mc_TargetIds_str", mc_TargetIds_str);
command.SetParameter("mc_FetchXml_str", CrmEncodeDecode.CrmXmlEncode(mc_FetchXml_str));
command.SetParameter("mc_MiniCampaignType_str", mc_MiniCampaignType_str);
command.SetParameter("mc_LeafActivitiesOwner_int", mc_LeafActivitiesOwner_int);
command.SetParameter("mc_ActivityXml_str", CrmEncodeDecode.CrmXmlEncode(mc_ActivityXml_str));
command.SetParameter("mc_MiniCampaignName_str", CrmEncodeDecode.CrmHtmlEncode(mc_MiniCampaignName_str));
command.SetParameter("mc_PostWorkflowEvent_bool", true);
command.SetParameter("ownerType", mc_Owner_int);
command.SetParameter("ownerIdUser", mc_OwnerId_str);
command.SetParameter("sendEmail", mc_SendEmail_bool);

var result = command.Execute();
}
catch(e)
{
}




_done = true;


finish();

return;
}
else
{

btn_id_Back.disabled = false;


switch(_currentPageIndex)
{

case _typeOwnerPageIndex :


if(_selectedItem.id.substring(0, 5) == "mm_id")
{
mcHeader.innerText = LOCID_MC_MSG_DETAILS_MM;
}
else
{
mcHeader.innerText = LOCID_MC_MSG_DETAILS;
}


if(!_selectedItem)
{
alert(LOCID_MC_TYPE_MANDATORY);


return;
}

if((!rad_Myself.checked && !rad_Record.checked && !rad_user_queue.checked)||
(rad_user_queue.checked && IsNull(ActivityLookup.DataValue)))
{
alert(LOCID_MC_OWNER_MANDATORY);


return;
}

break;


case _activityPageIndex :

var isValid = true;

if(_selectedItem.id.substring(0, 5) == "mm_id")
{
if (mailMergeFormFrame.validateInputs())
{

_done = true;
finish();
return;
}
else
{

return;
}
}
else
{

if(_selectedItem.item == Fax)
{
isValid = faxFormFrame.crmForm.IsValid();
}
else if(_selectedItem.item == PhoneCall)
{
isValid = phoneFormFrame.crmForm.IsValid();
}
else if(_selectedItem.item == Appointment)
{
isValid = appointmentFormFrame.crmForm.IsValid();
}
else if(_selectedItem.item == Letter)
{
isValid = letterFormFrame.crmForm.IsValid();
}
else if(_selectedItem.item == Email)
{
isValid = emailFormFrame.crmForm.IsValid();
}
}

if(isValid == false)
{

return;
}

break;


case _miniCampaignPageIndex :


if(Trim(MC_Name.value) == "")
{
alert(LOCID_MC_NAME_MANDATORY);
MC_Name.focus();
return;
}

break;
}


_currentPageIndex++;
}


enableDisplayForPage(_currentPageIndex);
}

function enable()
{
ActivityLookup.Disabled=(rad_user_queue.checked) ? false:true;
}


function btnBack()
{

btn_id_Next.innerHTML = LOCID_MC_BUTTON_NEXT;
_done = false;


if(_currentPageIndex == _welcomePageIndex)
{

return;
}
else
{

_currentPageIndex--;
}


enableDisplayForPage(_currentPageIndex);
}


function enableDisplayForPage(pageIndex)
{

if(pageIndex == _welcomePageIndex)
{
btn_id_Back.disabled = true;
}
else
{
btn_id_Back.disabled = false;
}


var pageHeader = "";
var pageBody = "";



for(var i=1; i<=_totalPages; i++)
{
pageHeader = document.all["mcPageHeader" + i];
pageBody = document.all["mcPageBody" + i];

pageHeader.style.display = "none";
pageBody.style.display = "none";
}

pageHeader = document.all["mcPageHeader" + pageIndex];
pageBody = document.all["mcPageBody" + pageIndex];

pageHeader.style.display = "inline";
pageBody.style.display = "inline";


if(pageIndex == _activityPageIndex)
{

activityHeader = document.all["ActivityHeaderLabel"];
activityHeader.innerHTML = LOCID_MC_ACTIVITYHEADER;

var activityIframe = "";


for(var i=1; i<=_activityTypes.length; i++)
{
activityIframe = document.all["mcPageBody3_" + _activityTypes[i-1]];
activityIframe.style.display = "none";
}


activityIframe = document.all["mcPageBody3_MailMerge"];
activityIframe.style.display = "none";

if(_selectedItem.id.substring(0, 5) == "mm_id")
{
activityHeader.innerHTML = LOCID_MC_MM_HEADER;


activityIframe = document.all["mcPageBody3_MailMerge"];
activityIframe.style.display = "inline";


btn_id_Next.innerHTML = LOCID_MC_BTN_LAUNCHWORD;
}
else
{

activityIframe = document.all["mcPageBody3_" + _selectedItem.item];
activityIframe.style.display = "inline";
}
}


if(pageIndex == _miniCampaignPageIndex)
{
MC_Name.focus();
}


if(pageIndex == _summaryPageIndex)
{
updateSummary();
}
}

    </script>
</head>
<body scroll="no" class="MiniCampaign_body">
    <table cellspacing="0" cellpadding="0" border="0" bordercolor="blue" height="100%"
        width="100%">
        <tr height="40">
            <td style="border-bottom: 1px solid #CCCCCC">
                <table cellpadding="0" cellspacing="5" class="header" width="100%" style="table-layout: fixed;">
                    <tr valign="top" style="display: inline;">
                        <td width="100%" class="MiniCampaign_td_PageHeader1">
                            <span id="mcPageHeader1" style="display: inline">
                                <table width="100%" cellpadding="5" cellspacing="0">
                                    <tr>
                                        <td width="80%">
                                            <font style="color: #ffffff; font-size: <%= CrmStyles.GetFontResourceAttribute("General.14px.font_size") %>;
                                                font-weight: <%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>;">
                                                <loc:Text ResourceId="MC_MSG_Header_Welcome" runat="server" />
                                            </font>
                                        </td>
                                        <td width="20%" class="MiniCampaign_td_ImgWizHeader">
                                            <img alt="" src="/_imgs/minicamp_wizard_header.gif" width="48" height="48">
                                        </td>
                                    </tr>
                                </table>
                            </span><span id="mcPageHeader2" style="display: none">
                                <table width="100%" cellpadding="5" cellspacing="0">
                                    <tr>
                                        <td width="80%">
                                            <font style="color: #ffffff; font-size: <%= CrmStyles.GetFontResourceAttribute("General.14px.font_size") %>;
                                                font-weight: <%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>;">
                                                <loc:Text ResourceId="MC_MSG_Header_SpecifyName" runat="server" />
                                            </font>
                                        </td>
                                        <td width="20%" class="MiniCampaign_td_ImgWizHeader">
                                            <img alt="" src="/_imgs/minicamp_wizard_header.gif" width="48" height="48">
                                        </td>
                                    </tr>
                                </table>
                            </span><span id="mcPageHeader3" style="display: none">
                                <table width="100%" cellpadding="5" cellspacing="0">
                                    <tr>
                                        <td width="80%">
                                            <font style="color: #ffffff; font-size: <%= CrmStyles.GetFontResourceAttribute("General.14px.font_size") %>;
                                                font-weight: <%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>;">
                                                <loc:Text ResourceId="MC_MSG_Header_ActivityAndAssignment" runat="server" />
                                            </font>
                                        </td>
                                        <td width="20%" class="MiniCampaign_td_spacer">
                                            <img alt="" src="/_imgs/minicamp_wizard_header.gif" width="48" height="48">
                                        </td>
                                    </tr>
                                </table>
                            </span><span id="mcPageHeader4" style="display: none">
                                <table width="100%" cellpadding="5" cellspacing="0">
                                    <tr>
                                        <td width="80%">
                                            <font style="color: #ffffff; font-size: <%= CrmStyles.GetFontResourceAttribute("General.14px.font_size") %>;
                                                font-weight: <%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>;">
                                                <label id="ActivityHeaderLabel" />
                                            </font>
                                        </td>
                                        <td width="20%" class="MiniCampaign_td_ImgWizHeader">
                                            <img alt="" src="/_imgs/minicamp_wizard_header.gif" width="48" height="48">
                                        </td>
                                    </tr>
                                </table>
                            </span><span id="mcPageHeader5" style="display: none">
                                <table width="100%" cellpadding="5" cellspacing="0">
                                    <tr>
                                        <td width="80%">
                                            <font style="color: #ffffff; font-size: <%= CrmStyles.GetFontResourceAttribute("General.14px.font_size") %>;
                                                font-weight: <%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>;">
                                                <loc:Text ResourceId="MC_MSG_Header_Complete" runat="server" />
                                            </font>
                                        </td>
                                        <td width="20%" class="MiniCampaign_td_ImgWizHeader">
                                            <img alt="" src="/_imgs/minicamp_wizard_header.gif" width="48" height="48">
                                        </td>
                                    </tr>
                                </table>
                            </span>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td valign="top" style="padding-bottom: 10px; padding-top: 10px;">
                <span id="mcInit" style="display: inline">
                    <table height='100%' width='100%' cellspacing="0" cellpadding="0" style='cursor: wait'>
                        <tr>
                            <td valign='middle' align='center'>
                                <img alt='' src='/_imgs/AdvFind/progress.gif' />
                                <br>
                                <loc:Text ResourceId="MC_MSG_Progress_Init" runat="server" />
                            </td>
                        </tr>
                    </table>
                </span><span id="mcPageBody1" style="display: none">
                    <table width="100%" cellpadding="0" cellspacing="0">
                        <tr valign="top">
                            <td class="MiniCampaign_td_PageBody1">
                                <table width="100%" cellpadding="0" cellspacing="0">
                                    <tr valign="top">
                                        <td>
                                            <loc:Text ResourceId="MC_MSG_WizardPurpose" runat="server" />
                                        </td>
                                    </tr>
                                    <tr height="20">
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td>
                                            <loc:Text ResourceId="MC_MSG_WizardUsage" runat="server" />
                                        </td>
                                    </tr>
                                    <tr height="20">
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td>
                                            <font style="color: #333333;">
                                                <loc:Text ResourceId="MC_MSG_WizardSteps" runat="server" />
                                            </font>
                                        </td>
                                    </tr>
                                    <tr height="20">
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td>
                                            <ul>
                                                <li>
                                                    <loc:Text ResourceId="MC_MSG_WizardSteps_ChooseActivity" runat="server" />
                                                    <li>
                                                        <loc:Text ResourceId="MC_MSG_WizardSteps_AssignActivity" runat="server" />
                                                        <li>
                                                            <loc:Text ResourceId="MC_MSG_WizardSteps_SupplyContent" runat="server" />
                                            </ul>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td>
                                            <loc:Text ResourceId="MC_MSG_WizardSteps_Continue" runat="server" />
                                        </td>
                                    </tr>
                                    <tr height="20">
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </span><span id="mcPageBody2" style="display: none;">
                    <table height="100%" width="100%" border="0" bordercolor="red" cellpadding="0" cellspacing="0">
                        <tr valign="top">
                            <td class="MiniCampaign_td_PageBody2">
                                <table width="100%" cellpadding="0" cellspacing="0">
                                    <tr valign="top">
                                        <td>
                                            <loc:Text ResourceId="MC_MSG_Response" runat="server" />
                                        </td>
                                    </tr>
                                    <tr height="20">
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td>
                                            <label for="MC_Name">
                                                <loc:Text ResourceId="MC_MSG_EnterTheName" runat="server" />
                                            </label>
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td class="MiniCampaign_td_MCNameBody1">
                                            <input class="text" type="text" size="83" id="MC_Name" maxlength="200" name="minicampname">
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </span><span id="mcPageBody3" style="display: none;">
                    <table height="100%" width="100%" cellpadding="0" cellspacing="0" border="0">
                        <tr valign="top" height="40%">
                            <td class="MiniCampaign_td_ChooseActivity">
                                <table width="100%" cellpadding="0" cellspacing="4">
                                    <tr valign="top">
                                        <td>
                                            <font style="color: #333333; font-weight: <%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>">
                                                <loc:Text ResourceId="MC_MSG_ChooseActivity" runat="server" />
                                            </font></label>
                                        </td>
                                    </tr>
                                    <tr valign="bottom">
                                        <td>
                                            <loc:Text ResourceId="MC_MSG_ActivityType" runat="server" />
                                            </label>
                                        </td>
                                    </tr>
                                    <tr class="main">
                                        <td>
                                            <table width="100%" border="0" bordercolor="green" cellspacing="0" cellpadding="0">
                                                <tr>
                                                    <td style="padding-top: 6px; height: 130px">
                                                        <div class="ms-crm-Dialog-List">
                                                            <ul onfocus="ActivityControlFocus();" onblur="ActivityControlBlur();" id="tblItems"
                                                                class="ms-crm-Dialog-List" style="border: 1px solid #CCCCCC; height: 100px" tabindex="3">
                                                                <%
                                                                    RenderListItem(Util.PhoneCall, Privileges.CreateActivity);
                                                                    RenderListItem(Util.Appointment, Privileges.CreateActivity);
                                                                    RenderListItem(Util.Letter, Privileges.CreateActivity);
                                                                    RenderMailMergeListItem(Util.Letter, Privileges.CreateActivity);
                                                                    RenderListItem(Util.Fax, Privileges.CreateActivity);
                                                                    RenderMailMergeListItem(Util.Fax, Privileges.CreateActivity);
                                                                    RenderListItem(Util.Email, Privileges.CreateActivity);
                                                                    RenderMailMergeListItem(Util.Email, Privileges.CreateActivity);
                                                                %>
                                                            </ul>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr height="15px">
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr height="60%">
                            <td valign="top" class="MiniCampaign_td_ActivitiesOwn">
                                <table cellpadding="0" cellspacing="0" border="0" bordercolor="red" width="100%">
                                    <tr valign="top">
                                        <td>
                                            <table height="100%" cellpadding="0" cellspacing="0" width="100%">
                                                <tr valign="top">
                                                    <td>
                                                        <font style="color: #333333; font-weight: <%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>">
                                                            <loc:Text ResourceId="MC_MSG_ActivitiesOwn" runat="server" />
                                                        </font></label>
                                                    </td>
                                                </tr>
                                                <tr valign="top">
                                                    <td style="padding-top: 4px">
                                                        <loc:Text ResourceId="MC_MSG_AssignActivitiesTo" runat="server" />
                                                        </label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <table width="100%" height="100%" border="0" style="padding-top: 4px" cellspacing="0"
                                                            cellpadding="0">
                                                            <tr>
                                                                <td class="MiniCampaign_td_OwnerGroup" valign="top">
                                                                    <input class="ms-crm-RadioButton" type="radio" id="rad_Myself" name="radOwnerGroup"
                                                                        editable checked onclick="enable();">
                                                                </td>
                                                                <td id="id_owner_myself">
                                                                    <label for="rad_Myself">
                                                                        <loc:Text ResourceId="MC_MSG_Owner_Myself" runat="server" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td class="MiniCampaign_td_OwnerGroup" valign="top">
                                                                    <input class="ms-crm-RadioButton" type="radio" id="rad_Record" name="radOwnerGroup"
                                                                        editable onclick="enable();">
                                                                </td>
                                                                <td id="id_owner_record">
                                                                    <label for="rad_Record">
                                                                        <loc:Text ResourceId="MC_MSG_Owner_Leaf" runat="server" />
                                                                    </label>
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td align="left" valign="top">
                                                                    <input class="ms-crm-RadioButton" type="radio" id="rad_user_queue" name="radOwnerGroup"
                                                                        editable onclick="enable();">
                                                                </td>
                                                                <td id="id_owner_user">
                                                                    <label class="ms-crm-MiniCampaign-LabelAbove" for="rad_user_queue">
                                                                        <loc:Text ResourceId="Dialog_AssignQueue_Assign_Label" runat="server" />
                                                                    </label>
                                                                    <sdk:LookupControl id="ActivityLookup" LookupClass="BasicOwner" runat="server" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr height="35px">
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr valign="top">
                                                    <td>
                                                        <font style="color: #333333; font-weight: <%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>">
                                                            <loc:Text ResourceId="MC_MSG_Email_Close" runat="server" />
                                                        </font></label>
                                                    </td>
                                                </tr>
                                                <tr valign="top">
                                                    <td>
                                                        <input class="checkbox" type="checkbox" id="SendEmail" runat="server" disabled>
                                                        <label for="SendEmail">
                                                            <loc:Text ResourceId="MC_MSG_Email_check" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr height="10%">
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </span><span id="mcPageBody4" style="display: none">
                    <table height="100%" width="100%" border="0" bordercolor="red" cellpadding="0" cellspacing="0">
                        <tr class="main" height="1%">
                            <td class="MiniCampaign_td_MSGDetails">
                                <span id="mcHeader">
                                    <loc:Text ResourceId="MC_MSG_Details" runat="server" />
                                    </label> </span>
                            </td>
                        </tr>
                        <tr>
                            <td width="100%" height="100%" valign="top">
                                <span id="mcPageBody3_<% =Util.PhoneCall %>" style="display: none">
                                    <iframe name="phoneFormFrame" id="phoneFormFrame" src="<%=Microsoft.Crm.Application.Utility.Util.PrependOrganizationName("/MA/MiniCampaign/iframes/phonecallForm.aspx")%>"
                                        width="100%" height="100%" scrolling="auto"></iframe>
                                </span><span id="mcPageBody3_<% =Util.Letter %>" style="display: none">
                                    <iframe name="letterFormFrame" id="letterFormFrame" src="<%=Microsoft.Crm.Application.Utility.Util.PrependOrganizationName("/MA/MiniCampaign/iframes/letterForm.aspx")%>"
                                        width="100%" height="100%" scrolling="auto"></iframe>
                                </span><span id="mcPageBody3_<% =Util.Fax %>" style="display: none">
                                    <iframe name="faxFormFrame" id="faxFormFrame" src="<%=Microsoft.Crm.Application.Utility.Util.PrependOrganizationName("/MA/MiniCampaign/iframes/faxForm.aspx")%>"
                                        width="100%" height="100%" scrolling="auto"></iframe>
                                </span><span id="mcPageBody3_<% =Util.Appointment %>" style="display: none">
                                    <iframe name="appointmentFormFrame" id="appointmentFormFrame" src="<%=Microsoft.Crm.Application.Utility.Util.PrependOrganizationName("/MA/MiniCampaign/iframes/appointmentForm.aspx")%>"
                                        width="100%" height="100%" scrolling="auto"></iframe>
                                </span><span id="mcPageBody3_<% =Util.Email %>" style="display: none">
                                    <iframe name="emailFormFrame" id="emailFormFrame" src="<%=Microsoft.Crm.Application.Utility.Util.PrependOrganizationName("/MA/MiniCampaign/iframes/emailForm.aspx")%>"
                                        width="100%" height="100%" scrolling="auto"></iframe>
                                </span><span id="mcPageBody3_MailMerge" style="display: none">
                                    <iframe name="mailMergeFormFrame" id="mailMergeFormFrame" src="<%=Microsoft.Crm.Application.Utility.Util.PrependOrganizationName("/_root/Blank.aspx")%>"
                                        width="100%" height="100%" scrolling="auto"></iframe>
                                </span>
                            </td>
                        </tr>
                    </table>
                </span><span id="mcPageBody5" style="display: none">
                    <table height="100%" width="100%" cellpadding="0" cellspacing="0">
                        <tr class="MiniCampaign_tr_SummaryTop" valign="top">
                            <td>
                                <table width="100%" cellpadding="0" cellspacing="0">
                                    <tr valign="top">
                                        <td id="id_summary_top">
                                            <loc:Text ResourceId="MC_MSG_StepsCompleted" runat="server" />
                                        </td>
                                    </tr>
                                    <tr height="20">
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td>
                                            <table style="table-layout: fixed">
                                                <col width="140">
                                                <col>
                                                <tr>
                                                    <td>
                                                        <loc:Text ResourceId="MC_Summary_Name" runat="server" />
                                                    </td>
                                                    <td class="MiniCampaign_td_MCName">
                                                        <nobr id="td_MC_Name">&nbsp;</nobr>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <loc:Text ResourceId="MC_Summary_Type" runat="server" />
                                                    </td>
                                                    <td id="td_MC_Type" class="MiniCampaign_td_MCType">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <loc:Text ResourceId="MC_Summary_Scope" runat="server" />
                                                    </td>
                                                    <td id="td_MC_Scope" class="MiniCampaign_td_MCScope">
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <loc:Text ResourceId="MC_Summary_Owner" runat="server" />
                                                    </td>
                                                    <td id="td_MC_Owner" class="MiniCampaign_td_MCOwner">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr height="20">
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td id="id_summary_intermed">
                                            <loc:Text ResourceId="MC_MSG_Info" runat="server" />
                                        </td>
                                    </tr>
                                    <tr height="20">
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <tr valign="top">
                                        <td id="id_summarys_bottom">
                                            <loc:Text ResourceId="MC_MSG_CreateMC" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </span>
            </td>
        </tr>
        <tr height="47">
            <td style="border-top: 1px solid #CCCCCC">
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tr class="MiniCampaign_tr_MCButtons" valign="center">
                        <td class="ms-crm-Dialog-Buttons MiniCampaign_td_Buttons">
                            <cui:Button ID="btn_id_Back" Disabled="true" OnClick="btnBack();" ResourceId="MC_Button_Back"
                                runat="server">
                            </cui:Button>
                            &nbsp;
                            <cui:Button ID="btn_id_Next" Disabled="true" OnClick="btnNext();" ResourceId="MC_Button_Next"
                                runat="server">
                            </cui:Button>
                            &nbsp;
                            <cui:Button ID="btn_id_Cancel" Disabled="true" OnClick="btnCancel();" ResourceId="MC_Button_Cancel"
                                runat="server">
                            </cui:Button>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
