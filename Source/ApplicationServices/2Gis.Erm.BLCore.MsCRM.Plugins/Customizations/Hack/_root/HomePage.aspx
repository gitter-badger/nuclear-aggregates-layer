<%@ Page language="c#" Inherits="Microsoft.Crm.Web.EntityHomePage" CodeBehind="Microsoft.Crm.Application.Pages.dll" %>
<%@ Register TagPrefix="mnu" Namespace="Microsoft.Crm.Application.Menus" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="cnt" Namespace="Microsoft.Crm.Application.Controls" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="loc" Namespace="Microsoft.Crm.Application.Controls.Localization" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Import Namespace="Microsoft.Crm.Application.Pages.Common" %>
<html>
<head>
<cnt:AppHeader runat="server" id="crmHeader"/>

<script language="JavaScript">
/* CRM Hack */
function hideMenuItem(targetMenuItem) {
    var menu = document.getElementById("mnuBar1"); 
    if(menu){ 
        var menuLIs = menu.getElementsByTagName("LI");
	for (var i = 0; i < menuLIs.length; i++) {
		if (menuLIs[i].title && menuLIs[i].title.indexOf(targetMenuItem) > -1) {
			menuLIs[i].style.display = "none";
			return;
		}
	}
    }
}
function hideDropDownItem(targetMenu, targetMenuItem) {
    var menu = document.getElementById("mnuBar1"); 
    if(menu){ 
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
function hideById(id) {
    var element = document.getElementById(id);
    if (!element) {
        return;
    }
    element.style.display = "none";
}

/* End CRM Hack */
function window.onload()
{
    // Разрешаем прокрутку колесиком в комбобоксе выбора представления.
    if(SavedQuerySelector)
    {
        SavedQuerySelector.onmousewheel = function() {};
    }
    HandleBackButtonIssues(_currentTypeCode);
    /* CRM Hack - удаляем определённые кнопки на всех тулбарах всех сущностей */
    hideMenuItem("Назначение");
    hideMenuItem("Удалить");
    hideMenuItem("Слияние...");
    hideDropDownItem("Другие действия", "Сделать неактивным");
    hideDropDownItem("Другие действия", "Активировать");
    hideById("_MBopenObj10006"); // Кнопка "Создать" в гриде "Лимиты"
    hideById("_MBopenObj10011"); // Кнопка "Создать" в гриде "Юр. лица клиента"
    hideById("_MBopenObj10013"); // Кнопка "Создать" в гриде "Фирма"
    hideById("_MBopenObj10023"); // Кнопка "Создать" в гриде "Лицевой счет"
    /* End CRM Hack */
}
</script>

</head>

<body class="stage">

<table class="stdTable" cellpadding="0" cellspacing="0">
<tr height="34">
<td>

<table class="homepage_table" width="100%" cellpadding="0" cellspacing="0">
<col width="60%"><col width="20"><col><col width="40%">
<tr>
<td><cnt:AppQuickFind id="crmQuickFind" runat="server"/></td>
<td align="center"><span class="homepage_span">&nbsp;</span></td>
<td nowrap class="homepage_td"><span style="color:#000000;font-weight:<%= CrmStyles.GetFontResourceAttribute("General.Bold.font_weight") %>;"><loc:Text ResourceId="Web.View_Label_70" runat="server" ID="Text1"/></span></td>
<td><cnt:AppViewSelector runat="server" id="crmViewSelector"/></td>
</tr>
</table>
</td>
</tr>
<tr height="25">
<td><mnu:AppGridMenuBar id="crmMenuBar" runat="server"/></td>
</tr>
<tr>
<td ><cnt:AppGrid runat="server" id="crmGrid" /></td>
</tr>
</table>

</body>
</html>