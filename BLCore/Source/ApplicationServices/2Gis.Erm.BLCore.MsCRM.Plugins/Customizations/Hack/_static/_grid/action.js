function getCommonObjectType(sGridName)
{
var a = document.all[sGridName].InnerGrid.SelectedRecords;
for (var i=1; i < a.length; i++)
{
if (a[i][1] != a[i-1][1]) return null;
}
return a[0][1];
}
function getSelected(sGridName, details) {
    var a = document.all[sGridName].InnerGrid.SelectedRecords;
    var backCompatArray = new Array(a.length);
    for (var i = 0; i < a.length; i++) {
        if (details) {
            backCompatArray[i] = {
                EntityId: a[i][0],
                EntityName: GetErmEntityName(a[i][1])
            };
        } else {
            backCompatArray[i] = a[i][0];
        }

    }
    return backCompatArray;
}



function getParentEntityIdParams()
{

var currWindow = window;
var oForm = null;
do
{
    /*CRM Hack*/
    try {
        oForm = currWindow.document.all["crmFormSubmit"];
    } catch(e) {
        break;
    }
    /*End CRM Hack*/

if (!IsNull(oForm))
break;
if (currWindow == currWindow.parent)
break;
currWindow = currWindow.parent;
} while ((IsNull(oForm)) && (!IsNull(currWindow)))
var sParams;

if (!IsNull(oForm))
{
if ((!IsNull(oForm.crmFormSubmitId)) && (!IsNull(oForm.crmFormSubmitObjectType)))
{
sParams = "?_CreateFromType=" + CrmEncodeDecode.CrmUrlEncode(oForm.crmFormSubmitObjectType.value) + "&_CreateFromId=" + CrmEncodeDecode.CrmUrlEncode(oForm.crmFormSubmitId.value);
}
}
return sParams;
}
function getNotSelected(sGridName)
{
var iTotal = document.all[sGridName].InnerGrid.NumberOfRecords;
if (iTotal == 0)
{
return false;
}
var o = document.all[sGridName].InnerGrid;
var a = new Array;
var iLen = document.all[sGridName].InnerGrid.SelectedRecords.length;
var ii = 0;
for (var i=0; i < iTotal; i++)
{
if (!(o.rows[i].selected))
{
a[ii] = o.rows[i].oid;
ii++;
}
if (ii == (iTotal- iLen)) break;
}
return a;
}





