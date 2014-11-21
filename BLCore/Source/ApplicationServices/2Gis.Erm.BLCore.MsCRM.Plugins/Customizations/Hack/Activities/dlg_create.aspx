<%@ Page Language="c#" Inherits="Microsoft.Crm.Web.Activities.dlg_create" %>

<%@ Register TagPrefix="frm" Namespace="Microsoft.Crm.Application.Forms" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Register TagPrefix="cnt" Namespace="Microsoft.Crm.Application.Controls" Assembly="Microsoft.Crm.Application.Components.Application" %>
<%@ Import Namespace="Microsoft.Crm" %>
<html>
<head>
    <cnt:AppHeader runat="server" id="crmHeader" />
    <script language="JavaScript">
        function applychanges() {
            var selectedItem = parseInt(_selectedItem.item, 10);

            var parentWindow = window.dialogArguments;
            var queryParameters = ParseQueryString(parentWindow.location.search);

            if (queryParameters.FromErm) {

                var extraQueryString = "&pName=" + queryParameters.pName;
                
                if (queryParameters.partyId && queryParameters.partyId != "") {
                    extraQueryString +=
                    "&partyid=" + queryParameters.partyId +
                    "&partytype=" + queryParameters.partyType +
                    "&partyname=" + queryParameters.partyName
                }

                parentWindow.openObjEx(selectedItem,
                    queryParameters.pType,
                    queryParameters.pId,
                    extraQueryString);
            }
            else {
                parentWindow.openObj(selectedItem);
            }
            
            window.close();
        }

        function cancel() {
            window.close();
        }
    </script>
</head>
<body>
    <frm:DialogForm id="crmForm" runat="server">
        <table width="100%" cellpadding="0">
            <tr class="main">
                <td>
                    <div class="ms-crm-Dialog-List" style="overflow: auto;">
                        <ul id="tblItems" class="ms-crm-Dialog-List" style="height: 100%;">

                            <%
                                RenderListItem(Util.Task, Privileges.CreateActivity);
                                RenderListItem(Util.Fax, Privileges.CreateActivity);
                                RenderListItem(Util.PhoneCall, Privileges.CreateActivity);
                                RenderListItem(Util.Email, Privileges.CreateActivity);
                                RenderListItem(Util.Letter, Privileges.CreateActivity);
                                RenderListItem(Util.Appointment, Privileges.CreateActivity);
                                //RenderServiceAppointment();
                                //RenderCampaignResponse();
                            %>
                        </ul>
                    </div>
                </td>
            </tr>
        </table>
    </frm:DialogForm>
</body>
</html>
