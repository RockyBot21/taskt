﻿using System;
using System.Xml.Serialization;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("Excel Commands")]
    [Attributes.ClassAttributes.SubGruop("Other")]
    [Attributes.ClassAttributes.Description("This command runs a macro.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to get a run a specific macro in the Excel workbook.")]
    [Attributes.ClassAttributes.ImplementationDescription("This command implements Excel Interop to achieve automation.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class ExcelRunMacroCommand : ScriptCommand
    {
        [XmlAttribute]
        //[PropertyDescription("Please Enter the instance name")]
        //[InputSpecification("Enter the unique instance name that was specified in the **Create Excel** command")]
        //[SampleUsage("**myInstance** or **{{{vInstance}}}**")]
        //[Remarks("Failure to enter the correct instance name or failure to first call **Create Excel** command will cause an error")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyInstanceType(PropertyInstanceType.InstanceType.Excel)]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyValidationRule("Instance", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyDisplayText(true, "Instance")]
        //[PropertyFirstValue("%kwd_default_excel_instance%")]
        [PropertyVirtualProperty(nameof(ExcelControls), nameof(ExcelControls.v_InputInstanceName))]
        public string v_InstanceName { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(GeneralPropertyControls), nameof(GeneralPropertyControls.v_DisallowNewLine_OneLineTextBox))]
        [PropertyDescription("Macro Name")]
        [InputSpecification("Macro Name", true)]
        //[SampleUsage("**Macro1** or **Module1.Macro1** or **{{{vMacro}}}**")]
        [PropertyDetailSampleUsage("**Macro1**", PropertyDetailSampleUsage.ValueType.Value, "Macro")]
        [PropertyDetailSampleUsage("**Module1.Macro1**", PropertyDetailSampleUsage.ValueType.Value, "Macro")]
        [PropertyDetailSampleUsage("**{{{vMacro}}}**", PropertyDetailSampleUsage.ValueType.VariableValue, "Macro")]
        [PropertyValidationRule("Macro", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "Macro")]
        public string v_MacroName { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please Enter the macro argument1")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("Enter the value of the macro argument")]
        [SampleUsage("**1** or **Hello** or **{{{vArgument}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyIsOptional(true)]
        public string v_Argument1 { get; set; }

        public ExcelRunMacroCommand()
        {
            this.CommandName = "ExcelRunMacroCommand";
            this.SelectionName = "Run Macro";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }
        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var excelInstance = v_InstanceName.GetExcelInstance(engine);

            var vMacroName = v_MacroName.ConvertToUserVariable(engine);

            var vArg1 = v_Argument1.ConvertToUserVariable(engine);

            if (String.IsNullOrEmpty(vArg1))
            {
                excelInstance.Run(vMacroName);
            }
            else
            {
                excelInstance.Run(vMacroName, vArg1);
            }
        }
    }
}