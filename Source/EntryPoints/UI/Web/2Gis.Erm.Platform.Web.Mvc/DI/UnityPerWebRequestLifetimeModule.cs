using System;
using System.Collections.Concurrent;
using System.Web;

using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.Platform.Web.Mvc.DI
{
    public class UnityPerWebRequestLifetimeModule : IHttpModule
    {
        private static readonly object Key = new object();

        public UnityPerWebRequestLifetimeModule()
        {
        }

        private HttpContextBase Context
        {
            get
            {
                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += (sender, e) => RemoveAllInstances();
        }

        void IHttpModule.Dispose()
        {
        }

        internal static object GetValue(HttpContextBase httpContext, UnityPerWebRequestLifetimeManager lifetimeManagerInstance)
        {
            var instances = GetInstances(httpContext);

            lock (SynchObject)
            {
                if (instances != null && instances.Count > 0)
                {
                    Object value = null;
                    return instances.TryGetValue(lifetimeManagerInstance, out value)?value:null;
                }
            }

            return null;
        }

        internal static void SetValue(HttpContextBase httpContext, UnityPerWebRequestLifetimeManager lifetimeManagerInstance, object newValue)
        {
            var instances = GetInstances(httpContext);

            if (instances != null)
            {
                if (!instances.TryAdd(lifetimeManagerInstance, newValue))
                {
                    Object oldValue = null;
                    if (instances.TryGetValue(lifetimeManagerInstance, out oldValue) && !ReferenceEquals(newValue, oldValue))
                    {
                        var disposable = oldValue as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }

                        if (newValue == null)
                        {
                            instances.TryRemove(lifetimeManagerInstance, out newValue);
                        }
                        else
                        {
                            instances.TryUpdate(lifetimeManagerInstance, newValue, oldValue);
                        }
                    }
                }
            }
        }

        private static readonly object SynchObject = new object();

        private static ConcurrentDictionary<UnityPerWebRequestLifetimeManager, object> GetInstances(HttpContextBase httpContext)
        {
            ConcurrentDictionary<UnityPerWebRequestLifetimeManager, object> instances;
            lock (httpContext.Items)
            {
                if (httpContext.Items.Contains(Key))
                {
                    instances = (ConcurrentDictionary<UnityPerWebRequestLifetimeManager, object>)httpContext.Items[Key];
                }
                else
                {
                    instances = new ConcurrentDictionary<UnityPerWebRequestLifetimeManager, object>();
                    httpContext.Items.Add(Key, instances);
                }
            }
            
            return instances;
        }

        private void RemoveAllInstances()
        {
            var httpContext = Context;
            ConcurrentDictionary<UnityPerWebRequestLifetimeManager, object> instances = null;
            lock (httpContext.Items)
            {
                if (httpContext.Items.Contains(Key))
                {
                    instances = (ConcurrentDictionary<UnityPerWebRequestLifetimeManager, object>)httpContext.Items[Key];
                    lock (SynchObject)
                    {
                        httpContext.Items[Key] = new ConcurrentDictionary<UnityPerWebRequestLifetimeManager, object>();
                    }
                }
            }

            if (instances != null && instances.Count > 0)
            {
                foreach (var entry in instances)
                {
                    var disposable = entry.Value as IDisposable;
                    if (disposable != null)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (PendingChangesNotHandledException ex)
                        {
                            // Если код и так сообщает об ошибке, то незачем накидывать сверху. 
                            // В противном случае сервер сообщит об ожидающих изменениях, а не о сути проблемы.
                            if(Context.Response.StatusCode != 500)
                                httpContext.AddError(ex);
                        }
                    }
                }

                instances.Clear();
            }
        }
    }
}