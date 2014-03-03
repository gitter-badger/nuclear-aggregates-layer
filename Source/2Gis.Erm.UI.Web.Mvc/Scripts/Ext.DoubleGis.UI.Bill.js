﻿window.InitPage = function () {
    Ext.apply(this,
    {
        PrintWithoutProfileChoosing: function (profileId) {
            var entityId = Ext.getDom('Id').value;
            url = '/Bill/PrintBill/' + entityId + '?profileId=' + profileId + '&__dc=' + Ext.util.Format.cacheBuster();
            this.Items.Toolbar.disable();

            var iframe;
            iframe = document.getElementById("hiddenDownloader");
            if (iframe === null) {
                iframe = document.createElement('iframe');
                iframe.id = "hiddenDownloader";
                iframe.style.visibility = 'hidden';

                var iframeEl = new Ext.Element(iframe);
                iframeEl.on("load", function () {
                    var iframeContent = iframe.contentWindow.document.documentElement.innerText;
                    if (iframeContent != "") {
                        alert(iframeContent);
                    }
                });
                document.body.appendChild(iframe);
            }

            iframe.src = url;
            this.Items.Toolbar.enable();
        },
        PrintWithProfileChoosing: function () {
            var entityId = Ext.getDom('Id').value;
            url = '/Bill/Print/' + entityId + '?__dc=' + Ext.util.Format.cacheBuster();
            var params = "dialogWidth:500px; dialogHeight:250px; status:yes; scroll:no;resizable:no;";
            window.showModalDialog(url, null, params);
        },
        PrintBill: function () {
            var entityId = Ext.getDom('Id').value;
            var url = '/Bill/IsChooseProfileNeeded?billId=' + entityId + '&__dc=' + Ext.util.Format.cacheBuster();
            Ext.Ajax.request(
                {
                    url: url,
                    method: 'POST',
                    extinstance: this,
                    success: function (result, opts) {
                        var jsonData = Ext.decode(result.responseText);
                        if (!jsonData.IsChooseProfileNeeded) {
                            opts.extinstance.PrintWithoutProfileChoosing(jsonData.LegalPersonProfileId);
                        } else {
                            opts.extinstance.PrintWithProfileChoosing();
                        }
                    },
                    params: { billId: entityId }
                });
        }
    });
}