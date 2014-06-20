using System;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Shared;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLFlex.API.Operations.Global.Emirates.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Concrete.Integration.Import.FlowCards.Deserializers;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.ApplicationServices.Operations.Global.Emirates.Concrete.Integration.Import.FlowCards.Deserializers
{
    public class EmiratesDeserializeCardServiceSpecs
    {
        static XElement GetValidCardXElement()
        {
            return XElement.Parse(Properties.Resources.valid_flowCards_Card);
        }

        abstract class BaseDeserializeCardContext
        {
            protected static EmiratesDeserializeCardService service;
            protected static XElement xmlToParse;
            protected static IServiceBusDto result;

            Because of = () =>
                {
                    result = service.Deserialize(xmlToParse);
                };

            Establish context = () =>
                {
                    service = new EmiratesDeserializeCardService();
                    xmlToParse = GetValidCardXElement();
                };
        }

        abstract class BaseValidateCardContext
        {
            protected static EmiratesDeserializeCardService service;
            protected static XElement xmlToParse;
            protected static bool result;
            protected static string errorsResult;

            Establish context = () =>
                {
                    service = new EmiratesDeserializeCardService();
                    xmlToParse = GetValidCardXElement();
                };

            Because of = () =>
            {
                result = service.Validate(xmlToParse, out errorsResult);
            };
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card : BaseDeserializeCardContext
        {
            const long cardCode = 8822011445897;
            const long firmCode = 1111111111111;
            const int branchCode = 222222222;

            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Pos";

                    xmlToParse.Attribute("Code").Value = cardCode.ToString();
                    xmlToParse.Attribute("FirmCode").Value = firmCode.ToString();
                    xmlToParse.Attribute("BranchCode").Value = branchCode.ToString();
                };

            It should_return_proper_type = () => result.Should().BeOfType<EmiratesPosCardServiceBusDto>();
            It should_have_proper_code = () => ((EmiratesPosCardServiceBusDto)result).Code.Should().Be(cardCode);
            It should_have_proper_firmCode = () => ((EmiratesPosCardServiceBusDto)result).FirmCode.Should().Be(firmCode);
            It should_have_proper_branchCode = () => ((EmiratesPosCardServiceBusDto)result).BranchCode.Should().Be(branchCode);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_not_local_pos_card : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    xmlToParse.Attribute("IsLocal").Value = "False";
                };

            It should_not_be_linked_to_the_map = () => ((EmiratesPosCardServiceBusDto)result).IsLinkedToTheMap.Should().BeFalse();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_without_building_code : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Pos";

                    var addressElement = xmlToParse.Element("Address");

                    // BuildinCode не задаем
                    var newAddressElement = new XElement("Address",
                                                         new XAttribute("AddressCode", 123));

                    if (addressElement != null)
                    {
                        addressElement.ReplaceWith(newAddressElement);
                    }
                    else
                    {
                        xmlToParse.Add(newAddressElement);
                    }
                };

            It should_not_be_linked_to_the_map = () => ((EmiratesPosCardServiceBusDto)result).IsLinkedToTheMap.Should().BeFalse();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_local_pos_card_with_building_code : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Pos";
                    xmlToParse.Attribute("IsLocal").Value = "True";

                    var addressElement = xmlToParse.Element("Address");

                    //BuildinCode не задаем
                    var newAddressElement = new XElement("Address",
                                                         new XAttribute("AddressCode", 123),
                                                         new XAttribute("BuildingCode", 456));

                    if (addressElement != null)
                    {
                        addressElement.ReplaceWith(newAddressElement);
                    }
                    else
                    {
                        xmlToParse.Add(newAddressElement);
                    }
                };

            It should_not_be_linked_to_the_map = () => ((EmiratesPosCardServiceBusDto)result).IsLinkedToTheMap.Should().BeTrue();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_building_code_and_without_isLocal_attribute : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";
                var isLocalAttribute = xmlToParse.Attribute("IsLocal");
                if (isLocalAttribute != null)
                {
                    isLocalAttribute.Remove();
                }

                var addressElement = xmlToParse.Element("Address");

                // BuildinCode не задаем
                var newAddressElement = new XElement("Address",
                                                     new XAttribute("AddressCode", 123),
                                                     new XAttribute("BuildingCode", 456));

                if (addressElement != null)
                {
                    addressElement.ReplaceWith(newAddressElement);
                }
                else
                {
                    xmlToParse.Add(newAddressElement);
                }
            };

            It should_not_be_linked_to_the_map = () => ((EmiratesPosCardServiceBusDto)result).IsLinkedToTheMap.Should().BeTrue();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_contacts : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    var contactsElement = xmlToParse.Element("Contacts");

                    var newContactsElement = new XElement("Contacts",
                                                          new XElement("Phone",
                                                                       new XAttribute("ZoneCode", 48),
                                                                       new XAttribute("CanReceiveFax", true),
                                                                       new XAttribute("Value", 555),
                                                                       new XAttribute("FormatCode", 1500)));

                    if (contactsElement != null)
                    {
                        contactsElement.ReplaceWith(newContactsElement);
                    }
                    else
                    {
                        xmlToParse.Add(newContactsElement);
                    }
                };

            It should_have_contacts = () => ((EmiratesPosCardServiceBusDto)result).Contacts.Should().NotBeEmpty();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_schedules : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    var schedulesElement = xmlToParse.Element("Schedules");

                    var newSchedulesElement = new XElement("Schedules",
                                                           new XElement("Schedule",
                                                                        new XAttribute("Name", "График приема юр.лиц"),
                                                                        new XElement("Day",
                                                                                     new XAttribute("Label", "Mon"),
                                                                                     new XAttribute("From", "08:00:00"),
                                                                                     new XAttribute("To", "19:00:00"),
                                                                                     new XElement("Break",
                                                                                                  new XAttribute("From", "08:00:00"),
                                                                                                  new XAttribute("To", "19:00:00"))),
                                                                        new XElement("Day",
                                                                                     new XAttribute("Label", "Tue"),
                                                                                     new XAttribute("From", "08:00:00"),
                                                                                     new XAttribute("To", "19:00:00")),
                                                                        new XElement("Day",
                                                                                     new XAttribute("Label", "Wed"),
                                                                                     new XAttribute("From", "08:00:00"),
                                                                                     new XAttribute("To", "19:00:00")),
                                                                        new XElement("Day",
                                                                                     new XAttribute("Label", "Thu"),
                                                                                     new XAttribute("From", "08:00:00"),
                                                                                     new XAttribute("To", "19:00:00")),
                                                                        new XElement("Day",
                                                                                     new XAttribute("Label", "Fri"),
                                                                                     new XAttribute("From", "08:00:00"),
                                                                                     new XAttribute("To", "19:00:00")),
                                                                        new XElement("Day",
                                                                                     new XAttribute("Label", "Sat")),
                                                                        new XElement("Day",
                                                                                     new XAttribute("Label", "Sun"))));

                    if (schedulesElement != null)
                    {
                        schedulesElement.ReplaceWith(newSchedulesElement);
                    }
                    else
                    {
                        xmlToParse.Add(newSchedulesElement);
                    }
                };

            It should_have_schedule = () => ((EmiratesPosCardServiceBusDto)result).Schedule.Should().NotBeNull();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_without_schedules : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    var schedulesElement = xmlToParse.Element("Schedules");

                    if (schedulesElement != null)
                    {
                        schedulesElement.Remove();
                    }
                };

            It should_have_schedule = () => ((EmiratesPosCardServiceBusDto)result).Schedule.Should().BeNull();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_rubrics : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Pos";

                    var rubricsElement = xmlToParse.Element("Rubrics");

                    var newRubricsElement = new XElement("Rubrics",
                                                         new XElement("Rubric",
                                                                      new XAttribute("Code", 48),
                                                                      new XAttribute("IsPrimary", true)));

                    if (rubricsElement != null)
                    {
                        rubricsElement.ReplaceWith(newRubricsElement);
                    }
                    else
                    {
                        xmlToParse.Add(newRubricsElement);
                    }
                };

            It should_have_rubrics = () => ((EmiratesPosCardServiceBusDto)result).Rubrics.Should().NotBeEmpty();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_address : BaseDeserializeCardContext
        {
            const long addressCode = 333333333333;
            const long buildingCode = 444444444444;
            const string ReferencePoint = "TestReferencePoint";
            const string Address = "TestAddress";

            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";

                var addressElement = xmlToParse.Element("Address");
               
                var newAddressElement = new XElement("Address",
                                                     new XAttribute("AddressCode", addressCode),
                                                     new XAttribute("BuildingCode", buildingCode),
                                                     new XAttribute("ReferencePoint", ReferencePoint),
                                                     new XAttribute("Text", Address));

                if (addressElement != null)
                {
                    addressElement.ReplaceWith(newAddressElement);
                }
                else
                {
                    xmlToParse.Add(newAddressElement);
                }
            };

            It should_have_proper_addressCode = () => ((EmiratesPosCardServiceBusDto)result).AddressCode.Should().Be(addressCode);
            It should_have_proper_buildingCode = () => ((EmiratesPosCardServiceBusDto)result).BuildingCode.Should().Be(buildingCode);
            It should_have_proper_address = () => ((EmiratesPosCardServiceBusDto)result).Address.Should().Be(Address);
            It should_have_proper_referencePoint = () => ((EmiratesPosCardServiceBusDto)result).ReferencePoint.Should().Be(ReferencePoint);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_without_addressCode_or_buildingCode : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";

                var addressElement = xmlToParse.Element("Address");

                // BuildingCode и AddressCode не задаем
                var newAddressElement = new XElement("Address");

                if (addressElement != null)
                {
                    addressElement.ReplaceWith(newAddressElement);
                }
                else
                {
                    xmlToParse.Add(newAddressElement);
                }
            };

            It should_have_empty_addressCode = () => ((EmiratesPosCardServiceBusDto)result).AddressCode.Should().Be(null);
            It should_have_empty_buildingCode = () => ((EmiratesPosCardServiceBusDto)result).BuildingCode.Should().Be(null);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_without_referencePoint : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";

                var addressElement = xmlToParse.Element("Address");

                // ReferencePoint и PromotionalReferencePoint не задаем
                var newAddressElement = new XElement("Address");

                if (addressElement != null)
                {
                    addressElement.ReplaceWith(newAddressElement);
                }
                else
                {
                    xmlToParse.Add(newAddressElement);
                }
            };

            It should_have_empty_referencePoint = () => ((EmiratesPosCardServiceBusDto)result).ReferencePoint.Should().Be(null);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_referencePoint_and_promotionalReferencePoint : BaseDeserializeCardContext
        {
            const string ReferencePoint = "TestReferencePoint";
            const string PromotionalReferencePoint = "TestPromotionalReferencePoint";

            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Pos";

                    var addressElement = xmlToParse.Element("Address");

                    var newAddressElement = new XElement("Address",
                                                         new XAttribute("ReferencePoint", ReferencePoint),
                                                         new XAttribute("PromotionalReferencePoint", PromotionalReferencePoint));

                    if (addressElement != null)
                    {
                        addressElement.ReplaceWith(newAddressElement);
                    }
                    else
                    {
                        xmlToParse.Add(newAddressElement);
                    }
                };

            It should_have_promotionalReferencePoint =
                () => ((EmiratesPosCardServiceBusDto)result).ReferencePoint.Should().Be(PromotionalReferencePoint);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_payment_methods : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";

                    var fieldsElement = xmlToParse.Element("Fields");

                    var newFieldsElement = new XElement("Fields",
                                                        new XElement("ReferenceListField",
                                                                     new XAttribute("Code", "PaymentMethod"),
                                                                     new XElement("Items",
                                                                                  new XElement("Item", new XAttribute("Code", 7)))));

                    if (fieldsElement != null)
                    {
                        fieldsElement.ReplaceWith(newFieldsElement);
                    }
                    else
                    {
                        xmlToParse.Add(newFieldsElement);
                    }
            };

            It should_have_paymentsMethod = () => ((EmiratesPosCardServiceBusDto)result).PaymentMethodCodes.Should().NotBeEmpty();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_hidden_payment_methods : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";

                var fieldsElement = xmlToParse.Element("Fields");

                var newFieldsElement = new XElement("Fields",
                                                    new XElement("ReferenceListField",
                                                                 new XAttribute("Code", "PaymentMethod"),
                                                                 new XAttribute("IsHidden", "true"),
                                                                 new XElement("Items",
                                                                              new XElement("Item", new XAttribute("Code", 7)))));

                if (fieldsElement != null)
                {
                    fieldsElement.ReplaceWith(newFieldsElement);
                }
                else
                {
                    xmlToParse.Add(newFieldsElement);
                }
            };

            It should_not_have_paymentsMethod = () => ((EmiratesPosCardServiceBusDto)result).PaymentMethodCodes.Should().BeEmpty();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_pobox : BaseDeserializeCardContext
        {
            const string poboxValue = "26195";

            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Pos";

                    var fieldsElement = xmlToParse.Element("Fields");

                    var newFieldsElement = new XElement("Fields",
                                                        new XElement("TextField",
                                                                     new XAttribute("Code", "PO Box"),
                                                                     new XAttribute("Value", poboxValue)));

                    if (fieldsElement != null)
                    {
                        fieldsElement.ReplaceWith(newFieldsElement);
                    }
                    else
                    {
                        xmlToParse.Add(newFieldsElement);
                    }
                };

            It should_have_proper_pobox_value = () => ((EmiratesPosCardServiceBusDto)result).PoBox.Should().Be(poboxValue);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_pos_card_with_hidden_pobox : BaseDeserializeCardContext
        {
            const string poboxValue = "26195";

            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Pos";

                    var fieldsElement = xmlToParse.Element("Fields");

                    var newFieldsElement = new XElement("Fields",
                                                        new XElement("TextField",
                                                                     new XAttribute("Code", "PO Box"),
                                                                     new XAttribute("IsHidden", "true"),
                                                                     new XAttribute("Value", poboxValue)));

                    if (fieldsElement != null)
                    {
                        fieldsElement.ReplaceWith(newFieldsElement);
                    }
                    else
                    {
                        xmlToParse.Add(newFieldsElement);
                    }
                };

            It should_not_have_pobox_value = () => ((EmiratesPosCardServiceBusDto)result).PoBox.Should().BeBlank();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_dep_card : BaseDeserializeCardContext
        {
            const long cardCode = 1122040605;

            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Dep";
                    xmlToParse.Attribute("Code").Value = cardCode.ToString();
                    var contactsElement = xmlToParse.Element("Contacts");

                    var newContactsElement = new XElement("Contacts",
                                                          new XElement("Phone",
                                                                       new XAttribute("ZoneCode", 48),
                                                                       new XAttribute("CanReceiveFax", true),
                                                                       new XAttribute("Value", 555),
                                                                       new XAttribute("FormatCode", 1500)));
                    if (contactsElement != null)
                    {
                        contactsElement.ReplaceWith(newContactsElement);
                    }
                    else
                    {
                        xmlToParse.Add(newContactsElement);
                    }
                };

            It should_return_proper_type = () => result.Should().BeOfType<EmiratesDepCardServiceBusDto>();
            It should_have_proper_code = () => ((EmiratesDepCardServiceBusDto)result).Code.Should().Be(cardCode);
            It should_have_contacts = () => ((EmiratesDepCardServiceBusDto)result).Contacts.Should().HaveCount(1);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_phone_which_can_recieve_fax : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Dep";
                    var contactsElement = xmlToParse.Element("Contacts");

                    var newContactsElement = new XElement("Contacts",
                                                          new XElement("Phone",
                                                                       new XAttribute("ZoneCode", 48),
                                                                       new XAttribute("CanReceiveFax", true),
                                                                       new XAttribute("Value", 555),
                                                                       new XAttribute("FormatCode", 1500)));
                    if (contactsElement != null)
                    {
                        contactsElement.ReplaceWith(newContactsElement);
                    }
                    else
                    {
                        xmlToParse.Add(newContactsElement);
                    }
                };

            It should_become_fax = () =>
                                   ((EmiratesDepCardServiceBusDto)result).Contacts.Single().ContactType.Should().Be(ContactType.Fax);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_hidden_or_archived_or_notToPublish_contact : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Dep";
                    var contactsElement = xmlToParse.Element("Contacts");

                    var newContactsElement = new XElement("Contacts",
                                                          new XElement("Phone",
                                                                       new XAttribute("Value", 555),
                                                                       new XAttribute("IsHidden", true)),
                                                          new XElement("Phone",
                                                                       new XAttribute("Value", 556),
                                                                       new XAttribute("IsArchived", true)),
                                                          new XElement("Phone",
                                                                       new XAttribute("Value", 557),
                                                                       new XAttribute("NotPublish", true)));
                    if (contactsElement != null)
                    {
                        contactsElement.ReplaceWith(newContactsElement);
                    }
                    else
                    {
                        xmlToParse.Add(newContactsElement);
                    }
                };

            It should_not_be_loaded = () =>
                                      ((EmiratesDepCardServiceBusDto)result).Contacts.Should().BeEmpty();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_hidden_dep_card : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Dep";
                    xmlToParse.SetAttributeValue("IsHidden", true);
                };

            It should_be_IsHiddenOrArchived = () => ((EmiratesDepCardServiceBusDto)result).IsHiddenOrArchived.Should().BeTrue();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_archived_dep_card : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                    xmlToParse.Attribute("Type").Value = "Dep";
                    xmlToParse.SetAttributeValue("IsArchived", true);
            };

            It should_be_IsHiddenOrArchived = () => ((EmiratesDepCardServiceBusDto)result).IsHiddenOrArchived.Should().BeTrue();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_deleted_dep_card : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Dep";
                    xmlToParse.SetAttributeValue("IsDeleted", true);
                };

            It should_be_IsHiddenOrArchived = () => ((EmiratesDepCardServiceBusDto)result).IsHiddenOrArchived.Should().BeTrue();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_hidden_pos_card : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";
                xmlToParse.SetAttributeValue("IsHidden", true);
            };

            It should_be_ClosedForAscertainment = () => ((EmiratesPosCardServiceBusDto)result).ClosedForAscertainment.Should().BeTrue();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_archived_pos_card : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";
                xmlToParse.SetAttributeValue("IsArchived", true);
            };

            It should_be_inactive = () => ((EmiratesPosCardServiceBusDto)result).IsActive.Should().BeFalse();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_not_archived_pos_card : BaseDeserializeCardContext
        {
            Establish context = () =>
                {
                    xmlToParse.Attribute("Type").Value = "Pos";
                    xmlToParse.SetAttributeValue("IsArchived", false);
                };

            It should_be_active = () => ((EmiratesPosCardServiceBusDto)result).IsActive.Should().BeTrue();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_deleted_pos_card : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                xmlToParse.Attribute("Type").Value = "Pos";
                xmlToParse.SetAttributeValue("IsDeleted", true);
            };

            It should_be_deleted = () => ((EmiratesPosCardServiceBusDto)result).IsDeleted.Should().BeTrue();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_deserializes_active_dep_card : BaseDeserializeCardContext
        {
            Establish context = () =>
            {
                    xmlToParse.Attribute("Type").Value = "Dep";
                    xmlToParse.SetAttributeValue("IsDeleted", false);
                    xmlToParse.SetAttributeValue("IsArchived", false);
                    xmlToParse.SetAttributeValue("IsHidden", false);
            };

            It should_false_value_for_IsHiddenOrArchived = () => ((EmiratesDepCardServiceBusDto)result).IsHiddenOrArchived.Should().BeFalse();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_validate_card_without_required_elements : BaseValidateCardContext
        {
            static string[] requiredAttributes;
            static string[] errors;

            Establish context = () =>
                {
                    const string RequiredAttributeNotFound = "Отсутсвует обязательный атрибут '{0}'";
                    requiredAttributes = new[] { "Type", "Code" };

                    foreach (var requiredAttribute in requiredAttributes)
                    {
                        xmlToParse.Attribute(requiredAttribute).Remove();
                    }

                    errors = requiredAttributes.Select(x => string.Format(RequiredAttributeNotFound, x)).ToArray();
                };

            It should_return_false_as_validation_result = () => result.Should().BeFalse();

            It should_have_proper_errors_description =
                () => errorsResult
                          .Split(new[]
                              {
                                  Environment.NewLine
                              },
                                 StringSplitOptions.RemoveEmptyEntries)
                          .Should()
                          .BeEquivalentTo(errors);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_validate_dep_card_without_required_for_pos_card_elements : BaseValidateCardContext
        {
            static string[] requiredAttributes;

            Establish context = () =>
                {
                    requiredAttributes = new[] { "Name", "FirmCode" };

                    foreach (var requiredAttribute in requiredAttributes)
                    {
                        xmlToParse.Attribute(requiredAttribute).Remove();
                    }

                    xmlToParse.Attribute("Type").Value = "Dep";
                };

            It should_return_true_as_validation_result = () => result.Should().BeTrue();

            It should_empty_proper_errors_description =
                () => errorsResult
                          .Should()
                          .BeBlank();
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_validate_pos_card_without_required_for_pos_card_elements : BaseValidateCardContext
        {
            static string[] requiredAttributes;
            static string[] errors;

            Establish context = () =>
                {
                    requiredAttributes = new[] { "FirmCode", "BranchCode" };

                    const string RequiredAttributeNotFound = "Отсутсвует обязательный атрибут '{0}'";

                    foreach (var requiredAttribute in requiredAttributes)
                {
                        xmlToParse.Attribute(requiredAttribute).Remove();
                    }

                    errors = requiredAttributes.Select(x => string.Format(RequiredAttributeNotFound, x)).ToArray();

                    xmlToParse.Attribute("Type").Value = "Pos";
                };

            It should_return_false_as_validation_result = () => result.Should().BeFalse();

            It should_have_proper_errors_description =
                () => errorsResult
                          .Split(new[]
                              {
                                  Environment.NewLine
                              },
                                 StringSplitOptions.RemoveEmptyEntries)
                          .Should()
                          .BeEquivalentTo(errors);
        }

        [Tags("Deserializer")]
        [Subject(typeof(EmiratesDeserializeCardService))]
        class When_validate_card_with_unknown_type : BaseValidateCardContext
        {
            Establish context = () => xmlToParse.Attribute("Type").Value = "TestCardType";

            It should_return_false_as_validation_result = () => result.Should().BeFalse();

            It should_empty_proper_errors_description =
                () => errorsResult
                          .Should()
                          .Contain("Неизвестное значение атрибута 'Type'");
        }
    }
}
