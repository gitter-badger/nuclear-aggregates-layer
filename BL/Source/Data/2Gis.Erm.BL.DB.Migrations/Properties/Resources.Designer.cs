﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BL.DB.Migrations.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DoubleGis.Erm.BL.DB.Migrations.Properties.Resources", typeof(Resources).Assembly);
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
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [Billing].[ReplicateDeal]    Script Date: 01.09.2014 10:28:28 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///-- changes
        ///--   24.06.2013, a.rechkalov: замена int -&gt; bigint
        ///--   23.07.2014, i.maslennikov: drop deal profit indicators 
        ///--   12.08.2014, y.baranikhin: добавилась репликация полей по рекламной кампании
        ///--   11.09.2014, a.tukaev: выпиливаем like при поиске пользователя по account
        ///--   02.10.2014, a.rechkalov: выполнил слияние двух предыдущих измен [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string BusinessDirectory_ReplicateDeal_24906 {
            get {
                return ResourceManager.GetString("BusinessDirectory_ReplicateDeal_24906", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /****** Object:  StoredProcedure [Billing].[ReplicateDeal]    Script Date: 01.09.2014 10:28:28 ******/
        ///SET ANSI_NULLS ON
        ///GO
        ///SET QUOTED_IDENTIFIER ON
        ///GO
        ///-- changes
        ///--   24.06.2013, a.rechkalov: замена int -&gt; bigint
        ///--   23.07.2014, i.maslennikov: drop deal profit indicators 
        ///--   12.08.2014, y.baranikhin: добавилась репликация полей по рекламной кампании
        ///--   11.09.2014, a.tukaev: выпиливаем like при поиске пользователя по account
        ///--   02.10.2014, a.rechkalov: выполнил слияние двух предыдущих измен [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string BusinessDirectory_ReplicateDeal_25168 {
            get {
                return ResourceManager.GetString("BusinessDirectory_ReplicateDeal_25168", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ImportExportXml version=&quot;4.0.0.0&quot; languagecode=&quot;1049&quot; generatedBy=&quot;OnPremise&quot;&gt;
        ///  &lt;Entities&gt;
        ///  &lt;/Entities&gt;
        ///  &lt;Roles&gt;
        ///  &lt;/Roles&gt;
        ///  &lt;Workflows&gt;
        ///  &lt;/Workflows&gt;
        ///  &lt;IsvConfig&gt;
        ///    &lt;configuration version=&quot;3.0.0000.0&quot;&gt;
        ///      &lt;Root&gt;
        ///        &lt;ToolBar&gt;
        ///          &lt;Button Icon=&quot;/_imgs/AdvFind/new.GIF&quot; JavaScript=&quot;openStdWin(&apos;https://web-app.prod.erm.2gis.ru/Grid/View/Order?singleDataView=DListOrdersFast&apos;, &apos;FastSearch&apos;, 1000, 600)&quot; Client=&quot;Web&quot;&gt;
        ///            &lt;Titles&gt;
        ///              &lt;Title LCID=&quot;1049&quot; Text=&quot;Бы [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string customizations_201502121619 {
            get {
                return ResourceManager.GetString("customizations_201502121619", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ImportExportXml version=&quot;4.0.0.0&quot; languagecode=&quot;1049&quot; generatedBy=&quot;OnPremise&quot;&gt;
        ///  &lt;Entities&gt;
        ///    &lt;Entity&gt;
        ///      &lt;Name LocalizedName=&quot;Договор&quot; OriginalName=&quot;&quot;&gt;Dg_bargain&lt;/Name&gt;
        ///      &lt;ObjectTypeCode&gt;10010&lt;/ObjectTypeCode&gt;
        ///      &lt;EntityInfo&gt;
        ///        &lt;entity Name=&quot;Dg_bargain&quot;&gt;
        ///          &lt;HasRelatedNotes&gt;False&lt;/HasRelatedNotes&gt;
        ///          &lt;HasRelatedActivities&gt;False&lt;/HasRelatedActivities&gt;
        ///          &lt;ObjectTypeCode&gt;10010&lt;/ObjectTypeCode&gt;
        ///          &lt;CollectionName&gt;Dg_bargains&lt;/CollectionName&gt;
        ///           [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string customizations_22356 {
            get {
                return ResourceManager.GetString("customizations_22356", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ImportExportXml version=&quot;4.0.0.0&quot; languagecode=&quot;1049&quot; generatedBy=&quot;OnPremise&quot;&gt;
        ///  &lt;Entities&gt;
        ///    &lt;Entity&gt;
        ///      &lt;Name LocalizedName=&quot;Договор&quot; OriginalName=&quot;&quot;&gt;Dg_bargain&lt;/Name&gt;
        ///      &lt;ObjectTypeCode&gt;10010&lt;/ObjectTypeCode&gt;
        ///      &lt;EntityInfo&gt;
        ///        &lt;entity Name=&quot;Dg_bargain&quot;&gt;
        ///          &lt;HasRelatedNotes&gt;False&lt;/HasRelatedNotes&gt;
        ///          &lt;HasRelatedActivities&gt;False&lt;/HasRelatedActivities&gt;
        ///          &lt;ObjectTypeCode&gt;10010&lt;/ObjectTypeCode&gt;
        ///          &lt;CollectionName&gt;Dg_bargains&lt;/CollectionName&gt;
        ///           [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string customizations_22366 {
            get {
                return ResourceManager.GetString("customizations_22366", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ImportExportXml version=&quot;4.0.0.0&quot; languagecode=&quot;1049&quot; generatedBy=&quot;OnPremise&quot;&gt;
        ///  &lt;Entities&gt;
        ///    &lt;Entity&gt;
        ///      &lt;Name LocalizedName=&quot;Сделка&quot; OriginalName=&quot;Opportunity&quot;&gt;Opportunity&lt;/Name&gt;
        ///      &lt;ObjectTypeCode&gt;3&lt;/ObjectTypeCode&gt;
        ///      &lt;EntityInfo&gt;
        ///        &lt;entity Name=&quot;Opportunity&quot;&gt;
        ///          &lt;HasRelatedNotes&gt;True&lt;/HasRelatedNotes&gt;
        ///          &lt;HasRelatedActivities&gt;True&lt;/HasRelatedActivities&gt;
        ///          &lt;ObjectTypeCode&gt;3&lt;/ObjectTypeCode&gt;
        ///          &lt;CollectionName&gt;Opportunities&lt;/CollectionName&gt;
        ///       [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string customizations_23099 {
            get {
                return ResourceManager.GetString("customizations_23099", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ImportExportXml version=&quot;4.0.0.0&quot; languagecode=&quot;1049&quot; generatedBy=&quot;OnPremise&quot;&gt;
        ///  &lt;Entities&gt;
        ///    &lt;Entity&gt;
        ///      &lt;Name LocalizedName=&quot;Рубрика&quot; OriginalName=&quot;&quot;&gt;Dg_category&lt;/Name&gt;
        ///      &lt;ObjectTypeCode&gt;10008&lt;/ObjectTypeCode&gt;
        ///      &lt;EntityInfo&gt;
        ///        &lt;entity Name=&quot;Dg_category&quot;&gt;
        ///          &lt;HasRelatedNotes&gt;True&lt;/HasRelatedNotes&gt;
        ///          &lt;HasRelatedActivities&gt;True&lt;/HasRelatedActivities&gt;
        ///          &lt;ObjectTypeCode&gt;10008&lt;/ObjectTypeCode&gt;
        ///          &lt;CollectionName&gt;Dg_categories&lt;/CollectionName&gt;
        ///         [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string customizations_23209 {
            get {
                return ResourceManager.GetString("customizations_23209", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ImportExportXml version=&quot;4.0.0.0&quot; languagecode=&quot;1049&quot; generatedBy=&quot;OnPremise&quot;&gt;
        ///  &lt;Entities&gt;
        ///    &lt;Entity&gt;
        ///      &lt;Name LocalizedName=&quot;Заказ&quot; OriginalName=&quot;&quot;&gt;Dg_order&lt;/Name&gt;
        ///      &lt;ObjectTypeCode&gt;10014&lt;/ObjectTypeCode&gt;
        ///      &lt;EntityInfo&gt;
        ///        &lt;entity Name=&quot;Dg_order&quot;&gt;
        ///          &lt;HasRelatedNotes&gt;True&lt;/HasRelatedNotes&gt;
        ///          &lt;HasRelatedActivities&gt;True&lt;/HasRelatedActivities&gt;
        ///          &lt;ObjectTypeCode&gt;10014&lt;/ObjectTypeCode&gt;
        ///          &lt;CollectionName&gt;Dg_orders&lt;/CollectionName&gt;
        ///          &lt;LogicalCo [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string customizations_23921 {
            get {
                return ResourceManager.GetString("customizations_23921", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ImportExportXml version=&quot;4.0.0.0&quot; languagecode=&quot;1049&quot; generatedBy=&quot;OnPremise&quot;&gt;
        ///  &lt;Entities&gt;
        ///  &lt;/Entities&gt;
        ///  &lt;Roles&gt;
        ///  &lt;/Roles&gt;
        ///  &lt;Workflows&gt;
        ///  &lt;/Workflows&gt;
        ///  &lt;IsvConfig&gt;
        ///    &lt;configuration version=&quot;3.0.0000.0&quot;&gt;
        ///      &lt;Root&gt;
        ///        &lt;ToolBar&gt;
        ///          &lt;Button Icon=&quot;/_imgs/AdvFind/new.GIF&quot; JavaScript=&quot;openStdWin(&apos;https://web-app.prod.erm.2gis.ru/Grid/View/Order?singleDataView=DListOrdersFast&apos;, &apos;FastSearch&apos;, 1000, 600)&quot; Client=&quot;Web&quot;&gt;
        ///            &lt;Titles&gt;
        ///              &lt;Title LCID=&quot;1049&quot; Text=&quot;Бы [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string customizations_24070 {
            get {
                return ResourceManager.GetString("customizations_24070", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;ImportExportXml version=&quot;4.0.0.0&quot; languagecode=&quot;1049&quot; generatedBy=&quot;OnPremise&quot;&gt;
        ///  &lt;Entities&gt;
        ///  &lt;/Entities&gt;
        ///  &lt;Roles&gt;
        ///  &lt;/Roles&gt;
        ///  &lt;Workflows&gt;
        ///  &lt;/Workflows&gt;
        ///  &lt;IsvConfig&gt;
        ///    &lt;configuration version=&quot;3.0.0000.0&quot;&gt;
        ///      &lt;Root&gt;
        ///        &lt;!-- 		Application Level Tool Bar, не удалять !		--&gt;
        ///      &lt;/Root&gt;
        ///      &lt;Entities&gt;
        ///        &lt;Entity name=&quot;activitypointer&quot;&gt;
        ///          &lt;Grid&gt;
        ///            &lt;MenuBar&gt;
        ///              &lt;ActionsMenu&gt;
        ///                &lt;MenuItem JavaScript=&quot;window.showModalDialog(&apos;https://w [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Migration15501 {
            get {
                return ResourceManager.GetString("Migration15501", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- changes
        ///--   24.06.2013, a.rechkalov: замена int -&gt; bigint
        ///--   30.07.2013, a.tukaev: [ERM-387] заменил все вхождения Territories.DgppId на Territories.Id
        ///--   10.09.2013, y.baranihin: dgppid-&gt;id
        ///--	 16.09.2013, v.lapeev: Перевел строки в Unicode
        ///--   25.11.2013, y.baranihin: изменен алгоритм обновления территории у фирмы
        ///--   29.01.2014, y.baranihin: при изменении территории у фирмы будем проставлять дату изменения
        ///ALTER PROCEDURE [Integration].[UpdateBuildings]
        ///       @buildingsXml [xml],
        ///	    [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Migration15518 {
            get {
                return ResourceManager.GetString("Migration15518", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to -- changes
        ///--   24.06.2013, a.rechkalov: замена int -&gt; bigint
        ///--   30.07.2013, a.tukaev: [ERM-387] заменил все вхождения Territories.DgppId на Territories.Id
        ///--   10.09.2013, y.baranihin: dgppid-&gt;id
        ///--	 16.09.2013, v.lapeev: Перевел строки в Unicode
        ///--   25.11.2013, y.baranihin: изменен алгоритм обновления территории у фирмы
        ///--   29.01.2014, y.baranihin: при изменении территории у фирмы будем проставлять дату изменения
        ///--   30.01.2014, y.baranihin: включаем репликацию клиентов
        ///ALTER PROCEDURE [Integ [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Migration15632 {
            get {
                return ResourceManager.GetString("Migration15632", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///-- changes
        ///--   24.06.2013, a.rechkalov: замена int -&gt; bigint
        ///--   15.07.2014, y.baranihin: репликация типа договора и даты окончания действия
        ///ALTER PROCEDURE [Billing].[ReplicateBargain]
        ///	@Id bigint = NULL
        ///AS
        ///    SET NOCOUNT ON;
        ///	
        ///    IF @Id IS NULL
        ///	    RETURN 0;
        ///		
        ///    SET XACT_ABORT ON;
        ///
        ///    DECLARE @CrmId UNIQUEIDENTIFIER;
        ///    DECLARE @CreatedByUserId UNIQUEIDENTIFIER;
        ///    DECLARE @CreatedByUserDomainName NVARCHAR(250);
        ///    DECLARE @ModifiedByUserId UNIQUEIDENTIFIER;
        ///    DECLARE @Mod [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Migration22361 {
            get {
                return ResourceManager.GetString("Migration22361", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///-- changes
        ///--   24.06.2013, a.rechkalov: замена int -&gt; bigint
        ///--   15.07.2014, y.baranihin: репликация типа договора и даты окончания действия
        ///ALTER PROCEDURE [Billing].[ReplicateBargain]
        ///	@Id bigint = NULL
        ///AS
        ///    SET NOCOUNT ON;
        ///	
        ///    IF @Id IS NULL
        ///	    RETURN 0;
        ///		
        ///    SET XACT_ABORT ON;
        ///
        ///    DECLARE @CrmId UNIQUEIDENTIFIER;
        ///    DECLARE @CreatedByUserId UNIQUEIDENTIFIER;
        ///    DECLARE @CreatedByUserDomainName NVARCHAR(250);
        ///    DECLARE @ModifiedByUserId UNIQUEIDENTIFIER;
        ///    DECLARE @Mod [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string Migration22367 {
            get {
                return ResourceManager.GetString("Migration22367", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string Migration23100 {
            get {
                return ResourceManager.GetString("Migration23100", resourceCulture);
            }
        }
    }
}
