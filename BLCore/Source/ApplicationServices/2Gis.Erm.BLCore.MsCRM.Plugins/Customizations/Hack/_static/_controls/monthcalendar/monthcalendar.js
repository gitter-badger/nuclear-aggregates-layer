function Refresh(type)
{
if (frmGrid.ObjectTypeId.value == type)
{
frmGrid.submit();
}
}


function openCrmEntity( oSpan, sParams )
{
    openObj(oSpan.itemType, oSpan.itemId);
}


function openCrmObj(sUrl, sName,  oSpan)
{
var oWindowInfo = GetWindowInformation(oSpan.itemType);
var rawUrl = oWindowInfo.Url;
var iX	= oWindowInfo.Width;
var iY	= oWindowInfo.Height;
var url = prependOrgName("/") + rawUrl + sUrl;

var clickDate = oSpan.parentElement.parentElement.parentElement.parentElement.parentElement.parentElement.d;

switch (Number(oSpan.itemType))
{
case ServiceRestrictionCalendarRule:
openStdDlg( url + '&oType=' + frmGrid.oType.value+'&calendarId='+frmGrid.calendarId.value, top.window, iX, iY);
break;

case TimeOffCalendarRule:
openStdDlg( url+'&calendarId='+frmGrid.calendarId.value+'&resourceId='+frmGrid.oId.value + '&oType=' + frmGrid.oType.value+'&name=Time off&mode=Edit', top.window, iX, iY);
break;

case OccurrenceCalendarRule:
case RecurrenceCalendarRule:
openStdDlg( url+'&calendarId='+frmGrid.calendarId.value+'&innerCalendarId='+frmGrid.oId.value + '&oType=' + frmGrid.oType.value+'&name=Working Hours&mode=Edit', top.window, iX, iY);
break;

case HolidayCalendarRule:
if (openStdDlg( url+'&mode=Edit', top.window, iX, iY))
{

window.document.frames.parent.auto(CalendarRule);
}
break;

case OccurringWorkShift:

if (openStdDlg( url+'&calendarId='+oSpan.itemId+'&resourceId='+frmGrid.oId.value + '&oType=' + frmGrid.oType.value + '&selecteddates='+clickDate+'&mode=Edit', null, iX, iY))
{

window.document.frames.parent.auto(CalendarRule);
}
break;

case NotWorkingWorkShift:
case RecurringWorkShift:
var resultValue = openStdDlg(prependOrgName('/SM/workplans/Dialogs/changeschedule.aspx?id='+oSpan.subId+'&calendarid='+oSpan.itemId+'&resourceId='+frmGrid.oId.value+'&date='+clickDate), null, iX, iY);
if (resultValue != null)
{
var selectedValue = resultValue.value;
if (selectedValue == 0)
{

var oWindowInfoTS = GetWindowInformation(OccurringWorkShift);
var iXTS	= oWindowInfoTS.Width;
var iYTS	= oWindowInfoTS.Height;
if (openStdDlg(prependOrgName('/SM/workplans/Dialogs/timesheet.aspx?id=&calendarId=' + frmGrid.calendarId.value + '&resourceId=' + frmGrid.oId.value + '&oType=' + frmGrid.oType.value + '&selecteddates=' + clickDate), null, iXTS, iYTS))
{

window.document.frames.parent.auto(CalendarRule);
}
}
else if (selectedValue == 1)
{
var copiedRuleId = resultValue.idValue;
if (copiedRuleId.length > 0)
{

openStdWin(prependOrgName("/") + rawUrl + '?id='+copiedRuleId+'&calendarId='+frmGrid.calendarId.value+'&resourceId='+frmGrid.oId.value + '&oType=' + frmGrid.oType.value + '&selecteddates='+clickDate+'&name=Working Hours&mode=New', 'Calendar', iX, iY);
}
else
{

openStdWin(prependOrgName("/") + rawUrl + '?id=&calendarId='+frmGrid.calendarId.value+'&resourceId='+frmGrid.oId.value + '&oType=' + frmGrid.oType.value +'&selecteddates='+clickDate+'&name=Working Hours&mode=New', 'Calendar', iX, iY);
}
}
else if (selectedValue == 2)
{
openStdWin(url+'&calendarId='+frmGrid.calendarId.value+'&resourceId='+frmGrid.oId.value + '&oType=' + frmGrid.oType.value+'&date='+clickDate+'&mode=Edit', sName, iX, iY);
}
}
break;

case None:

break;

default:
openStdWin( url, sName);
}
}


function appendQueryString(initialQueryString, keys, values)
{
var queryString = initialQueryString;

if (IsNull(keys) || IsNull(values) || keys == "")
{
return queryString;
}

var keysArray = keys.split(",");
var valuesArray = values.split(",");

if (keysArray.length != valuesArray.length)
{
return queryString;
}

for (var i = 0; i < keysArray.length; i++)
{
queryString += (queryString.length == 0)? "?" : "&";
queryString += keysArray[i] + "=" + valuesArray[i];
}

return queryString;
}

function Delete(sCalendar, sNotSelectedError, iCalendarType)
{
var selectedItems = new Array();
var itemTypes = new Array();
var itemIds = new Array();
var sCustParams = "";
var iObjType = Calendar;
var distinctTypes = false;

if (!IsNull(iCalendarType))
{
if (frmGrid.selectedItemType.value != "" && frmGrid.selectedItem.value != "")
{
itemTypes[0] = MapVirtualToRealTypes(frmGrid.selectedItemType.value);
itemIds[0] = frmGrid.selectedItem.value.toUpperCase();
iObjType = itemTypes[0];
}
}
else
{
selectedItems = document.frames.parent.getSelectedSubTypes(sCalendar);
for (var i = 0; i < selectedItems.length; i++)
{
itemTypes[i] = MapVirtualToRealTypes(selectedItems[i][1]);
if (i > 0)
{
if ((!distinctTypes) && (itemTypes[i-1] != itemTypes[i]))
{
distinctTypes = true;
}
}
itemIds[i] = selectedItems[i][0];
}

if (distinctTypes)
{
iObjType = QueueItem;
}
else
{
iObjType = itemTypes[0];
}
}

if (itemIds.length == 0)
{
alert(sNotSelectedError);
return;
}

sCustParams = "&sSubTypes=" + itemTypes.join(",");
sCustParams += "&sCalendarId=" + frmGrid.calendarId.value;

oResult = openStdDlg(prependOrgName("/_grid/cmds/dlg_delete.aspx?iObjType=" + iObjType + "&iTotal=" + itemIds.length + sCustParams), itemIds, 450, 250 );

if (oResult)
{

document.frames.parent.auto(CalendarRule);
}
}


function MapVirtualToRealTypes(iItemType)
{
switch (Number(iItemType))
{
case HolidayCalendarRule:
case TimeOffCalendarRule:
return CalendarRule;
break;

case OccurringWorkShift:
case NotWorkingWorkShift:
case RecurringWorkShift:
return Calendar;
break;

case None:
return 0;
break;

default:
return iItemType;
}

}
