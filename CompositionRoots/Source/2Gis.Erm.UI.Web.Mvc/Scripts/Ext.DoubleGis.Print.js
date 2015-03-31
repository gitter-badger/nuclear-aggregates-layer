var PrintLogic = {
    PrintWithoutProfileChoosing: function (methodName, entityId, profileId, businessModelSpecificArea) {
        var urlBase = '/Print/' + methodName + '/' + entityId;
        if (businessModelSpecificArea) {
            urlBase = '/' + businessModelSpecificArea + urlBase;
        }
        var urlArguments = {
            _dc: Ext.util.Format.cacheBuster()
        };
        if (profileId) {
            urlArguments.profileId = profileId;
        }
        var url = window.Ext.urlAppend(urlBase, window.Ext.urlEncode(urlArguments));
        this.StartFileDownloadingWithoutPageRefresh(url);
    },

    StartFileDownloadingWithoutPageRefresh: function (url) {
        var iframe = document.getElementById("hiddenDownloader");
        if (!iframe) {
            iframe = document.createElement('iframe');
            iframe.id = "hiddenDownloader";
            iframe.style.visibility = 'hidden';
            document.body.appendChild(iframe);

            var iframeEl = Ext.get(iframe);
            iframeEl.on("load", function () {
                // При загрузке файла событие 'load' не возникает.
                // А при возникновении server-side ошибки её текст возвращается как контент страницы.
                var iframeContent = iframe.contentWindow.document.documentElement.innerText;
                if (iframeContent != "") {
                    window.Card.AddNotification(iframeContent, "CriticalError");
                } 
            });
        }

        iframe.src = url;
    }
}