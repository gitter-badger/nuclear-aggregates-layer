





var SelectedColorFocused = "#c4ddff";
var SelectedColorUnfocused = "#eeeeee";


var LookupBrowse = 0x1;
var LookupShowColumns = 0x2;
var LookupMultiSelect = 0x4;

function checkMode(mode, option) {
    return ((mode & option) == option);
}


function lookupSearch() {
    search();
}

function search() {
    document.body.style.cursor = "wait";
    crmGrid.SetParameter("Mode", _mode);
    crmGrid.SetParameter("ObjectType", selObjects.value);

    if (!checkMode(_mode, LookupBrowse)) {
        crmGrid.SetParameter("searchvalue", findValue.value);
        findValue.focus();
        findValue.NotifyFocus();
    }


    crmGrid.ClearPagingCookie();
    crmGrid.PageNumber = 1;



}

function selectChange(o) {
    var objectType = o.DataValue;
    var guid = o.options[o.selectedIndex].guid

    var selectTypeCode = parseInt(o.DataValue, 10);
    btnNew.disabled = _canCreate[selectTypeCode] !== true;

    crmGrid.SetParameter("viewid", guid);
    if (checkMode(_mode, LookupBrowse)) {
        crmGrid.SetParameter("searchvalue", "");
    }
    crmGrid.SetParameter("ObjectType", objectType);


    if (IsActivityTypeCode(objectType)) {
        crmGrid.SetParameter("DataProviderOverride", "Microsoft.Crm.Application.Controls.ActivitiesGridDataProvider");

        crmGrid.SetParameter("SourceIsLookup", "true");
    }
    else {
        crmGrid.SetParameter("DataProviderOverride", "Microsoft.Crm.Application.Controls.LookupGridDataProvider");
    }

    crmGrid.Reset();
}

function buildReturnValue(rows) {
    var lookupItems = new LookupItems();

    var len = rows.length;

    for (var i = 0; i < len; i++) {
        var tr = rows[i][3];
        var columns = tr.parentElement.parentElement.getElementsByTagName("col");



        if (tr.oid != undefined) {
            lookupItems.add(tr, columns)
        }
    }

    return lookupItems;
}

function getActiveItem(elem) {
    while (elem.tagName != "TR") {
        elem = elem.parentElement;
    }

    return elem;
}

function findValueKeyDown() {
    if (event.keyCode == 13) {
        search();
    }
}

function activateItems() {
    if (!this.contains(event.toElement)) {
        focusSelectedItems(this, true);
    }
}

function deactivateItems() {
    if (!this.contains(event.toElement)) {
        focusSelectedItems(this, false);
    }
}

function showProperties() {
    var items = crmGrid.InnerGrid.SelectedRecords;

    if (items.length == 0) {
        alert(LOCID_SELECT_AN_OBJECT);
    }
    else if (items.length > 1) {
        alert(LOCID_SELECT_ONE_OBJECT);
    }
    else {
        var nWidth = 560;
        var nHeight = 525;
        var oWindowInfo = GetWindowInformation(items[0][3].otype);
        if (oWindowInfo != null) {
            nWidth = oWindowInfo.Width;
            nHeight = oWindowInfo.Height;
        }

        switch (Number(items[0][3].otype)) {
            case Service:
                openStdWin(prependOrgName("/sm/services/readonly.aspx?objTypeCode=" + items[0][3].otype + "&id=" + items[0][3].oid), "readonly" + buildWinName(items[0][3].oid), nWidth, nHeight);
                break;
            case Workflow:
                openObj(items[0][3].otype, items[0][3].oid);
                break;
            case ImportMap:
                openStdWin(prependOrgName("/tools/managemaps/readonly.aspx?objTypeCode=" + items[0][3].otype + "&id=" + items[0][3].oid), "readonly" + buildWinName(items[0][3].oid), nWidth, nHeight);
                break;
            default:
                /*CRM Hack*/
                if (!(openIntegrateWindow && openIntegrateWindow(items[0][3].otype, items[0][3].oid) === true))
                    openStdWin(prependOrgName("/_forms/readonly/readonly.aspx?objTypeCode=" + items[0][3].otype + "&id=" + items[0][3].oid), "readonly" + buildWinName(items[0][3].oid), nWidth, nHeight);
                /*End CRM Hack*/
                break;
        }
    }
}


function createNew() {


    var iTypeCode = parseInt(selObjects.value, 10);
    var sParams = "";
    var oQueryString = ParseQueryString();


    if (!IsNull(oQueryString["parentId"]) && !IsNull(oQueryString["parentType"])) {


        if (iTypeCode === CustomerAddress) {
            sParams += "objecttypecode=" + oQueryString["parentType"];
            sParams += "&parentid=" + oQueryString["parentId"];
        }
        else {
            sParams += "_CreateFromType=" + oQueryString["parentType"];
            sParams += "&_CreateFromId=" + oQueryString["parentId"];
        }
    }


    if (sParams === "") {
        sParams = null;
    }

    openObj(iTypeCode, null, sParams);
}


function LookupItem() {

    this.id = "";
    this.name = "";
    this.html = "";
    this.type = "";
    this.values = null;
    this.keyValues = null;
}

function LookupItemData(name, value) {

    this.name = name;
    this.value = value;
}

function LookupItems() {

    this.add = _add;
    this.items = new Array();


    function _add(tr, columns) {
        var li = new LookupItem();
        var td = tr.cells[1];

        li.id = tr.oid;
        li.name = td.innerText;
        li.html = td.innerHTML;
        li.type = tr.otype;
        li.values = new Array();
        li.keyValues = new Array();




        var len = columns.length;
        if (len > 1) {
            for (var i = 1; i < len; ++i) {
                var cellValue = "";

                if (tr.cells[i] != null) {
                    cellValue = tr.cells[i].innerText;
                }

                if (tr.attributes[columns[i].name] != null) {
                    cellValue = tr.attributes[columns[i].name].value;
                }

                li.keyValues[new String(columns[i].name)] = new LookupItemData(columns[i].name, cellValue);
                li.values.push(new LookupItemData(columns[i].name, cellValue));
            }
        }

        this.items.push(li);
    }
}


function RenderToolTip() {
    var o = event.srcElement;

    if (o && o.tagName == "NOBR" && o.title == "" && o.innerText != "") {
        o.title = o.innerText;
    }
}

function lookupPageReady() {
    if (document.readyState == "complete") {
        setNavigationState();
    }
    else {
        window.setTimeout("lookupPageReady", 500);
    }
}
