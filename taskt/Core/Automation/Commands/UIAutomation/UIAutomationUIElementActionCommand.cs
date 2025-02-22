﻿using System;
using System.Linq;
using System.Xml.Serialization;
using System.Data;
using System.Windows.Forms;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{

    [Serializable]
    [Attributes.ClassAttributes.Group("UIAutomation Commands")]
    [Attributes.ClassAttributes.SubGruop("UIElement Action")]
    [Attributes.ClassAttributes.CommandSettings("UIElement Action")]
    [Attributes.ClassAttributes.Description("Combined implementation of the ThickAppClick/GetText command but includes an advanced Window Recorder to record the required element.")]
    [Attributes.ClassAttributes.ImplementationDescription("This command implements 'Windows UI Automation' to find elements and invokes a Variable Command to assign data and achieve automation")]
    [Attributes.ClassAttributes.CommandIcon(nameof(Properties.Resources.command_window))]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class UIAutomationUIElementActionCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowControls), nameof(WindowControls.v_WindowName))]
        public string v_WindowName { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(GeneralPropertyControls),  nameof(GeneralPropertyControls.v_ComboBox))]
        [PropertyDescription("UIElement Action")]
        [PropertyUISelectionOption("Click UIElement")]
        [PropertyUISelectionOption("Expand Collapse Items In UIElement")]
        [PropertyUISelectionOption("Scroll UIElement")]
        [PropertyUISelectionOption("Select UIElement")]
        [PropertyUISelectionOption("Select Item In UIElement")]
        [PropertyUISelectionOption("Set Text To UIElement")]
        [PropertyUISelectionOption("Get Property Value From UIElement")]
        [PropertyUISelectionOption("Check UIElement Exists")]
        [PropertyUISelectionOption("Get Text From UIElement")]
        [PropertyUISelectionOption("Get Selected State From UIElement")]
        [PropertyUISelectionOption("Get Text From Table UIElement")]
        [PropertyUISelectionOption("Get UIElement Position")]
        [PropertyUISelectionOption("Get UIElement Size")]
        [PropertyUISelectionOption("Wait For UIElement To Exists")]
        [PropertySelectionChangeEvent(nameof(cmbActionType_SelectedItemChange))]
        [PropertyDisplayText(true, "Action")]
        public string v_AutomationType { get; set; }

        [XmlElement]
        [PropertyVirtualProperty(nameof(UIElementControls), nameof(UIElementControls.v_SearchParameters))]
        public DataTable v_UIASearchParameters { get; set; }

        [XmlElement]
        [PropertyDescription("Action Parameters")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.DataGridView)]
        [PropertyDataGridViewSetting(false, false, true, 400, 250)]
        [PropertyDataGridViewColumnSettings("Parameter Name", "Parameter Name", true)]
        [PropertyDataGridViewColumnSettings("Parameter Value", "Parameter Value", false)]
        public DataTable v_UIAActionParameters { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowControls), nameof(WindowControls.v_CompareMethod))]
        public string v_CompareMethod { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowControls), nameof(WindowControls.v_MatchMethod_Single))]
        [PropertySelectionChangeEvent(nameof(MatchMethodComboBox_SelectionChangeCommitted))]
        public string v_MatchMethod { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowControls), nameof(WindowControls.v_TargetWindowIndex))]
        public string v_TargetWindowIndex { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowControls), nameof(WindowControls.v_WaitTime))]
        public string v_WaitTimeForWindow { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(UIElementControls), nameof(UIElementControls.v_WaitTime))]
        public string v_ElementWaitTime { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowControls), nameof(WindowControls.v_WindowNameResult))]
        public string v_NameResult { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowControls), nameof(WindowControls.v_OutputWindowHandle))]
        public string v_HandleResult { get; set; }

        public UIAutomationUIElementActionCommand()
        {
        }

        public override void RunCommand(Engine.AutomationEngineInstance engine)
        {
            var elemAction = this.ExpandValueOrUserVariableAsSelectionItem(nameof(v_AutomationType), engine);

            var winElemVar = VariableNameControls.GetInnerVariableName(0, engine);
            var winElem = new UIAutomationSearchUIElementFromWindowCommand()
            {
                v_WindowName = this.v_WindowName,
                v_CompareMethod = this.v_CompareMethod,
                v_MatchMethod = this.v_MatchMethod,
                v_TargetWindowIndex = this.v_TargetWindowIndex,
                v_WaitTimeForWindow = this.v_WaitTimeForWindow,
                v_AutomationElementVariable = winElemVar,
                v_NameResult = this.v_NameResult,
                v_HandleResult = this.v_HandleResult,
            };
            winElem.RunCommand(engine);

            var p = DataTableControls.GetFieldValues(v_UIAActionParameters, "Parameter Name", "Parameter Value", false, engine);
            var trgElemVar = VariableNameControls.GetInnerVariableName(1, engine);

            switch (elemAction)
            {
                case "check uielement exists":
                    var chkElem = new UIAutomationCheckUIElementExistCommand()
                    {
                        v_TargetElement = winElemVar,
                        v_SearchParameters = this.v_UIASearchParameters,
                        v_WaitTime = this.v_ElementWaitTime,
                        v_Result = p["Apply To Variable"],
                    };
                    chkElem.RunCommand(engine);
                    return;
                    
                default:
                    var trgElem = new UIAutomationSearchUIElementFromUIElementCommand()
                    {
                        v_TargetElement = winElemVar,
                        v_SearchParameters = this.v_UIASearchParameters,
                        v_WaitTime = this.v_ElementWaitTime,
                        v_AutomationElementVariable = trgElemVar,
                    };
                    trgElem.RunCommand(engine);
                    break;
            }

            // todo: use same method
            switch (elemAction)
            {
                case "click uielement":
                    var clickCmd = new UIAutomationClickUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_ClickType = p["Click Type"],
                        v_XOffset = p["X Offset"],
                        v_YOffset = p["Y Offset"],
                    };
                    clickCmd.RunCommand(engine);
                    break;
                case "expand collapse items in uielement":
                    var expandCmd = new UIAutomationExpandCollapseItemsInUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_ItemsState = p["Items State"],
                    };
                    expandCmd.RunCommand(engine);
                    break;
                case "scroll uielement":
                    var scrollCmd = new UIAutomationScrollUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_ScrollBarType = p["ScrollBar Type"],
                        v_DirectionAndAmount = p["Scroll Method"],
                    };
                    scrollCmd.RunCommand(engine);
                    break;
                case "select uielement":
                    var selectCmd = new UIAutomationSelectUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                    };
                    selectCmd.RunCommand(engine);
                    break;
                case "select item in uielement":
                    var selectItemCmd = new UIAutomationSelectItemInUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_Item = p["Item Value"],
                    };
                    selectItemCmd.RunCommand(engine);
                    break;
                case "set text to uielement":
                    var setTextCmd = new UIAutomationSetTextToUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_TextVariable = p["Text To Set"],
                    };
                    setTextCmd.RunCommand(engine);
                    break;
                case "get property value from uielement":
                    var propValueCmd = new UIAutomationGetPropertyValueFromUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_PropertyName = p["Property Name"],
                        v_Result = p["Apply To Variable"],
                    };
                    propValueCmd.RunCommand(engine);
                    break;
                case "check uielement exists":
                    true.StoreInUserVariable(engine, p["Apply To Variable"]);
                    break;
                case "get text from uielement":
                    var getTextCmd = new UIAutomationGetTextFromUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_TextVariable = p["Apply To Variable"],
                    };
                    getTextCmd.RunCommand(engine);
                    break;
                case "get selected state from uielement":
                    var getSelectedCmd = new UIAutomationGetSelectedStateFromUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_ResultVariable = p["Apply To Variable"],
                    };
                    getSelectedCmd.RunCommand(engine);
                    break;
                case "get text from table uielement":
                    var getTableCmd = new UIAutomationGetTextFromTableUIElementCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_Row = p["Row"],
                        v_Column = p["Column"],
                        v_TextVariable = p["Apply To Variable"],
                    };
                    getTableCmd.RunCommand(engine);
                    break;
                case "get uielement position":
                    var getElemPosCmd = new UIAutomationGetUIElementPositionCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_XPosition = p["X Variable"],
                        v_YPosition = p["Y Variable"],
                        v_PositionBase = p["Base Position"],
                    };
                    getElemPosCmd.RunCommand(engine);
                    break;
                case "get uielement size":
                    var getElemSizeCmd = new UIAutomationGetUIElementSizeCommand()
                    {
                        v_TargetElement = trgElemVar,
                        v_Width = p["Width Variable"],
                        v_Height = p["Height Variable"],
                    };
                    getElemSizeCmd.RunCommand(engine);
                    break;
            }
        }

        public override void AfterShown(UI.Forms.ScriptBuilder.CommandEditor.frmCommandEditor editor)
        {
            var cmb = FormUIControls.GetPropertyControl<ComboBox>(ControlsList, nameof(v_AutomationType));
            var dgv = FormUIControls.GetPropertyControl<DataGridView>(ControlsList, nameof(v_UIAActionParameters));
            actionParameterProcess(dgv, cmb.SelectedItem?.ToString() ?? "");
        }

        private void cmbActionType_SelectedItemChange(object sender, EventArgs e)
        {
            var a = ((ComboBox)sender).SelectedItem?.ToString() ?? "";

            var dgv = FormUIControls.GetPropertyControl<DataGridView>(this.ControlsList, nameof(v_UIAActionParameters));
            var table = v_UIAActionParameters;
            table.Rows.Clear();
            switch (a.ToLower())
            {
                case "click uielement":
                    table.Rows.Add(new string[] { "Click Type", "" });
                    table.Rows.Add(new string[] { "X Offset", "" });
                    table.Rows.Add(new string[] { "Y Offset", "" });
                    break;

                case "expand collapse items in uielement":
                    table.Rows.Add(new string[] { "Items State", "" });
                    break;
                case "scroll uielement":
                    table.Rows.Add(new string[] { "ScrollBar Type", "" });
                    table.Rows.Add(new string[] { "Scroll Method", "" });
                    break;

                case "select item in uielement":
                    table.Rows.Add(new string[] { "Item Value", "" });
                    break;

                case "set text to uielement":
                    table.Rows.Add(new string[] { "Text To Set", "" });
                    break;

                case "get property value from uielement":
                    table.Rows.Add(new string[] { "Property Name", "" });
                    table.Rows.Add(new string[] { "Apply To Variable", "" });
                    break;

                case "get text from table uielement":
                    table.Rows.Add(new string[] { "Row", "" });
                    table.Rows.Add(new string[] { "Column", "" });
                    table.Rows.Add(new string[] { "Apply To Variable", "" });
                    break;

                case "get uielement position":
                    table.Rows.Add(new string[] { "X Variable", "" });
                    table.Rows.Add(new string[] { "Y Variable", "" });
                    table.Rows.Add(new string[] { "Base Position", ""});
                    break;

                case "get uielement size":
                    table.Rows.Add(new string[] { "Width Variable", "" });
                    table.Rows.Add(new string[] { "Height Variable", "" });
                    break;

                case "check uielement exists":
                case "get text from uielement":
                case "get selected state from uielement":
                    table.Rows.Add(new string[] { "Apply To Variable", "" });
                    break;

                case "select uielement":
                case "wait for uielement to exists":
                    // nothing
                    break;
            }

            actionParameterProcess(dgv, a);
        }

        private static void actionParameterProcess(DataGridView dgv, string actionType)
        {
            switch (actionType.ToLower())
            {
                case "click uielement":
                    var clickType = new DataGridViewComboBoxCell();
                    clickType.Items.AddRange(new string[]
                    {
                        "Left Click",
                        "Middle Click",
                        "Right Click",
                        "Left Down",
                        "Middle Down",
                        "Right Down",
                        "Left Up",
                        "Middle Up",
                        "Right Up",
                        "Double Left Click",
                        "None",
                    });
                    dgv.Rows[0].Cells[1] = clickType;
                    break;
                case "expand collapse items in uielement":
                    var itemState = new DataGridViewComboBoxCell();
                    itemState.Items.AddRange(new string[]
                    {
                        "Expand",
                        "Collapse"
                    });
                    dgv.Rows[0].Cells[1] = itemState;
                    break;
                case "scroll uielement":
                    var barType = new DataGridViewComboBoxCell();
                    barType.Items.AddRange(new string[]
                    {
                        "Vertical",
                        "Horizonal",
                    });
                    var scrollMethod = new DataGridViewComboBoxCell();
                    scrollMethod.Items.AddRange(new string[]
                    {
                        "Scroll Small Down or Right",
                        "Scroll Large Down or Right",
                        "Scroll Small Up or Left",
                        "Scroll Large Up or Left",
                    });
                    dgv.Rows[0].Cells[1] = barType;
                    dgv.Rows[1].Cells[1] = scrollMethod;
                    break;
                case "get property value from uielement":
                    var propNames = new DataGridViewComboBoxCell();
                    propNames.Items.AddRange(new string[]
                    {
                        "Name",
                        "ControlType",
                        "LocalizedControlType",
                        "IsEnabled",
                        "IsOffscreen",
                        "IsKeyboardFocusable",
                        "HasKeyboardFocusable",
                        "AccessKey",
                        "ProcessId",
                        "AutomationId",
                        "FrameworkId",
                        "ClassName",
                        "IsContentElement",
                        "IsPassword",
                        "AcceleratorKey",
                        "HelpText",
                        "IsControlElement",
                        "IsRequiredForForm",
                        "ItemStatus",
                        "ItemType",
                        "NativeWindowHandle",
                    });
                    dgv.Rows[0].Cells[1] = propNames;
                    break;
                case "get uielement position":
                    var positionName = new DataGridViewComboBoxCell();
                    positionName.Items.AddRange(new string[]
                    {
                        "Top Left",
                        "Bottom Right",
                        "Top Right",
                        "Bottom Left",
                        "Center",
                    });
                    dgv.Rows[2].Cells[1] = positionName;
                    break;
            }
        }

        private void MatchMethodComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            WindowControls.MatchMethodComboBox_SelectionChangeCommitted(ControlsList, (ComboBox)sender, nameof(v_TargetWindowIndex));
        }

        public override void BeforeValidate()
        {
            var dgvAction = FormUIControls.GetPropertyControl<DataGridView>(ControlsList, nameof(v_UIAActionParameters));
            DataTableControls.BeforeValidate(dgvAction, v_UIAActionParameters);

            var dgvSearch = FormUIControls.GetPropertyControl<DataGridView>(ControlsList, nameof(v_UIASearchParameters));
            DataTableControls.BeforeValidate(dgvSearch, v_UIASearchParameters);
        }

        //public override string GetDisplayValue()
        //{
        //    if (v_AutomationType == "Click Element")
        //    {
        //        //create search params
        //        var clickType = (from rw in v_UIAActionParameters.AsEnumerable()
        //                         where rw.Field<string>("Parameter Name") == "Click Type"
        //                         select rw.Field<string>("Parameter Value")).FirstOrDefault();


        //        return base.GetDisplayValue() + " [" + clickType + " element in window '" + v_WindowName + "']";
        //    }
        //    else if(v_AutomationType == "Check If Element Exists")
        //    {

        //        //apply to variable
        //        var applyToVariable = (from rw in v_UIAActionParameters.AsEnumerable()
        //                               where rw.Field<string>("Parameter Name") == "Apply To Variable"
        //                               select rw.Field<string>("Parameter Value")).FirstOrDefault();

        //        return base.GetDisplayValue() + " [Check for element in window '" + v_WindowName + "' and apply to '" + applyToVariable + "']";
        //    }
        //    else if (v_AutomationType == "Get Text Value From Element")
        //    {
        //        //apply to variable
        //        var applyToVariable = (from rw in v_UIAActionParameters.AsEnumerable()
        //                               where rw.Field<string>("Parameter Name") == "Apply To Variable"
        //                               select rw.Field<string>("Parameter Value")).FirstOrDefault();

        //        return base.GetDisplayValue() + " [Text Value for element in window '" + v_WindowName + "' and apply to '" + applyToVariable + "']";
        //    }
        //    else if (v_AutomationType == "Get Selected State From Element")
        //    {
        //        //apply to variable
        //        var applyToVariable = (from rw in v_UIAActionParameters.AsEnumerable()
        //                               where rw.Field<string>("Parameter Name") == "Apply To Variable"
        //                               select rw.Field<string>("Parameter Value")).FirstOrDefault();

        //        return base.GetDisplayValue() + " [Selected State for element in window '" + v_WindowName + "' and apply to '" + applyToVariable + "']";
        //    }
        //    else
        //    {
        //        //get value from property
        //        var propertyName = (from rw in v_UIAActionParameters.AsEnumerable()
        //                            where rw.Field<string>("Parameter Name") == "Get Value From"
        //                            select rw.Field<string>("Parameter Value")).FirstOrDefault();

        //        //apply to variable
        //        var applyToVariable = (from rw in v_UIAActionParameters.AsEnumerable()
        //                               where rw.Field<string>("Parameter Name") == "Apply To Variable"
        //                               select rw.Field<string>("Parameter Value")).FirstOrDefault();

        //        return base.GetDisplayValue() + " [Get value from '" + propertyName + "' in window '" + v_WindowName + "' and apply to '" + applyToVariable + "']";
        //    }
        //}

        //public override bool IsValidate(frmCommandEditor editor)
        //{
        //    base.IsValidate(editor);

        //    if (String.IsNullOrEmpty(this.v_WindowName))
        //    {
        //        this.validationResult += "Window Name is empty.\n";
        //        this.IsValid = false;
        //    }
        //    if (String.IsNullOrEmpty(this.v_AutomationType))
        //    {
        //        this.validationResult += "Action is empty.\n";
        //        this.IsValid = false;
        //    }
        //    else
        //    {
        //        switch (this.v_AutomationType)
        //        {
        //            case "Click Element":
        //                ClickElementValidate();
        //                break;
        //            case "Get Value From Element":
        //                GetValueFromElementValidate();
        //                break;
        //            case "Check If Element Exists":
        //            case "Get Text Value From Element":
        //            case "Get Selected State From Element":
        //                CheckIfElementExistsValidate();
        //                break;

        //            case "Get Value From Table Element":
        //                GetValueFromTableElement();
        //                break;

        //            default:
        //                break;
        //        }
        //    }

        //    return this.IsValid;
        //}

        private void ClickElementValidate()
        {
            var clickType = (from rw in v_UIAActionParameters.AsEnumerable()
                             where rw.Field<string>("Parameter Name") == "Click Type"
                             select rw.Field<string>("Parameter Value")).FirstOrDefault();
            if (String.IsNullOrEmpty(clickType))
            {
                this.validationResult += "Click Type is empty.\n";
                this.IsValid = false;
            }
            
            var x = (from rw in v_UIAActionParameters.AsEnumerable()
                     where rw.Field<string>("Parameter Name") == "X Adjustment"
                     select rw.Field<string>("Parameter Value")).FirstOrDefault();
            var y = (from rw in v_UIAActionParameters.AsEnumerable()
                     where rw.Field<string>("Parameter Name") == "Y Adjustment"
                     select rw.Field<string>("Parameter Value")).FirstOrDefault();

            if (String.IsNullOrEmpty(x))
            {
                this.validationResult += "X Adjustment is empty.\n";
                this.IsValid = false;
            }
            if (String.IsNullOrEmpty(y))
            {
                this.validationResult += "Y Adjustment is empty.\n";
                this.IsValid = false;
            }
        }

        private void GetValueFromElementValidate()
        {
            var valueFrom = (from rw in v_UIAActionParameters.AsEnumerable()
                             where rw.Field<string>("Parameter Name") == "Get Value From"
                             select rw.Field<string>("Parameter Value")).FirstOrDefault();
            var variable = (from rw in v_UIAActionParameters.AsEnumerable()
                             where rw.Field<string>("Parameter Name") == "Apply To Variable"
                             select rw.Field<string>("Parameter Value")).FirstOrDefault();

            if (String.IsNullOrEmpty(valueFrom))
            {
                this.validationResult += "Get Value From is empty.\n";
                this.IsValid = false;
            }
            if (String.IsNullOrEmpty(variable))
            {
                this.validationResult += "Variable is empty.\n";
                this.IsValid = false;
            }
        }

        private void CheckIfElementExistsValidate()
        {
            var variable = (from rw in v_UIAActionParameters.AsEnumerable()
                            where rw.Field<string>("Parameter Name") == "Apply To Variable"
                            select rw.Field<string>("Parameter Value")).FirstOrDefault();
            if (String.IsNullOrEmpty(variable))
            {
                this.validationResult += "Variable is empty.\n";
                this.IsValid = false;
            }
        }

        private void GetValueFromTableElement()
        {
            var row = (from rw in v_UIAActionParameters.AsEnumerable()
                            where rw.Field<string>("Parameter Name") == "Row"
                            select rw.Field<string>("Parameter Value")).FirstOrDefault();
            var column = (from rw in v_UIAActionParameters.AsEnumerable()
                            where rw.Field<string>("Parameter Name") == "Column"
                            select rw.Field<string>("Parameter Value")).FirstOrDefault();
            var variable = (from rw in v_UIAActionParameters.AsEnumerable()
                            where rw.Field<string>("Parameter Name") == "Apply To Variable"
                            select rw.Field<string>("Parameter Value")).FirstOrDefault();

            if (String.IsNullOrEmpty(row))
            {
                this.validationResult += "Row is empty.\n";
                this.IsValid = false;
            }
            if (String.IsNullOrEmpty(column))
            {
                this.validationResult += "Column is empty.\n";
                this.IsValid = false;
            }
            if (String.IsNullOrEmpty(variable))
            {
                this.validationResult += "Variable is empty.\n";
                this.IsValid = false;
            }
        }

        //public override void ConvertToIntermediate(EngineSettings settings, List<Script.ScriptVariable> variables)
        //{
        //    var cnv = new Dictionary<string, string>();
        //    cnv.Add("v_WindowName", "convertToIntermediateWindowName");
        //    ConvertToIntermediate(settings, cnv, variables);
        //}

        //public override void ConvertToRaw(EngineSettings settings)
        //{
        //    var cnv = new Dictionary<string, string>();
        //    cnv.Add("v_WindowName", "convertToRawWindowName");
        //    ConvertToRaw(settings, cnv);
        //}
    }
}