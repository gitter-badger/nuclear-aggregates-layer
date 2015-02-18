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