function getSelectedSubTypes(sGridName)
{
return document.all[sGridName].InnerGrid.SelectedRecords;
}
function doIsvAction(sGridId, bPassParams, sPath, sMode, sParams)
{
openIsvWin(sPath, bPassParams, sMode, sParams, getSelected(sGridId));
}
function doAction(sGrid, iObjType, sAction, sObjId)
{
var a;
var sCustParams = "";
var arySubType;
var iLen;
var sIds		= "";
if (!sObjId)
{
a = getSelected(sGrid);
}
else
{
a = new Array(sObjId);
}
var iX = 400;
var iY = 200;

switch (sAction)
{
case "addquota":		iX = 600;	iY = 425;	break;
case "allowscheduling":	iX = 400;	iY = 225;	break;
case "runworkflow":
launchOnDemandWorkflow(sGrid,iObjType);
return;
break;
case "actsetrespon":	iX = 456;	iY = 310;	break;
case "bulkedit":		iX = 700;	iY = 540;	break;
case "changeorg":		iX = 400;	iY = 325;	break;
case "changecaptain":	iX = 400;	iY = 225;	break;
case "changeparent":	iX = 400;	iY = 225;	break;
case "createopportunity":	iX = 700;	iY = 540;	break;
case "disallowscheduling":	iX = 400;	iY = 225;	break;
case "role":			iX = 500;	iY = 330;	break;
case "share":			iX = 800;	iY = 480;	break;
case "status":			iX = 456;	iY = 250;	break;
case "webmailmerge":		iX = 450;	iY = 500;	break;
case "merge":			iX = 800;	iY = 570;   break;
case "mergeduplicates":	iX = 800;	iY = 570;   break;
case "cancel":			iY = 175;	break;
case "activate":
iX = 500;
if(iObjType == Workflow)
{
try
{
var selectedItem = getSelectedSubTypes("crmGrid");
var subtype=selectedItem[0][3].type;
sCustParams = sCustParams + "&iObjSubtype="+subtype;
}catch(everything)
{
}
}
break;
case "deactivate":
iX = 500;
if (iObjType == BusinessUnit || iObjType == DiscountType || iObjType == Account || iObjType == Contact)
{
iY = 280;
}
if (iObjType == BusinessUnit)
{
iX = 650;
if (a.length != 0)
{
sIds = a[0];
iLen = a.length;
for(var i=1; i< iLen; i++)
{
sIds += a[i];
}
}
}
if(iObjType == Workflow)
{
try
{
var selectedItem = getSelectedSubTypes("crmGrid");
var subtype=selectedItem[0][3].type;
sCustParams = sCustParams + "&iObjSubtype="+subtype;
}catch(everything)
{
}
}
break;
case "merge":
{
alert('Привет');
break;
}
case "assignwip":
case "assign":
switch (parseInt(iObjType, 10))
{
case QueueItem:


arySubType = getSelectedSubTypes(sGrid);


if("undefined" != typeof(crmQList))
{
sCustParams = "&sSrcId=" + CrmEncodeDecode.CrmUrlEncode(crmQList.firstChild.GetSelectedId());
}
sCustParams += "&sQType=" + ((-1 == sAction.lastIndexOf("wip")) ? 1 : 3);
iX = 500;
iY = 310;
sAction = "assignqueue";
break;
case Incident:



sCustParams += "&sQType=" + ((-1 == sAction.lastIndexOf("wip")) ? 1 : 3);
iX = 500;
iY = 310;
sAction = "assignqueue";
break;
case CampaignResponse:
case CampaignActivity:
case Task:
sCustParams += "&sQType=1";
iX = 500;
iY = 310;
sAction = "assignqueue";
break;
case ActivityPointer:
case Email:
case Fax:
case PhoneCall:
case Letter:
case Appointment:
case ServiceAppointment:
arySubType = getSelectedSubTypes(sGrid);
sCustParams += "&sQType=1";
iX = 500;
iY = 310;
sAction = "assignqueue";
break;
default:
iX = 456;
iY = 310;
break;
}
break;
}

var bActionApplicable = true;
if (!a || a.length == 0)
{
switch(sAction)
{

case "minicampaignallitemsonpage":
case "minicampaignallitemsonview":
case "detectduplicatesallrecords":
case "resetdatafilters":
case "uploadimportmap":
case "addusers":
bActionApplicable = true;
break;

default:
bActionApplicable = false;
alert(LOCID_ACTION_NOITEMSELECTED);
}
}
if(bActionApplicable == true)
{
var sSubTypes	= "";
var i;
var iRelationshipType = None;
switch (sAction)
{
case "addcustomeropportunityrole":
iRelationshipType = CustomerOpportunityRole;
case "addcustomerrelationship":
if (iRelationshipType == None)
{
iRelationshipType = CustomerRelationship;
}
if (a.length == 1)
{
locAddRelatedToNonForm(iRelationshipType, iObjType, a[0], "");
}
else
{
alert(LOCID_RELATIONSHIPS_TOO_MANY);
}
return;
case "bulkedit":

if (a.length == 1)
{
var sParams = getParentEntityIdParams();
openObj(iObjType, a[0], sParams);
return;
}
break;
case "viewreport":
if (a.length == 1)
{
arySubType = getSelectedSubTypes(sGrid);
with(arySubType[0][3])
{
ViewReport(parseInt(iReportType, 10), surlprefix, oid, sfilename, "run");
}
}
else
{
alert(LOCID_CUSTMSG_TOOMANY_REC);
}
return;
case "editreport":
if (a.length == 1)
{
var oWindowInfo = GetWindowInformation(ReportPropertyDialog);
var url = prependOrgName("/CRMReports/reportproperty.aspx?id=" + CrmEncodeDecode.CrmUrlEncode(a[0]));
var iX	= oWindowInfo.Width;
var iY	= oWindowInfo.Height;
openStdWin( url, buildWinName(a[0]), iX, iY);
}
else
{
alert(LOCID_CUSTMSG_TOOMANY_REC);
}
return;
case "downloadreport":
if (a.length == 1)
{
arySubType = getSelectedSubTypes(sGrid);
var reportType = parseInt(arySubType[0][3].iReportType, 10);
DownloadReport(reportType, a[0]);
}
else
{
alert(LOCID_CUSTMSG_TOOMANY_REC);
}
return;
case "editreportfilter":
if (a.length == 1)
{
arySubType = getSelectedSubTypes(sGrid);
with(arySubType[0][3])
{
ViewReport(parseInt(iReportType, 10), surlprefix, oid, sfilename, "editfilter");
}
}
else
{
alert(LOCID_CUSTMSG_TOOMANY_REC);
}
return;
case "reportschedulewizard":
if (a.length == 1)
{
arySubType = getSelectedSubTypes(sGrid);
var reportType = parseInt(arySubType[0][3].iReportType, 10);
if (reportType == 1)
{
var iWidth = 700;
var iHeight = 550;
var sUrl = prependOrgName("/CRMReports/ReportSchedule/ScheduleWizard.aspx?id=" + CrmEncodeDecode.CrmUrlEncode(a[0]));
var result = openStdDlg(sUrl, null, iWidth, iHeight, true);

if (result)
auto(iObjType);
}
else
{
alert(LOCID_NON_SRS_FILTERSCHEDULE);
}
}
else
{
alert(LOCID_CUSTMSG_TOOMANY_REC);
}
return;
case "assignqueue":


if ( undefined != arySubType )
{
for(i=0; i<a.length; i++)
{
sSubTypes += arySubType[i][1] + ",";
}


sCustParams += "&sSubTypes=" + sSubTypes.replace(/,$/, "");
}
if(a.length > 0)
{
sCustParams += ("&uid=" + CrmEncodeDecode.CrmUrlEncode(a[0]));
}
break;
case "addquota":
if (a.length == 1)
{
sCustParams += "&uid=" + CrmEncodeDecode.CrmUrlEncode(a[0]);
}
break;
case "applyrule":
if(a.length > 0)
{
iObjType = getCommonObjectType(sGrid);
if (IsNull(iObjType))
{
alert(LOCID_HETEROGENOUS_TYPES);
return;
}
sIds += (a[0] + ";");
}
break;
case "copyrole":
if (a.length == 1)
{
copyRole(a[0]);
auto(iObjType);
}
else
{
alert(LOCID_ROLES_TOO_MANY);
}
return;
case "asyncOperationCancel":
sAction = "setstate_asyncoperation";
sCustParams = "&sNewState=Completed";
break;
case "asyncOperationResume":
sAction = "setstate_asyncoperation";
sCustParams = "&sNewState=Ready";
break;
case "asyncOperationPostpone":
if(IsSelectedJobCompleted())
{
return;
}
sAction = "setstate_asyncoperation";
sCustParams = "&sNewState=Suspended";
break;
case "asyncOperationPause":
sAction = "setstate_asyncoperation";
sCustParams = "&sNewState=Paused";
break;
case "bulkDeleteCancel":
sAction = "setstate_bulkdelete";
sCustParams = "&sNewState=Completed";
break;
case "bulkDeleteResume":
sAction = "setstate_bulkdelete";
sCustParams = "&sNewState=Ready";
break;
case "bulkDeletePostpone":
sAction = "setstate_bulkdelete";
sCustParams = "&sNewState=Suspended";
break;

case "bulkDeletePause":
sAction = "setstate_bulkdelete";
sCustParams = "&sNewState=Paused";
break;
}

var oResult;
if( sAction == "delete" )
{
var nWidth = 570;
var nShortHeight = 205;
var nLongHeight = 250;

switch( parseInt(iObjType, 10) )
{
case BulkOperation:
var viewId = document.all[sGrid].GetParameter("viewid");
switch(viewId)
{
case "{FD4DF17C-386E-4E29-9D31-529A568A3BBC}":
case "{BA825777-1A7B-4837-8778-B86904D1115C}":
case "{83DC2389-0A4C-4249-A5D3-BA1AB401FF1C}":
case "{2C9E8543-3E3B-4F8D-AEDF-B8A43446619D}":
sCustParams += "&iObjSubType=7";
break;
}
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, nWidth, nShortHeight );
break;
case Account:
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_delete_account.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, nWidth, nLongHeight );
break;
case Contact:
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_delete_contact.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, nWidth, nLongHeight );
break;
case OpportunityProduct:
case QuoteDetail:
case SalesOrderDetail:
case InvoiceDetail:
var oArg = new Array();
oArg[0] = window.parent;
oArg[1] = a;
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_deleteqoiproduct.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sParentId=" + CrmEncodeDecode.CrmUrlEncode(document.all[sGrid].GetParameter("oId")), oArg, nWidth, nShortHeight );
break;
case Queue:

for(var i=0; i<a.length; i++)
{
sIds += a[i] + ";";
}
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_delete_queue.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, nWidth, nLongHeight );
break;
case QueueItem:

