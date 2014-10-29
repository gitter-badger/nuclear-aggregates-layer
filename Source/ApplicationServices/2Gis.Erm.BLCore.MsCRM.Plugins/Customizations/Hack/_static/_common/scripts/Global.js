//-------------------------------------------------------------------------
// Hotfix History:
// CRM SE 6342: V4: NavBarItem button opens URLs passing parameters even if PassParams=0, some websites cannot handle the invalid passed parameters
// doruro 02/01/2008
//-------------------------------------------------------------------------
// CRM SE 8150: V4: Elevation of Privilege & Man in the middle  cause of credentials for users'  email account flowing in clear text over the wire
// Chandraa 05/14/2009
//-------------------------------------------------------------------------
// CRM SE 10798: V4: Note information is not persisted to entity form when associated record is updated via lookup field
// Chandraa 1/12/2010
//-------------------------------------------------------------------------
// CRM SE 14373: V4: authentication info is lost due to KB831678
// Chandraa 07/08/2010
//-------------------------------------------------------------------------
//	platform object type consts -- SEE UTIL.CS
var None = 0;
var Account = 1;
var ActivityMimeAttachment = 1001;
var ActivityParty = 135;
var ActivityPointer = 4200;
var Annotation = 5;
var AnnualFiscalCalendar = 2000;
var Appointment = 4201;
var AsyncOperation = 4700;
var AttributeMap = 4601;
var BulkDeleteFailure = 4425;
var BulkDeleteOperation = 4424;
var BulkOperation = 4406;
var BulkOperationLog = 4405;
var BusinessUnit = 10;
var BusinessUnitNewsArticle = 132;
var Calendar = 4003;
var CalendarRule = 4004;
var Campaign = 4400;
var CampaignActivity = 4402;
var CampaignResponse = 4401;
var Competitor = 123;
// LATER: (TobinZ, 2007-09-05) - Rename ResourceGroup to ConstraintBasedGroup to be consistant with metadata.
var ResourceGroup = 4007;
var Contact = 2;
var Contract = 1010;
var ContractDetail = 1011;
var ContractTemplate = 2011;
var CustomerAddress = 1071;
var CustomerOpportunityRole = 4503;
var CustomerRelationship = 4502;
var Discount = 1013;
var DiscountType = 1080;
var DisplayString = 4102;
var DocumentIndex = 126;
var DuplicateRule = 4414;
var Email = 4202;
var Equipment = 4000;
var Fax = 4204;
var FixedMonthlyFiscalCalendar = 2004;
var Import = 4410;
var ImportFile = 4412;
var ImportJob = 9107;
var ImportLog = 4423;
var ImportMap = 4411;
var Incident = 112;
var IncidentResolution = 4206;
var Invoice = 1090;
var InvoiceDetail = 1091;
var KbArticle = 127;
var KbArticleComment = 1082;
var KbArticleTemplate = 1016;
var Lead = 4;
var Letter = 4207;
var List = 4300;
var ListMember = 4301;
var MailMergeTemplate = 9106;
var MonthlyFiscalCalendar = 2003;
var Opportunity = 3;
var OpportunityClose = 4208;
var OpportunityProduct = 1083;
var OrderClose = 4209;
var Organization = 1019;
var PhoneCall = 4210;
var PriceLevel = 1022;
var Privilege = 1023;
var Product = 1024;
var ProductPriceLevel = 1026;
var QuarterlyFiscalCalendar = 2002;
var Queue = 2020;
var QueueItem = 2029;
var Quote = 1084;
var QuoteClose = 4211;
var QuoteDetail = 1085;
var RelationshipRole = 4500;
var RelationshipRoleMap = 4501;
var Report = 9100;
var ResourceSpec = 4006;
var Role = 1036;
var SalesLiterature = 1038;
var SalesLiteratureItem = 1070;
var SalesOrder = 1088;
var SalesOrderDetail = 1089;
var SavedQuery = 1039;
var SemiAnnualFiscalCalendar = 2001;
var Service = 4001;
var ServiceAppointment = 4214;
var Site = 4009;
var Subject = 129;
var SystemUser = 8;
var Task = 4212;
var Team = 9;
var Template = 2010;
var Territory = 2013;
var TransactionCurrency = 9105;
var UnresolvedAddress = 2012;
var UoM = 1055;
var UoMSchedule = 1056;
var UserFiscalCalendar = 1086;
var UserQuery = 4230;
var UserSettings = 150;
var WFProcess = 121;
var WFProcessInstance = 122;
var WFActionLog = 1062;
var Workflow = 4703;
// Custom Dialogs
var ServiceRestrictionCalendarRule = 5003;
var TimeOffCalendarRule = 5004;
var HolidayCalendarRule = 5005;
var OccurrenceCalendarRule = 5006;
var RecurrenceCalendarRule = 5007;
var OccurringWorkShift = 5008;
var RecurringWorkShift = 5009;
var NotWorkingWorkShift = 5010;
var ActivityScheduling = 5011;
// 5100-5999 reserved for use by HRM Team
//	App Objects type consts -- SEE UTIL.CS
var AppLicense = 9002;
var AppSalesPerson = 9003;
var AppOfflineFilter = 9004;
var AppWorkflowInstance = 9005;
var ReportPropertyDialog = 9099;
var ScriptErrorDialog = 9200;
var ScriptErrorDetailsDialog = 9201;
var UnresolvedEmailParty = 9206;
var AccountAddress = 1000;
var ContactAddress = 1008;
var BaseAddEntityObjectTypeCode = 10000;
// Field Consts
var FIELD_NOT_REQUIRED = 0;
var FIELD_RECOMMENDED = 1;
var FIELD_REQUIRED = 2;
// Error Consts
var ERROR_STOP = 0;
var ERROR_NONE = 1;
var ERROR_CONTINUE = 2;
var DialogRetryReturnValue = "___RETRY___";
function document.onkeypress()
{
    var event = window.event;
    if (event.ctrlKey && event.keyCode == 80)
    {
        event.keyCode = 0;
        return false;
    }
}
document.onkeydown = document.onkeypress;
/// <summary>
/// Trap all script errors
/// </summary>
/// <param name="sMsg">Error Message</param>
/// <param name="sUrl">Page on which error occurred</param>
/// <param name="sLn">Line number of error</param>
function window.onerror(sMsg, sUrl, sLn)
{
    try
    {
        // Check whether user may want to report this error and whether we should catch errors
        if ((IsNull(window.SUPPRESS_SCRIPT_ERRORS) || window.SUPPRESS_SCRIPT_ERRORS != true) && (1 == ScriptErrorReportingPreference || 2 == ScriptErrorReportingPreference))
        {
            // Set window's status bar
            window.status = ScriptErrorStatusBar;
            // Generate call stack if available
            var callStack = "";
            var func = "";
            if (null != arguments.caller)
            {
                var clr = arguments.caller.callee;
                var depth = 0;
                while (clr != null && depth < 20)
                {
                    // Convert the function name to a string so we can parse it.
                    var sClr = clr.toString();
                    // Get rid of function body
                    var funcName = sClr.substring(0, sClr.indexOf("{"));
                    // Now funcName is something similar to "function foo(x,y,z)\r\n"
                    // Get rid of "function" and trim.
                    funcName = Trim(funcName.substring(9));
                    // For anonymous functions, include the whole function.
                    if (funcName === "anonymous()")
                    {
                        funcName = sClr;
                    }
                    // If we are at depth 0, this is the function that the error occurred in.
                    if (depth == 0)
                    {
                        func = funcName;
                    }
                    // Add function to callStack string
                    callStack += "	  <Function>" + CrmEncodeDecode.CrmXmlEncode(funcName) + "</Function>\r\n";
                    // Advance to next function in stack
                    clr = clr.caller;
                    depth++;
                }
            }
            // Gather crash data.
            var oCrashData = CollectCrashData(sMsg, sUrl, sLn, func, callStack);
            if (1 == ScriptErrorReportingPreference) // Prompt to report on page unload
            {
                // Save crash data on page for use on page unload
                StoreCrashData(oCrashData);
            }
            else // Automatically report now
            {
                // Report the error synchronously, without adding extra server information
                ReportScriptError(oCrashData, true, false);
                // Clear the error from window's status bar.
                window.status = "";
            }
            // Return true to suppress IE's default handling of errors.
            return true;
        }
    }
    catch (e)
    {
    }
    // Return false to pass on error handling to IE
    return false;
}
/// <summary>
/// Window.onbeforeunload is called as a user navigates away from a particular page.
/// Test to see if any errors were saved on this page, if so report them.
/// </summary>
window.attachEvent("onbeforeunload", PromptAndReportAnyErrors);
function document.oncontextmenu()
{
    var s = event.srcElement.tagName;
    // Only allow context menus if:
    // the element is not disabled AND
    // the element is either a TextArea OR a TextBox OR a user selection in some TextBox/TextArea
    event.returnValue =
(!event.srcElement.disabled &&
(document.selection.createRange().text.length > 0 ||
s == "TEXTAREA" ||
s == "INPUT" && event.srcElement.type == "text"));
}
function document.onselectstart()
{
    var s = event.srcElement.tagName;
    if (s != "INPUT" && s != "TEXTAREA" && !(s == "DIV" && event.srcElement.className == "ms-crm-Lookup")) event.returnValue = false;
}
var GlobalAllowDrag = false;
function document.ondragstart()
{
    event.returnValue = GlobalAllowDrag;
}
// Defines a JS variable by name.
function defineVar(key, value)
{
    if (typeof (value) === "string")
    {
        eval(key + ' = "' + value + '"');
    }
    else
    {
        eval(key + " = " + value);
    }
}
//This variable is set as a window is about to open, then can be checked in the window.onload (or similar).  Don't use it in any
// other event handlers as it is global and may have changed due to other windows opening.
// Also, there is a chance that two separate clicks could cause windows to open and the first value set gets overriden
// by the second click.  This would be a rare occurance, but useage of this variable should take this into account.
var _bWindowSizeAdjusted = false;
// Given a screen width will adjust it to fit in the current screen.  If it is empty it will default it to the application default.
function getAdjustedWidthForScreen(iX)
{
    var iWidth = iX;
    if (IsNull(iX) || iX == 0)
    {
        iX = (screen.availWidth >= 1000) ? 820 : 750;
    }
    iX = (iX >= screen.availWidth) ? screen.availWidth : iX;
    _bWindowSizeAdjusted = (iX != iWidth);
    return iX;
}
// Given a screen height will adjust it to fit in the current screen.  If it is empty it will default it to the application default.
function getAdjustedHeightForScreen(iY)
{
    if (IsNull(iY) || iY == 0)
    {
        return (screen.availHeight >= 600) ? 560 : 510;
    }
    else
    {
        return (iY >= screen.availHeight) ? screen.availHeight : iY;
    }
}
// Calculate the left and top margin to place the form in center.
function getLeftMargin(iX)
{
    var iLeft = 0;
    if ((screen.availWidth - iX) > 0)
    {
        iLeft = (screen.availWidth - iX) / 2;
    }
    return iLeft;
}
function getTopMargin(iY)
{
    var iTop = 0;
    if ((screen.availHeight - iY) > 0)
    {
        iTop = (screen.availHeight - iY) / 2;
    }
    return iTop;
}
function appendOrgName(sUrl)
{
    var sNewUrl = sUrl;
    if (IS_PATHBASEDURLS)
    {
        if (sUrl.substring(sUrl.length - 1) == "/")
        {
            sNewUrl = sUrl + ORG_UNIQUE_NAME;
        }
        else
        {
            sNewUrl = sUrl + "/" + ORG_UNIQUE_NAME;
        }
    }
    return sNewUrl;
}
function prependOrgName(sUrl)
{
    var sNewUrl = sUrl;
    if (IS_PATHBASEDURLS && ORG_UNIQUE_NAME.length > 0)
    {
        sNewUrl = "/" + ORG_UNIQUE_NAME + sUrl;
    }
    return sNewUrl;
}
function stripOrgName(sUrl)
{
    stripOrgNameWithForce(sUrl, false);
}
function stripOrgNameWithForce(sUrl, bForce)
{
    var sNewUrl = sUrl;
    if (IS_PATHBASEDURLS || bForce && ORG_UNIQUE_NAME.length > 0)
    {
        if (sNewUrl.substr(0, 1) != "/")
        {
            sNewUrl = "/" + sNewUrl;
        }

        sNewUrl = sNewUrl.replace("/" + ORG_UNIQUE_NAME, "");
    }

    return sNewUrl;
}
// The following functions do try/catch, and display the popup blocker error message as an alert when the window fails to open.
function safeWindowOpen(sUrl, sName, sFeatures, bReplace)
{
    var windowOpened = null;
    try
    {
        // CRM SE 14373 - Open the window using the opener passed in if present.
        // This will be the case only for Modal Dialogs and if the client var CHANGE_WINDOW_OPENER
        // is set to 1 which will be controlled by Registry Key ChangeWindowOpener.
        if (!IsNull(CHANGE_WINDOW_OPENER) &&
CHANGE_WINDOW_OPENER == 1 &&
typeof (dialogArguments) != "undefined" && !IsNull(dialogArguments) &&
typeof (dialogArguments.openerWindow) != "undefined" && !IsNull(dialogArguments.openerWindow))
        {
            windowOpened = dialogArguments.openerWindow.open(sUrl, sName, sFeatures, bReplace);
            windowOpened.opener = window;
        }
        else
            windowOpened = window.open(sUrl, sName, sFeatures, bReplace);
    }
    catch (e)
    {
    }
    if (IsNull(windowOpened))
    {
        handlePopupBlockerError();
    }
    return windowOpened;
}
// ---------------------------------------------------------------------------------------------
// Отображение модального диалога с View возвращаемым в общем случае с другого домена.
// Назначение - обход ограничение same origin policy на доступ к настройкам диалога: dialogArguments  и т.п. (Если вызывающая страница, и отбражаемая в диалоговом окне имеют разные origin). 
function showCrossDomainModalDialog(sTargetUrl, vArguments, sOptions, onCloseHandler)
{
    var _dialogUrl = sTargetUrl;
	var _dialogArguments = vArguments === null ? "" : vArguments;
	var _dialogOptions = sOptions;
	var _dialogOnCloseHandler = onCloseHandler;
    
    var parsedUrl = parseURL(sTargetUrl);
    var _dialogProxyUrl = parsedUrl.protocol+'://'+parsedUrl.host+(parsedUrl.port ? ':'+parsedUrl.port: '') + "/Content/dialogProxy.html";

    ensureJSONSerializerAvailability();
    attachProxyListener();
    var _proxyDialogFrame = attachProxyFrame();

	function onProxyDialogMessageHandler(e)
	{
		var message = deserialize(e.data);
        if (message.Type == 'ready') {
			onProxyDialogReady();
		} else if (message.Type == 'dialogClosed') {
			onProxyDialogClosed(message.Value);
		}
	}

	function onProxyDialogReady()
	{
		var message = {
			Type: "dialogSettings",
			Value: {
				Url: _dialogUrl,
				Args: _dialogArguments,
				Options: _dialogOptions}
		};

        var strMessage = serialize(message);
		_proxyDialogFrame.contentWindow.postMessage(strMessage, "*");
	}

	function onProxyDialogClosed(result)
	{
		detachProxyListener();
		deattachProxyFrame();
		_dialogOnCloseHandler(result);
	}

	function serialize(data)
	{
        return JSON.stringify(data); 
	}

	function deserialize(sData)
	{
        return JSON.parse(sData);
	}

	function attachProxyListener()
	{
		if (window.addEventListener) {
		  window.addEventListener('message', onProxyDialogMessageHandler, false);
		}
		else { // IE8 or earlier
		  window.attachEvent('onmessage', onProxyDialogMessageHandler, false);
		}
	}

	function detachProxyListener()
	{
		if (window.addEventListener) {
		  window.removeEventListener('message', onProxyDialogMessageHandler, false);
		}
		else { // IE8 or earlier
		  window.detachEvent ('onmessage', onProxyDialogMessageHandler, false);
		}
	}

	function attachProxyFrame()
	{
		// Add the iframe with a unique name
	  var iframe = document.createElement("iframe");
	  var uID = guid();
	  document.body.appendChild(iframe);
	  iframe.style.display = "none";
	  iframe.contentWindow.name = uID;
	  iframe.src = _dialogProxyUrl;
	  return iframe;
	}

	function deattachProxyFrame()
	{
        document.body.removeChild(_proxyDialogFrame);
	}

    function ensureJSONSerializerAvailability()
    {
        if (!window.JSON) {
          window.JSON = {
            parse: function (sJSON) { return eval("(" + sJSON + ")"); },
            stringify: function (vContent) {
              if (vContent instanceof Object) {
                var sOutput = "";
                if (vContent.constructor === Array) {
                  for (var nId = 0; nId < vContent.length; sOutput += this.stringify(vContent[nId]) + ",", nId++);
                    return "[" + sOutput.substr(0, sOutput.length - 1) + "]";
                }
                if (vContent.toString !== Object.prototype.toString) { 
                  return "\"" + vContent.toString().replace(/"/g, "\\$&") + "\"";
                }
                for (var sProp in vContent) { 
                  sOutput += "\"" + sProp.replace(/"/g, "\\$&") + "\":" + this.stringify(vContent[sProp]) + ",";
                }
                return "{" + sOutput.substr(0, sOutput.length - 1) + "}";
             }
             return typeof vContent === "string" ? "\"" + vContent.replace(/"/g, "\\$&") + "\"" : String(vContent);
            }
          };
        }
    }

    function parseURL(url) {
        var a =  document.createElement('a');
        a.href = url;
        return {
            source: url,
            protocol: a.protocol.replace(':',''),
            host: a.hostname,
            port: a.port,
            query: a.search,
            params: (function(){
                var ret = {},
                    seg = a.search.replace(/^\?/,'').split('&'),
                    len = seg.length, i = 0, s;
                for (;i<len;i++) {
                    if (!seg[i]) { continue; }
                    s = seg[i].split('=');
                    ret[s[0]] = s[1];
                }
                return ret;
            })(),
            file: (a.pathname.match(/\/([^\/?#]+)$/i) || [,''])[1],
            hash: a.hash.replace('#',''),
            path: a.pathname.replace(/^([^\/])/,'/$1'),
            relative: (a.href.match(/tps?:\/\/[^\/]+(.+)/) || [,''])[1],
            segments: a.pathname.replace(/^\//,'').split('/')
        };
    }

    function guid() {
      function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
                   .toString(16)
                   .substring(1);
      }

      return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
               s4() + '-' + s4() + s4() + s4();
  }
}
//---------------------------------------------------------------------------------------------------
function safeWindowShowModalDialog(sUrl, vArguments, sFeatures)
{
    // Make sure the dialog arguments are set.
    if (vArguments === null)
    {
        vArguments = "";
    }
    var returnVar = null;
    try
    {
        do
        {
            vArguments.openerWindow = window;
            returnVar = window.showModalDialog(sUrl, vArguments, sFeatures);
        }
        while (returnVar === DialogRetryReturnValue);
    }
    catch (e)
    {
        handlePopupBlockerError();
    }
    return returnVar;
}
function safeWindowShowModelessDialog(sUrl, vArguments, sFeatures)
{
    var windowOpened = null;
    try
    {
        windowOpened = window.showModelessDialog(sUrl, vArguments, sFeatures);
    }
    catch (e)
    {
    }
    if (IsNull(windowOpened))
    {
        handlePopupBlockerError();
    }
    return windowOpened;
}
function openErrorDlg(sErrorCode, sMessage, iWidth, iHeight)
{
    if (IsNull(iWidth))
    {
        var iWidth = parseInt(LOCID_ERROR_DIALOG_WIDTH, 10);
    }

    if (IsNull(iHeight))
    {
        var iHeight = parseInt(LOCID_ERROR_DIALOG_HEIGHT, 10);
    }
    // Clear the error message if it's not an ISV error.
    if (sErrorCode !== _ErrorCodesIsvAborted)
    {
        sMessage = "";
    }
    var sb = new StringBuilder();
    sb.Append("/_common/error/dlg_error.aspx?hresult=");
    sb.Append(!IsNull(sErrorCode) ? CrmEncodeDecode.CrmUrlEncode(sErrorCode) : "");
    sb.Append("&errMessage=");
    sb.Append(!IsNull(sMessage) ? CrmEncodeDecode.CrmUrlEncode(sMessage) : "");

    var sUrl = prependOrgName(sb.ToString());
    return openStdDlg(sUrl, null, iWidth, iHeight);
}
function openStdWin(sPath, sName, iX, iY, sCustomWinFeatures)
{
    iX = getAdjustedWidthForScreen(iX);
    iY = getAdjustedHeightForScreen(iY);
    // Get the left and top margin.
    var iLeft = getLeftMargin(iX);
    var iTop = getTopMargin(iY);
    return safeWindowOpen(sPath, sName, "width=" + iX + ",height=" + iY + ",status=1,resizable=1,left=" + iLeft + ",top=" + iTop + (IsNull(sCustomWinFeatures) ? "" : "," + sCustomWinFeatures));
}
function openStdDlg(sPath, oArgs, iWidth, iHeight, bResizable, bModeless, sCustomWinParams)
{
    if (IsNull(sCustomWinParams))
    {
        sCustomWinParams = "";
    }

    // 	By default all modal dialog is resizable
    if (IsNull(bResizable))
    {
        bResizable = true;
    }
    // Take into consideration any global increases in dialog size.  Used by localization teams.
    // Only use these increased sizes if the dialog is already resizeable.  If it is not resizeable then the increased sizes will
    // most likely not help much because the dialog probably has a fixed layout.
    if (bResizable)
    {
        iWidth += parseInt(LOCID_DIALOG_OFFSET_WIDTH, 10);
        iHeight += parseInt(LOCID_DIALOG_OFFSET_HEIGHT, 10);
    }
    // Adjust height for screen size.
    iWidth = getAdjustedWidthForScreen(iWidth);
    iHeight = getAdjustedHeightForScreen(iHeight);
    bResizable = (!IsNull(bResizable) && bResizable) ? "yes" : "no";
    // Get the left and top margin.
    var iLeft = getLeftMargin(iWidth);
    var iTop = getTopMargin(iHeight);
    if (LOCID_UI_DIR == "RTL")
        iLeft = screen.availWidth - iLeft - iWidth;
    sCustomWinParams = "dialogWidth:" + iWidth + "px;dialogHeight:" + iHeight + "px;dialogLeft=" + iLeft + "px;dialogTop=" + iTop + "px;help:0;status:1;scroll:0;center:1;resizable:" + bResizable + ";" + sCustomWinParams;
    if (IsNull(bModeless) || !bModeless)
    {
        return safeWindowShowModalDialog(sPath, oArgs, sCustomWinParams);
    }
    else
    {
        return safeWindowShowModelessDialog(sPath, oArgs, sCustomWinParams);
    }
}
function openIsvWin(sPath, bPassParams, sMode, sParams, oDialogArgs)
{
    if (bPassParams == true)
    {
        sPath += sPath.indexOf("?") < 0 ? "?" : "&";
        sPath += "orgname=" + CrmEncodeDecode.CrmUrlEncode(ORG_UNIQUE_NAME);
        sPath += "&userlcid=" + CrmEncodeDecode.CrmUrlEncode(USER_LANGUAGE_CODE);
        sPath += "&orglcid=" + CrmEncodeDecode.CrmUrlEncode(ORG_LANGUAGE_CODE);

        if (typeof (crmFormSubmit) != "undefined" &&
!IsNull(crmFormSubmit))
        {
            sPath += "&type=" + CrmEncodeDecode.CrmUrlEncode(crmFormSubmit.crmFormSubmitObjectType.value);
            sPath += "&typename=" + CrmEncodeDecode.CrmUrlEncode(crmFormSubmit.crmFormSubmitObjectTypeName.value);
            sPath += "&id=" + CrmEncodeDecode.CrmUrlEncode(crmFormSubmit.crmFormSubmitId.value);
        }
    }
    switch (sMode)
    {
        case -1:
            var o = event.srcElement;
            // Find the A tag with the area id
            while (!IsNull(o) && o.tagName != "A")
            {
                o = o.parentElement;
            }
            // Detail Form Level... requires details.js
            // CRM SE #6342 - Pass the 4th parameter
            loadArea(o.id + "Area", "", sPath, true, bPassParams);
            // End CRM SE  #6342
            break;
        case "1": safeWindowShowModalDialog(sPath, oDialogArgs, sParams); break;
        case "2": safeWindowShowModelessDialog(sPath, oDialogArgs, sParams); break;
        default: safeWindowOpen(sPath, buildWinName(), sParams); break;
    }
}
// This function now returns true or false - false if it did not open anything, true if it did.
function openFrmObj(sUrl, sName, iType, sUrlPrefix, bNewWindow)
{
    if (IsNull(bNewWindow))
    {
        bNewWindow = true;
    }

    // There are a certain number of objects that do not have forms, we should allow list these here
    switch (eval(iType))
    {
        case AppLicense:
        case None:
        case ImportLog:
        case BulkOperationLog:
        case BulkDeleteFailure:
            // don't open anything
            return false;

        case Report:
            if (IsNull(sUrl) || sUrl.length == 0)
            {
                // "New" opens up properties dialog instead of viewer
                iType = ReportPropertyDialog;
            }
            break;
    }
    // Get the window URL and sizes for this object type.  GetWindowInformation is an auto generated function pulled from WindowInformation.aspx.cs
    // When you create a new entity or need to adjust an entity window size, please update the resource in WindowSizes.resx for the entity
    // window url that needs changing.
    var oWindowInfo = GetWindowInformation(iType);
    var url = oWindowInfo.Url;
    var iX = oWindowInfo.Width;
    var iY = oWindowInfo.Height;
    var serverUrl = oWindowInfo.ServerUrl;
    // Check to see if anything special needs to happen for this object type
    if (!IsNull(url))
    {
        url = prependOrgName("/" + url + sUrl);
        if (!IsNull(serverUrl) && iType == Queue)
        {
            url = serverUrl + url;
        }
        else if (!IsNull(sUrlPrefix))
        {
            url = sUrlPrefix + url;
        }

        // check if user is creating a new instance of the entity
        var sId = null;
        if (!IsNull(sUrl) && sUrl.length > 0)
        {
            sId = ParseQueryString(sUrl)["id"];
        }
        switch (Number(iType))
        {
            case KbArticle:

                if (IsNull(sId))
                {
                    // user is trying to create a new KbArticle
                    // we need to open the dialog where the user needs to select the template and language first. 
                    url = prependOrgName("/cs/articles/lookup_template.aspx");
                }

                break;

            case DocumentIndex:
                sName += "Viewer";
                break;
            case DiscountType:
                if (IsNull(sUrl) || sUrl.length == 0)
                {
                    url = openStdDlg(prependOrgName("/Tools/ProductCatalog/dlg_create.aspx?ObjType=" + DiscountType), null, 350, 250);
                    if (IsNull(url))
                    {
                        return false;
                    }
                }
                break;
            case UoMSchedule:
                if (IsNull(sUrl) || sUrl.length == 0)
                {
                    url = openStdDlg(prependOrgName("/Tools/ProductCatalog/dlg_create.aspx?ObjType=" + UoMSchedule), null, 400, 240);
                    if (IsNull(url))
                    {
                        return false;
                    }
                }
                break;
            case KbArticleComment:
                return (!IsNull(openStdDlg(url, top.window, iX, iY)));
            case Subject:
                return (!IsNull(openStdDlg(prependOrgName("/Tools/SubjectManager/dialogs/edit.aspx" + sUrl + "&readOnly=1"), null, iX, iY)));
            case ServiceRestrictionCalendarRule:
                return (!IsNull(openStdDlg(url, top.window, iX, iY)));
            case TimeOffCalendarRule:
                return (!IsNull(openStdDlg(url + '&calendarId=' + calendarId.value + '&resourceId=' + oId.value + '&name=Time off&mode=Edit', top.window, iX, iY)));
            case OccurrenceCalendarRule:
            case RecurrenceCalendarRule:
                return (!IsNull(openStdDlg(url + '&calendarId=' + calendarId.value + '&innerCalendarId=' + oId.value + '&name=Working Hours&mode=Edit', top.window, iX, iY)));
            case HolidayCalendarRule:
                if (openStdDlg(url + '&mode=Edit', top.window, iX, iY))
                {
                    auto(CalendarRule);
                    return true;
                }
                return false;
            case OccurringWorkShift:
                // Getting the innerHTML of the grid row start time is hacky, but given where we are in the product this compromise is okay. Please see MSCRM #11713 for more details
                return (!IsNull(openStdDlg(url + '&calendarId=' + calendarId.value + '&resourceId=' + oId.value + '&selecteddates=' + crmGrid.InnerGrid.SelectedRecords[0][3].cells[crmGrid.InnerGrid.FindColumnIndex("starttime")].firstChild.innerHTML + '&mode=' + mode.value, null, iX, iY)));
            case RecurringWorkShift:
            case NotWorkingWorkShift:
                url += '&calendarId=' + calendarId.value + '&resourceId=' + oId.value + '&name=Working Hours&mode=' + mode.value;
                break;
            case Calendar:
                url += '&name=Availability Template&mode=Edit';
                sName = 'Calendar';
                break;
            case UserQuery:
                // Nothing gets opened for this
                return false;
            case AppOfflineFilter:
                if (sName && sName.length == 32)	// Open existing query, sName is GUID, otherwise it's time from buildWinName()
                {
                    url += '&QueryId=' + sName;
                }
                var oRetVal = openStdDlg(url, top.window, iX, iY);
                auto(UserQuery); // refresh grid
                return !IsNull(oRetVal);
            case Report:
                // sName contains path to file and breaks window.open, so we buildWinName with time only
                sName = buildWinName();
                break;
            case Workflow:
                // If user click new workflow icon, sUrl is empty, new workflow create wizard will get launched.
                // If user click a grid cell, with a existing workflow regard, it will by-pass new grid creattion screen.
                if (!IsNull(sUrl))
                {
                    if (sUrl.length > 1)
                    {
                        break; // edit a existent workflow					
                    }
                } 		  	// provide options to create workflow from template
                var pathLength = url.lastIndexOf("/");
                var workflowPath = "";
                if (pathLength > 0)
                {
                    workflowPath = url.substring(0, pathLength + 1); // get path with "/"	
                }
                var wfTemplateUrl = workflowPath + "workflowTemplate/workflowTemplatePage.aspx";
                var oRetVal = openStdDlg(wfTemplateUrl, sName, 640, 480);
                if (IsNull(oRetVal) || oRetVal.length < 2)
                {
                    return; // cancel clicked				
                }

                if (oRetVal.indexOf("id=") >= 0)
                {
                    url = url + "?" + oRetVal;
                    auto(Workflow); // refresh grid
                    break;
                }
                return;
                // continue open window

            case Contract:
                if (IsNull(sUrl) || sUrl.length == 0)
                {
                    // If not parameters then launch template picker.				
                    return (!IsNull(openStdDlg(prependOrgName("/cs/contracts/lookup_template.aspx"), window, parseInt(LOCID_CONTRACTS_TDIALOG_WIDTH, 10), parseInt(LOCID_CONTRACTS_TDIALOG_HEIGHT, 10))));
                }
                break;

            default:
                // ADD ENTITY
                if (IsUserDefinedEntityObjectTypeCode(iType))
                {
                    url += ((sUrl == "") ? "?" : "&") + "etc=" + iType;
                }
                break;
        }
        // Open up window with localized size
        if (bNewWindow)
        {
            return (!IsNull(openStdWin(url, sName, iX, iY)));
        }
        else
        {
            openInPlace(url, iX, iY);
        }
    }
    return false;
}
function openInPlace(sUrl, iX, iY)
{
    iX = getAdjustedWidthForScreen(iX);
    iY = getAdjustedHeightForScreen(iY);
    // Get the left and top margin.
    var iLeft = getLeftMargin(iX);
    var iTop = getTopMargin(iY);

    // Resize and reposition window then open in place
    window.moveTo(iLeft, iTop);
    window.resizeTo(iX, iY);
    window.location = sUrl;
}
//sParams: QueryString parameters, does not start with '?' or '&' (e.g. "foo=bar&who=ha")
/*CRM Hack*/
window.CrmUrlToErmUrlTemplateMapping =
[
    // production environment
    { CrmUrl: "http://uk-crm01/DoubleGis", ErmUrlTemplate: "//web-app.prod.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app.prod.erm.2gis.ru/" },

    // test environments
    { CrmUrl: "http://uk-erm-edu01/DoubleGis", ErmUrlTemplate: "//web-app.edu.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app.edu.erm.2gis.ru/" },
    { CrmUrl: "http://uk-erm-edu02/DoubleGis", ErmUrlTemplate: "//web-app.business.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app.business.erm.2gis.ru/" },
    { CrmUrl: "http://uk-crm10/DoubleGis", ErmUrlTemplate: "//web-app.load.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app.load.erm.2gis.ru/" },
    { CrmUrl: "http://uk-test-int02/DoubleGis", ErmUrlTemplate: "//web-app.int.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app.int.erm.2gis.ru/" },
    { CrmUrl: "http://uk-erm-test01/DoubleGis01", ErmUrlTemplate: "//web-app01.test.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app01.test.erm.2gis.ru/" },
    { CrmUrl: "http://uk-erm-test02/DoubleGis02", ErmUrlTemplate: "//web-app02.test.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app02.test.erm.2gis.ru/" },
    { CrmUrl: "http://uk-erm-test03/DoubleGis03", ErmUrlTemplate: "//web-app03.test.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app03.test.erm.2gis.ru/" },
    { CrmUrl: "http://uk-erm-test04/DoubleGis04", ErmUrlTemplate: "//web-app04.test.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app04.test.erm.2gis.ru/" },
    { CrmUrl: "http://uk-erm-test05/DoubleGis05", ErmUrlTemplate: "//web-app05.test.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app05.test.erm.2gis.ru/" },
    { CrmUrl: "http://uk-erm-test06/DoubleGis06", ErmUrlTemplate: "//web-app06.test.erm.2gis.ru/Crm/CreateOrUpdate/{0}?crmId={1}", ErmUrl: "//web-app06.test.erm.2gis.ru/" }
];

window.CrmEntityMapping =
[
    { ObjectTypeCode: "1", EntityName: "Client" },
	{ ObjectTypeCode: "2", EntityName: "Contact" },
	{ ObjectTypeCode: "3", EntityName: "Deal" },
    { ObjectTypeCode: "10002", EntityName: "Currency" },
    { ObjectTypeCode: "10004", EntityName: "Territory" },
    { ObjectTypeCode: "10005", EntityName: "Position" },
    { ObjectTypeCode: "10006", EntityName: "Limit" },
    { ObjectTypeCode: "10007", EntityName: "FirmAddress" },
    { ObjectTypeCode: "10008", EntityName: "Category" },
    { ObjectTypeCode: "10009", EntityName: "BranchOfficeOrganizationUnit" },
    { ObjectTypeCode: "10010", EntityName: "Bargain" },
    { ObjectTypeCode: "10011", EntityName: "LegalPerson" },
    { ObjectTypeCode: "10013", EntityName: "Firm" },
    { ObjectTypeCode: "10014", EntityName: "Order" },
    { ObjectTypeCode: "10015", EntityName: "OrderPosition" },
    { ObjectTypeCode: "10016", EntityName: "BranchOffice" },
    { ObjectTypeCode: "10017", EntityName: "OrganizationUnit" },
	{ ObjectTypeCode: "10023", EntityName: "Account" },
    { ObjectTypeCode: "10026", EntityName: "OrderProcessingRequest" },
    { ObjectTypeCode: Appointment, EntityName: "Appointment" },
    { ObjectTypeCode: Letter, EntityName: "Letter" },
    { ObjectTypeCode: PhoneCall, EntityName: "Phonecall" },
    { ObjectTypeCode: Task, EntityName: "Task" }
];

function GetErmWebAppUrl() {
    for (var i = 0; i < CrmUrlToErmUrlTemplateMapping.length; i++) {
        if (CrmUrlToErmUrlTemplateMapping[i].CrmUrl == SERVER_URL)
            return CrmUrlToErmUrlTemplateMapping[i].ErmUrl;
    }

    return null;
}

function GetErmUrlTemplate() {
    for (var i = 0; i < CrmUrlToErmUrlTemplateMapping.length; i++) {
        if (CrmUrlToErmUrlTemplateMapping[i].CrmUrl == SERVER_URL)
            return CrmUrlToErmUrlTemplateMapping[i].ErmUrlTemplate;
    }

    return null;
}

function GetErmEntityName(objectTypeCode) {

    for (var i = 0; i < CrmEntityMapping.length; i++) {
        if (CrmEntityMapping[i].ObjectTypeCode == objectTypeCode)
            return CrmEntityMapping[i].EntityName;
    }

    return null;
}

function GetErmUrl(iType, sId){

    var ermEntityName = GetErmEntityName(iType);
    if (ermEntityName == null){
        return null;
    }

    var ermUrlTemplate = GetErmUrlTemplate();
    if (ermUrlTemplate == null){
        return null;
    }

    var ermUrl = ermUrlTemplate.replace("{0}", ermEntityName).replace("{1}", sId)
    return ermUrl;
}

/*End CRM Hack*/
function openObj(iType, sId, sParams, sUrlPrefix, bNewWindow)
{
    /*CRM Hack*/
    var ermUrl = GetErmUrl(iType, sId);
    var entityName = GetErmEntityName(iType);
    var rate = 0.7;
    if (entityName == 'Order') {
        rate = 0.9;
    }
    if (ermUrl != null) {
        var params = "width=" + (window.screen.width * rate) + ",height=" + (window.screen.height * rate) + ",status=yes,resizable=yes";
        window.open(ermUrl, "_blank", params);
        return;
    }
    /*End CRM Hack*/

    try
    {
        var sUrl = "";
        if (sId)
        {
            // If the user is trying to open a saved query, check if it is customizable.
            if (iType == SavedQuery)
            {
                var oCommand = new RemoteCommand("SystemCustomization", "CheckSavedQuery");
                oCommand.SetParameter("savedQueryId", sId);
                var oResult = oCommand.Execute();
                if (!oResult.Success)
                {
                    return false;
                }
            }
            sUrl += "?id=" + sId;
        }

        if (!IsNull(sParams))
        {
            sUrl += (sUrl.length == 0) ? "?" : "&";
            // Remove leading question mark from sParams if there is one.
            if (sParams.length > 0 && (sParams.charAt(0) == '?'))
            {
                sParams = sParams.substr(1, sParams.length - 1);
            }
            sUrl += sParams;
        }
        return openFrmObj(sUrl, buildWinName(sId), iType, sUrlPrefix, bNewWindow);
    }
    catch (e)
    {
        return false;
    }
}
function openObjEx(iType, iParentType, sParentId, sParams, sUrlPrefix)
{
    try
    {
        var url = "";
        if (sParentId)
        {
            url += "?pId=" + sParentId + "&pType=" + iParentType;
        }
        if (typeof (sParams) != "undefined")
        {
            url += sParams;
        }
        return openFrmObj(url, buildWinName(), iType, sUrlPrefix);
    }
    catch (e)
    {
        return false;
    }
}
// Previously this method was used to return a form/dialog URL for a given object type code.  We have moved that logic
// into WindowInformation.aspx to auto generate and cache this at runtime.  This is done to provide localized window
// offsets so that our localizers can adjust these default window sizes at run time.  Please add any new form objects to
// WindowInformation.aspx.
function getObjUrl(iObjectType)
{
    // Get window information, this method throws an alert if it does no exists for the given object type code
    var oWinInfo = GetWindowInformation(iObjectType);
    if (!IsNull(oWinInfo))
    {
        return oWinInfo.Url;
    }
    return null;
}
// Get the current window view url
function GetViewUrl()
{
    var sUrl = "";
    sUrl = window.location.href;

    // if the application is OutlookLaptopClient, we need to do some processing in order to get the correct url
    if (IsOutlookLaptopClient() && !IsOnline())
    {
        // the url will look like
        // http://localhost:2526/myorgname/tools/home_page.aspx
        // we want to remove the server and orgname parts
        var path = stripOrgNameWithForce(sUrl.substr(sUrl.indexOf(window.location.pathname)));
        sUrl = WEB_APP_URL + path;
    }

    // if the current homepage doesnt have parameter info return, return current window url
    if (typeof (_homepageParameterInfo) == 'undefined')
    {
        return sUrl;
    }

    // replace query string with new values
    for (var i = 0; i < _homepageParameterInfo.length; i++)
    {
        var oParamInfo = _homepageParameterInfo[i];
        var el = document.getElementById(oParamInfo.controlId);

        if (!IsNull(el) && !IsNull(el.DataValue))
        {
            sUrl = SetQueryString(sUrl, oParamInfo.parameterName, el.DataValue);
        }
    }
    return sUrl;
}
function CopyTextToClipboard(sText, sSuccessMessage, sFailMessage)
{
    var el = window.clipboardData;
    el.clearData();
    var iRetries = 2;
    var temp = null;
    //try at least twice, sometimes IE acts wierd and does nto copy the first time
    while (iRetries > 0 && temp == null)
    {
        el.setData("Text", sText);
        // make sure that the data is copied over to the clipboard
        temp = el.getData("Text");
        iRetries--;
    }
    if (!IsNull(temp) && temp.length > 0)
    {
        if (sSuccessMessage.length > 0)
            alert(sSuccessMessage);
    }
    else
    {
        if (sFailMessage.length > 0)
            alert(sFailMessage);
    }
}
function OpenEmailForm(sTo, sSubject, sBody)
{
    var sb = new StringBuilder();
    sb.Append("mailto:");
    sb.Append(CrmEncodeDecode.CrmUrlEncode(sTo));
    sb.Append("?");
    if (sSubject.length > 0)
    {
        sb.Append("subject=");
        sb.Append(CrmEncodeDecode.CrmUrlEncode(sSubject));
        sb.Append("&");
    }
    sb.Append("body=");
    sb.Append(CrmEncodeDecode.CrmUrlEncode(sBody));

    var sMailtoLink = sb.ToString();
    if (sMailtoLink.length > 2020)
    {
        // When a mailto link is 2021 or more characters, IE can't handle it
        openErrorDlg("0x80044331");
    }
    else
    {
        window.open(sMailtoLink);
    }
}
// Create a popup and apply global styles to it
function openPopup()
{
    var oPopup = window.createPopup();
    SetDefaultStyles(oPopup);
    return oPopup;
}
function buildWinName(s)
{
    if (s) return s.toLowerCase().replace(/[-\{\}:]/g, "");
    var d = new Date();
    return d.getTime();
}
function handlePopupBlockerError()
{
    alert(LOCID_POPUP_BLOCKER_ERROR + window.location.hostname);
}
function handleXMLErr(xml, bContinue)
{
    if (bContinue == null) bContinue = false;
    if (xml.parseError.errorCode != 0)
    {
        alert(LOCID_XML_PROCESS_ERROR);
        if (!bContinue)
        {
            return ERROR_STOP;
        }
        else
        {
            return ERROR_CONTINUE;
        }
    }
    var node = xml.selectSingleNode("/error");
    if (!IsNull(node))
    {
        if (!bContinue)
        {
            var oError = node.selectSingleNode("code");
            var oDescription = node.selectSingleNode("description");
            openErrorDlg((!IsNull(oError) ? oError.text : ""), (!IsNull(oDescription) ? oDescription.text : ""));
            return ERROR_STOP;
        }
        else
        {
            return ERROR_CONTINUE;
        }
    }
    return ERROR_NONE;
}
function loadHelp(sArea)
{
    var sDefaultHelp, sUrl;
    var x = 0;
    var y = 0;
    var iWidth = 285;
    var s = "";
    if (LOCID_UI_DIR == "RTL")
    {
        x = 3;
    }
    else
    {
        if ((top.screenLeft + top.document.body.clientWidth) < (top.screen.width - iWidth))
        {
            x = top.screenLeft + top.document.body.clientWidth + 3;
        }
        else
        {
            x = top.screen.width - 300;
        }
        // Offset the window positon to handle the wider "Area" pane
        if (sArea)
        {
            x -= 280;
        }
    }

    if ((top.screenTop + top.document.body.clientHeight) < top.screen.height)
    {
        y = top.screenTop - 23;
    }
    else
    {
        y = top.screen.height - 450;
    }
    if (sArea)
    {
        s = sArea;
        iWidth = 565;
    }
    else
    {
        s = stripOrgNameWithForce(window.location.pathname, true);
        try
        {
            if ((!IsOutlookClient() &&
s.toLowerCase().indexOf("/_root/bar_top.aspx") != -1) ||
(window.location.protocol == "outlook:"))
            {
                s = stripOrgNameWithForce(top.stage.location.pathname, true);
            }
        }
        catch (e)
        {
            // There was a problem getting the URL, this could mean an ISV cross-domain site was loaded.
            // we will just pop-up the TOC.
            return loadHelp("##WHATSNEW##");
        }
    }
    if (s.toLowerCase().indexOf("/userdefined/edit.aspx") != -1)
    {
        // do nothing
        // we don't want to distinguish help for custom entities.
    }
    else if (s.toLowerCase().indexOf("/tools/systemcustomization/systemcustomization.aspx") != -1)
    {
        var sPid = GetQueryString(top.stage.location.search, "pid");
        if (!IsNull(sPid))
        {
            if (sPid != "0")
            {
                s += "?pid=" + sPid;
            }
        }
    }
    else if (s.toLowerCase().indexOf("/_root/homepage.aspx") != -1)
    {
        var sEtc = GetQueryString(top.stage.location.search, "etc");
        if (!IsNull(sEtc))
        {
            s += "?etc=" + sEtc;
        }
    }
    else if (
s.toLowerCase().indexOf("/tools/home_tools.aspx") != -1 ||
s.toLowerCase().indexOf("/tools/admin/admin.aspx") != -1 ||
s.toLowerCase().indexOf("/tools/business/business.aspx") != -1 ||
s.toLowerCase().indexOf("/tools/templates/templates.aspx") != -1 ||
s.toLowerCase().indexOf("/tools/datamanagement/datamanagement.aspx") != -1 ||
s.toLowerCase().indexOf("/tools/productcatalog/productcatalog.aspx") != -1 ||
s.toLowerCase().indexOf("/tools/systemcustomization/systemcustomization.aspx") != -1
)
    {
        var sPid = GetQueryString(top.stage.location.search, "pid");
        if (!IsNull(sPid))
        {
            s += "?pid=" + sPid;
        }
    }
    else if (s.toLowerCase().indexOf("/tools/formeditor/formeditor.aspx") != -1)
    {
        //extract the fromtype from the query string
        var sFormType = GetQueryString(top.location.search, "formtype");
        if (!IsNull(sFormType))
        {
            sHelpId = sFormType;
            if (sFormType.length > 0)
            {
                s += "?formtype=" + sFormType;
            }
        }
    }
    else if (s.toLowerCase().indexOf("/tools/regionaloptions/dialogs/regionaloptions.aspx") != -1)
    {
        //extract the usersettings from the query string
        var sUserSettings = GetQueryString(top.location.search, "usersettings");
        if (!IsNull(sUserSettings))
        {
            sHelpId = sUserSettings;
            if (sUserSettings.length > 0 && sUserSettings.toLowerCase() == "true")
            {
                s += "?context=user";
            }
            else
            {
                s += "?context=sys";
            }
        }
    }
    else if (s.toLowerCase().indexOf("/tools/systemcustomization/relationships/managerelationship.aspx") != -1)
    {
        //extract the entityrole from the query string
        var sEntityRole = GetQueryString(top.location.search, "entityrole");
        if (!IsNull(sEntityRole))
        {
            sHelpId = sEntityRole;
            if (sEntityRole.length > 0)
            {
                s += "?entityrole=" + sEntityRole;
            }
        }
    }
    else if (s.toLowerCase().indexOf("/crmreports/viewer/viewer.aspx") != -1)
    {
        //extract the help id from the querystring
        var sHelpId = GetQueryString(top.location.search, "helpID");
        if (!IsNull(sHelpId))
        {
            if (sHelpId.length > 0)
            {
                s += "?helpID=" + sHelpId;
            }
        }
        sDefaultHelp = "/crmreports/viewer/viewer.aspx";
    }
    else
    {
        //default:
        try
        {
            var sHelpContext = getHelpContext();
            if (typeof (sHelpContext) == "string")
            {
                s += "#" + sHelpContext;
            }

        }
        catch (e)
        {
        }
    }
    // Check for homepage, set a default page to custom entities just in case the current entity cannot be found
    if (s.toLowerCase().indexOf("/_root/homepage.aspx") != -1)
    {
        sDefaultHelp = "custom_entity_list";
    }
    sUrl = "/help/default.aspx?area=" + CrmEncodeDecode.CrmUrlEncode(s);
    sUrl += "&user_lcid=" + CrmEncodeDecode.CrmUrlEncode(USER_HELP_LCID);
    if (!IsNull(sDefaultHelp))
    {
        sUrl += "&defaultHelp=" + CrmEncodeDecode.CrmUrlEncode(sDefaultHelp);
    }
    var iY = top.document.body.clientHeight - 5;
    if (iY < 300)
    {
        iY = 300;
    }

    if (HELP_SERVER_URL.length > 0 && IsOnline())
    {
        sUrl = ConcatenateUrl(HELP_SERVER_URL, sUrl);
    }

    // Open the help window, return false if it is popup blocked.
    try
    {
        var sWinDir = "left";
        if (!IsNull(safeWindowOpen(sUrl, "MSCRMHelp", "resizable=1,width=" + iWidth + ",height=" + iY + "," + sWinDir + "=" + x + ",top=" + y)))
        {
            return true;
        }
    }
    catch (e) { }
    return false;
}
//avoiding adding on extra /'s in the URLs
function ConcatenateUrl(s1, s2)
{
    if (IsNull(s1) || s1.length == 0)
    {
        return s2;
    }
    if (IsNull(s2) || s2.length == 0)
    {
        return s1;
    }

    if (s1.charAt(s1.length - 1) != "/" && s2.charAt(0) != "/")
    {
        return s1 + "/" + s2;
    }
    if (s1.charAt(s1.length - 1) == "/" && s2.charAt(0) == "/")
    {
        return s1 + s2.substr(1, s2.length - 1);
    }
    return s1 + s2;
}
function IsActivityTypeCode(iObjTypeCode)
{
    switch (parseInt(iObjTypeCode, 10))
    {
        case ActivityPointer:
        case Email:
        case Fax:
        case PhoneCall:
        case Letter:
        case Appointment:
        case ServiceAppointment:
        case Task:
        case CampaignResponse:
        case CampaignActivity:
        case IncidentResolution:
        case OpportunityClose:
        case OrderClose:
        case QuoteClose:
            return true;
        default: return false;
    }
}
// Auto Grid Refresher
// iObjectTypeCode = TypeCode of the entity that was just created/changed.
// sFindValue = The display name of the entity that was just created/changed.  Used to select the item in the lookup dialog.
function auto(iObjectTypeCode, sFindValue)
{
    // Find out whether the current object is an activity.
    var bIsActivity = IsActivityTypeCode(iObjectTypeCode);
    if (iObjectTypeCode == AppOfflineFilter)
    {
        iObjectTypeCode = UserQuery; // to refresh grids properly, because OTC of records in grids is UserQuery
    }
    var oDoc = document;
    try
    {
        // Handle the situation where the current window is a "SubArea" page.
        // This happens when a user loads a record and then loads a record from a sub-grid of the parent records.
        // If they update the record, the "Window.Opener" is wrong and needs to be made the parent
        // detailed record's window.
        if (window.frameElement && window.frameElement.isArea)
        {
            oDoc = window.parent.document;
        }
        else if (window.frameElement && window.frameElement.className == "ms-crm-Home-Cal-Workplace")
        {
            oDoc.location.reload();
            return;
        }
        else if (window.frameElement && window.frameElement.className == "ms-crm-Home-Cal-Month")
        {
            oDoc = window.parent.document;
            oDoc.location.reload();
            return;
        }
    }
    catch (e)
    {
        // Ignore the errors because they are expected in some situations (e.g. see MSCRM bug #25035)
    }
    for (var i = 0; i < oDoc.frames.length; i++)
    {
        try
        {
            var o = oDoc.frames[i].frameElement;
            if (o.className == "ms-crm-Home-Cal-Month")
            {
                // Refresh the monthly calendar if it's there...
                if (!IsNull(oDoc.frames[i].Refresh))
                    oDoc.frames[i].Refresh(iObjectTypeCode);
            }
            else if (o.className == "noteData")
            {
                // Note: we need to call the Refresh() method because we want to reload using the default URL (ie. no edit mode)
                // We should only be reloading the notes data page if it is currently visible.
                // CRM SE 10798 - We need to refresh the notes control only if the auto got called
                // for otc - annotation or if the notes control is not dirty. (If it is not dirty, we should 
                // not block calling refresh which will block other user's notes updates.
                if (o.IsParentTabVisible() && (iObjectTypeCode == Annotation || !o.IsDirty))
                {
                    o.Refresh();
                }
            }
            else if (o.className == "ms-crm-Home-Cal-Workplace")
            {
                oDoc.frames[i].location.reload();
                return;
            }
            else if (o.className == "ms-crm-Dialog-Lookup-Results")
            {
                // Refresh the lookup grid if it still has this type selected.
                // Use == not === since one is int and the other is string.
                if (iObjectTypeCode == selObjects.value)
                {
                    search(sFindValue);
                    return;
                }
            }
            else if (o.isArea)
            {
                if (oDoc.frames[i].frames && oDoc.frames[i].frames.length > 0)
                {
                    // This is a Detailed Form - Recurse 1 level refresh the first IFRAME (should only be 1 grid)
                    o = oDoc.frames[i].frames[0].frameElement;
                    if (o.className == "ms-crm-Home-Cal-Month")
                    {
                        if (!IsNull(oDoc.frames[i].frames[0].Refresh))
                            oDoc.frames[i].frames[0].Refresh(iObjectTypeCode);
                    }
                }
                // Handle Refresh for new Grid v2 - Controls
                findAndRefreshGrids(oDoc.frames[i].document.all.tags("TABLE"), iObjectTypeCode, bIsActivity);
            }
        }
        catch (e)
        {
            // Ignore the errors because they are expected in some situations (e.g. see the MSCRM BUG: #24949)
        }
    }
    // Handle Refresh for new Grid v2 - Controls
    findAndRefreshGrids(oDoc.all.tags("TABLE"), iObjectTypeCode, bIsActivity, sFindValue);
}
/// <summary>
/// Finds and refreshes AppGrid v2.0 Grids with the provided Object Type Code
/// </summary>
function findAndRefreshGrids(oTables, iObjectTypeCode, bIsActivity, sFindValue)
{
    var o;
    for (var i = 0; i < oTables.length; i++)
    {
        o = oTables[i];
        if (o.className.indexOf("ms-crm-ListControl") > -1 && o.GetParameter("autorefresh") == "1" &&
(o.GetParameter("otc") == iObjectTypeCode ||
(bIsActivity && (o.GetParameter("otc") == ActivityPointer || iObjectTypeCode == ActivityPointer)) || o.GetParameter("otc") == 0))
        {
            // When on the lookup single page, populate the quick find text before refreshing.
            if (!IsNull(sFindValue) && document.getElementById("findValue") != null && document.location.pathname.indexOf("lookupsingle.aspx") > 0)
            {
                findValue.value = sFindValue;
                search();
                return;
            }

            o.ClearPagingCookie();
            o.Refresh();
        }
    }
}
//When an array is created in an HTC and sent to JS it is not recognized as an array.
// In this case check for the splice method being defined.
function isArray(item)
{
    return item != null && ((item instanceof Array) || (typeof item.splice) == "function");
}
function formatString()
{
    if (arguments.length == 0)
    {
        return "";
    }
    var s = arguments[0];
    var aArguments = arguments;
    if (arguments.length > 1 && isArray(arguments[1]))
    {
        aArguments = arguments[1];
        //Shift the string template into the first slot so that the new array matches the function arguments.
        aArguments.unshift(s);
    }
    for (var i = 1; i < aArguments.length; i++)
    {
        var argIndex = i - 1;
        s = s.replace(new RegExp("\\{" + argIndex + "\\}", "g"), aArguments[i].toString());
    }
    return s;
}
function myonError(oError)
{
    if (IsOutlookClient())
    {
        openErrorDlg(oError.number);
    }
}
function IsNull(o)
{
    return ("undefined" == typeof (o) || "unknown" == typeof (o) || null == o);
}
// This is a helper function, that fires the Click event on an element that has focus
// This is required for HTML elements that do not handle TabIndex correctly and that do not
// respond to the press of the ENTER key.  The object MUST support the "Click" method for this to work
//
// Proper usage example: <table tabIndex="0" onkeyup="AccessibilityClick()">
//
// Note: TabIndex must be set, otherwise the element won't receive focus
//
function AccessibilityClick()
{
    if (event.keyCode == 13) // ENTER Key
    {
        event.srcElement.click();
        // Cancel the event, since we are handling it here... (MSCRM BUG: #11917)
        event.cancelBubble = true;
        return false;
    }
}
/// <summary>
/// Determines if an Object Type Code is of type User Defined
/// NOTE: This method does not valid the OTC against the Metabase, it is possible the
/// OTC will be User Defined while an Entity with that OTC may not exist.
/// </summary>
/// <param name="iOTC">The Object Type Code to test</param>
/// <returns>True if it is user defined, false if it is not.</returns>
function IsUserDefinedEntityObjectTypeCode(iOTC)
{
    return iOTC >= BaseAddEntityObjectTypeCode;
}
// Whenever a popup is used, it should have default stylesheets applied to it to conform to our
// application
function SetDefaultStyles(oPopup)
{
    if (!IsNull(oPopup))
    {
        var sFontsFilePath = prependOrgName("/_common/styles/fonts.aspx");

        if (IsHelpContext())
        {
            sFontsFilePath = "/help/common/fonts.aspx";
        }

        sFontsFilePath = SetQueryString(sFontsFilePath, "lcid", USER_LANGUAGE_CODE);

        oPopup.document.createStyleSheet(sFontsFilePath);
    }
}
// Indicates if the code should render a page for an Outlook client (light or fat).
function IsOutlookClient()
{
    // Check if if this could be a fat client or a light client
    return IsOutlookLaptopClient() || IsOutlookWorkstationClient();
}
// Indicates if the code should render a page for an Outlook light client.
function IsOutlookWorkstationClient()
{

    // Pages compiled without the RC directive may be light client pages or IE client pages depending on the existence of a cookie
    return (document.cookie.indexOf("LightClient") != -1);

}
// Indicates if the code should render a page for an Outlook fat client.
function IsOutlookLaptopClient()
{

    // Pages compiled without the RC directive may be light client pages or IE client pages depending on the existence of a cookie
    return (document.cookie.indexOf("FullClient") != -1);

}
// Indicates if the client for which this page is rendered is online or offline.
function IsOnline()
{

    // Pages compiled without the RC directive may be light client mode pages or IE client mode pages depending on the existence of a cookie
    // In any case, consider that the code is running online
    return true;

}
function FormatUrl(sUrl)
{
    if (IsNull(sUrl) || sUrl.length == 0)
    {
        sUrl = window.location.pathname;
        // Sometimes the pathname does not include the leading '/'.  Add it if it's missing.
        if (sUrl.substr(0, 1) !== "/")
        {
            sUrl = "/" + sUrl;
        }
    }
    return sUrl.split('?')[0].toUpperCase();
}
// Parses the QueryString into a Dictionary of name/value pairs
// sQueryString: [Optional] The querystring to parse.  When not passed to this function the current page's query string will be used.
function ParseQueryString(sQueryString)
{
    // If sQueryString was not passed in use the page's query string.
    if (IsNull(sQueryString))
    {
        sQueryString = window.location.search;
    }
    // If there is nothing to parse return the empty object.
    var oQueryString = new Object();
    if (IsNull(sQueryString))
    {
        return sQueryString;
    }
    // Trim off the leading ? if it's there.
    if (sQueryString.charAt(0) === "?")
    {
        sQueryString = sQueryString.substring(1);
    }
    // Parse the query string into name/value pairs.
    var aParts = sQueryString.split("&");
    var iLen = aParts.length;
    for (var i = 0; i < iLen; i++)
    {
        var aNameValue = aParts[i].split("=");
        if (aNameValue.length === 2)
        {
            oQueryString[aNameValue[0]] = aNameValue[1];
        }
    }
    return oQueryString;
}
// Sets WRPC token in XMLHttp header.
function SetTokenInHeader(oXmlHttp, sUrl)
{
    sUrl = FormatUrl(sUrl);
    var oToken = GetCrmWrpcToken(sUrl);
    if (oToken !== null)
    {
        oXmlHttp.setRequestHeader("CRMWRPCToken", oToken.Token);
        oXmlHttp.setRequestHeader("CRMWRPCTokenTimeStamp", oToken.Timestamp);
    }
}
// Sets WRPC token in querystring.
function GetTokenAsQS(sUrl)
{
    sUrl = FormatUrl(sUrl);
    var oToken = GetCrmWrpcToken(sUrl);
    if (oToken !== null)
    {
        return "CRMWRPCToken=" + CrmEncodeDecode.CrmUrlEncode(oToken.Token) + "&CRMWRPCTokenTimeStamp=" + CrmEncodeDecode.CrmUrlEncode(oToken.Timestamp);
    }
}
// Sets the token for the given url.
function SetCrmWrpcToken(sUrl, sToken, sTimestamp)
{
    // Loop through the token list.
    var bFound = false;
    for (var i = 0; i < _aWrpcTokens.length; i++)
    {
        var tokens = _aWrpcTokens[i];
        if (tokens.Timestamp === sTimestamp)
        {
            bFound = true;
            tokens[sUrl] = sToken;
        }
    }
    // If the token list for the given time stamp was not found, create a new one.
    if (!bFound)
    {
        var tokens = new Object();
        tokens.Timestamp = sTimestamp;
        tokens[sUrl] = sToken;
        // Add it to token array.
        _aWrpcTokens[_aWrpcTokens.length] = tokens;
    }
}
// Gets the token information for the specified URL.
// The return value has three properties:
//  Token: The wrpc token.
//  Timestamp: The token time stamp.
//  Url: The action URL for the token.
// Returns null if the token is not present.
function GetCrmWrpcToken(sUrl)
{
    // Loop through the token list.
    for (var i = 0; i < _aWrpcTokens.length; i++)
    {
        var tokens = _aWrpcTokens[i];
        var token = tokens[sUrl];
        if (!IsNull(token))
        {
            var oToken = new Object();
            oToken.Token = token;
            oToken.Timestamp = tokens.Timestamp;
            oToken.Url = sUrl;
            return oToken;
        }
    }
    return null;
}
/// <summary>
/// Gathers data relevant to a script error for use in Watson error reporting.
/// </summary>
/// <param name="sMsg">Error Message</param>
/// <param name="sUrl">Page on which error occurred</param>
/// <param name="sLn">Line number of error</param>
/// <param name="func">Function where error occurred</param>
/// <param name="callStack">Call stack of error</param>
/// <returns>Object containing the appropriate data</returns>
function CollectCrashData(sMsg, sUrl, sLn, func, callStack)
{
    // Create data object.
    var oCrashData = new Object();
    oCrashData.lineNumber = sLn;
    oCrashData.func = func;
    // We only need relative URL
    var relativeUrl = GetRelativeUrl(sUrl);
    var relativePageUrl = GetRelativeUrl(location.href);
    // For the bucketing parameter, we want to make sure to get rid of any query parameters
    var startOfQuery = relativeUrl.indexOf("?");
    if (-1 != startOfQuery)
    {
        oCrashData.fileName = relativeUrl.substring(0, startOfQuery);
    }
    else
    {
        oCrashData.fileName = relativeUrl;
    }
    var clientType = "Web";
    if (IsOutlookLaptopClient())
    {
        clientType = "Outlook Laptop - " + (IsOnline() ? "Online" : "Offline");
    }
    else if (IsOutlookWorkstationClient())
    {
        clientType = "Outlook Desktop";
    }
    // Put together report
    oCrashData.report = "<CrmScriptErrorReport>\r\n" +
"  <ReportVersion>1.0</ReportVersion>\r\n" +
"  <ScriptErrorDetails>\r\n" +
"	<Message>" + CrmEncodeDecode.CrmXmlEncode(sMsg) + "</Message>\r\n" +
"	<Line>" + sLn + "</Line>\r\n" +
"	<URL>/" + CrmEncodeDecode.CrmXmlEncode(relativeUrl) + "</URL>\r\n" +
"	<PageURL>/" + CrmEncodeDecode.CrmXmlEncode(relativePageUrl) + "</PageURL>\r\n" +
"	<Function>" + CrmEncodeDecode.CrmXmlEncode(func) + "</Function>\r\n" +
"	<CallStack>\r\n" + callStack + "	</CallStack>\r\n" +
"  </ScriptErrorDetails>\r\n" +
"  <ClientInformation>\r\n" +
"	<BrowserUserAgent>" + CrmEncodeDecode.CrmXmlEncode(navigator.userAgent) + "</BrowserUserAgent>\r\n" +
"	<BrowserLanguage>" + CrmEncodeDecode.CrmXmlEncode(navigator.browserLanguage) + "</BrowserLanguage>\r\n" +
"	<SystemLanguage>" + CrmEncodeDecode.CrmXmlEncode(navigator.systemLanguage) + "</SystemLanguage>\r\n" +
"	<UserLanguage>" + CrmEncodeDecode.CrmXmlEncode(navigator.userLanguage) + "</UserLanguage>\r\n" +
"	<ScreenResolution>" + screen.width + "x" + screen.height + "</ScreenResolution>\r\n" +
"	<ClientName>" + clientType + "</ClientName>\r\n" +
"  </ClientInformation>\r\n" +
"</CrmScriptErrorReport>\r\n";
    return oCrashData;
}
function IsHelpContext()
{
    return !((typeof (HELP_CONTEXT) == "undefined") || (typeof (HELP_CONTEXT) == "unknown") || (HELP_CONTEXT != 1));
}
/// <summary>
/// Given an absolute Url (http://server/page.htm), 
/// returns a relative Url (/page.htm)
/// </summary>
function GetRelativeUrl(sAbsoluteUrl)
{
    var startOfServer = sAbsoluteUrl.indexOf("//");
    if (-1 == startOfServer)
        startOfServer = 0;
    else
        startOfServer += 2;

    return sAbsoluteUrl.substring(sAbsoluteUrl.indexOf("/", startOfServer) + 1);
}
/// <summary>
/// Save crash data on page for use on page unload.
/// If not the top frame, store on the parent frame.
/// </summary>
function StoreCrashData(oCrashData)
{
    // If not the parent frame, call this function on the parent
    if (window.parent != self && window.parent.StoreCrashData != null)
    {
        window.parent.StoreCrashData(oCrashData);
        return;
    }
    // Store the data here
    if (window.crashData == null)
    {
        window.crashData = new Array();
    }
    // In order to mitigate DoS attacks, there is a hard limit of reporting/storing 7 errors per page
    if (window.crashData.length < 7)
    {
        window.crashData.push(oCrashData);
    }
}
/// <summary>
/// This is an empty callback function used to allow the remote command object to run a command asynchronously.
/// </summary>
function emptyCallback(oResult)
{
}
/// <summary>
/// This is an empty error handler used to allow the remote command object to run a command asynchronously.
/// </summary>
function emptyErrorHandler(sHResult, oXmlNode)
{
}
var FORM_FIELD_TYPE_NORMAL = 0;
var FORM_FIELD_TYPE_REQUIRED = 1;
var FORM_FIELD_TYPE_RECOMMENDED = 2;
/// <summary>
/// This is a utility function, used to handle changes to the label from required/recommended to normal and back again
/// We need this because required/recommended style now comes with an icon that must be displayed or hidden.
/// </summary>
function SetFieldRequiredOrRecommended(oField, nToType, sToAlt)
{
    if (IsNull(oField)) { return; }
    if (IsNull(oField.className)) { return; }
    if (IsNull(sToAlt)) { sToAlt = ""; }
    var sRemove1, sRemove2, sNewClass, sNewImageSrc;
    var bHideImage = false;
    switch (nToType)
    {
        case FORM_FIELD_TYPE_REQUIRED:
            sRemove1 = "ms-crm-Field-Normal";
            sRemove2 = "ms-crm-Field-Recommended";
            sNewClass = "ms-crm-Field-Required";
            sNewImageSrc = "/_imgs/frm_required.gif";
            break;
        case FORM_FIELD_TYPE_RECOMMENDED:
            sRemove1 = "ms-crm-Field-Required";
            sRemove2 = "ms-crm-Field-Normal";
            sNewClass = "ms-crm-Field-Recommended";
            sNewImageSrc = "/_imgs/frm_recommended.gif";
            break;
        default:
            sRemove1 = "ms-crm-Field-Required";
            sRemove2 = "ms-crm-Field-Recommended";
            sNewClass = "ms-crm-Field-Normal";
            bHideImage = true;
            break;
    }

    // Remove any Required/Recommneded/Normal from existing field
    var sTempClassName = oField.className.replace(sRemove1, "");
    sTempClassName = sTempClassName.replace(sRemove2, "");

    // Set to requested type
    if (sTempClassName.indexOf(sNewClass) < 0)
    {
        sTempClassName = sTempClassName + " " + sNewClass;
    }
    oField.className = sTempClassName;

    // Get the image (if any) of the required/recommended icon
    var oImgs = oField.getElementsByTagName("IMG");
    var oImgsCount = 0;
    if (!IsNull(oImgs))
    {
        oImgsCount = oImgs.length;
    }

    // Display or hide icon - set proper image and new alt tags.
    if (bHideImage)
    {
        if (oImgsCount > 0)
        {
            oImgs[oImgsCount - 1].style.display = "none";
        }
    }
    else
    {
        // If no image present, create one
        if (oImgsCount <= 0)
        {
            // Expecting a TD with a LABEL child, the image should go into the label child.
            var positionToAdd = oField;
            if (!IsNull(oField.firstChild))
            {
                positionToAdd = oField.firstChild;
            }
            positionToAdd.innerHTML += "<img alt=\"" + CrmEncodeDecode.CrmHtmlAttributeEncode(sToAlt) + "\" src=\"" + sNewImageSrc + "\"/>";
        }
        else
        {
            var oImage;
            oImage = oImgs[oImgsCount - 1];
            oImage.src = sNewImageSrc;
            oImage.alt = sToAlt;
            oImage.style.display = "inline";
        }
    }
}
/// <summary>
/// If any error reports are stored on this page, prompt the user to report them to Microsoft.
/// If not the top frame, call this function on the parent frame.
/// </summary>
function PromptAndReportAnyErrors()
{
    // This function is used for error handling.  If an error occurs, we want it to fail silently,
    // so the entire functionality is enclosed in a try/catch block which will catch any errors
    // and do nothing.
    try
    {
        // If not the parent frame, call this function on the parent
        try
        {
            if (window.parent != self && window.parent.PromptAndReportAnyErrors != null)
            {
                window.parent.PromptAndReportAnyErrors();
                return;
            }
        }
        catch (e)
        {
            // If this is an Outlook client page, we may not be able to read window.parent.PromptAndReportAnyErrors
            // Indeed, we may get an access denied error because the URL changed due to online/offline transitions
            return;
        }
        // Test for existence of crash data and whether the user would like to be prompted regarding script errors
        if (null != window.crashData && 1 == ScriptErrorReportingPreference)
        {
            // Prompt user to upload error report
            // The dialog will take care of submitting reports for upload
            // if the user selects to do so.
            var oWindowInfo = GetWindowInformation(ScriptErrorDialog);
            var returnValue = openStdDlg(prependOrgName("/_common/error/dlg_scriptError.aspx"), window.crashData, oWindowInfo.Width, oWindowInfo.Height);
            // Erase crash data whether or not we reported
            window.crashData = null;
            // Clear the error from window's status bar.
            window.status = "";
        }
    }
    catch (e)
    {
    }
}
/// <summary>
/// Call the ScriptError webservice which will upload an error report to Watson servers
/// </summary>
function ReportScriptError(oCrashData, addServerInformation, synchronous)
{
    // Set up remote command
    var command = new RemoteCommand("ScriptError", "SendScriptErrorReport");
    command.SetParameter("fileName", oCrashData.fileName);
    command.SetParameter("lineNumber", oCrashData.lineNumber);
    command.SetParameter("function", oCrashData.func);
    command.SetParameter("errorReport", CrmEncodeDecode.CrmXmlEncode(oCrashData.report));
    command.SetParameter("addServerInformation", addServerInformation);
    command.SetParameter("errorReportingPreference", ScriptErrorReportingPreference);
    // Turn off error handling - we want this to be silent if it fails.
    command.ErrorHandler = emptyErrorHandler;
    // Run command
    if (synchronous)
    {
        command.Execute();
    }
    else
    {
        command.Execute(emptyCallback);
    }
}
function StringBuilder()
{
    //-------- Public methods ----------
    this.Append = _append;
    this.Reset = _reset;
    this.ToString = _toString;
    //-------- Private members ---------
    var _arr = new Array();
    function _append(str)
    {
        _arr[_arr.length] = str;
    }
    function _reset()
    {
        _arr = new Array();
    }
    function _toString()
    {
        return _arr.join("");
    }
}
function getBrowserMajorVersion()
{
    // navigator.userAgent returns a string formatted as:
    // Mozilla/4.0 (compatible; MSIE 7.0; Windows.........
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf("MSIE ");
    var dot = ua.indexOf(".", msie);
    if (msie > 0 && dot > 0)      // If Internet Explorer, return version number
    {
        return parseInt(ua.substring(msie + 5, dot));
    }
    else                 // If another browser, return 0
    {
        return 0;
    }
}
