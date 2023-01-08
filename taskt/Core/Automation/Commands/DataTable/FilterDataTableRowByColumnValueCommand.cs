﻿using System;
using System.Xml.Serialization;
using System.Data;
using System.Windows.Forms;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("DataTable Commands")]
    [Attributes.ClassAttributes.SubGruop("DataTable Action")]
    [Attributes.ClassAttributes.Description("This command allows you to Filter Rows by reference to Column values.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to Filter Rows by reference to Column values.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class FilterDataTableRowByColumnValueCommand : ScriptCommand
    {
        [XmlAttribute]
        //[PropertyDescription("Please select a DataTable Variable Name to Filter")]
        //[InputSpecification("")]
        //[SampleUsage("**vTable** or **{{{vTable}}}**")]
        //[Remarks("")]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyInstanceType(PropertyInstanceType.InstanceType.DataTable)]
        //[PropertyValidationRule("DataTable to Filter", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyDisplayText(true, "DataTable to Filter")]
        [PropertyVirtualProperty(nameof(DataTableControls), nameof(DataTableControls.v_InputDataTableName))]
        [PropertyDescription("DataTable Variable Name to Filter")]
        [PropertyValidationRule("DataTable to Filter", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "DataTable to Filter")]
        public string v_InputDataTable { get; set; }

        [XmlAttribute]
        //[PropertyDescription("Please specify Column type")]
        //[InputSpecification("")]
        //[SampleUsage("**Column Name** or **Index**")]
        //[Remarks("")]
        //[PropertyUISelectionOption("Column Name")]
        //[PropertyUISelectionOption("Index")]
        //[PropertyIsOptional(true, "Column Name")]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyDisplayText(true, "Column Type")]
        [PropertyVirtualProperty(nameof(DataTableControls), nameof(DataTableControls.v_ColumnType))]
        public string v_ColumnType { get; set; }

        [XmlAttribute]
        //[PropertyDescription("Please enter the Name or Index of the Column")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[InputSpecification("Enter a valid Column index value")]
        //[SampleUsage("**id** or **0** or **{{{vColumn}}}** or **-1**")]
        //[Remarks("If **-1** is specified for Column Index, it means the last column.")]
        //[PropertyShowSampleUsageInDescription(true)]
        //[PropertyTextBoxSetting(1, false)]
        //[PropertyValidationRule("Column", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyDisplayText(true, "Column")]
        [PropertyVirtualProperty(nameof(DataTableControls), nameof(DataTableControls.v_ColumnNameIndex))]
        public string v_TargetColumnIndex { get; set; }

        [XmlAttribute]
        //[PropertyDescription("filter target value type")]
        //[InputSpecification("")]
        //[SampleUsage("**Text** or **Number**")]
        //[Remarks("")]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyUISelectionOption("Text")]
        //[PropertyUISelectionOption("Numeric")]
        //[PropertyValidationRule("Target Type", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyDisplayText(true, "Type")]
        [PropertyVirtualProperty(nameof(ConditionControls), nameof(ConditionControls.v_FilterValueType))]
        [PropertySelectionChangeEvent(nameof(cmbTargetType_SelectionChangeCommited))]
        public string v_TargetType { get; set; }

        [XmlAttribute]
        //[PropertyDescription("filter action")]
        //[InputSpecification("")]
        //[SampleUsage("")]
        //[Remarks("")]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyValidationRule("Filter Action", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyDisplayText(true, "Action")]
        [PropertyVirtualProperty(nameof(ConditionControls), nameof(ConditionControls.v_FilterAction))]
        [PropertySelectionChangeEvent(nameof(cmbFilterAction_SelectionChangeCommited))]
        public string v_FilterAction { get; set; }

        [XmlElement]
        //[PropertyDescription("Additional Parameters")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[InputSpecification("")]
        //[SampleUsage("")]
        //[Remarks("")]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.DataGridView)]
        //[PropertyDataGridViewSetting(false, false, true, 400, 120)]
        //[PropertyDataGridViewColumnSettings("ParameterName", "Parameter Name", true)]
        //[PropertyDataGridViewColumnSettings("ParameterValue", "Parameter Value", false)]
        //[PropertyDataGridViewCellEditEvent(nameof(DataTableControls) + "+" + nameof(DataTableControls.FirstColumnReadonlySubsequentEditableDataGridView_CellBeginEdit), PropertyDataGridViewCellEditEvent.DataGridViewCellEvent.CellBeginEdit)]
        //[PropertyDataGridViewCellEditEvent(nameof(DataTableControls) + "+" + nameof(DataTableControls.FirstColumnReadonlySubsequentEditableDataGridView_CellClick), PropertyDataGridViewCellEditEvent.DataGridViewCellEvent.CellClick)]
        [PropertyVirtualProperty(nameof(ConditionControls), nameof(ConditionControls.v_ActionParameterTable))]
        public DataTable v_FilterActionParameterTable { get; set; }

        [XmlAttribute]
        //[PropertyDescription("Please select a DataTable Variable Name of the Filtered DataTable")]
        //[InputSpecification("")]
        //[SampleUsage("**vNewTable** or **{{{vNewTable}}}**")]
        //[Remarks("")]
        //[PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        //[PropertyIsVariablesList(true)]
        //[PropertyInstanceType(PropertyInstanceType.InstanceType.DataTable)]
        //[PropertyParameterDirection(PropertyParameterDirection.ParameterDirection.Output)]
        //[PropertyValidationRule("Filtered DataTable", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyDisplayText(true, "New DataTable")]
        [PropertyVirtualProperty(nameof(DataTableControls), nameof(DataTableControls.v_NewOutputDataTableName))]
        //[PropertyDescription("New DataTable Variable Name")]
        //[PropertyValidationRule("New DataTable", PropertyValidationRule.ValidationRuleFlags.Empty)]
        //[PropertyDisplayText(true, "New DataTable")]
        //[PropertyDetailSampleUsageBehavior(MultiAttributesBehavior.Overwrite)]
        //[PropertyDetailSampleUsage("**vNewDataTale**", PropertyDetailSampleUsage.ValueType.VariableName)]
        //[PropertyDetailSampleUsage("**{{{vNewDataTale}}}**", PropertyDetailSampleUsage.ValueType.VariableName)]
        public string v_OutputDataTable { get; set; }

        public FilterDataTableRowByColumnValueCommand()
        {
            this.CommandName = "FilterDataTableRowByColumnValueCommand";
            this.SelectionName = "Filter DataTable Row By Column Value";
            this.CommandEnabled = true;
            this.CustomRendering = true;

            //this.v_TargetType = "Text";
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            (var targetDT, var colIndex) = this.GetDataTableVariableAndColumnIndex(nameof(v_InputDataTable), nameof(v_ColumnType), nameof(v_TargetColumnIndex), engine);

            var parameters = DataTableControls.GetFieldValues(v_FilterActionParameterTable, "ParameterName", "ParameterValue", engine);
            var checkFunc = ConditionControls.GetFilterDeterminStatementTruthFunc(nameof(v_ColumnType), nameof(v_FilterAction), parameters, engine, this);

            var res = DataTableControls.CloneDataTableOnlyColumnName(targetDT);

            int cols = targetDT.Columns.Count;

            int rows = targetDT.Rows.Count;
            for (int i = 0; i < rows; i++)
            {
                string value = targetDT.Rows[i][colIndex]?.ToString() ?? "";
                if (checkFunc(value, parameters))
                {
                    int r = res.Rows.Count;
                    res.Rows.Add();
                    for (int j = 0; j < cols; j++)
                    {
                        res.Rows[r][j] = targetDT.Rows[i][j];
                    }
                }
            }

            res.StoreInUserVariable(engine, v_OutputDataTable);
        }

        private void cmbTargetType_SelectionChangeCommited(object sender, EventArgs e)
        {
            var TargetTypeComboboxHelper = (ComboBox)ControlsList[nameof(v_TargetType)];
            var FilterActionComboboxHelper = (ComboBox)ControlsList[nameof(v_FilterAction)];
            ConditionControls.AddFilterActionItems(TargetTypeComboboxHelper, FilterActionComboboxHelper);
        }

        private void cmbFilterAction_SelectionChangeCommited(object sender, EventArgs e)
        {
            var FilterParametersGridViewHelper = (DataGridView)ControlsList[nameof(v_FilterActionParameterTable)];
            var TargetTypeComboboxHelper = (ComboBox)ControlsList[nameof(v_TargetType)];
            var FilterActionComboboxHelper = (ComboBox)ControlsList[nameof(v_FilterAction)];
            ConditionControls.RenderFilter(v_FilterActionParameterTable, FilterParametersGridViewHelper, FilterActionComboboxHelper, TargetTypeComboboxHelper);
        }
        public override void AfterShown()
        {
            var FilterParametersGridViewHelper = (DataGridView)ControlsList[nameof(v_FilterActionParameterTable)];
            var TargetTypeComboboxHelper = (ComboBox)ControlsList[nameof(v_TargetType)];
            var FilterActionComboboxHelper = (ComboBox)ControlsList[nameof(v_FilterAction)];
            ConditionControls.AddFilterActionItems(TargetTypeComboboxHelper, FilterActionComboboxHelper);
            ConditionControls.RenderFilter(v_FilterActionParameterTable, FilterParametersGridViewHelper, FilterActionComboboxHelper, TargetTypeComboboxHelper);
        }
    }
}