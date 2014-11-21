window.InitPage = function () {
    var downloadFilePattern = Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/ThemeTemplate/{0}';
    var uploadUrl = '/Upload/ThemeTemplate';
    
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
                },
                scope: this
            },
            fileInfo: fileInfo
        });
    });

    window.Card.on("afterbuild", function (card) {
    });
};
