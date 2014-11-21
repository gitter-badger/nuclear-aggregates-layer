window.InitPage = function () {
    var downloadFilePattern = Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/Theme/{0}';
    var uploadUrl = '/Upload/Theme';
    
    function showUploadedImage() {
        var fileId = Ext.getDom('FileId').value;
        var cacheBuster = { _dc: Ext.util.Format.cacheBuster() };
        if (fileId) {
            var url = window.Ext.urlAppend(String.format(downloadFilePattern, fileId), window.Ext.urlEncode(cacheBuster));
            Ext.getDom('UploadedImage').src = url;
        }
    }

    window.Card.on("beforebuild", function(card) {
        
        var fileInfo = {
            fileId: Ext.getDom('FileId').value,
            fileName: Ext.getDom('FileName').value,
            contentType: Ext.getDom('FileContentType').value,
            fileSize: Ext.getDom('FileContentLength').value
        };
        
        var fileUploadControl = new Ext.ux.AsyncFileUpload({
            applyTo: 'FileId',
            uploadUrl: uploadUrl,
            downloadUrl: downloadFilePattern,
            listeners: {
                fileuploadbegin: function() {
                    this.Items.Toolbar.disable();
                },
                fileuploadcomplete: function() {
                    this.Items.Toolbar.enable();
                    showUploadedImage();
                },
                scope: this
            },
            fileInfo: fileInfo
        });
    });
    
    window.Card.on("afterbuild", function (card) {
        showUploadedImage();
    });

    var setDefault = function (value) {
        var params = {
            id: Ext.getDom('Id').value,
            isDefault: value
        };
        
        this.progressWindow = Ext.MessageBox.wait(Ext.LocalizedResources.WorkInProgressPleaseStandBy, '', { animate: false });

        Ext.Ajax.request({
            timeout: 1200000,
            method: 'POST',
            url: '/Theme/SetAsDefault',
            params: params,
            scope: this,
            success: function (xhr) {
                self.progressWindow.hide();
                window.Card.refresh();
            },
            failure: function (xhr) {
                self.progressWindow.hide();
                Ext.Msg.show({
                    title: Ext.LocalizedResources.Error,
                    msg: xhr.responseText || Ext.LocalizedResources.CannotSetDefaultTheme,
                    buttons: Ext.Msg.OK,
                    icon: Ext.MessageBox.ERROR
                });
            }
        });
    };

    this.SetDefaultTheme = function () {
        setDefault(true);
    };

    this.UnSetDefaultTheme = function () {
        setDefault(false);
    };
};
