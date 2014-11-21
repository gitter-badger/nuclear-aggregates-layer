var PrintLogic = {
    PrintWithoutProfileChoosing: function (methodName, entityId, profileId) {
        var urlBase = '/Print/' + methodName + '/' + entityId;
        var urlArguments = {
            profileId: profileId,
            _dc: Ext.util.Format.cacheBuster()
        };
        var url = window.Ext.urlAppend(urlBase, window.Ext.urlEncode(urlArguments));
        this.StartFileDownloadingWithoutPageRefresh(url);
    },

    /**
     * Скрывает детали определения профиля: запрос на сервер и, при необходимости, диалог с пользователем.
     * @param {Object} entityId идентификатор сущности, может быть { orderId: 321 } или { billId: 123 }
     * @param {Function} callback функция, вызываемая после определения профиля. Должен принимать единственный аргумент - идентифкатор профиля.
     */
    ChooseProfile: function (entityId, callback) {
        Ext.Ajax.request(
        {
            url: '/Print/IsChooseProfileNeeded',
            params: entityId,
            method: 'GET',
            scope: this,
            success: function(result) {
                var jsonData = Ext.decode(result.responseText);
                if (jsonData.IsChooseProfileNeeded) {
                    var profileId = this.DoChooseProfile(entityId, jsonData.LegalPersonProfileId);
                    if (profileId) {
                        callback.call(this, profileId);
                    }
                } else {
                    callback.call(this, jsonData.LegalPersonProfileId);
                }
            }
        });
    },

    DoChooseProfile: function (entityId, profileId) {
        var urlBase = '/Print/ChooseProfile';
        var urlArguments = {
            profileId: profileId,
            _dc: Ext.util.Format.cacheBuster()
        };
        Ext.apply(urlArguments, entityId); // Добавляем идентфиикатор заказа или счёта к параметрам
        var url = window.Ext.urlAppend(urlBase, window.Ext.urlEncode(urlArguments));
        var params = "dialogWidth:500px; dialogHeight:250px; status:yes; scroll:no;resizable:no;";
        var selectedProfileId = window.showModalDialog(url, null, params);
        return selectedProfileId;
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
                // Предполагаю, что этот код предназначен для вывода сообщений об ошибке при печати.
                var iframeContent = iframe.contentWindow.document.documentElement.innerText;
                if (iframeContent != "") {
                    alert(iframeContent);
                } 
            });
        }

        iframe.src = url;
    }
}