arySubType = getSelectedSubTypes(sGrid);
for(i=0; i<a.length; i++)
{
sSubTypes += arySubType[i][1] + ",";
}


sCustParams += "&sSubTypes=" + CrmEncodeDecode.CrmUrlEncode(sSubTypes.replace(/,$/, ""));


sCustParams += "&sSrcQueueId=" + CrmEncodeDecode.CrmUrlEncode(crmQList.firstChild.GetSelectedId());
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_delete.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + sCustParams, a, nWidth, nLongHeight );
break;


case ActivityPointer:

arySubType = getSelectedSubTypes(sGrid);
for(i=0; i<a.length; i++)
{
sSubTypes += arySubType[i][1] + ",";
}


sCustParams += "&sSubTypes=" + CrmEncodeDecode.CrmUrlEncode(sSubTypes.replace(/,$/, ""));
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_delete.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + sCustParams, a, nWidth, nLongHeight );
break;
case ImportMap:
sIds = a[0];
for(var i=1; i<a.length; i++)
{
sIds += ";" + a[i];
}
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_delete_importmap.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, nWidth, nLongHeight );
break;
default:
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, nWidth, nShortHeight );
break;
}
}
else if(sAction == "resetdatafilters")
{
if (IsOutlookClient())
{
if (window.confirm(LOCID_RESET_DATA_FILTERS))
{
var command = new RemoteCommand("UserQuery", "ResetDataFilters");
var oResult = command.Execute();
if (oResult.Success)
{
refreshGrid(sGrid);
}
}
}
}
else if (sAction == "assignqueue")
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + sCustParams, a, iX, iY );
}
else if (sAction == "assign")
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, iX, iY );
}
else if (sAction == "merge")
{


if (a.length > 2)
{
alert(LOCID_MERGE_TOOMANY_RECORDS);
}
else
{
sIds = a[0];
for(var i=1; i<a.length; i++)
{
sIds += ";" + a[i];
}
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, "", iX, iY );
}
}
else if (sAction == "mergeduplicates")
{
var b = parent.document.frames["baseRecordsIframe"].document.all["baseRecordsGrid"].InnerGrid.SelectedRecords;
var baseRecord = new Array(b.length);
for (var i=0; i < b.length; i++)
{
baseRecord[i] = b[i][0];
}
if (a.length > 1 || baseRecord.length > 1)
{
alert(LOCID_MRGE_DUPS_TOOMANY_RCRDS);
}
else
{
sIds = baseRecord[0];
sIds += ";" + a[0];

oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_merge.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, "", iX, iY );
}
}
else if (sAction == "automergeduplicates")
{
var b = parent.document.frames["baseRecordsIframe"].document.all["baseRecordsGrid"].InnerGrid.SelectedRecords;
var baseRecord = new Array(b.length);
for (var i=0; i < b.length; i++)
{
baseRecord[i] = b[i][0];
}
if (a.length > 1 || baseRecord.length > 1)
{
alert(LOCID_MRGE_DUPS_TOOMANY_RCRDS);
}
else
{
var commandMerge	= new RemoteCommand("DuplicateDetection", "AutoMergeDuplicates");
commandMerge.SetParameter("masterId", baseRecord[0]);
commandMerge.SetParameter("subordinateId", a[0]);
commandMerge.SetParameter("objectTypeCode", iObjType);
commandMerge.Execute();
refreshGrid(sGrid);
}
}

else if(sAction == "minicampaignselecteditems")
{

if (iObjType == List)
{
if(a.length > 1)
{
alert(LOCID_MINICAMP_TOOMANY_RECORDS);
}
else
{
var columns = new Array();
columns.type = "string";
columns[0] = "statecode";
var command = new RemoteCommand("MarketingAutomation", "RetrieveList");
command.SetParameter("id", a[0]);
command.SetParameter("columns", columns);
var oResult = command.Execute();
if (oResult.Success)
{
var resultXml = CreateXmlDocument(false);
resultXml.loadXML(oResult.ReturnValue);
if (resultXml.selectSingleNode("/list/statecode").text == "Inactive")
{
alert(LOCID_MC_CANNOT_RUN_INACTLST);
}
else
{
RunMiniCampaign(sGrid, iObjType, 1);
}
}
}
}
else
{
RunMiniCampaign(sGrid, iObjType, 1);
}
}

else if(sAction == "minicampaignallitemsonpage")
{

if(document.all[sGrid].InnerGrid.NumberOfRecords == 0)
{
alert(LOCID_MINICAMP_NORECORDS_MSG);
}
else
{
RunMiniCampaign(sGrid, iObjType, 2);
}
}

else if(sAction == "minicampaignallitemsonview")
{

var queryApi = document.all[sGrid].GetParameter("queryapi");
if(!IsNull(queryApi))
{
alert(LOCID_QC_CANNOT_RUN_ASSO);
return;
}

var viewId = document.all[sGrid].GetParameter("viewid");
if(isQueryApiBasedView(viewId) == true)
{
alert(LOCID_MINICAMP_CANNOT_RUN);
return;
}

if(document.all[sGrid].InnerGrid.NumberOfRecords == 0)
{
alert(LOCID_MINICAMP_NORECORDS_MSG);
}
else
{
RunMiniCampaign(sGrid, iObjType, 3);
}
}

