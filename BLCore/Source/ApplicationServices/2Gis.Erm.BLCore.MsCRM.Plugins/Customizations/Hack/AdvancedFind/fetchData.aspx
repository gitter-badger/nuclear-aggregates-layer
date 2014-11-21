<%@ Page Inherits="Microsoft.Crm.Web.AdvancedFind.FetchData" Language="c#" %>
<%@ Register TagPrefix="mnu" Namespace="Microsoft.Crm.Application.Menus" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="cnt" Namespace="Microsoft.Crm.Application.Controls" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Import Namespace="Microsoft.Crm.Application.Utility" %>
<html>
<head>
<cnt:AppHeader runat="server" id="crmHeader"/>
</head>

<body style="padding-top:0px;padding:10px;background-color:#fafafd;">
    
<!-- CRM Hack -->
<script language="javascript">

    var Spacer = {
        Right: 1, // hides a right spacer if it exists
        Left: 2, // hides a left spacer if it exists
        Both: 3, // and so on...
        None: 4
    };

    var Display = {
        Show: "inline",
        Hide: "none"
    };

    var ObjectTypeCodes = {
        Account: 1,
        Opportunity: 3,
        Dg_branchoffice_organizationunit: 10009,
        Dg_legalperson: 10011,
        Dg_firm: 10013,
        Dg_order: 10014,
        Dg_branchoffice: 10016,
        Dg_account: 10023,
        Dg_accountdetail: 10024
    };

    window.ShowHideToolbarButton = function(buttons, btnTitle, spacer, state)
    {
        if (isNullOrEmpty(document.all.mnuBar1))
            return;
        if (isNullOrEmpty(btnTitle))
            return;
        if (isNullOrEmpty(spacer))
            spacer = ToolbarSpacer.None;
        if (isNullOrEmpty(state))
            state = ButtonDisplay.Hide;

        //Get all toolbar buttons
        var toolBarButtons = document.all.mnuBar1.rows[0].cells[0].childNodes[0].childNodes;

        //Loop through each button
        for (var i = 0; i < toolBarButtons.length; i++)
        {
            var button = toolBarButtons[i];
            if (button.title.match(btnTitle) != null ||
                button.innerText.match(btnTitle) != null)
            {
                button.style.display = state;
                switch (spacer)
                {
                    case Spacer.Right:
                        ShowHideSpacer(button.nextSibling);
                        break;
                    case Spacer.Left:
                        ShowHideSpacer(button.previousSibling);
                        break;
                    case Spacer.Both:
                        ShowHideSpacer(button.nextSibling);
                        ShowHideSpacer(button.previousSibling);
                        break;
                }

                return;
            }
        }

        function ShowHideSpacer(btnSpacer)
        {
            if (!isNullOrEmpty(btnSpacer))
                btnSpacer.style.display = state;
        }

        function isNullOrEmpty(obj)
        {
            return obj == null || typeof(obj) == "undefined" || obj == "";
        }
    };

    function window.onload()
    {
        // Для фирм, заказа, сделки и клиента скрываем CRM-ные кнопки, дублирующие функционал ERM.
        //alert(objecttypecode);
        if (objecttypecode == ObjectTypeCodes.Dg_branchoffice_organizationunit ||
            objecttypecode == ObjectTypeCodes.Dg_legalperson ||
            objecttypecode == ObjectTypeCodes.Dg_firm ||
            objecttypecode == ObjectTypeCodes.Dg_order ||
            objecttypecode == ObjectTypeCodes.Dg_branchoffice ||
            objecttypecode == ObjectTypeCodes.Dg_account ||
            objecttypecode == ObjectTypeCodes.Dg_accountdetail ||
            objecttypecode == ObjectTypeCodes.Account ||
            objecttypecode == ObjectTypeCodes.Opportunity)
        {
            var buttonsNode = mnuBar1.rows[0].cells[0].childNodes[0].childNodes;
            window.ShowHideToolbarButton(buttonsNode, 'Назначение', Spacer.None, Display.Hide);
            window.ShowHideToolbarButton(buttonsNode, 'Удалить', Spacer.None, Display.Hide);
        
            if (objecttypecode == ObjectTypeCodes.Account) {
                window.ShowHideToolbarButton(buttonsNode, 'Слияние...', Spacer.None, Display.Hide);
            }
        }

        if (objecttypecode == Report)
        {
            crmGrid.onbeforeformload = handleReportDblClick;
        }
        crmGrid.SetParameter("disableDblClick", "0");
    };

</script>
<!-- End CRM Hack -->

<table height="100%" width="100%" cellpadding=0 cellspacing=0>
<tr id="gridMenuBar" height="34" runat="server">
<td valign="bottom" height=20 width="100%">
<mnu:AppGridMenuBar id="crmGridMenuBar" runat="server"/>
</td>
</tr>
<tr>
<td><cnt:AppGrid id="crmGrid" runat="server"/></td>
</tr>
</table>

</body>
</html>