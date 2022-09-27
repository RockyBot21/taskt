﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using taskt.UI.CustomControls;
using taskt.UI.Forms;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("EMail Commands")]
    [Attributes.ClassAttributes.SubGruop("")]
    [Attributes.ClassAttributes.Description("This command allows you to get Addresses from EMail.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to get Addresses from EMail.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class MailKitGetAddressesAsDictionaryCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Please specify EMail Variable Name")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("")]
        [SampleUsage("**{{{vEMail}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyValidationRule("EMail", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyInstanceType(PropertyInstanceType.InstanceType.MailKitEMail, true)]
        [PropertyParameterDirection(PropertyParameterDirection.ParameterDirection.Input)]
        [PropertyDisplayText(true, "EMal")]
        public string v_MailName { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please specify Address Type")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("")]
        [SampleUsage("")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyUISelectionOption("From")]
        [PropertyUISelectionOption("To")]
        [PropertyUISelectionOption("CC")]
        [PropertyUISelectionOption("BCC")]
        [PropertyUISelectionOption("Reply-To")]
        [PropertyUISelectionOption("Resent-From")]
        [PropertyUISelectionOption("Resent-To")]
        [PropertyUISelectionOption("Resent-CC")]
        [PropertyUISelectionOption("Resent-BCC")]
        [PropertyUISelectionOption("Resent-Reply-To")]
        [PropertyValidationRule("Address Type", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "Type")]
        public string v_AddressesType { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please specify Variable Name to Store Addresses")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("")]
        [SampleUsage("**vAddresses** or **{{{vAddresses}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsVariablesList(true)]
        [PropertyValidationRule("Addresses Variable", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyInstanceType(PropertyInstanceType.InstanceType.Dictionary, true)]
        [PropertyParameterDirection(PropertyParameterDirection.ParameterDirection.Output)]
        [PropertyDisplayText(true, "Store")]
        public string v_AddressesDictionary { get; set; }

        public MailKitGetAddressesAsDictionaryCommand()
        {
            this.CommandName = "MailKitGetAddressesAsDictionaryCommand";
            this.SelectionName = "Get Addresses As Dictionary";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var mail = v_MailName.GetMailKitMailVariable(engine);

            var addressType = v_AddressesType.GetUISelectionValue("v_AddressesType", this, engine);

            MimeKit.InternetAddressList lst = null;
            switch (addressType)
            {
                case "from":
                    lst = mail.From;
                    break;
                case "to":
                    lst = mail.To;
                    break;
                case "cc":
                    lst = mail.Cc;
                    break;
                case "bcc":
                    lst = mail.Bcc;
                    break;
                case "reply-to":
                    lst = mail.ReplyTo;
                    break;
                case "resent-from":
                    lst = mail.ResentFrom;
                    break;
                case "resent-to":
                    lst = mail.ResentTo;
                    break;
                case "resent-cc":
                    lst = mail.ResentCc;
                    break;
                case "resent-bcc":
                    lst = mail.ResentBcc;
                    break;
                case "resent-reply-to":
                    lst = mail.ResentReplyTo;
                    break;
            }

            Dictionary<string, string> addresses = new Dictionary<string, string>();
            foreach(MimeKit.MailboxAddress item in lst)
            {
                addresses.Add(item.Name, item.Address);
            }
            addresses.StoreInUserVariable(engine, v_AddressesDictionary);
        }

        //public override List<Control> Render(frmCommandEditor editor)
        //{
        //    base.Render(editor);

        //    var ctrls = CommandControls.MultiCreateInferenceDefaultControlGroupFor(this, editor);
        //    RenderedControls.AddRange(ctrls);

        //    return RenderedControls;
        //}

        //public override string GetDisplayValue()
        //{
        //    return base.GetDisplayValue() + " [EMail: '" + v_MailName + "', Type: '" + v_AddressesType + "', Store: '" + v_AddressesDictionary + "']"; ;
        //}
    }
}
