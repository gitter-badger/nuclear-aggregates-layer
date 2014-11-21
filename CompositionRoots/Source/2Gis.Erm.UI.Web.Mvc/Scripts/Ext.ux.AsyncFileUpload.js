Ext.ns('Ext.ux');
Ext.ux.AsyncFileUpload = Ext.extend(Ext.Component,
    {
        btnDis: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_dis_lookup.gif"),
        btnOff: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_off_lookup.gif"),
        btnOn: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_on_lookup.gif"),
        btnResolving: Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/btn_lookup_resolving.gif"),
        entityIcon: '',
        uploadMessageText: Ext.LocalizedResources.UploadFileQuestion,
        uploadMessageTitle: Ext.LocalizedResources.Alert,
        emptySizeText: Ext.LocalizedResources.UnknownFileSize,
        fileUploadText: Ext.LocalizedResources.FileUploadText,
        fileUploadCompleteText: Ext.LocalizedResources.FileUploadCompleteText,
        uploadUrl: 'STUB_VALUE_MUST_BE_SPECIFIED_BY_CONSUMER',
        uploadUrlSuffix: '',
        downloadUrl: 'STUB_VALUE_MUST_BE_SPECIFIED_BY_CONSUMER',
        inputName: 'file',
        isWaiting: false,
        contentTypes:
            {
                'image/pjpeg': 'x-async-file-image',
                'image/x-png': 'x-async-file-image',
                'image/gif': 'x-async-file-image',
                'image/bmp': 'x-async-file-image',
                'application/vnd.openxmlformats-officedocument.wordprocessingml.document': 'x-async-file-word',
                'application/msword': 'x-async-file-word',
                'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet': 'x-async-file-excel',
                'application/vnd.ms-excel': 'x-async-file-excel',
                'application/vnd.openxmlformats-officedocument.presentationml.presentation': 'x-async-file-powerpoint',
                'application/vnd.ms-powerpoint': 'x-async-file-powerpoint',
                'application/vnd.visio': 'x-async-file-visio',
                'application/pdf': 'x-async-file-acrobat',
                'application/x-shockwave-flash': 'x-async-file-flash',
                'text/plain': 'x-async-file-text',
                'text/html': 'x-async-file-html',
                'application/x-zip-compressed': 'x-async-file-zip',
                'application/octet-stream': 'x-async-file-exe',
                'video/x-msvideo': 'x-async-file-media',
                'video/x-ms-wmv': 'x-async-file-media',
                'generic': 'x-async-file-attach'
            },
        initComponent: function ()
        {
            Ext.ux.AsyncFileUpload.superclass.initComponent.call(this);
            this.addEvents('fileselected', 'fileuploadbegin', 'fileuploadcomplete', 'fileuploadsuccess', 'fileuploadfailure');
        },
        setReadOnly: function (readOnly)
        {
            if (readOnly === true)
            {
                this.readOnly = true;
                this.fileInput.dom.disabled = true;
                this.content.addClass('ReadOnly');
            }
            else
            {
                this.readOnly = false;
                if (!this.disabled)
                {
                    this.fileInput.dom.disabled = false;
                    this.content.removeClass('ReadOnly');
                }
            }
            this.setBtnOff();
        },
        setDisabled: function (disabled)
        {
            if (disabled === true)
            {
                this.disable();
            }
            else
            {
                this.enable();
            }
        },
        disable: function ()
        {
            this.disabled = true;
            this.setBtnOff();
            this.el.dom.disabled = true;
            this.fileInput.dom.disabled = true;
            this.content.addClass('ReadOnly');
            this.content.dom.tabIndex = -1;
        },
        enable: function ()
        {
            this.disabled = false;
            this.content.dom.tabIndex = this.tabIndex;
            this.el.dom.disabled = false;
            this.setBtnOff();
            if (!this.readOnly)
            {
                this.fileInput.dom.disabled = false;
                this.content.removeClass('ReadOnly');
            }
        },
        initHandlers: function ()
        {
            this.mon(this.button, 'mouseout', this.setBtnOff, this);
            this.mon(this.button, 'mouseover', this.setBtnOn, this);
            this.mon(this.content, 'focusin', this.contentFocusIn, this);
            this.mon(this.content, 'focusout', this.contentFocusOut, this);
            this.mon(this.fileInput, 'mouseenter', this.setBtnOn, this);
            this.mon(this.fileInput, 'mouseleave', this.setBtnOff, this);
            this.mon(this.fileInput, 'change', this.onFileSelected, this);
            this.mon(this.link, 'click', this.download, this);
        },
        createFileInput: function ()
        {

            this.fileInput = Ext.get(document.createElement('input'));
            this.fileInput.dom.id = this.id + '-file';
            this.fileInput.dom.name = this.inputName;
            this.fileInput.dom.className = 'x-async-file-input';
            this.fileInput.dom.type = 'file';

            this.uploadForm = Ext.get(document.createElement('form'));
            this.syncUploadUrl();
            this.uploadForm.appendChild(this.fileInput);
            this.button.parent().appendChild(this.uploadForm);
        },
        syncUploadUrl: function() {
            var url = this.uploadUrl;
            if (this.el.dom.value) {
                url = url + '/' + this.el.dom.value;
            }
            url = url + this.uploadUrlSuffix;
            this.uploadForm.dom.action = url;
        },
        uploadDialog: function ()
        {
            Ext.Msg.show({
                animEl: this.content,
                closable: false,
                scope: this,
                title: this.uploadMessageTitle,
                msg: String.format(this.uploadMessageText, this.fileInput.dom.value),
                buttons: Ext.Msg.OKCANCEL,
                icon: Ext.MessageBox.QUESTION,
                fn: function (btn, val, msg)
                {
                    if (btn == 'ok') this.uploadFile();
                    else if (btn == 'cancel') this.clearUpload();
                }
            });
        },
        uploadFile: function ()
        {
            if (this.fireEvent('fileuploadbegin', this, this.fileInput.dom.value) !== false)
            {
                Ext.Ajax.request(
                {
                    scope: this,
                    form: this.uploadForm,
                    isUpload: true,
                    success: this.uploadFileSuccess,
                    failure: this.uploadFileFailure,
                    callback: this.clearUpload
                });
                
                this.showUpload();
            }
        },
        uploadFileSuccess: function (response, opts)
        {
            var fileInfo = Ext.decode(response.responseText);
            if (!fileInfo.Message)
            {
                this.applyFileInfo({ fileName: fileInfo.FileName, fileSize: fileInfo.ContentLength, contentType: fileInfo.ContentType, fileId: fileInfo.FileId });
                this.fireEvent('fileuploadsuccess', this, fileInfo);
            }
            else
            {
                Ext.Msg.show({
                    animEl: this.content,
                    closable: false,
                    scope: this,
                    title: this.uploadMessageTitle,
                    msg: fileInfo.Message,
                    buttons: Ext.Msg.OK,
                    icon: Ext.MessageBox.ERROR
                });
                this.fireEvent('fileuploadfailure', this, fileInfo.Message);
            }
        },
        uploadFileFailure: function (xhr)
        {
            Ext.Msg.show({
                animEl: this.content,
                closable: false,
                scope: this,
                title: this.uploadMessageTitle,
                msg: xhr.responseText || xhr.statusText,
                buttons: Ext.Msg.OK,
                icon: Ext.MessageBox.ERROR
            });
            this.fireEvent('fileuploadfailure', this, xhr.responseText || xhr.statusText);
        },
        showUpload: function ()
        {
            this.pbar = this.pbar ? this.pbar.show().wait({ text: this.fileUploadText }) : new Ext.ProgressBar({ renderTo: this.content }).wait({ text: this.fileUploadText });
            this.isWaiting = true;
        },
        clearUpload: function (opts, result, xhr)
        {
            this.uploadForm.dom.reset();
            this.isWaiting = false;
            if (this.pbar && this.pbar.isWaiting())
            {
                this.pbar.reset().updateProgress(1, this.fileUploadCompleteText);
                (function () { this.pbar.reset(true); }).defer(1000, this);
            }
            this.fireEvent('fileuploadcomplete', this, result, xhr);
        },
        download: function ()
        {
            if (!this.el.dom.value) {
                return;
            }
            var url = String.format(this.downloadUrl, this.el.dom.value);
            if (!this.frame)
            {
                this.frame = window.Ext.getBody().createChild({
                        tag: 'iframe',
                        style: 'visibility: hidden; opacity : 0; width : 0; height: 0; display: none;'
                    });
                this.frame.on('load', function ()
                {
                    var iframeContent = this.dom.contentWindow.document.documentElement.innerText;
                    if (iframeContent != '')
                    {
                        Ext.MessageBox.show({
                            title: Ext.LocalizedResources.Error,
                            msg: iframeContent,
                            width: 500,
                            buttons: window.Ext.MessageBox.OK,
                            icon: window.Ext.MessageBox.ERROR
                        });
                    }
                });
            }
            this.frame.dom.contentWindow.location.href = url;
        },
        onRender: function (ct)
        {
            Ext.ux.AsyncFileUpload.superclass.onRender.call(this, ct);
            this.tabIndex = this.el.dom.tabIndex;

            var template = new window.Ext.Template(
                '<div id="{id}-wrap" class="x-async-file-wrap">',
                    '<div id="{id}-content" class="x-async-file">',
                        '<span id="{id}-link" class="x-async-file-item"></span>&nbsp;',
                    '</div>',
                    '<img id="{id}-button" alt="" title="" class="x-async-file-button" src="{btnOff}" />',
                '</div>',
                {
                    compiled: true,
                    disableFormats: true
                }
            );
            this.wrap = Ext.get(template.insertBefore(this.el.dom, this));
            this.content = this.wrap.child(String.format('#{0}-content', this.id));
            this.link = this.content.child(String.format('#{0}-link', this.id));
            this.button = this.wrap.child(String.format('#{0}-button', this.id));
            this.createFileInput();

            this.content.tabIndex = this.tabIndex;
            this.el.applyStyles({ display: "none" });
            this.initHandlers();
            this.setReadOnly(this.readOnly);
            this.setDisabled(this.disabled);
            if (this.fileInfo)
            {
                this.applyFileInfo(this.fileInfo);
            }
        },
        applyFileInfo: function (fileInfo)
        {
            if (fileInfo.fileId) {
                var fileName = fileInfo.fileName || Ext.LocalizedResources.FileNameNotSpecified;
                this.el.dom.value = (fileInfo.fileId && fileInfo.fileId != "0") ? fileInfo.fileId : "";
                this.link.update(String.format('{0} ({1})', fileName, fileInfo.fileSize && fileInfo.fileSize * 1 ? Ext.util.Format.fileSize(fileInfo.fileSize) : this.emptySizeText));
                this.link.removeClass(this.currentCls);
                this.currentCls = this.contentTypes[fileInfo.contentType || 'generic'] || this.contentTypes['generic'];
                this.link.addClass(this.currentCls);
            } else {
                this.el.dom.value = "";
            }
            this.syncUploadUrl();
        },
        contentFocusIn: function (evt, el)
        {
            this.disabled ? this.content.removeClass('x-async-file-focus') : this.content.addClass('x-async-file-focus');

        },
        contentFocusOut: function (evt, el)
        {
            this.content.removeClass('x-async-file-focus');
        },
        setBtnOff: function (evt, el)
        {
            this.button.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnOff;
        },
        setBtnOn: function (evt, el)
        {
            this.button.dom.src = this.disabled || this.readOnly ? this.btnDis : this.btnOn;
        },
        onFileSelected: function (evt, el)
        {
            if (this.fireEvent('fileselected', this, this.fileInput.dom.value) !== false)
                this.uploadDialog();
        },
        onDestroy: function ()
        {
            Ext.ux.form.AsyncFileUpload.superclass.onDestroy.call(this);
            if (this.pbar)
            {
                this.pbar.destriy();
            }
            Ext.destroy(this.fileInput, this.button, this.wrap, this.content, this.uploadForm, this.frame);
        }
    });
Ext.reg('asyncfileupload', Ext.ux.AsyncFileUpload);