else if(sAction == "detectduplicatesselectedrecords")
{
DetectDuplicates(sGrid, iObjType, 1);
}

else if(sAction == "detectduplicatesallrecords")
{

var qApi = document.all[sGrid].GetParameter("queryapi");
if(!IsNull(qApi))
{
alert(LOCID_DEDUPE_CANNOT_RUN_ASSO);
return;
}

var vid = document.all[sGrid].GetParameter("viewid");
if(isQueryApiBasedView(vid) == true)
{
alert(LOCID_DEDUPE_CANNOT_RUN);
return;
}
if(document.all[sGrid].InnerGrid.NumberOfRecords == 0)
{
alert(LOCID_DEDUPE_NORECORDS_MSG);
}
else
{
DetectDuplicates(sGrid, iObjType, 2);
}
}

else if(sAction == "publishduplicaterule")
{
var oArgs = new Object();
oArgs.Ids = getSelected(sGrid);
try
{

var command = new RemoteCommand("DuplicateDetection", "IsSystemWideDuplicateDetectionEnabled");
var result = command.Execute();
if (result.ReturnValue == false)
{
alert(LOCID_DEDUPE_DISABLE_ERR_MSG);
return;
}
}
catch(e)
{}
openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx", oArgs.Ids, iX, iY, true, false, "maximize:yes;minimize:yes");

refreshGrid(sGrid);
}
else if(sAction == "modifyrecurrence")
{



var recurrenceCheck = true;

var command, result;
sIds = a[0];


if(iObjType == 4424)
{


var aIds = new Array();
aIds.type = "guid";
for (var i=0; i < a.length; i++)
{
aIds.push(a[i]);
}
command = new RemoteCommand("BulkDelete", "IsBulkDeleteJobRecurring");
command.SetParameter("bulkDeleteIdList", aIds);
result = command.Execute();
recurrenceCheck = result.ReturnValue;
}

else if(a.length == 1)
{
command = new RemoteCommand("DuplicateDetection", "CanRecurrenceBeModified");
command.SetParameter("jobId", a[0]);
result = command.Execute();
recurrenceCheck = result.ReturnValue;
}
if (recurrenceCheck == false)
{
alert(LOCID_MODIFY_RECURRENCE_ERR_MSG);
return;
}
else
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, 500, 350 );
}
}
else if(sAction == "uploadimportmap")
{
var url = prependOrgName("/_grid/cmds/dlg_uploadimportmap.aspx");
openStdDlg(url, null, 400, 300, true, false, "maximize:no;minimize:no");
refreshGrid(sGrid);
}

else if(sAction == "unpublishduplicaterule")
{
var oArgs = new Object();
oArgs.Ids = getSelected(sGrid);
openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx", oArgs.Ids, iX, iY, true, false, "maximize:yes;minimize:yes");

refreshGrid(sGrid);
}

else if(sAction == "publishmailmergetemplate" || sAction == "unpublishmailmergetemplate")
{
var oArgs = new Object();
oArgs.Ids = getSelected(sGrid);
openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx", oArgs.Ids, iX, iY, true, false, "maximize:yes;minimize:yes");

refreshGrid(sGrid);
}
else if( sAction == "addtolist" )
{
var qs = new QueryString();
var itemObjectId = qs.get("listId","");
var itemObjectTypeCode = "";
var bMakeCall = false;
if ( qs.get("invoker","") != "listmembers" )
{
var lookupItems = LookupObjects( null, "single", "MemberTypeList", "4300", "0", null, "membertypecode=" + CrmEncodeDecode.CrmUrlEncode(iObjType), "1" );
if ( lookupItems )
{
if ( lookupItems.items.length > 0 )
{
itemObjectId = lookupItems.items[0].id;
itemObjectTypeCode = lookupItems.items[0].type;
bMakeCall = true;
}
}
}
else
{
bMakeCall = true;
}
if(bMakeCall)
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_addtolist.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams + "&itemObjectId=" + CrmEncodeDecode.CrmUrlEncode(itemObjectId) + "&itemObjectTypeCode=" + CrmEncodeDecode.CrmUrlEncode(itemObjectTypeCode), a, iX, iY );


