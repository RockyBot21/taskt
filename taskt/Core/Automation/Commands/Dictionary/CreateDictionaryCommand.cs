﻿using System;
using System.Xml.Serialization;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("Dictionary Commands")]
    [Attributes.ClassAttributes.SubGruop("Dictionary Action")]
    [Attributes.ClassAttributes.Description("This command created a DataTable with the column names provided")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to create a new Dictionary")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class CreateDictionaryCommand : ScriptCommand
    {
        [XmlAttribute]
        //[PropertyDescription("Please Indicate Dictionary Variable Name")]
        //[InputSpecification("Indicate a unique reference name for later use")]
        //[SampleUsage("**vMyDictionary** or **{{{vMyDictionary}}}**")]
        //[Remarks("Create Dictionary<string, string>")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyIsVariablesList(true)]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyInstanceType(PropertyInstanceType.InstanceType.Dictionary)]
        //[PropertyParameterDirection(PropertyParameterDirection.ParameterDirection.Output)]
        //[PropertyValidationRule("Dictionary", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyDisplayText(true, "Dictionary")]
        [PropertyVirtualProperty(nameof(DictionaryControls), nameof(DictionaryControls.v_InputDictionaryName))]
        public string v_DictionaryName { get; set; }

        [XmlElement]
        //[PropertyDescription("Define Keys and Values")]
        //[InputSpecification("Enter the Keys and Values required for your dictionary")]
        //[SampleUsage("")]
        //[Remarks("")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.DataGridView)]
        //[PropertyDataGridViewColumnSettings("Keys", "Keys", false)]
        //[PropertyDataGridViewColumnSettings("Values", "Values", false)]
        //[PropertyDataGridViewSetting(true, true, true)]
        //[PropertyControlIntoCommandField("ColumnNameDataGridViewHelper")]
        //[PropertyDataGridViewCellEditEvent(nameof(DataTableControls)+"+"+nameof(DataTableControls.AllEditableDataGridView_CellClick), PropertyDataGridViewCellEditEvent.DataGridViewCellEvent.CellClick)]
        //[PropertyDisplayText(true, "Items")]
        [PropertyVirtualProperty(nameof(DictionaryControls), nameof(DictionaryControls.v_KeyAndValue))]
        public DataTable v_ColumnNameDataTable { get; set; }

        public CreateDictionaryCommand()
        {
            this.CommandName = "CreateDictionaryCommand";
            this.SelectionName = "Create Dictionary";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            Dictionary<string, string> outputDictionary = new Dictionary<string, string>();

            outputDictionary.AddDataAndValueFromDataTable(v_ColumnNameDataTable, engine);

            outputDictionary.StoreInUserVariable(engine, v_DictionaryName);
        }

        public override void BeforeValidate()
        {
            base.BeforeValidate();
            DataTableControls.BeforeValidate((DataGridView)ControlsList[nameof(v_ColumnNameDataTable)], v_ColumnNameDataTable);
        }
    }
}