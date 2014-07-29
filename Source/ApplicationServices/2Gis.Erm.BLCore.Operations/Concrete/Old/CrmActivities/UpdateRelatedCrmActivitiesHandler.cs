using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Common.Infrastructure.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.Sdk.Query;
using Microsoft.Crm.SdkTypeProxy;
using Microsoft.Xrm.Client.Data.Services;

using Response = DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse.Response;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.CrmActivities
{
    public sealed class UpdateRelatedCrmActivitiesRequest : Platform.API.Core.Operations.RequestResponse.Request
    {
        public Guid CrmFromObjectCode { get; set; }
        public Guid CrmToObjectCode { get; set; }
        public EntityName CrmObjectType { get; set; }
    }

    public sealed class UpdateRelatedCrmActivitiesHandler : RequestHandler<UpdateRelatedCrmActivitiesRequest, EmptyResponse>
    {
        private readonly IMsCrmSettings _msCrmSettings;

        public UpdateRelatedCrmActivitiesHandler(IMsCrmSettings msCrmSettings)
        {
            _msCrmSettings = msCrmSettings;
        }

        protected override EmptyResponse Handle(UpdateRelatedCrmActivitiesRequest request)
        {
            var crmDataContext = _msCrmSettings.CreateDataContext();

            UpdateTasks(crmDataContext, request.CrmObjectType, request.CrmFromObjectCode, request.CrmToObjectCode);
            UpdateFaxes(crmDataContext, request.CrmObjectType, request.CrmFromObjectCode, request.CrmToObjectCode);
            UpdatePhonecalls(crmDataContext, request.CrmObjectType, request.CrmFromObjectCode, request.CrmToObjectCode);
            UpdateEmails(crmDataContext, request.CrmObjectType, request.CrmFromObjectCode, request.CrmToObjectCode);

            UpdateLetters(crmDataContext, request.CrmObjectType, request.CrmFromObjectCode, request.CrmToObjectCode);
            UpdateAppointments(crmDataContext, request.CrmObjectType, request.CrmFromObjectCode, request.CrmToObjectCode);
            UpdateServiceAppointments(crmDataContext, request.CrmObjectType, request.CrmFromObjectCode, request.CrmToObjectCode);

            return Response.Empty;
        }

        private void UpdateTasks(CrmDataContext crmDataContext, EntityName crmObjectType, Guid fromObjectId, Guid toObjectId)
        {
            const EntityName ActivityType = EntityName.task;

            var regardingTasks = GetRegardingActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var task in regardingTasks.BusinessEntities)
            {
                var localTask = (task)task;
                localTask.regardingobjectid = new Lookup
                {
                    Value = toObjectId,
                    type = crmObjectType.ToString()
                };

                crmDataContext.UsingService(s => s.Update(localTask));
            }

            if (crmObjectType == EntityName.systemuser)
            {
                var ownedActivities = GetOwnedActivitiesFor(crmDataContext, fromObjectId, ActivityType);
                foreach (task task in ownedActivities.BusinessEntities)
                {
                    var assignee = new SecurityPrincipal
                                       {
                                           PrincipalId = toObjectId
                                       };
                    var target = new TargetOwnedDynamic
                                     {
                                         EntityId = task.activityid.Value, 
                                         EntityName = ActivityType.ToString()
                                     };
                    crmDataContext.UsingService(service => service.Execute(new AssignRequest
                                                                                {
                                                                                    Assignee = assignee, 
                                                                                    Target = target
                                                                                }));
                }
            }
        }

        private void UpdateFaxes(CrmDataContext crmDataContext, EntityName crmObjectType, Guid fromObjectId, Guid toObjectId)
        {
            //Факс: В отношении 
            const EntityName ActivityType = EntityName.fax;

            var regardingFaxes = GetRegardingActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var fax in regardingFaxes.BusinessEntities)
            {
                var localFax = (fax)fax;
                localFax.regardingobjectid = new Lookup
                {
                    Value = toObjectId,
                    type = crmObjectType.ToString()
                };

                crmDataContext.UsingService(s => s.Update(localFax));
            }

            //отправители и получатели
            var participantFaxes = GetParticipantActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var participantFax in participantFaxes.BusinessEntities)
            {
                var localFax = (fax)participantFax;
                var senders = localFax.from.ToList();
                var oldIndex = senders.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    senders[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localFax.from = senders.ToArray();

                var recipients = localFax.to.ToList();
                oldIndex = recipients.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    recipients[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localFax.to = recipients.ToArray();

                crmDataContext.UsingService(s => s.Update(localFax));
            }
            if (crmObjectType == EntityName.systemuser)
            {
                var ownedActivities = GetOwnedActivitiesFor(crmDataContext, fromObjectId, ActivityType);
                foreach (fax fax in ownedActivities.BusinessEntities)
                {
                    var assignee = new SecurityPrincipal
                    {
                        PrincipalId = toObjectId
                    };
                    var target = new TargetOwnedDynamic
                    {
                        EntityId = fax.activityid.Value,
                        EntityName = ActivityType.ToString()
                    };
                    
                    crmDataContext.UsingService(service => service.Execute(new AssignRequest
                    {
                        Assignee = assignee,
                        Target = target
                    }));
                }
            }
        }

        private void UpdatePhonecalls(CrmDataContext crmDataContext, EntityName crmObjectType, Guid fromObjectId, Guid toObjectId)
        {
            const EntityName ActivityType = EntityName.phonecall;
            var regardingPhonecalls = GetRegardingActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var phonecall in regardingPhonecalls.BusinessEntities)
            {
                var localphonecall = (phonecall)phonecall;
                localphonecall.regardingobjectid = new Lookup
                {
                    Value = toObjectId,
                    type = crmObjectType.ToString()
                };

                crmDataContext.UsingService(s => s.Update(localphonecall));
            }

            //отправители и получатели
            var participantPhoneCalls = GetParticipantActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var participantCall in participantPhoneCalls.BusinessEntities)
            {

                var localPhonecall = (phonecall)participantCall;
                var senders = localPhonecall.from.ToList();
                var oldIndex = senders.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    senders[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localPhonecall.from = senders.ToArray();

                var recipients = localPhonecall.to.ToList();
                oldIndex = recipients.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    recipients[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localPhonecall.to = recipients.ToArray();

                crmDataContext.UsingService(s => s.Update(localPhonecall));
            }

            if (crmObjectType == EntityName.systemuser)
            {
                var ownedActivities = GetOwnedActivitiesFor(crmDataContext, fromObjectId, ActivityType);
                foreach (phonecall phonecall in ownedActivities.BusinessEntities)
                {
                    var assignee = new SecurityPrincipal
                    {
                        PrincipalId = toObjectId
                    };
                    var target = new TargetOwnedDynamic
                    {
                        EntityId = phonecall.activityid.Value,
                        EntityName = ActivityType.ToString()
                    };
                    
                    crmDataContext.UsingService(service => service.Execute(new AssignRequest
                    {
                        Assignee = assignee,
                        Target = target
                    }));
                }
            }

        }

        private void UpdateEmails(CrmDataContext crmDataContext, EntityName crmObjectType, Guid fromObjectId, Guid toObjectId)
        {
            const EntityName ActivityType = EntityName.email;
            var regardingEmails = GetRegardingActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var email in regardingEmails.BusinessEntities)
            {
                var localEmail = (email)email;
                localEmail.regardingobjectid = new Lookup
                {
                    Value = toObjectId,
                    type = crmObjectType.ToString()
                };
                
                crmDataContext.UsingService(s => s.Update(localEmail));
            }

            //отправители и получатели
            var participantEmails = GetParticipantActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var participantEmail in participantEmails.BusinessEntities)
            {

                var localEmail = (email)participantEmail;
                var senders = localEmail.from.ToList();
                var oldIndex = senders.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    senders[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localEmail.from = senders.ToArray();

                var recipients = localEmail.to.ToList();
                oldIndex = recipients.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    recipients[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localEmail.to = recipients.ToArray();

                var cc = localEmail.cc.ToList();
                oldIndex = cc.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    cc[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localEmail.cc = cc.ToArray();

                var bcc = localEmail.bcc.ToList();
                oldIndex = bcc.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    bcc[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localEmail.bcc = bcc.ToArray();

                crmDataContext.UsingService(s => s.Update(localEmail));
            }
            if (crmObjectType == EntityName.systemuser)
            {
                var ownedActivities = GetOwnedActivitiesFor(crmDataContext, fromObjectId, ActivityType);
                foreach (email email in ownedActivities.BusinessEntities)
                {
                    var assignee = new SecurityPrincipal
                    {
                        PrincipalId = toObjectId
                    };
                    var target = new TargetOwnedDynamic
                    {
                        EntityId = email.activityid.Value,
                        EntityName = ActivityType.ToString()
                    };
                    
                    crmDataContext.UsingService(service => service.Execute(new AssignRequest
                    {
                        Assignee = assignee,
                        Target = target
                    }));
                }
            }

        }

        private void UpdateLetters(CrmDataContext crmDataContext, EntityName crmObjectType, Guid fromObjectId, Guid toObjectId)
        {
            const EntityName ActivityType = EntityName.letter;
            var regardingLetters = GetRegardingActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var letter in regardingLetters.BusinessEntities)
            {
                var localLetter = (letter)letter;
                localLetter.regardingobjectid = new Lookup
                {
                    Value = toObjectId,
                    type = crmObjectType.ToString()
                };

                crmDataContext.UsingService(s => s.Update(localLetter));
            }
            //отправители и получатели
            var participantLetters = GetParticipantActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var participantLetter in participantLetters.BusinessEntities)
            {

                var localLetter = (letter)participantLetter;
                var senders = localLetter.from.ToList();
                var oldIndex = senders.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    senders[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localLetter.from = senders.ToArray();

                var recipients = localLetter.to.ToList();
                oldIndex = recipients.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    recipients[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localLetter.to = recipients.ToArray();

                crmDataContext.UsingService(s => s.Update(localLetter));
            }

            if (crmObjectType == EntityName.systemuser)
            {
                var ownedActivities = GetOwnedActivitiesFor(crmDataContext, fromObjectId, ActivityType);
                foreach (letter letter in ownedActivities.BusinessEntities)
                {
                    var assignee = new SecurityPrincipal
                    {
                        PrincipalId = toObjectId
                    };
                    var target = new TargetOwnedDynamic
                    {
                        EntityId = letter.activityid.Value,
                        EntityName = ActivityType.ToString()
                    };
                    
                    crmDataContext.UsingService(service => service.Execute(new AssignRequest
                    {
                        Assignee = assignee,
                        Target = target
                    }));
                }
            }

        }

        private void UpdateAppointments(CrmDataContext crmDataContext, EntityName crmObjectType, Guid fromObjectId, Guid toObjectId)
        {
            const EntityName ActivityType = EntityName.appointment;
            var regardingAppointments = GetRegardingActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var appointment in regardingAppointments.BusinessEntities)
            {
                var localAppointment = (appointment)appointment;
                localAppointment.regardingobjectid = new Lookup
                {
                    Value = toObjectId,
                    type = crmObjectType.ToString()
                };
                
                crmDataContext.UsingService(s => s.Update(localAppointment));
            }

            //отправители и получатели
            var participantAppointments = GetParticipantActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var participantAppointment in participantAppointments.BusinessEntities)
            {

                var localAppointment = (appointment)participantAppointment;
                var senders = localAppointment.requiredattendees.ToList();
                var oldIndex = senders.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    senders[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localAppointment.requiredattendees = senders.ToArray();

                var recipients = localAppointment.optionalattendees.ToList();
                oldIndex = recipients.FindIndex(ap => ap.partyid.Value == fromObjectId && ap.partyid.type == crmObjectType.ToString());
                if (oldIndex != -1)
                {
                    recipients[oldIndex] =
                        new activityparty
                        {
                            partyid = new Lookup(crmObjectType.ToString(), toObjectId)
                        };
                }
                localAppointment.optionalattendees = recipients.ToArray();

                crmDataContext.UsingService(s => s.Update(localAppointment));
            }

            if (crmObjectType == EntityName.systemuser)
            {
                var ownedActivities = GetOwnedActivitiesFor(crmDataContext, fromObjectId, ActivityType);
                foreach (appointment appointment in ownedActivities.BusinessEntities)
                {
                    var assignee = new SecurityPrincipal
                    {
                        PrincipalId = toObjectId
                    };
                    var target = new TargetOwnedDynamic
                    {
                        EntityId = appointment.activityid.Value,
                        EntityName = ActivityType.ToString()
                    };

                    crmDataContext.UsingService(service => service.Execute(new AssignRequest
                    {
                        Assignee = assignee,
                        Target = target
                    }));
                }
            }

        }

        private void UpdateServiceAppointments(CrmDataContext crmDataContext, EntityName crmObjectType, Guid fromObjectId, Guid toObjectId)
        {
            const EntityName ActivityType = EntityName.serviceappointment;
            var regardingServiceAppointments = GetRegardingActivitiesFor(crmDataContext, fromObjectId, ActivityType);
            foreach (var serviceAppointment in regardingServiceAppointments.BusinessEntities)
            {
                var localServiceAppointment = (serviceappointment)serviceAppointment;
                localServiceAppointment.regardingobjectid = new Lookup
                {
                    Value = toObjectId,
                    type = crmObjectType.ToString()
                };
                
                crmDataContext.UsingService(s => s.Update(localServiceAppointment));
            }

            if (crmObjectType == EntityName.systemuser)
            {
                var ownedActivities = GetOwnedActivitiesFor(crmDataContext, fromObjectId, ActivityType);
                foreach (serviceappointment serviceappointment in ownedActivities.BusinessEntities)
                {
                    var assignee = new SecurityPrincipal
                    {
                        PrincipalId = toObjectId
                    };
                    var target = new TargetOwnedDynamic
                    {
                        EntityId = serviceappointment.activityid.Value,
                        EntityName = ActivityType.ToString()
                    };
                    
                    crmDataContext.UsingService(service => service.Execute(new AssignRequest
                    {
                        Assignee = assignee,
                        Target = target
                    }));
                }
            }

        }

        private BusinessEntityCollection GetRegardingActivitiesFor(CrmDataContext crmDataContext, Guid regardingObjectId, EntityName activityType)
        {
            var filter = new FilterExpression
            {
                FilterOperator = LogicalOperator.And
            };
            filter.AddCondition(new ConditionExpression
            {
                AttributeName = "regardingobjectid",
                Operator = ConditionOperator.Equal,
                Values = new object[] { regardingObjectId.ToString() }
            });

            //filter.AddCondition(new ConditionExpression
            //{
            //    AttributeName = "regardingobjecttypecode",
            //    Operator = ConditionOperator.Equal,
            //    Values = new object[] { (int)_crmObjectType }
            //});
            filter.AddCondition(new ConditionExpression
            {
                AttributeName = "statecode",
                Operator = ConditionOperator.Equal,
                Values = new object[] { "Open" }
            });

            var query = new QueryExpression
            {
                Criteria = filter,
                EntityName = activityType.ToString(),
                ColumnSet = new AllColumns()
            };
            var retrieve = new RetrieveMultipleRequest
            {
                Query = query
            };

            return ((RetrieveMultipleResponse)crmDataContext.UsingService(s => s.Execute(retrieve))).BusinessEntityCollection;
        }

        private BusinessEntityCollection GetParticipantActivitiesFor(CrmDataContext crmDataContext, Guid participantId, EntityName activityType)
        {
            var linkedFilter = new FilterExpression
            {
                FilterOperator = LogicalOperator.And
            };
            linkedFilter.AddCondition(new ConditionExpression
            {
                AttributeName = "partyid",
                Operator = ConditionOperator.Equal,
                Values = new object[] { participantId.ToString() }
            });
            //linkedFilter.AddCondition(new ConditionExpression
            //{
            //    AttributeName = "partyobjecttypecode",
            //    Operator = ConditionOperator.Equal,
            //    Values = new object[] { (int)_crmObjectType }
            //});


            var filter = new FilterExpression
            {
                FilterOperator = LogicalOperator.And
            };
            
            
            filter.AddCondition(new ConditionExpression
            {
                AttributeName = "statecode",
                Operator = ConditionOperator.Equal,
                Values = new object[] { "Open" }
            });

            var link = new LinkEntity
            {
                LinkCriteria = linkedFilter,
                LinkFromEntityName = activityType.ToString(),
                LinkFromAttributeName = "activityid",
                LinkToEntityName = EntityName.activityparty.ToString(),
                LinkToAttributeName = "activityid"
            };

            var query = new QueryExpression
            {
                Criteria = filter,
                EntityName = activityType.ToString(),
                ColumnSet = new AllColumns()
            };
            query.LinkEntities.Add(link);
            var retrieve = new RetrieveMultipleRequest
            {
                Query = query
            };

            return ((RetrieveMultipleResponse)crmDataContext.UsingService(s => s.Execute(retrieve))).BusinessEntityCollection;
        }

        private BusinessEntityCollection GetOwnedActivitiesFor(CrmDataContext crmDataContext, Guid ownerId, EntityName activityType)
        {
            var filter = new FilterExpression
            {
                FilterOperator = LogicalOperator.And
            };
            filter.AddCondition(new ConditionExpression
            {
                AttributeName = "owninguser",
                Operator = ConditionOperator.Equal,
                Values = new object[] { ownerId.ToString() }
            });
            filter.AddCondition(new ConditionExpression
            {
                AttributeName = "statecode",
                Operator = ConditionOperator.Equal,
                Values = new object[] { "Open" }
            });

            var query = new QueryExpression
            {
                Criteria = filter,
                EntityName = activityType.ToString(),
                ColumnSet = new AllColumns()
            };
            var retrieve = new RetrieveMultipleRequest
            {
                Query = query
            };

            return ((RetrieveMultipleResponse)crmDataContext.UsingService(s => s.Execute(retrieve))).BusinessEntityCollection;
        }
    }
}