oResult = false;
}
}
else if( sAction == "copyto" )
{
var lookupItems = LookupObjects( null, "single", "BasicList", "4300", 0, null, null, "1" );
if ( lookupItems )
{
if ( lookupItems.items.length > 0 )
{
var itemObjectId = lookupItems.items[0].id;
var itemObjectTypeCode = lookupItems.items[0].type;

oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_copyto.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams + "&itemObjectId=" + CrmEncodeDecode.CrmUrlEncode(itemObjectId) + "&itemObjectTypeCode=" + CrmEncodeDecode.CrmUrlEncode(itemObjectTypeCode), a, iX, iY );
}
}
}
else if (sAction == "copylistmember")
{
var qs = new QueryString();
_sListId = qs.get("oId","");
var sMemberType = GetListMemberType();
var itemObjectId = "";
var itemObjectTypeCode = "";
var bMakeCall = false;
var lookupItems = LookupObjects(null, "single","membertypelistwithomission", "4300",0,null,"id="+CrmEncodeDecode.CrmUrlEncode(_sListId)+"&membertypecode="+CrmEncodeDecode.CrmUrlEncode(sMemberType),"1");
if ( lookupItems )
{
if ( lookupItems.items.length > 0 )
{
itemObjectId = lookupItems.items[0].id;
itemObjectTypeCode = lookupItems.items[0].type;
bMakeCall = true;
}
}
if(bMakeCall)
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_addtolist.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&autoTrigger=1" + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams + "&itemObjectId=" + CrmEncodeDecode.CrmUrlEncode(itemObjectId) + "&itemObjectTypeCode=" + CrmEncodeDecode.CrmUrlEncode(itemObjectTypeCode), a, iX, iY );

oResult = false;
}
}
else if( sAction == "convertlead" )
{
var objRet = openStdDlg(prependOrgName("/SFA/Leads/Dialogs/conv_lead.aspx?showNew=NO"), null, 410, 400);
if ( objRet )
{
sCustParams = objRet.sCustParams;
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + sCustParams, a, iX, iY );
}
}
else if( sAction == "addtocampaign" )
{
var lookupItems = LookupObjects( null, "single", "BasicCampaign", "4400", "0");
if ( lookupItems )
{
if ( lookupItems.items.length > 0 )
{
iX=450;
iY=250;
itemObjectId = lookupItems.items[0].id;
itemObjectTypeCode = lookupItems.items[0].type;
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_addtocampaign.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams + "&itemObjectId=" + CrmEncodeDecode.CrmUrlEncode(itemObjectId) + "&itemObjectTypeCode=" + CrmEncodeDecode.CrmUrlEncode(itemObjectTypeCode), a, iX, iY );
oResult = false;
}
}
}
else if(sAction == "emailviamailmergeoff")
{
alert(LOCID_PROPAGATE_MM_EXE_OFF);
}
else if((sAction == "letterviamailmerge" ) || ( sAction == "faxviamailmerge" ) || ( sAction == "emailviamailmerge" ))
{

if( (iObjType == "4402") &&  (crmForm.statuscode.value != "1") )
{
alert(LOCID_PROPAGATE_ERROR);
}

else if( (iObjType == CampaignActivity) &&  (crmForm.IsDirty) )
{
alert(LOCID_PROPAGATE_DIRTY);
}
else
{
if (a.length != 0)
{











var mergetype = 0;
if ( sAction == "faxviamailmerge" )
{
mergetype = 5;
}
else if ( sAction == "emailviamailmerge" )
{
mergetype = 4;
}
openStdDlg(prependOrgName("/_grid/cmds/dlg_webmailmerge.aspx?objectTypeCode=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&objectId=" + CrmEncodeDecode.CrmUrlEncode(a[0]) + "&mergetype=" + CrmEncodeDecode.CrmUrlEncode(mergetype), null, 600, 500, true, false, "maximize:yes;minimize:yes");
}
}
}
else if( ( sAction == "listappointment" )||( sAction == "listletter" )||( sAction == "listfax" )||( sAction == "listphone" )||( sAction == "listemail" )||( sAction == "createopportunity" ))
{

sIds = a[0];
if( (iObjType == "4402") &&  (crmForm.statuscode.value != "1") )
{
alert(LOCID_PROPAGATE_ERROR);
}

else if( (iObjType == CampaignActivity) &&  (crmForm.IsDirty) )
{
alert(LOCID_PROPAGATE_DIRTY);
}
else
{
if (a.length != 0)
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, 820, 615, true, false, "maximize:yes;minimize:yes");

if ( iObjType == "4402" && oResult)
{
crmForm.SubmitCrmForm( 4, true, true, false);
}
}
}
}
else if( sAction == "qualifylist" )
{

a = getNotSelected(sGrid);

if (a.length != 0)
{
sIds = a[0];
iLen = a.length;
for(var i=1; i< iLen; i++)
{
sIds += ";" + a[i];
}
}
var o = document.frames[sGrid].frmGrid;
var viewId = crmGrid.GetParameter("viewid");
_sListId = crmGrid.GetParameter("oId");
var _fetchxml = null;
var _isDirty = false;
_fetchxml = crmGrid.GetParameter("listFetchXml");
_isDirty = crmGrid.GetParameter("isDirty");
if (_isDirty == null)
crmGrid.SetParameter("isDirty","false");
var oParams = "iListId="+CrmEncodeDecode.CrmUrlEncode(_sListId)+ "&iTotal="+CrmEncodeDecode.CrmUrlEncode(a.length)+"&sIds="+CrmEncodeDecode.CrmUrlEncode(sIds)+"&savedQueryId="+CrmEncodeDecode.CrmUrlEncode(viewId)+"&isDirty="+ CrmEncodeDecode.CrmUrlEncode(_isDirty);
oResult = OpenQualWin (_fetchxml, oParams, a);
}
else if(sAction == "changeCAstate")
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_deactivate.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + "&bulkChangeStateMode=1" + sCustParams, a, iX, iY );
}
else if(sAction == "changeTaskstate")
{
sIds = a;
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_deactivate.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + "&bulkChangeStateMode=1" + sCustParams, a, iX, iY );
}
else if(sAction == "deactivate")
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + "&bulkChangeStateMode=1" + sCustParams, a, iX, iY );
}
else if(sAction == "reactivateCR")
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_activate.aspx?iObjType=") + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + "&bulkChangeStateMode=1" + sCustParams, a, iX, iY );
}
else if (sAction == "share")
{

oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, iX, iY, true );
}
else if (sAction == "updatecustomstring")
{
if (a.length > 1)
{
alert(LOCID_CUSTMSG_TOOMANY_REC);
return;
}
oResult = openObj(DisplayString, a[0]);
}
else if (sAction == "applyrule")
{

sRet = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, iX, iY );
oResult = (sRet == null);

