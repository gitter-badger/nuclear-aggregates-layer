window.InitPage = function () {
    window.Card.on("beforebuild", function (card) {
        // FIXME {y.baranihin, 16.06.2014}: Давай все же будем использовать ООП. Пусть будет некий объект в этом scope, в не глобальная функция
        CultureSpecificBeforeBuildActions(this);

        Ext.apply(this, {
            ChangeLegalPersonClient: function () {
                var params = "dialogWidth:" + 500 + "px; dialogHeight:" + 250 + "px; status:yes; scroll:no;resizable:no;";
                var url = '/GroupOperation/ChangeClient/LegalPerson';
                window.showModalDialog(url, [Ext.getDom("Id").value], params);
                this.refresh();
            },

            ChangeLegalPersonRequisites: function () {
                var params = "dialogWidth:" + 700 + "px; dialogHeight:" + 330 + "px; status:yes; scroll:no;resizable:no;";
                var url = GetChangeLegalPersonRequisitesUrl(Ext.getDom("Id").value);
                var result = window.showModalDialog(url, null, params);
                if (result == 'OK') {
                    this.refresh();
                }
            }
        });

    });

    Ext.apply(this, {
        buildProfilesList: function () {
            if (this.form.Id.value != 0) {
                var cnt = Ext.getCmp('ContentTab_holder');
                var tp = Ext.getCmp('TabWrapper');

                tp.anchor = "100%, 60%";
                delete tp.anchorSpec;
                cnt.add(new Ext.Panel({
                    id: 'profileFrame_holder',
                    anchor: '100%, 40%',
                    html: '<iframe id="profileFrame_frame"></iframe>'
                }));
                cnt.doLayout();
                var mask = new window.Ext.LoadMask(window.Ext.get("profileFrame_holder"));
                mask.show();
                var iframe = Ext.get('profileFrame_frame');

                iframe.dom.src = '/Grid/View/LegalPersonProfile/LegalPerson/{0}/{1}?extendedInfo=filterToParent%3Dtrue'.replace(/\{0\}/g, this.form.Id.value).replace(/\{1\}/g, this.ReadOnly ? 'Inactive' : 'Active');
                iframe.on('load', function (evt, el) {
                    el.height = Ext.get(el.parentElement).getComputedHeight();
                    el.width = Ext.get(el.parentElement).getComputedWidth();
                    el.style.height = "100%";
                    el.style.width = "100%";
                    el.contentWindow.Ext.onReady(function () {
                        el.contentWindow.IsBottomOrderPositionDataList = true;
                    });
                    this.hide();
                }, mask);
                cnt.doLayout();

            }
        }
    });

    this.on("afterbuild", this.buildProfilesList, this);
    this.on("afterrelatedlistready", function (card, details) {
        var dataListName = details.dataList.currentSettings.Name;

        if (dataListName === 'LegalPersonProfile') {
            var dataListWindow = details.dataList.ContentContainer.container.dom.document.parentWindow;
            if (dataListWindow.IsBottomOrderPositionDataList) {
                dataListWindow.Ext.getDom('Toolbar').style.display = 'none';
                details.dataList.Items.Grid.getBottomToolbar().hide();
                details.dataList.ContentContainer.doLayout();
            }
        }
    }, this);
};