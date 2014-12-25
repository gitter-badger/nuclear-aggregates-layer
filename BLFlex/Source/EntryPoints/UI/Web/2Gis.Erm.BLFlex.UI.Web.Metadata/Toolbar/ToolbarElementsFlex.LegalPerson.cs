﻿using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar
{
    public sealed partial class ToolbarElementsFlex
    {
        public static class LegalPersons
        {
            public static UIElementMetadataBuilder ChangeClient()
            {
                return

                    // COMMENT {all, 29.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("ChangeLegalPersonClient")
                                     .Title.Resource(() => ErmConfigLocalization.ControlChangeLegalPersonClient)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .Handler.Name("scope.ChangeLegalPersonClient")
                                     .Operation.SpecificFor<ChangeClientIdentity, LegalPerson>();
            }

            public static UIElementMetadataBuilder ChangeRequisites()
            {
                return

                    // COMMENT {all, 29.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("ChangeLPRequisites")
                                     .Title.Resource(() => ErmConfigLocalization.ControlChangeLPRequisites)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .LockOnNew()
                                     .Handler.Name("scope.ChangeLegalPersonRequisites")
                                     .Operation.NonCoupled<ChangeRequisitesIdentity>();
            }

            public static class Russia
            {
                public static UIElementMetadataBuilder Merge()
                {
                    return
                        UIElementMetadata.Config
                                         .Name.Static("Merge")
                                         .Icon.Path("Merge.gif")
                                         .Title.Resource(() => ErmConfigLocalization.ControlMerge)
                                         .ControlType(ControlType.ImageButton)
                                         .LockOnInactive()
                                         .LockOnNew()
                                         .Handler.Name("scope.Merge")
                                         .AccessWithPrivelege(FunctionalPrivilegeName.MergeLegalPersons)
                                         .Operation
                                         .SpecificFor<MergeIdentity, LegalPerson>();
                }
            }
        }
    }
}