if(!oResult)
{
arySubType = getSelectedSubTypes(sGrid);
var len = arySubType.length;
for(var i = 0; i < len; i++)
{
if(sRet == arySubType[i][0])
{
document.all[sGrid].InnerGrid.UnselectRecords(arySubType[i][3]);
break;
}
}
oResult = false;
}
}
else if (sAction == "nochannel")
{

if( (iObjType == CampaignActivity) &&  (crmForm.IsDirty) )
{
alert(LOCID_PROPAGATE_DIRTY);
return;
}
alert(LOCID_PROPAGATE_ERROR_NOCHANNEL);
return;
}
else if (sAction == "invalidchannel")
{

if( (iObjType == CampaignActivity) &&  (crmForm.IsDirty) )
{
alert(LOCID_PROPAGATE_DIRTY);
return;
}
alert(LOCID_PROPAGATE_ERR_INVLIDCHNL);
return;
}
else if (sAction == "invalidmailmergechannel")
{

if( (iObjType == CampaignActivity) &&  (crmForm.IsDirty) )
{
alert(LOCID_PROPAGATE_DIRTY);
return;
}
alert(LOCID_P_ERR_INVLIDCHNL_MM);
return;
}
else if (sAction == "nochannelfield")
{

if( (iObjType == CampaignActivity) &&  (crmForm.IsDirty) )
{
alert(LOCID_PROPAGATE_DIRTY);
return;
}
alert(LOCID_PROPAGATE_ERR_NOCHNLFLD);
return;
}
else if (sAction == "nolistassociated")
{
if( (iObjType == CampaignActivity) &&  (crmForm.IsDirty) )
{
alert(LOCID_PROPAGATE_DIRTY);
return;
}
alert(LOCID_PROPAGATE_ERR_NOLISTASSOC);
return;
}
else if(sAction == "inviteusernotavailable")
{
openStdWin(LOCID_INVITE_NOTAVAILABLE_URL);
return;
}
else if (sAction == "inviteuser")
{
if (a.length ==1)
{
oResult = openStdDlg("../../_grid/cmds/dlg_sendinvite.aspx?userid="+ CrmEncodeDecode.CrmUrlEncode(a[0])+ "&isform=false", a, 450, 270);
document.all[sGrid].Refresh();
}
else
{
alert(LOCID_INVITE_MULTIPLE);
}

return;
}
else if (sAction == "promotetoadmin")
{
if (a.length ==1)
{
oResult = openStdDlg("../../_grid/cmds/dlg_promotetoadmin.aspx?userid="+ CrmEncodeDecode.CrmUrlEncode(a[0]), a, 300, 200);
}
else
{
alert(LOCID_PROMOTETOADMIN_MULTIPLE);
}
}
else if (sAction == "addusers")
{
oResult = openStdDlg(prependOrgName("/WebWizard/WizardContainer.aspx?WizardId=2631659F-A668-4A48-833B-D20E187B5A89"), null, 800, 530, true);
}
else if(sAction=="setstate_bulkdelete")
{
iObjType = AsyncOperation;
sAction = "setstate_asyncoperation";



var aGuids = new Array();
aGuids.type = "guid";
for (var i=0; i < a.length; i++)
{
aGuids.push(a[i]);
}

var command = new RemoteCommand("BulkDelete", "GetAsyncJobIds");
command.SetParameter("bulkDeleteIdList", aGuids);
var result = command.Execute();
if (result.ReturnValue.string != null)
{


var aId = (typeof(result.ReturnValue.string) == "string") ? new Array(result.ReturnValue.string) : result.ReturnValue.string;
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams,aId, iX, iY );
}

}
else
{
oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + "&sIds=" + CrmEncodeDecode.CrmUrlEncode(sIds) + sCustParams, a, iX, iY );
}
if ( oResult || sAction == "delete")
{

auto(iObjType);
}
}
}
function isQueryApiBasedView(viewId)
{
switch(viewId)
{
case "{C147F1F7-1D78-4D10-85BF-7E03B79F74FA}":
case "{CFBCD7AF-AEE5-4E45-8ECC-C040D4020581}":
case "{9818766E-7172-4D59-9279-013835C3DECD}":
case "{927E6CD8-B3ED-4C20-A154-B8BD8A86D172}":
case "{AFE23D8A-6651-474D-B8EE-90210A8231F6}":
case "{FE961BBB-E5EA-44BA-AFF3-DB1D8BBBA18B}":
return true;
}
return false;
}
function refreshGrid(sGrid)
{
document.all[sGrid].Refresh();
}
function launchOnDemandWorkflow(sGrid, iObjType,workflowId)
{
var itemObjectId = workflowId;
var iY = 200;
var	iX = 500;
var a = getSelected(sGrid);
if (!a || a.length == 0)
{
alert(LOCID_ACTION_NOITEMSELECTED);
return;
}
var selectedTypeCode = getCommonObjectType(sGrid);
if (IsNull(selectedTypeCode))
{
alert(LOCID_HETEROGENOUS_TYPES);
return;
}
if(IsNull(workflowId)||workflowId=="")
{
var lookupItems = LookupObjects( null, "single", "OnDemandWorkflow", "4703", "0", null, "membertypecode=" + CrmEncodeDecode.CrmUrlEncode(selectedTypeCode), "0");
if ( IsNull(lookupItems) ||  lookupItems.items.length == 0 )
{
return;
}
itemObjectId = lookupItems.items[0].id;
}

var oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_runworkflow.aspx")+"?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&iTotal=" +
CrmEncodeDecode.CrmUrlEncode(a.length) + "&wfId=" + CrmEncodeDecode.CrmUrlEncode(itemObjectId) , a, iX, iY );
oResult = false;
}
function doActionEx(sGrid, iObjType, sParentId, sAction, iParentType, sParams)
{
var a = getSelected(sGrid);
var iX = 400;
var iY = 200;
if ( iObjType == "4301" & sAction == "delete")
{
iX = 450;
}
if (!a || a.length == 0)
{
alert(LOCID_ACTION_NOITEMSELECTED);
}
else
{



if (!sParams)
{
sParams = "";
}
else if (sParams.charAt(0) != "&")
{
sParams = "&" + sParams;
}
if (sAction=="createopportunity")
{
if (openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&sParentId=" + CrmEncodeDecode.CrmUrlEncode(sParentId) + "&iParentType=" + CrmEncodeDecode.CrmUrlEncode(iParentType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + sParams, a, 800, 550, true, false, "maximize:yes;minimize:yes"))
{
document.all[sGrid].Refresh();
}
}
else
if (openStdDlg(prependOrgName("/_grid/cmds/dlg_") + CrmEncodeDecode.CrmUrlEncode(sAction) + ".aspx?iObjType=" + CrmEncodeDecode.CrmUrlEncode(iObjType) + "&sParentId=" + CrmEncodeDecode.CrmUrlEncode(sParentId) + "&iParentType=" + CrmEncodeDecode.CrmUrlEncode(iParentType) + "&iTotal=" + CrmEncodeDecode.CrmUrlEncode(a.length) + sParams, a, iX, iY))
{
document.all[sGrid].Refresh();
}
}
}
function locAssocOneToMany(iType, sRelationshipName)
{

var showNewButton = (iType == ActivityPointer) ? "0" : "1";
var showPropButton = (iType == ActivityPointer) ? "0" : "1";

var lookupItems = LookupObjects( null, "multi", "", iType, 0, null, null, showNewButton, showPropButton );
if (lookupItems)
{
if ( lookupItems.items.length > 0 )
{
var commandAssociate = new RemoteCommand("AssociateRecords", "AssociateOneToMany");

var i = 0;
var objs = lookupItems.items;
var iLength = objs.length;

for(i = 0; i < iLength; ++i)
{
commandAssociate.SetParameter("childType", iType);
commandAssociate.SetParameter("childId", objs[i].id);
commandAssociate.SetParameter("parentType", crmFormSubmit.crmFormSubmitObjectType.value);
commandAssociate.SetParameter("parentId", crmFormSubmit.crmFormSubmitId.value);
commandAssociate.SetParameter("relationshipName", sRelationshipName);

if (!commandAssociate.Execute().Success)
{
break;
}
}
}


try
{
auto(iType);
}
catch(e)
{
}
}
}
function locAssocObj(iType , sSubType, sAssociationName, iRoleOrdinal)
{
var lookupItems = LookupObjects( null, "multi", "", iType, 0 );
if (lookupItems)
{
if ( lookupItems.items.length > 0 )
{
AssociateObjects( crmFormSubmit.crmFormSubmitObjectType.value, crmFormSubmit.crmFormSubmitId.value, iType, lookupItems, iRoleOrdinal==2 , sSubType, sAssociationName);
}
}
}
function AssociateObjects( type1, id1, type2, objects, parentInitiated, sSubType , sAssociationName )
{
var commandAssociate = new RemoteCommand("AssociateRecords", "Associate");

if (IsNull(sAssociationName))
{
sAssociationName = "" ;
}


if (IsNull(sSubType))
{
sSubType = "";
}
else
{



var aTypes = sSubType.split('=');
if (aTypes.length == 2 && aTypes[0] == "subType")
{

sSubType = aTypes[1];
}
}

commandAssociate.SetParameter("subType", sSubType);

var i	= 0;
var objs = objects.items;
var iLength = objs.length;




if ( parentInitiated )
{
for ( i=0; i < iLength; i++ )
{
commandAssociate.SetParameter("objectType", type1);
commandAssociate.SetParameter("parentObjectType", type2);
commandAssociate.SetParameter("objectId", id1);
commandAssociate.SetParameter("parentId", objs[i].id);
commandAssociate.SetParameter("associationName", sAssociationName);
if (!commandAssociate.Execute().Success)
{
break;
}
}
}
else
{
for ( i=0; i < iLength; i++ )
{
commandAssociate.SetParameter("objectType", type2);
commandAssociate.SetParameter("parentObjectType", type1);
commandAssociate.SetParameter("objectId", objs[i].id);
commandAssociate.SetParameter("parentId", id1);
commandAssociate.SetParameter("associationName", sAssociationName);
if (!commandAssociate.Execute().Success)
{
break;
}
}
}


try
{
auto(type2);
}
catch(e)
{
}
}
function RunMiniCampaign(sGridName, iObjectTypeCode, iOption)
{
var oCrmGrid	= document.all[sGridName].InnerGrid;
var oArgs = new Object();
oArgs.ObjectTypeCode	= iObjectTypeCode;
oArgs.MCOption			= iOption;
oArgs.TotalRecords		= oCrmGrid.NumberOfRecords;
oArgs.SelectedRecords	= oCrmGrid.SelectedRecords.length;
oArgs.GridXml		   = "";
oArgs.Ids			   = "";

if(iOption == 1)
{
oArgs.MCOptionTitle = LOCID_MC_SELECTION_SELECTED;
oArgs.Ids = getSelected(sGridName);
}

else if(iOption == 2)
{
oArgs.MCOptionTitle = LOCID_MC_SELECTION_ALLONPAGE;
oArgs.Ids = getAll(sGridName);
}

else if(iOption == 3)
{
oArgs.MCOptionTitle = LOCID_MC_SELECTION_ALL;
oArgs.GridXml	   = document.all[sGridName].gridXml;
}
return openStdDlg(prependOrgName("/MA/MiniCampaign/MiniCampaign.aspx"), oArgs, parseInt(LOCID_MC_WINDOW_WIDTH,10), parseInt(LOCID_MC_WINDOW_HEIGHT,10), true, false, "maximize:yes;minimize:yes");
}
function DetectDuplicates(sGridName, iObjectTypeCode, iOption)
{
var oCrmGrid	= document.all[sGridName].InnerGrid;
var oArgs = new Object();
oArgs.ObjectTypeCode	= iObjectTypeCode;
oArgs.GridXml           = "";
oArgs.Ids               = "";
oArgs.iOption           = iOption;
oArgs.ViewName          = "";


if(iOption == 1)
{
oArgs.Ids = getSelected(sGridName);
}

else if(iOption == 2)
{
oArgs.GridXml = document.all[sGridName].gridXml;






var viewSelector = document.getElementById("SavedQuerySelector");
if( !IsNull(viewSelector))
{
oArgs.ViewName = viewSelector.SelectedOption.getAttribute("Text");
}
}
return openStdDlg(prependOrgName("/Tools/DuplicateDetection/SystemWideDuplicateDetection/SystemWideDuplicateDetection.aspx?option=")+CrmEncodeDecode.CrmUrlEncode(iOption), oArgs, parseInt(LOCID_DEDUPE_WINDOW_WIDTH,10), parseInt(LOCID_DEDUPE_WINDOW_HEIGHT,10), true);
}
function SendBulkEmail(sGridName, iObjectTypeCode)
{
var oCrmGrid	= document.all[sGridName].InnerGrid;
var oArgs = new Object();
if (oCrmGrid.NumberOfRecords > 0)
{
oArgs.TotalRecords		= oCrmGrid.NumberOfRecords;
oArgs.SelectedRecords	= oCrmGrid.SelectedRecords.length;
oArgs.Ids				= ((oArgs.SelectedRecords > 0) ? getSelected(sGridName) : null);
oArgs.GridXml		   = document.all[sGridName].gridXml;
openStdDlg(prependOrgName("/_grid/cmds/dlg_bulkemail.aspx?bulkemail=true&multiPage=false&objectTypeCode=") + CrmEncodeDecode.CrmUrlEncode(iObjectTypeCode), oArgs, 600, 600);
}
else
{
alert(LOCID_EMAIL_NORECORDS_MSG);
}
}
function WebMailMerge(sGridName, iObjectTypeCode)
{
var oCrmGrid	= document.all[sGridName].InnerGrid;
var oArgs = new Object();
if (oCrmGrid.NumberOfRecords > 0)
{
oArgs.TotalRecords		= oCrmGrid.NumberOfRecords;
oArgs.SelectedRecords	= oCrmGrid.SelectedRecords.length;
oArgs.Ids				= ((oArgs.SelectedRecords > 0) ? getSelected(sGridName) : null);
oArgs.GridXml		   = document.all[sGridName].gridXml;
openStdDlg(prependOrgName("/_grid/cmds/dlg_webmailmerge.aspx?objectTypeCode=") + CrmEncodeDecode.CrmUrlEncode(iObjectTypeCode), oArgs, 600, 600);
}
else
{
alert(LOCID_ACTION_NOITEMSELECTED);
}
}
function getAll(sGridName)
{
var a = document.all[sGridName].InnerGrid.AllRecords;
var backCompatArray = new Array(a.length);
for (var i=0; i < a.length; i++)
{
backCompatArray[i] = a[i][0];
}
return backCompatArray;
}
function QueryString()
{

var querystring=location.search.substring(1,location.search.length);

var args = querystring.split('&');

for(var i=0; i<args.length; i++)
{
var pair = args[i].split('=');
temp = CrmEncodeDecode.CrmUrlDecode(pair[0]);
this[temp] = CrmEncodeDecode.CrmUrlDecode(pair[1]);
}
this.get = QueryString_get;
}
function QueryString_get(strKey, strDefault)
{
var value = this[strKey];
if(value == null)
{
value = strDefault;
}
return value;
}
function IsSelectedJobCompleted()
{
try{
var selectedItems = getSelectedSubTypes("crmGrid");
var subtype;
for(var i=0;i<selectedItems.length;i++)
{
subtype=selectedItems[i][3].statecode;
if(subtype == 3)
{
alert(LOCID_POSTPONE_COMPLETED_JOB);
return true;
}
}
}
catch(e)
{
}
return false;
}
function SendCurrentViewUrl(bUsingEmail )
{

var el = document.getElementById( "SavedQuerySelector");

if( IsNull(el) || el.DataValue == LOCID_SEARCH_RESULTS )
{
alert( LOCID_ERROR_INVALID_VIEW );
return;
}

var isUserOwned = el.SelectedOption.getAttribute( "isUserOwned" );
if( !IsNull( isUserOwned) && isUserOwned == "true" )
{
alert( LOCID_ERROR_INVALID_VIEW );
return;
}

var sb = new StringBuilder();
var sSubject = el != null ? el.InnerText : "";
sb.Append( sSubject);
sb.Append( "\r\n");
sb.Append( "<");
sb.Append( GetViewUrl() );
sb.Append( ">");


if( !bUsingEmail)
{
CopyTextToClipboard(sb.ToString(), "", LOCID_COPY_SHORTCUT_ERROR );
}
else
{
OpenEmailForm( "", sSubject, sb.ToString() );
}
}
function SendSelectedRecordsUrl(bUsingEmail,sGrid, iObjType)
{
var oGrid = document.getElementById( sGrid );
var arrSelectedRecords = oGrid.InnerGrid.SelectedRecords;
var primaryColumnIndex = oGrid.InnerGrid.PrimaryFieldColumnIndex;

if( IsNull( arrSelectedRecords) || arrSelectedRecords.length == 0 )
{
alert( LOCID_ACTION_NOITEMSELECTED );
return;
}


if( arrSelectedRecords.length > 10 )
{
alert( LOCID_MAX_RECORDS_ERROR );
return;
}

var sUrl = window.location.href;

if( IsOutlookLaptopClient() && !IsOnline() )
{
sUrl = WEB_APP_URL;
}
else
{
sUrl = sUrl.substring( 0, sUrl.indexOf( window.location.pathname ) );

sUrl = appendOrgName( sUrl );
}



var bWriteUrlOnly = !bUsingEmail && (arrSelectedRecords.length == 1);

var sb = new StringBuilder();
for( var i = 0; i < arrSelectedRecords.length; i++)
{
var iObjectType = arrSelectedRecords[i][1];
var sUrlPath = getObjUrl(iObjectType);

if( IsNull(sUrlPath) )
{
continue;
}


var sId = arrSelectedRecords[i][0];

var oTr = arrSelectedRecords[i][3];

var sSubject = oTr.cells[primaryColumnIndex].firstChild.innerText;

if(!bWriteUrlOnly)
{
if( i > 0 )
{
sb.Append("\r\n\r\n");
}
sb.Append( sSubject );
sb.Append("\r\n");


sb.Append("<");
}

sb.Append( sUrl );
sb.Append( '/' + sUrlPath );


switch ( Number(iObjectType) )
{
case UserQuery:

sb.Append("?");
var oQueryData = GetQueryData( iObjectType, sId );
if( !IsNull(oQueryData) )
{
sb.Append("etn=" + CrmEncodeDecode.CrmUrlEncode(oQueryData.ReturnType) + "&") ;
}
sb.Append("QueryId=" + CrmEncodeDecode.CrmUrlEncode(sId));
sb.Append("&ViewType=" + CrmEncodeDecode.CrmUrlEncode(iObjectType) );
sb.Append("&AutoRun=True");
break;

default:
sb.Append( "?id=" );
sb.Append( CrmEncodeDecode.CrmUrlEncode( sId ) );

if(IsUserDefinedEntityObjectTypeCode(iObjectType))
{
sb.Append( "&etc=" );
sb.Append( CrmEncodeDecode.CrmUrlEncode(iObjectType) );
}
break;
}

if(!bWriteUrlOnly)
{

sb.Append(">");
}
}

if( !bUsingEmail)
{
CopyTextToClipboard(sb.ToString(), "", LOCID_COPY_SHORTCUT_ERROR );
}
else
{
sSubject = arrSelectedRecords.length==1 ? sSubject : "";
OpenEmailForm( "", sSubject, sb.ToString() );
}
}
