﻿using System;
using System.Linq;
using System.Web.Mvc;

using DoubleGis.Erm.BL.UI.Web.Mvc.Models.Report;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Utils
{
    public class ModelBinderProvider : IModelBinderProvider
    {
        private readonly IModelBinder _modelBinder;

        public ModelBinderProvider()
        {
            _modelBinder = new DefaultModelBinder();
        }

        IModelBinder IModelBinderProvider.GetBinder(Type modelType)
        {
            return typeof(ReportModel).IsAssignableFrom(modelType)
                       ? _modelBinder
                       : null;
        }

        private sealed class DefaultModelBinder : System.Web.Mvc.DefaultModelBinder
        {
            public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                if (bindingContext.ModelType == typeof(ReportModel))
                {
                    return BindReport(controllerContext, bindingContext);
                }

                return base.BindModel(controllerContext, bindingContext);
            }

            private object BindReport(ControllerContext controllerContext, ModelBindingContext bindingContext)
            {
                var typeName = bindingContext.ValueProvider.GetValue("ReportType").AttemptedValue;
                if (string.IsNullOrWhiteSpace(typeName))
                {
                    throw new ArgumentException("Bad report type");
                }

                var type = AppDomain.CurrentDomain.GetAssemblies()
                                    .Select(assembly => assembly.GetType(typeName))
                                    .SingleOrDefault(t => t != null);

                if (type == null)
                {
                    // Сервер не знает о типе данных, который был использован для построения модели. Даём шанс позже вернуться к этому вопросу.
                    return new DelayedReportModel(this, controllerContext, bindingContext);
                }

                if (type == null || !typeof(ReportModel).IsAssignableFrom(type))
                {
                    throw new ArgumentException("Bad report type");
                }

                var constructor = type.GetConstructor(new Type[0]);

                if (constructor == null)
                {
                    throw new ArgumentException("Bad report type");
                }

                bindingContext.ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => constructor.Invoke(new object[0]), type);
                var model = base.BindModel(controllerContext, bindingContext);
                return model;
            }
        }
    }
}