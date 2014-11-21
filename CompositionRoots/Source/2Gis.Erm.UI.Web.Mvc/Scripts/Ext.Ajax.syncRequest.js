Ext.apply(Ext.lib.Ajax, {
    syncRequest: function (method, uri, cb, data, options)
    {
        var activeX = ['Msxml2.XMLHTTP.6.0',
                   'Msxml2.XMLHTTP.3.0',
                   'Msxml2.XMLHTTP'],
        CONTENTTYPE = 'Content-Type',
        pub = this,
                releaseObject = function (o)
                {
                    if (o.tId)
                    {
                        pub.conn[o.tId] = null;
                    }
                    o.conn = null;
                    o = null;
                },
            createResponseObject = function (o, callbackArg)
            {
                var headerObj = {}, headerStr, conn = o.conn, t, s,
                // see: https://prototype.lighthouseapp.com/projects/8886/tickets/129-ie-mangles-http-response-status-code-204-to-1223
                    isBrokenStatus = conn.status == 1223;
                try
                {
                    headerStr = o.conn.getAllResponseHeaders();
                    Ext.each(headerStr.replace(/\r\n/g, '\n').split('\n'), function (v)
                    {
                        t = v.indexOf(':');
                        if (t >= 0)
                        {
                            s = v.substr(0, t).toLowerCase();
                            if (v.charAt(t + 1) == ' ')
                            {
                                ++t;
                            }
                            headerObj[s] = v.substr(t + 1);
                        }
                    });
                } catch (e) { }

                return {
                    tId: o.tId,
                    // Normalize the status and statusText when IE returns 1223, see the above link.
                    status: isBrokenStatus ? 204 : conn.status,
                    statusText: isBrokenStatus ? 'No Content' : conn.statusText,
                    getResponseHeader: function (header) { return headerObj[header.toLowerCase()]; },
                    getAllResponseHeaders: function () { return headerStr; },
                    responseText: conn.responseText,
                    responseXML: conn.responseXML,
                    argument: callbackArg
                };
            },
            handleTransactionResponse = function (o, callback, isAbort, isTimeout)
            {
                if (!callback)
                {
                    releaseObject(o);
                    return;
                }

                var httpStatus, responseObject;

                try
                {
                    if (o.conn.status !== undefined && o.conn.status != 0)
                    {
                        httpStatus = o.conn.status;
                    }
                    else
                    {
                        httpStatus = 13030;
                    }
                }
                catch (e)
                {
                    httpStatus = 13030;
                }

                if ((httpStatus >= 200 && httpStatus < 300) || (Ext.isIE && httpStatus == 1223))
                {
                    responseObject = createResponseObject(o, callback.argument);
                    if (callback.success)
                    {
                        if (!callback.scope)
                        {
                            callback.success(responseObject);
                        }
                        else
                        {
                            callback.success.apply(callback.scope, [responseObject]);
                        }
                    }
                }
                else
                {
                    switch (httpStatus)
                    {
                        case 12002:
                        case 12029:
                        case 12030:
                        case 12031:
                        case 12152:
                        case 13030:
                            responseObject = {
                                tId: o.tId,
                                status: isAbort ? -1 : 0,
                                statusText: isAbort ? 'transaction aborted' : 'communication failure',
                                isAbort: isAbort,
                                isTimeout: isTimeout,
                                argument: callback.argument
                            };
                            if (callback.failure)
                            {
                                if (!callback.scope)
                                {
                                    callback.failure(responseObject);
                                }
                                else
                                {
                                    callback.failure.apply(callback.scope, [responseObject]);
                                }
                            }
                            break;
                        default:
                            responseObject = createResponseObject(o, callback.argument);
                            if (callback.failure)
                            {
                                if (!callback.scope)
                                {
                                    callback.failure(responseObject);
                                }
                                else
                                {
                                    callback.failure.apply(callback.scope, [responseObject]);
                                }
                            }
                    }
                }

                releaseObject(o);
                responseObject = null;
            },
                checkResponse = function (o, callback, conn, tId, poll, cbTimeout)
                {
                    if (conn && conn.readyState == 4)
                    {
                        clearInterval(poll[tId]);
                        poll[tId] = null;

                        if (cbTimeout)
                        {
                            clearTimeout(pub.timeout[tId]);
                            pub.timeout[tId] = null;
                        }
                        handleTransactionResponse(o, callback);
                    }
                },

            checkTimeout = function (o, callback)
            {
                pub.abort(o, callback, true);
            },
            handleReadyState = function (o, callback)
            {
                callback = callback || {};
                var conn = o.conn,
                    tId = o.tId,
                    poll = pub.poll,
                    cbTimeout = callback.timeout || null;

                if (cbTimeout)
                {
                    pub.conn[tId] = conn;
                    pub.timeout[tId] = setTimeout(checkTimeout.createCallback(o, callback), cbTimeout);
                }
                poll[tId] = setInterval(checkResponse.createCallback(o, callback, conn, tId, poll, cbTimeout), pub.pollInterval);
            },
            setHeader = function (o)
            {
                var conn = o.conn,
            prop,
            headers = {};

                function setTheHeaders(connection, headers)
                {
                    for (prop in headers)
                    {
                        if (headers.hasOwnProperty(prop))
                        {
                            connection.setRequestHeader(prop, headers[prop]);
                        }
                    }
                }

                Ext.apply(headers, pub.headers, pub.defaultHeaders);
                setTheHeaders(conn, headers);
                delete pub.headers;
            },
            initHeader = function (label, value)
            {
                (pub.headers = pub.headers || {})[label] = value;
            },
            createXhrObject = function (transactionId)
            {
                var http;
                try
                {
                    http = new XMLHttpRequest();
                } catch (e)
                {
                    for (var i = 0; i < activeX.length; ++i)
                    {
                        try
                        {
                            http = new ActiveXObject(activeX[i]);
                            break;
                        } catch (e) { }
                    }
                } finally
                {
                    return { conn: http, tId: transactionId };
                }
            },
            getConnectionObject = function ()
            {
                var o;

                try
                {
                    if (o = createXhrObject(pub.transactionId))
                    {
                        pub.transactionId++;
                    }
                } catch (e)
                {
                } finally
                {
                    return o;
                }
            },
        syncRequest = function (method, uri, callback, postData)
        {
            var o = getConnectionObject() || null;
            if (o)
            {
                o.conn.open(method, uri, false);

                if (pub.useDefaultXhrHeader)
                {
                    initHeader('X-Requested-With', pub.defaultXhrHeader);
                }

                if (postData && pub.useDefaultHeader && (!pub.headers || !pub.headers[CONTENTTYPE]))
                {
                    initHeader(CONTENTTYPE, pub.defaultPostHeader);
                }

                if (pub.defaultHeaders || pub.headers)
                {
                    setHeader(o);
                }

                handleReadyState(o, callback);
                o.conn.send(postData || null);
            }
            return o;
        };

        if (options)
        {
            var me = this,
                xmlData = options.xmlData,
                jsonData = options.jsonData,
                hs;

            Ext.applyIf(me, options);

            if (xmlData || jsonData)
            {
                hs = me.headers;
                if (!hs || !hs[CONTENTTYPE])
                {
                    initHeader(CONTENTTYPE, xmlData ? 'text/xml' : 'application/json');
                }
                data = xmlData || (!Ext.isPrimitive(jsonData) ? Ext.encode(jsonData) : jsonData);
            }
        }
        return syncRequest(method || options.method || "POST", uri, cb, data);
    }
});
Ext.apply(Ext.Ajax, {
    syncRequest: function (o)
    {
        var POST = "POST", GET = "GET", BEFOREREQUEST = "beforerequest", UNDEFINED = undefined, WINDOW = window;

        var me = this;
        if (me.fireEvent(BEFOREREQUEST, me, o))
        {
            if (o.el)
            {
                if (!Ext.isEmpty(o.indicatorText))
                {
                    me.indicatorText = '<div class="loading-indicator">' + o.indicatorText + "</div>";
                }
                if (me.indicatorText)
                {
                    Ext.getDom(o.el).innerHTML = me.indicatorText;
                }
                o.success = (Ext.isFunction(o.success) ? o.success : function () { }).createInterceptor(function (response)
                {
                    Ext.getDom(o.el).innerHTML = response.responseText;
                });
            }

            var p = o.params,
                    url = o.url || me.url,
                    method,
                    cb = { success: me.handleResponse,
                        failure: me.handleFailure,
                        scope: me,
                        argument: { options: o },
                        timeout: Ext.num(o.timeout, me.timeout)
                    },
                    form,
                    serForm;


            if (Ext.isFunction(p))
            {
                p = p.call(o.scope || WINDOW, o);
            }

            p = Ext.urlEncode(me.extraParams, Ext.isObject(p) ? Ext.urlEncode(p) : p);

            if (Ext.isFunction(url))
            {
                url = url.call(o.scope || WINDOW, o);
            }

            if ((form = Ext.getDom(o.form)))
            {
                url = url || form.action;
                if (o.isUpload || (/multipart\/form-data/i.test(form.getAttribute("enctype"))))
                {
                    return me.doFormUpload.call(me, o, p, url);
                }
                serForm = Ext.lib.Ajax.serializeForm(form);
                p = p ? (p + '&' + serForm) : serForm;
            }

            method = o.method || me.method || ((p || o.xmlData || o.jsonData) ? POST : GET);

            if (method === GET && (me.disableCaching && o.disableCaching !== false) || o.disableCaching === true)
            {
                var dcp = o.disableCachingParam || me.disableCachingParam;
                url = Ext.urlAppend(url, dcp + '=' + (new Date().getTime()));
            }

            o.headers = Ext.apply(o.headers || {}, me.defaultHeaders || {});

            if (o.autoAbort === true || me.autoAbort)
            {
                me.abort();
            }

            if ((method == GET || o.xmlData || o.jsonData) && p)
            {
                url = Ext.urlAppend(url, p);
                p = '';
            }
            return (me.transId = Ext.lib.Ajax.syncRequest(method, url, cb, p, o));
        } else
        {
            return o.callback ? o.callback.apply(o.scope, [o, UNDEFINED, UNDEFINED]) : null;
        }
    }
});
