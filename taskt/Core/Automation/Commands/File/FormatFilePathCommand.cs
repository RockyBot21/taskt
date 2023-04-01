﻿using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using taskt.UI.CustomControls;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{

    [Serializable]
    [Attributes.ClassAttributes.Group("File Operation Commands")]
    [Attributes.ClassAttributes.CommandSettings("Format File Path")]
    [Attributes.ClassAttributes.Description("This command allows you to format file path.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to format file path.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class FormatFilePathCommnad : ScriptCommand
    {
        [XmlAttribute]
        [PropertyVirtualProperty(nameof(FilePathControls), nameof(FilePathControls.v_FilePath))]
        [PropertyFilePathSetting(false, PropertyFilePathSetting.ExtensionBehavior.AllowNoExtension, PropertyFilePathSetting.FileCounterBehavior.NoSupport)]
        public string v_SourceFilePath { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(GeneralPropertyControls), nameof(GeneralPropertyControls.v_ComboBox))]
        [PropertyDescription("File Path Format")]
        [InputSpecification("File Path Format", true)]
        [PropertyCustomUIHelper("Format Checker", nameof(lnkFormatChecker_Click))]
        [PropertyUISelectionOption("FileName")]
        [PropertyUISelectionOption("FileNameWithoutExtension")]
        [PropertyUISelectionOption("Folder")]
        [PropertyUISelectionOption("Extension")]
        [PropertyUISelectionOption("DriveName")]
        [PropertyValidationRule("Format", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "Format")]
        public string v_Format { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(GeneralPropertyControls), nameof(GeneralPropertyControls.v_Result))]
        public string v_Result { get; set; }

        public FormatFilePathCommnad()
        {
            //this.CommandName = "Format File PathCommand";
            //this.SelectionName = "Format File Path";
            //this.CommandEnabled = true;
            //this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var filePath = this.ConvertToUserVariableAsFilePath(nameof(v_SourceFilePath), engine);
            string format = v_Format.ConvertToUserVariable(engine);

            string result = FilePathControls.FormatFileFolderPath(filePath, format);
            result.StoreInUserVariable(engine, v_Result);
        }

        private void lnkFormatChecker_Click(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)((CommandItemControl)sender).Tag;
            UI.Forms.Supplement_Forms.frmFormatChecker.ShowFormatCheckerFormLinkClicked(cmb, "File Folder");
        }
    }
}