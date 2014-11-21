<%@ Page language="c#" Inherits="Microsoft.Crm.Application.Pages.UserDefined.DetailPage"    %>
<%@ Register TagPrefix="loc" Namespace="Microsoft.Crm.Application.Controls.Localization" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="frm" Namespace="Microsoft.Crm.Application.Forms" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="mnu" Namespace="Microsoft.Crm.Application.Menus" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="cnt" Namespace="Microsoft.Crm.Application.Controls" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="sdk" Namespace="Microsoft.Crm.Application.Components.Sdk.FormControls.Web" Assembly="Microsoft.Crm.Application.Components.Sdk.FormControls" %>
<%@ Import Namespace="Microsoft.Crm"%>
<%@ Import Namespace="Microsoft.Crm.Application.Types"%>
<html>
<head>
<script type="text/javascript" src="/_static/_controls/lookup/ext-base.js"></script>
<script type="text/javascript" src="/_static/_controls/lookup/ext-all.js"></script>
<cnt:AppHeader id="crmHeader" runat="server" />
<script language="JavaScript">
    /* CRM Hack */
    function hideMenuItem(targetMenuItem) {
        var menu = document.getElementById("mnuBar1");
        if (menu) {
            var menuLIs = menu.getElementsByTagName("LI");
            for (var i = 0; i < menuLIs.length; i++) {
                if (menuLIs[i].title && menuLIs[i].title == targetMenuItem ) {
                    menuLIs[i].style.display = "none";
                    return;
                }
            }
        }
    }

    function hideDropDownItem(targetMenu, targetMenuItem) {
        var menu = document.getElementById("mnuBar1");
        if (menu) {
            var menuLIs = menu.getElementsByTagName("LI");
            for (var i = 0; i < menuLIs.length; i++) {
                if (menuLIs[i].title && menuLIs[i].title.indexOf(targetMenu) > -1) {
                    var targetDivs = menuLIs[i].getElementsByTagName("DIV");
                    for (var j = 0; j < targetDivs.length; j++) {
                        var targetLIs = targetDivs[j].getElementsByTagName("LI");
                        for (var k = 0; k < targetLIs.length; k++) {
                            if (targetLIs[k].innerHTML.indexOf(targetMenuItem) > -1) {
                                targetLIs[k].style.display = "none";
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    function addMenuItem(id, iconPath, title, tooltip, order, action) {
        var ulbar = Ext.query(".ms-crm-MenuBar-Left")[0];
        if (ulbar) {
            var menuLIs = Ext.query("LI.ms-crm-Menu", ulbar);
            order = order > menuLIs.length ? menuLIs.length-1 : order;
            var nextItem = menuLIs[order+1];

            var newItem = document.createElement("LI");
            if (nextItem) {
                ulbar.insertBefore(newItem, nextItem);
            }
            else {
                ulbar.appendChild(newItem);
            }

            newItem.setAttribute("tabindex", "-1");
            newItem.title = title;
            newItem.className = "ms-crm-Menu";
            newItem.onclick = function () { window.execScript(action); };
            newItem.setAttribute("action", action);
            newItem.id = id;

            var span = document.createElement("span");
            newItem.appendChild(span);
            span.className = "ms-crm-Menu-Label";

            var aLabel = document.createElement("a");
            span.appendChild(aLabel);
            aLabel.setAttribute("tabindex", "-1");
            aLabel.className = "ms-crm-Menu-Label";
            aLabel.onclick = function(){return false;};
            aLabel.href = "javascript:onclick();";
            aLabel.target = "_self";

            var icon = document.createElement("img");
            icon.alt = tooltip;
            icon.src = iconPath;
            aLabel.appendChild(icon);
            icon.setAttribute("tabindex", "-1");
            icon.className = "ms-crm-Menu-ButtonFirst";


            var txtSpan = document.createElement("span");
            aLabel.appendChild(txtSpan);
            txtSpan.setAttribute("tabIndex", "0");
            txtSpan.className = "ms-crm-MenuItem-TextRTL";
            txtSpan.innerText = title;
        }
    }

    function addDropDownItem(appendTo, id, title, tooltip, order, action) {
        var ulbar = Ext.query("LI.ms-crm-Menu");
        var parent = undefined;
        for (var i = 0; i < ulbar.length; i++) {
            if (ulbar[i].title && ulbar[i].title == appendTo) {
                parent = ulbar[i];
                break;
            }
        }
        if (parent) {
            var parentUL = parent.lastChild.lastChild;
            var menuLIs = parent.lastChild.lastChild.childNodes;
                order = order > menuLIs.length ? menuLIs.length - 1 : order;
                
                var nextItem = menuLIs[order + 1];

                var newItem = document.createElement("LI");
                if (nextItem) {
                    parentUL.insertBefore(newItem, nextItem);
                }
                else {
                    parentUL.appendChild(newItem);
                }

                newItem.setAttribute("tabindex", "-1");
                newItem.className = "ms-crm-MenuItem-Label";
                newItem.setAttribute("action", action);
                newItem.id = id;

                var span = document.createElement("span");
                newItem.appendChild(span);
                span.className = "ms-crm-MenuItem-Label";

                var aLabel = document.createElement("a");
                span.appendChild(aLabel);
                aLabel.setAttribute("tabindex", "-1");
                aLabel.className = "ms-crm-MenuLink";
                aLabel.onclick = function () { return false; };
                aLabel.href = "javascript:onclick();";
                aLabel.target = "_self";

                var icon = document.createElement("span");
                aLabel.appendChild(icon);
                icon.className = "ms-crm-MenuItem-Icon";


                var txtSpan = document.createElement("span");
                aLabel.appendChild(txtSpan);
                txtSpan.setAttribute("tabIndex", "0");
                txtSpan.className = "ms-crm-MenuItem-Text";
                txtSpan.innerText = title;
            }
    }

    function bindMenuItemHandler(targetMenuItem, scriptText) {
        var menu = document.getElementById("mnuBar1");
        if (menu) {
            var menuLIs = menu.getElementsByTagName("LI");
            for (var i = 0; i < menuLIs.length; i++) {
                if (menuLIs[i].title && menuLIs[i].title==targetMenuItem) {
                    targetLIs[k].setAttribute("action", scriptText);
                    return;
                }
            }
        }

    }

    function bindDropDownItemHandler(targetMenu, targetMenuItem, scriptText) {
        var menu = document.getElementById("mnuBar1");
        if (menu) {
            var menuLIs = menu.getElementsByTagName("LI");
            for (var i = 0; i < menuLIs.length; i++) {
                if (menuLIs[i].title && menuLIs[i].title==targetMenu) {
                    var targetDivs = menuLIs[i].getElementsByTagName("DIV");
                    for (var j = 0; j < targetDivs.length; j++) {
                        var targetLIs = targetDivs[j].getElementsByTagName("LI");
                        for (var k = 0; k < targetLIs.length; k++) {
                            if (targetLIs[k].innerHTML.indexOf(targetMenuItem) > -1) {
                                targetLIs[k].setAttribute("action", scriptText);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    /* End CRM Hack */
</script>

<script language="JavaScript">




    function locAddActTo(iActivityType) {
        var sParentId = null;
        var sParentType = null;
        var sParentName = null;
        var sPartyId = null;
        var sPartyType = null;
        var sPartyName = null;
        var sPartyLocation = null;


        sParentId = crmFormSubmit.crmFormSubmitId.value;
        sParentType = crmFormSubmit.crmFormSubmitObjectType.value;


        sParentName = "<%=( crmForm.State == FormState.New ? String.Empty : Microsoft.Crm.CrmEncodeDecode.CrmJavaScriptEncodeNoQuotes( crmForm.GetDataValue( crmForm.Entity.Metadata.PrimaryField.LogicalName ) ) )%>";

        addActivityTo(iActivityType, sParentId, sParentType, sParentName, sPartyId, sPartyType, sPartyName, sPartyLocation);
    }

    function locAssocObj(iType, sSubType, sAssociationName, iRoleOrdinal) {
        var lookupItems = LookupObjects(null, "multi", "", iType, 0);

        if (lookupItems) {
            if (lookupItems.items.length > 0) {
                iRoleOrdinal = (IsNull(iRoleOrdinal) ? -1 : iRoleOrdinal);
                AssociateObjects(crmFormSubmit.crmFormSubmitObjectType.value, crmFormSubmit.crmFormSubmitId.value, iType, lookupItems, (iRoleOrdinal == -1 ? true : (iRoleOrdinal == 2)), sSubType, sAssociationName);
            }
        }
    }
</script>
</head>
<body>
<table class="ms-crm-Form-Layout" cellspacing="0" cellpadding="0">
<col width="<loc:Text Encoding='HtmlAttribute' ResourceId='DetailForm_Left_Navigation_Width' runat='server'/>"><col>
<tr height="92">
<td colspan="2">
<mnu:AppFormMenuBar id="crmMenuBar" runat="server"/>
</td>
</tr>
<tr class="ms-crm-Form-Background">
<td class="ms-crm-Form-LeftBar">
<cnt:AppNavigationBar id="crmNavBar" DefaultItemID="navInfo" runat="server"/>
</td>
<td id="tdAreas">
<div id="areaForm" class="ms-crm-Form-Area">
<frm:CrudForm id="crmForm" runat="server" />
</div>
</td>
</tr>
<tr>
<td class="ms-crm-Form-StatusBar" colspan="2">
<sdk:RenderStatusControl id="crmRenderStatus" runat="server" />
</td>
</tr>
</table>

</body>
</html>
