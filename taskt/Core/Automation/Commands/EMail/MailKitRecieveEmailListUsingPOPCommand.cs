﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("EMail Commands")]
    [Attributes.ClassAttributes.SubGruop("")]
    [Attributes.ClassAttributes.Description("This command allows you to get EMailList(EMails) using POP protocol.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to get MailList(EMails) using POP protocol. Result Variable Type is EMailList.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class MailKitRecieveEmailListUsingPOPCommand : ScriptCommand
    {
        [XmlAttribute]
        //[PropertyDescription("Please specify POP Host Name")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[InputSpecification("Define the host/service name that the script should use")]
        //[SampleUsage("**pop.mymail.com** or **{{{vHost}}}**")]
        //[Remarks("")]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyValidationRule("POP Host", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyTextBoxSetting(1, false)]
        //[PropertyDisplayText(true, "Host")]
        [PropertyVirtualProperty(nameof(EMailControls), nameof(EMailControls.v_Host))]
        [PropertyDetailSampleUsage("**pop.example.com**", PropertyDetailSampleUsage.ValueType.Value, "Host")]
        public string v_POPHost { get; set; }

        [XmlAttribute]
        //[PropertyDescription("Please specify POP Port")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[InputSpecification("Define the port number that should be used when contacting the POP service")]
        //[SampleUsage("**110** or **995** or **{{{vPort}}}**")]
        //[Remarks("")]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyValidationRule("POP Port", PropertyValidationRule.ValidationRuleFlags.Empty | PropertyValidationRule.ValidationRuleFlags.LessThanZero)]
        //[PropertyTextBoxSetting(1, false)]
        //[PropertyDisplayText(true, "Port")]
        [PropertyVirtualProperty(nameof(EMailControls), nameof(EMailControls.v_Port))]
        [PropertyDetailSampleUsage("**110**", PropertyDetailSampleUsage.ValueType.Value, "Port")]
        [PropertyDetailSampleUsage("**995**", PropertyDetailSampleUsage.ValueType.Value, "Port")]
        [PropertyDetailSampleUsage("**{{{vPort}}}**", PropertyDetailSampleUsage.ValueType.VariableValue, "Port")]
        public string v_POPPort { get; set; }

        [XmlAttribute]
        //[PropertyDescription("Please specify POP Username")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[InputSpecification("Define the username to use when contacting the POP service")]
        //[SampleUsage("**username** or **{{{vUserName}}}**")]
        //[Remarks("")]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyTextBoxSetting(1, false)]
        //[PropertyDisplayText(true, "User")]
        [PropertyVirtualProperty(nameof(EMailControls), nameof(EMailControls.v_UserName))]
        public string v_POPUserName { get; set; }

        [XmlAttribute]
        //[PropertyDescription("Please specify POP Password")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[InputSpecification("Define the password to use when contacting the POP service")]
        //[SampleUsage("**password** or **{{{vPassword}}}**")]
        //[Remarks("")]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyValidationRule("Password", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyTextBoxSetting(1, false)]
        [PropertyVirtualProperty(nameof(EMailControls), nameof(EMailControls.v_Password))]
        public string v_POPPassword { get; set; }

        [XmlAttribute]
        //[PropertyDescription("Please specify Secure Option")]
        //[InputSpecification("")]
        //[SampleUsage("")]
        //[Remarks("")]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyFirstValue("Auto")]
        //[PropertyIsOptional(true, "Auto")]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyUISelectionOption("Auto")]
        //[PropertyUISelectionOption("No SSL or TLS")]
        //[PropertyUISelectionOption("Use SSL or TLS")]
        //[PropertyUISelectionOption("STARTTLS")]
        //[PropertyUISelectionOption("STARTTLS When Available")]
        [PropertyVirtualProperty(nameof(EMailControls), nameof(EMailControls.v_SecureOption))]
        public string v_POPSecureOption { get; set; }

        [XmlAttribute]
        //[PropertyDescription("Please specify Variable Name to Store EMailList")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[InputSpecification("")]
        //[SampleUsage("**vMailList** or **{{{vMailList}}}**")]
        //[Remarks("")]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyIsVariablesList(true)]
        //[PropertyValidationRule("EMailList", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyInstanceType(PropertyInstanceType.InstanceType.MailKitEMailList, true)]
        //[PropertyParameterDirection(PropertyParameterDirection.ParameterDirection.Output)]
        //[PropertyDisplayText(true, "Store")]
        [PropertyVirtualProperty(nameof(EMailControls), nameof(EMailControls.v_OutputMailListName))]
        public string v_MailListName { get; set; }

        public MailKitRecieveEmailListUsingPOPCommand()
        {
            this.CommandName = "MailKitRecieveEMailListUsingPOPCommand";
            this.SelectionName = "Recieve EMailList Using POP";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            // pop host
            string pop = v_POPHost.ConvertToUserVariable(engine);
            var port = this.ConvertToUserVariableAsInteger(nameof(v_POPPort), engine);

            // auth
            string user = v_POPUserName.ConvertToUserVariable(engine);
            string pass = v_POPPassword.ConvertToUserVariable(engine);
            //var secureOption = this.GetUISelectionValue(nameof(v_POPSecureOption), engine);

            using (var client = new MailKit.Net.Pop3.Pop3Client())
            {
                //var option = MailKit.Security.SecureSocketOptions.Auto;
                //switch (secureOption)
                //{
                //    case "no ssl or tls":
                //        option = MailKit.Security.SecureSocketOptions.None;
                //        break;
                //    case "use ssl or tls":
                //        option = MailKit.Security.SecureSocketOptions.SslOnConnect;
                //        break;
                //    case "starttls":
                //        option = MailKit.Security.SecureSocketOptions.StartTls;
                //        break;
                //    case "starttls when available":
                //        option = MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable;
                //        break;
                //}
                var option = this.GetMailKitSecureOption(nameof(v_POPSecureOption), engine);
                try
                {
                    lock (client.SyncRoot)
                    {
                        client.Connect(pop, port, option);
                        client.Authenticate(user, pass);

                        List<MimeKit.MimeMessage> messages = new List<MimeKit.MimeMessage>();

                        for (int i = 0; i < client.Count; i++)
                        {
                            var mes = client.GetMessage(i);
                            messages.Add(mes);
                        }

                        client.Disconnect(true);

                        messages.StoreInUserVariable(engine, v_MailListName);
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception("Fail Recieve EMail " + ex.ToString());
                }
            }
        }
    }
}
