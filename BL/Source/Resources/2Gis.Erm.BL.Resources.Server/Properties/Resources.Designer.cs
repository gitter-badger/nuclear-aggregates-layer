﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BL.Resources.Server.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DoubleGis.Erm.BL.Resources.Server.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Недостаточно прав для смены периода лимита.
        /// </summary>
        public static string AccessToChangePeriodOfLimitIsDenied {
            get {
                return ResourceManager.GetString("AccessToChangePeriodOfLimitIsDenied", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cумма для увелечения лимита неактуальна.
        /// </summary>
        public static string AmountToIncreaseLimitIsOutdated {
            get {
                return ResourceManager.GetString("AmountToIncreaseLimitIsOutdated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Рекламное агентство не может быть указано в качестве дочернего клиента.
        /// </summary>
        public static string AnAdvertisingAgencyCannotBeSpecifiedAsAChildClient {
            get {
                return ResourceManager.GetString("AnAdvertisingAgencyCannotBeSpecifiedAsAChildClient", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Выберите профиль юридического лица клиента.
        /// </summary>
        public static string ChangeProfilesOperationMessage {
            get {
                return ResourceManager.GetString("ChangeProfilesOperationMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Выберите профиль.
        /// </summary>
        public static string ChangeProfilesOperationTitle {
            get {
                return ResourceManager.GetString("ChangeProfilesOperationTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        public static string ClientHasParentLinkToAdvAgency {
            get {
                return ResourceManager.GetString("ClientHasParentLinkToAdvAgency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Связь уже существует.
        /// </summary>
        public static string ClientLinkAlreadyExists {
            get {
                return ResourceManager.GetString("ClientLinkAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Неверно заданы типы сущностей для добавления связи между клиентами.
        /// </summary>
        public static string InvalidEntityTypesForLinking {
            get {
                return ResourceManager.GetString("InvalidEntityTypesForLinking", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Увеличить можно только одобренный лимит.
        /// </summary>
        public static string OnlyApprovedLimitCanBeIncreased {
            get {
                return ResourceManager.GetString("OnlyApprovedLimitCanBeIncreased", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Внимание! Профиль был изменен. Указанный профиль будет прикреплен к заказу..
        /// </summary>
        public static string OrderLegalPersonProfileChangeNotification {
            get {
                return ResourceManager.GetString("OrderLegalPersonProfileChangeNotification", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Не указаны идентификаторы родительского или дочернего клиента.
        /// </summary>
        public static string ParentOrChildIdsNotSpecified {
            get {
                return ResourceManager.GetString("ParentOrChildIdsNotSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Поле {0} должно иметь формат xx.xxx.xxx-x.
        /// </summary>
        public static string RutFormatError {
            get {
                return ResourceManager.GetString("RutFormatError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Идентификаторы связываемых сущностей равны..
        /// </summary>
        public static string SameIdsForEntitiesToLink {
            get {
                return ResourceManager.GetString("SameIdsForEntitiesToLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Указана некорректная сумма для увелечения лимита.
        /// </summary>
        public static string WrongAmountToIncreaseLimit {
            get {
                return ResourceManager.GetString("WrongAmountToIncreaseLimit", resourceCulture);
            }
        }
    }
}
