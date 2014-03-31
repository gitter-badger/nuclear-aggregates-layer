Ext.namespace('Ext.ux');
Ext.ux.MergeClientsUtility = Ext.extend(Object,
            {
                addNotification: function (message, level, messageId)
                {
                    var nopt = { message: message, level: window.Ext.Notification.Icon[level], messageId: messageId };
                    var nc = Ext.get("Notifications");
                    if (!this.NotificationTemplate)
                    {
                        this.NotificationTemplate = new Ext.XTemplate(
                        '<div id="{messageId}" class="Notification">',
                        '<table cellspacing="0" cellpadding="0"><tbody><tr><td valign="top">' +
                            '<img class="ms-crm-Lookup-Item" alt="" src="{level}"/>',
                        '</td><td width="5px"></td><td><span id="NotificationText">{message}</span>',
                        '</td></tr></tbody></table></div>');
                    }
                    nc.show(true).dom.innerHTML = "";
                    this.NotificationTemplate.append(nc.dom, nopt);
                },
                getClientsData: function ()
                {
                    if (Ext.getCmp("Client1").getValue() && Ext.getCmp("Client2").getValue())
                    {
                        Ext.Ajax.request(
                            {
                                url: "/Client/MergeClientsGetData",
                                scope: this,
                                params: { masterId: Ext.getCmp("Client1").getValue().id, subordinateId: Ext.getCmp("Client2").getValue().id },
                                success: this.getClientsDataSuccess,
                                failure: this.getClientsDataFailure
                            });
                    }
                    else
                    {
                        Ext.get("dataRegion").update('');
                    }
                },
                getClientsDataSuccess: function (jsonResponse)
                {
                    this.clearEvents();
                    Ext.get("dataRegion").update(jsonResponse.responseText);
                    this.bindEvents();
                    this.onMainRadioClick(null, this.leftGroup.clientRad.dom.checked ? this.leftGroup.clientRad.dom : this.rightGroup.clientRad.dom);
                },
                getClientsDataFailure: function (xhr)
                {
                    Ext.get("dataRegion").update('');
                    Ext.MessageBox.show({
                        title: Ext.LocalizedResources.Error,
                        msg: xhr.responseText,
                        width: 300,
                        buttons: window.Ext.MessageBox.OK,
                        icon: window.Ext.MessageBox.ERROR
                    });
                },
                close: function ()
                {
                    window.close();
                },
                submitForm: function ()
                {
                    if (Ext.DoubleGis.FormValidator.validate(window.EntityForm))
                    {
                        Ext.MessageBox.confirm(Ext.LocalizedResources.Alert, Ext.LocalizedResources.MergeClientsAlert, function (buttonId)
                        {
                            if (buttonId == 'yes')
                            {
                                Ext.getDom("OK").disabled = "disabled";
                                Ext.getDom("Cancel").disabled = "disabled";
                                EntityForm.submit();
                            }

                        });
                    }
                },
                clearEvents: function ()
                {
                    Ext.each(this.leftGroup.sections, function (item)
                    {
                        Ext.get(item.el).un('click', this.onSectionRadioClick, this);

                        Ext.each(item.dataRads, function (el)
                        {
                            Ext.get(el).un('click', this.onDataRadioClick, this);
                        }, this);

                        item.dataRads = null;

                    }, this);
                    this.leftGroup.sections = null;

                    Ext.each(this.rightGroup.sections, function (item)
                    {
                        Ext.get(item.el).un('click', this.onSectionRadioClick, this);
                        Ext.each(item.dataRads, function (el)
                        {
                            Ext.get(el).un('click', this.onDataRadioClick, this);
                        }, this);

                        item.dataRads = null;
                    }, this);
                    this.rightGroup.sections = null;
                },
                bindEvents: function ()
                {
                    this.leftGroup.sections = [];
                    Ext.each(Ext.query("input[type='radio'].section.left"), function (el)
                    {
                        var section = { el: el, dataRads: Ext.query("input[type='radio'].left." + el.name) };

                        Ext.get(el).on('click', this.onSectionRadioClick, this);
                        Ext.each(section.dataRads, function (dataRad)
                        {
                            Ext.get(dataRad).on('click', this.onDataRadioClick, this);
                        }, this);
                        this.leftGroup.sections.push(section);

                    }, this);

                    this.rightGroup.sections = [];
                    Ext.each(Ext.query("input[type='radio'].section.right"), function (el)
                    {
                        var section = { el: el, dataRads: Ext.query("input[type='radio'].right." + el.name) };

                        Ext.get(el).on('click', this.onSectionRadioClick, this);
                        Ext.each(section.dataRads, function (dataRad)
                        {
                            Ext.get(dataRad).on('click', this.onDataRadioClick, this);
                        }, this);
                        this.rightGroup.sections.push(section);
                    }, this);
                },
                init: function ()
                {
                    if (Ext.getDom("Message").innerHTML.trim() == "OK")
                    {
                        window.close();
                        return;
                    }
                    else if (Ext.getDom("Message").innerHTML.trim() != "")
                    {
                        if (window.Ext.getDom("MessageType").innerHTML.trim()=="CriticalError")
                            Ext.getDom("OK").disabled = "disabled";
                        this.addNotification(window.Ext.getDom("Message").innerHTML.trim(), window.Ext.getDom("MessageType").innerHTML.trim(), "ServerNotification");
                    }

                    Ext.get("Cancel").on("click", this.close);
                    Ext.get("OK").on("click", this.submitForm);
                    Ext.getCmp("Client1").on('change', this.getClientsData, this);
                    Ext.getCmp("Client2").on('change', this.getClientsData, this);

                    this.leftGroup = {
                        clientRad: Ext.get("client_1"),
                        sections: []
                    };
                    this.rightGroup = {
                        clientRad: Ext.get("client_2"),
                        sections: []
                    };
                    this.leftGroup.clientRad.on('click', this.onMainRadioClick, this);
                    this.rightGroup.clientRad.on('click', this.onMainRadioClick, this);

                    this.getClientsData();
                },
                onMainRadioClick: function (evt, node)
                {
                    if (this.rightGroup.sections.length && this.leftGroup.sections.length)
                    {
                        var client1Id = Ext.getCmp("Client1").getValue().id;
                        var client2Id = Ext.getCmp("Client2").getValue().id;
                        var client1Timestamp =  Ext.getDom("Client1_Timestamp").value;
                        var client2Timestamp = Ext.getDom("Client2_Timestamp").value;
                        var group;
                        
                        var left = Ext.get(node).hasClass('left');
                        if (left) {
                            Ext.getDom('Id').value = client1Id;
                            Ext.getDom('AppendedClient').value = client2Id;
                            Ext.getDom('Timestamp').value = client2Timestamp;
                            group = this.leftGroup;
                        } else {
                            Ext.getDom('Id').value = client2Id;
                            Ext.getDom('AppendedClient').value = client1Id;
                            Ext.getDom('Timestamp').value = client1Timestamp;
                            group = this.rightGroup;
                        }
                        
                        Ext.each(group.sections, function (section)
                        {
                            section.el.checked = true;
                            this.onSectionRadioClick(null, section.el);
                        }, this);
                    }
                },
                onSectionRadioClick: function (evt, node)
                {
                    var group = Ext.get(node).hasClass('left') ? this.leftGroup : this.rightGroup;
                    Ext.each(group.sections, function (section)
                    {
                        if (section.el == node)
                        {
                            Ext.each(section.dataRads, function (rad)
                            {
                                if (rad.value)
                                {
                                    rad.checked = true;
                                    this.onDataRadioClick(null, rad);
                                }
                                else
                                {
                                    Ext.each(document.getElementsByName(rad.name), function (el)
                                    {
                                        if (el != rad && el.value)
                                        {
                                            node.checked = false;
                                            el.checked = true;
                                            this.onDataRadioClick(null, el);
                                        }
                                    }, this);
                                }
                            }, this);
                        }

                    }, this);
                },
                onDataRadioClick: function (evt, node)
                {
                    Ext.get(node.parentNode).addClass('selected');
                    Ext.get(node.parentNode.nextSibling).addClass('selected');
                    Ext.each(document.getElementsByName(node.name), function (el)
                    {
                        if (el != node)
                        {
                            el.checked = false;
                            Ext.get(el.parentNode).removeClass('selected');
                            Ext.get(el.parentNode.nextSibling).removeClass('selected');
                        }
                    });
                    if (evt !== null)
                    {
                        var group = Ext.get(node).hasClass('left') ? this.leftGroup : this.rightGroup;
                        Ext.each(group.sections, function (section)
                        {
                            if (Ext.get(node).hasClass(section.el.name))
                            {
                                Ext.each(document.getElementsByName(section.el.name), function (el)
                                {
                                    el.checked = false;
                                });

                                section.el.checked = true;

                                Ext.each(section.dataRads, function (dataRad)
                                {
                                    if (!dataRad.checked)
                                    {
                                        section.el.checked = false;
                                    }
                                }, this);
                            }
                        }, this);
                    }
                }
            });

Ext.onReady(function ()
{
    new Ext.ux.MergeClientsUtility().init();
});