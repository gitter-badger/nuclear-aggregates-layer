Ext.namespace('Ext.ux');
Ext.ux.AdvertisementBagPanel = Ext.extend(Ext.Panel, {
    bodyCssClass: "x-ads-body",
    constructor: function(config) {
        config = config || { id: 'adsBody' };
        config.items = [this.renderAdsList(config)];
        config.bodyCssClass = "x-ads-body";
        window.Ext.ux.AdvertisementBagPanel.superclass.constructor.call(this, config);
        window.AdvertisementBagPanel = this;
    },
    renderAdsList: function(config) {
        var emptyContent = '<span class="x-ads-empty" style="cursor:default">' + Ext.LocalizedResources.ContentIsEmpty + '</span>';

        var tpl = new window.Ext.XTemplate(
            '<tpl for=".">',
            '<div class="x-ads-thumb <tpl if="!isValid">x-ads-thumb-invalid</tpl>" id="{id}" style="position: relative; height: auto">',
            '<tpl if="needsValidation&&status==\'Draft\'"><img alt="" src="' + Ext.DoubleGis.Global.Helpers.GetStaticImagePath("ERM/draft.png") + '" class="x-ads"/></tpl>',
            '<tpl if="needsValidation&&status==\'ReadyForValidation\'"><img alt="" src="' + Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/Error/notification.png") + '" class="x-ads"/></tpl>',
            '<tpl if="needsValidation&&status==\'Valid\'"><img alt="" src="' + Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/Error/ok.png") + '" class="x-ads"/></tpl>',
            '<tpl if="needsValidation&&status==\'Invalid\'"><img alt="" src="' + Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/Error/critical.png") + '" class="x-ads"/></tpl>',
            '<tpl if="!needsValidation"><img alt="" src="' + Ext.DoubleGis.Global.Helpers.GetEntityIconPath("en_ico_16_Advertisement.gif") + '" class="x-ads"/></tpl>',
            '<span class="x-ads-thumb" style="cursor:default">{name} - ({restrictionName})</span><br/><br/>',
            '<tpl if="restrictionType==\'Image\'">',
            '<tpl if="fileId"><img class="x-ads-img" alt="" src="' + Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/AdvertisementElement/{fileId}?_dc={fileId:cacheBuster()}"/></tpl>',
            '<tpl if="!fileId">' + emptyContent + '</tpl>',
            '</tpl>',
            '<tpl if="restrictionType==\'Text\'||restrictionType==\'FasComment\'">',
            '<tpl if="text">',
            '<tpl if="formattedText"><div class="x-ads-richtext">{text}</div></tpl>',
            '<tpl if="!formattedText"><textarea readonly="true" class="x-ads-text">{text}</textarea></tpl>',
            '</tpl>',
            '<tpl if="!text">' + emptyContent + '</tpl>',
            '</tpl>',
            '<tpl if="restrictionType==\'Article\'">',
            '<tpl if="fileId"><span style="padding-left:15px;"></span><img id="fileLink" alt="" src="' + Ext.DoubleGis.Global.Helpers.GetStaticImagePath("CommonUI/attach.png") + '" class="x-ads" style="cursor:default"/><span id="fileLink" class="x-ads-link">{fileName}</span></tpl>',
            '<tpl if="!fileId">' + emptyContent + '</tpl>',
            '</tpl>',
            '<tpl if="restrictionType==\'Date\'">',
            '<tpl if="beginDate||endDate"><span class="x-ads-date" style="cursor:default">{beginDate:date()} - {endDate:date()}</span></tpl>',
            '<tpl if="!beginDate && !endDate">' + emptyContent + '</tpl>',
            '</tpl>',
            '<tpl if="needsValidation&&userCanValidateAdvertisement&&(status==\'ReadyForValidation\'||status==\'Valid\'||status==\'Invalid\')">',
            '<div class="buttonContainer" style="top: 5px; right: 10px; position: absolute;">',
            '<div class="buttonContainerAccept" id="buttonContainerAccept" style="float: left; padding: 5px;"></div>',
            '<div class="buttonContainerValidate" id="buttonContainerValidate" style="float: left; padding: 5px;"></div>',
            '</div>',
            '</tpl>',
            '</div>',
            '</tpl>',
            '<div class="x-clear"></div>');


        this.adsBagView = new window.Ext.DataView({
            tpl: tpl,
            store: this.store = new window.Ext.data.Store({
                xtype: 'jsonstore',
                url: "/Advertisement/GetAdvertisementBag",
                listeners: {
                    load: { fn: this.createButtons, scope: this }
                },
                reader: new window.Ext.data.JsonReader({
                    idProperty: 'Id',
                    root: 'data',

                    fields: [
                        { name: "id", mapping: "Id" },
                        { name: "name", mapping: "Name" },
                        { name: "text", mapping: "Text" },
                        { name: "fileId", mapping: "FileId" },
                        { name: "fileName", mapping: "FileName" },
                        { name: "beginDate", mapping: "BeginDate" },
                        { name: "endDate", mapping: "EndDate" },
                        { name: "restrictionType", mapping: "RestrictionType" },
                        { name: "restrictionName", mapping: "RestrictionName" },
                        { name: "isValid", mapping: "IsValid" },
                        { name: "formattedText", mapping: "FormattedText" },
                        { name: "needsValidation", mapping: "NeedsValidation" },
                        { name: "status", mapping: "Status" },
                        { name: "userCanValidateAdvertisement", mapping: "UserCanValidateAdvertisement" }
                    ]
                })
            }),
            singleSelect: true,
            loadingText: Ext.LocalizedResources.IndicatorText,
            overClass: 'x-ads-thumb-over',
            selectedClass: 'x-ads-thumb-selected',
            itemSelector: 'div.x-ads-thumb',
            style: 'background-color: #EAF3FF;',
            listeners: {
                dblclick: { fn: this.openAdsElem, scope: this },
                click: { fn: this.onRecordClick, scope: this }
            }
        });
        return this.adsBagView;
    },
    createButtons: function () {
        var container = Ext.get(this.id);
        var items = container.query('div.x-ads-thumb');
        Ext.each(items, this.addValidationButtons, this);
    },
    addValidationButtons: function(advDomItem) {
        var advExtElement = Ext.get(advDomItem);
        var accept = advExtElement.query('.buttonContainerAccept')[0];
        if (accept) {
            var acceptButton = new Ext.Button({
                text: Ext.LocalizedResources.AdvertisingElementAccept,
                icon: '/Content/images/CommonUI/Validation/accept.png',
                width: 128,
                height: 24,
                renderTo: accept
            });
            acceptButton.on('click', function() { this.acceptAdvertisingElement(advExtElement.id); }, this);
        }

        var validate = advExtElement.query('.buttonContainerValidate')[0];
        if (validate) {
            var validateButton = new Ext.Button({
                text: Ext.LocalizedResources.AdvertisingElementValidate,
                icon: '/Content/images/CommonUI/Validation/validate.png',
                width: 128,
                height: 24,
                renderTo: validate
            });
            validateButton.on('click', function() { this.openAdvertisingElementValidation(advExtElement.id); }, this);
        }
    },
    acceptAdvertisingElement: function(id) {
        var urlTemplate = Ext.SpecialOperationsServiceRestUrl + 'HandleAdsState.svc/Rest/transfer/to/approved/{0}';
        var url = String.format(urlTemplate, id);

        var mask = new Ext.LoadMask(document.body);

        mask.show();
        var xmlhttp = new XMLHttpRequest();
        xmlhttp.withCredentials = true;
        xmlhttp.open('POST', url, true);

        var bag = this;

        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState == 4) {
                mask.hide();
                if (xmlhttp.status == 200) {
                    bag.refresh();
                } else if (xmlhttp.responseText) {
                    bag.showError(xmlhttp.responseText);
                } else if (xmlhttp.statusText) {
                    bag.showErrorText(xmlhttp.statusText);
                } else {
                    var errorText = String.format(Ext.LocalizedResources.ErrorOnServiceAccess, Ext.SpecialOperationsServiceRestUrl);
                    bag.showErrorText(errorText);
                }
            }
        };
        xmlhttp.send();
    },
    openAdvertisingElementValidation: function(id) {
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}",
            window.Ext.DoubleGis.Global.UISettings.ActualCardWidth,
            window.Ext.DoubleGis.Global.UISettings.ActualCardHeight,
            window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop,
            window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var sUrl = '/CreateOrUpdate/AdvertisementElementStatus/' + id;
        window.open(sUrl, "_blank", params);
    },
    onRecordClick: function(view, index, node, evt) {
        if (evt.target.id == "fileLink") {
            var url = Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/AdvertisementElement/' + view.getRecord(node).data.fileId;
            var fileFrame;
            fileFrame = Ext.get("hiddenDownloader");
            if (fileFrame === null) {
                fileFrame = window.Ext.getBody()
                    .createChild({
                        tag: "iframe",
                        id: "hiddenDownloader",
                        style: "visibility: hidden;"
                    });
                fileFrame.on("load", function() {
                    var iframeContent = this.dom.contentWindow.document.documentElement.innerText;
                    if (iframeContent != "") {
                        Ext.MessageBox.show({
                            title: Ext.LocalizedResources.Error,
                            msg: iframeContent,
                            width: 300,
                            buttons: window.Ext.MessageBox.OK,
                            icon: window.Ext.MessageBox.ERROR
                        });
                    }
                });
            }
            fileFrame.dom.contentWindow.location.href = url;
        }

        var nodeEl = Ext.get(node);
        nodeEl.removeAllListeners();
        nodeEl.on("keydown", function(e, el) {
            if (e.keyCode == e.ENTER) {
                e.stopPropagation();
                this.openAdsElem();
            }

        }, this);
        nodeEl.on("focusout", function(e, el) {
            this.removeAllListeners();
        }, undefined, { single: true });
    },
    openAdsElem: function() {
        var record = this.adsBagView.getSelectedRecords()[0];
        var params = String.format("width={0},height={1},status=no,resizable=yes,top={2},left={3}", window.Ext.DoubleGis.Global.UISettings.ActualCardWidth, window.Ext.DoubleGis.Global.UISettings.ActualCardHeight, window.Ext.DoubleGis.Global.UISettings.ScreenCenterTop, window.Ext.DoubleGis.Global.UISettings.ScreenCenterLeft);
        var sUrl = Ext.DoubleGis.Global.Helpers.EvaluateUpdateEntityUrl('AdvertisementElement', record.id, '');
        window.open(sUrl, "_blank", params);
    },
    refresh: function() { this.adsBagView.store.reload(); },
    showError: function(error) {
        var errorInfo = Ext.decode(error);
        if (errorInfo && errorInfo.Message) {
            error = errorInfo.Message;
        }

        this.showErrorText(error);
    },
    showErrorText: function (error) {
        window.Ext.MessageBox.show({
            title: '',
            msg: error,
            width: 300,
            buttons: window.Ext.MessageBox.OK,
            icon: window.Ext.MessageBox.ERROR
        });
    }
});
Ext.reg('AdvertisementBagPanel', Ext.ux.AdvertisementBagPanel);