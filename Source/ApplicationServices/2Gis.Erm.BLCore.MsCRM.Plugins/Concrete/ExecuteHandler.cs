using System;
using System.Collections.Generic;
using System.Xml;

using Microsoft.Crm.Sdk;

namespace DoubleGis.Erm.BLCore.MsCRM.Plugins.Concrete
{
	public sealed class ExecuteHandler : BasePlugin, IPlugin
    {

        #region IPlugin Members

	    private static readonly List<String> AdditionalEntitiesToSkip = new List<string>()
	        {
	            Microsoft.Crm.SdkTypeProxy.EntityName.savedquery.ToString(), //To make Advanced Find Work
	            Microsoft.Crm.SdkTypeProxy.EntityName.businessunitnewsarticle.ToString(), //To make Literature work
	            Microsoft.Crm.SdkTypeProxy.EntityName.resource.ToString(), //To make Service calendar work
	            Microsoft.Crm.SdkTypeProxy.EntityName.systemuser.ToString(),//To make Service calendar work
	            Microsoft.Crm.SdkTypeProxy.EntityName.equipment.ToString(),//To make Service calendar work
	            Microsoft.Crm.SdkTypeProxy.EntityName.asyncoperation.ToString(),
	            Microsoft.Crm.SdkTypeProxy.EntityName.userquery.ToString()
	        };


        public void Execute(IPluginExecutionContext context)
		{
            if (context.Depth != 1 || System.Web.HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.Contains("_controls/lookup/lookupdata.aspx")) 
                //To calculate count of pages and records another one fetch will be executed
				return;//so to avoid infinite loops i need to check the depth of request - if it more then 2 - return

			if (context.MessageName == "Execute" && context.InputParameters.Contains("FetchXml"))
			{
				var indoc = new XmlDocument();
				indoc.LoadXml((string)context.InputParameters["FetchXml"]);

                //This fix Merge functionality
                if (indoc.DocumentElement.Attributes["count"] == null)
                    return;

				//Retrieve name of entity to display
				var entityName = indoc.SelectSingleNode("//fetch/entity").Attributes["name"].InnerText;
                if (Activities.Contains(entityName))
                {
                    return;
                }

                if (AdditionalEntitiesToSkip.Contains(entityName))
                {
                    return;
                }

			    //CrmService Creation
				ICrmService crmService = context.CreateCrmService(true);

				//Count of records by page - for calculation of pages count
				int pagecount = int.Parse(indoc.DocumentElement.Attributes["count"].InnerText);

                if (pagecount == 0)
                    return;

				//Total record count by fetch
                int totalRecordCount = GetTotalRecordCount(indoc, crmService, entityName);
                crmService.Dispose();

                //string primaryFieldName = GetPrimaryFieldName(context, entityName);
                int totalPageCount = (totalRecordCount / pagecount) + ((totalRecordCount % pagecount) == 0 ? 0 : 1);

                string result = string.Format("var tInfo={{tRec: {0}, tPages: {1}}}", totalRecordCount, totalPageCount);

				//Result XML which is the result shown in Grid
				var outdoc = new XmlDocument();
				outdoc.LoadXml((string)context.OutputParameters["FetchXmlResult"]);

                var primaryFieldName = GetPrimaryFieldName(context, entityName);
                // В xml с результатами запроса подпихиваем информацию о количестве записей путём
                // создания фейковой записи с индексом 0.
                // У этой записи поля с названиями entityName + "id" (ключ записи) и primaryFieldName для данной записи. 
                // На клиентской стороне похаканный grid.htc процессит запись и удаляет её.
                XmlNode resultNodeText = outdoc.CreateNode(XmlNodeType.Element, primaryFieldName, null);
				resultNodeText.InnerText = result;

                XmlNode resultNodeId = outdoc.CreateNode(XmlNodeType.Element, entityName + "id", null);
			    resultNodeId.InnerText = new Guid("{FF11FA35-7D4C-4AD1-8D1C-B52F44EBD12A}").ToString();

				XmlNode res = outdoc.CreateNode(XmlNodeType.Element, "result", null);
				res.AppendChild(resultNodeText);
				res.AppendChild(resultNodeId);

			    //This code repair report view
                if (entityName == Microsoft.Crm.SdkTypeProxy.EntityName.report.ToString())
				{
					XmlNode resultNodeType = outdoc.CreateNode(XmlNodeType.Element, "reporttypecode", null);
					resultNodeType.InnerText = "1";
					res.AppendChild(resultNodeType);
				}

				//Adding record with label of count of pages and records as a first record in recordset
				outdoc.SelectSingleNode("//resultset").InsertBefore(res, outdoc.SelectSingleNode("//resultset").FirstChild);
				context.OutputParameters["FetchXmlResult"] = outdoc.OuterXml;
			}
		}

        private static string GetNonKeyFieldName(XmlDocument document, string entityname)
        {
            string keyFieldName = entityname + "id";
            foreach(XmlNode node in document.SelectNodes("//fetch/entity/attribute"))
            {
                string fieldName = node.Attributes["name"].Value;
                if(fieldName != keyFieldName)
                {
                    return fieldName;
                }
            }
            return null;
        }

		#endregion

        private int GetTotalRecordCount(XmlDocument document, ICrmService cmrservice, string entityName)
        {
            TransformFetchXml(document, entityName);
            string fullResult = cmrservice.Fetch(document.OuterXml);

            var fullResultDocument = new XmlDocument();
            fullResultDocument.LoadXml(fullResult);

            return int.Parse(fullResultDocument.SelectSingleNode("//resultset/result/C").InnerText);
        }

        private void TransformFetchXml(XmlDocument document, string entityName)
        {
            document.DocumentElement.Attributes.Remove(document.DocumentElement.Attributes["count"]);
            document.DocumentElement.Attributes.Remove(document.DocumentElement.Attributes["page"]);

            var entityNode = document.SelectSingleNode("//fetch/entity");
            foreach (XmlNode node in entityNode.SelectNodes("./attribute"))
                entityNode.RemoveChild(node);

            foreach (XmlNode node in entityNode.SelectNodes("./order"))
                entityNode.RemoveChild(node);

            foreach (XmlNode node in entityNode.SelectNodes("./link-entity"))
                foreach (XmlNode subnode in node.SelectNodes("./attribute"))
                    node.RemoveChild(subnode);

            XmlAttribute aggrAttr = document.CreateAttribute("aggregate");
            aggrAttr.Value = "true";
            document.DocumentElement.Attributes.Append(aggrAttr);

            XmlNode field = document.CreateNode(XmlNodeType.Element, "attribute", null);

            XmlAttribute nameAttr = document.CreateAttribute("name");
            nameAttr.Value = string.Format("{0}id", (Activities.Contains(entityName) ? "activity" : entityName));
            field.Attributes.Append(nameAttr);

            XmlAttribute aggregateAttr = document.CreateAttribute("aggregate");
            aggregateAttr.Value = "count";
            field.Attributes.Append(aggregateAttr);

            XmlAttribute aliasAttr = document.CreateAttribute("alias");
            aliasAttr.Value = "C";
            field.Attributes.Append(aliasAttr);

            entityNode.AppendChild(field);
        }
    }
}