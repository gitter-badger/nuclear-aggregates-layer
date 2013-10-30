﻿window.InitPage = function () {
    window.Card.on("beforebuild", function (card) {
        this.Download = function () {
            var url = "/ReleaseInfo/DownloadResults/" + Ext.getDom('Id').value;
            this.Items.Toolbar.disable();
            var iframe = document.getElementById("hiddenDownloader");
            if (iframe === null) {
                iframe = document.createElement('iframe');
                iframe.id = "hiddenDownloader";
                iframe.style.visibility = 'hidden';

                var iframeEl = new Ext.Element(iframe);
                iframeEl.on("load", function () {
                    var title = iframe.contentWindow.document.title;
                    if (title != "") {
                        alert(title);
                    }
                });
                document.body.appendChild(iframe);
            }
            iframe.src = url;

            this.Items.Toolbar.enable();
        };
        this.DownloadResults = function () {
            this.Download();
        };
    });
};