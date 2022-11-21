﻿using System;
using System.Xml.Serialization;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("File Operation Commands")]
    [Attributes.ClassAttributes.Description("This command returns a existence of file paths from a specified location")]
    [Attributes.ClassAttributes.UsesDescription("Use this command to return a existence of file paths from a specific location.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class CheckFileExistsCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Specify the path of the file you want to check for existence")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowFileSelectionHelper)]
        [InputSpecification("Enter or Select the path to the file.")]
        [SampleUsage("**C:\\temp\\myfile.txt** or **{{{vFilePath}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyTextBoxSetting(1, false)]
        [PropertyValidationRule("File Path", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "File")]
        public string v_TargetFileName { get; set; }

        [XmlAttribute]
        [PropertyDescription("Specify the variable to assign the result")]
        [InputSpecification("")]
        [SampleUsage("**vSomeVariable**")]
        [Remarks("Result is **TRUE** or **FALSE**")]
        [PropertyParameterDirection(PropertyParameterDirection.ParameterDirection.Output)]
        [PropertyInstanceType(PropertyInstanceType.InstanceType.Boolean, true)]
        [PropertyValidationRule("Result", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "Store")]
        public string v_UserVariableName { get; set; }

        public CheckFileExistsCommand()
        {
            this.CommandName = "CheckFileExistsCommand";
            this.SelectionName = "Check File Exists";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var targetFile = v_TargetFileName.ConvertToUserVariable(sender);

            System.IO.File.Exists(targetFile).StoreInUserVariable(engine, v_UserVariableName);
        }
    }
}