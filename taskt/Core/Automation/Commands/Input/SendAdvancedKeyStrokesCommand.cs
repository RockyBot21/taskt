﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Data;
using taskt.Core.Automation.User32;
using taskt.UI.Forms;
using System.Windows.Forms;
using taskt.UI.CustomControls;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{

    [Serializable]
    [Attributes.ClassAttributes.Group("Input Commands")]
    [Attributes.ClassAttributes.Description("Sends advanced keystrokes to a targeted window")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to send advanced keystroke inputs to a window.")]
    [Attributes.ClassAttributes.ImplementationDescription("This command implements 'User32' method to achieve automation.")]
    public class SendAdvancedKeyStrokesCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Please Enter the Window name (ex. Untitled - Notepad, %kwd_current_window%, {{{vWindowName}}})")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("Input or Type the name of the window that you want to activate or bring forward.")]
        [SampleUsage("**Untitled - Notepad** or **%kwd_current_window%** or **{{{vWindowName}}}**")]
        [Remarks("")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsWindowNamesList(true)]
        public string v_WindowName { get; set; }

        [XmlAttribute]
        [PropertyDescription("Optional - Window name search method (Default is Contains)")]
        [InputSpecification("")]
        [PropertyUISelectionOption("Contains")]
        [PropertyUISelectionOption("Starts with")]
        [PropertyUISelectionOption("Ends with")]
        [PropertyUISelectionOption("Exact match")]
        [SampleUsage("**Contains** or **Starts with** or **Ends with** or **Exact match**")]
        [Remarks("")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsOptional(true)]
        public string v_SearchMethod { get; set; }

        [PropertyDescription("Set Keys and Parameters")]
        [InputSpecification("Define the parameters for the actions.")]
        [SampleUsage("n/a")]
        [Remarks("Select Valid Options from the dropdowns")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.DataGridView)]
        public DataTable v_KeyActions { get; set; }

        [XmlElement]
        [PropertyDescription("Optional - Return all keys to 'UP' position after execution (Default is No)")]
        [PropertyUISelectionOption("Yes")]
        [PropertyUISelectionOption("No")]
        [InputSpecification("Select either 'Yes' or 'No' as to a preference")]
        [SampleUsage("**Yes** or **No**")]
        [Remarks("")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsOptional(true)]
        public string v_KeyUpDefault { get; set; }

        [XmlIgnore]
        [NonSerialized]
        private DataGridView KeystrokeGridHelper;

        public SendAdvancedKeyStrokesCommand()
        {
            this.CommandName = "SendAdvancedKeyStrokesCommand";
            this.SelectionName = "Send Advanced Keystrokes";
            this.CommandEnabled = true;
            this.CustomRendering = true;
            this.v_KeyActions = new DataTable();
            this.v_KeyActions.Columns.Add("Key");
            this.v_KeyActions.Columns.Add("Action");
            this.v_KeyActions.TableName = "SendAdvancedKeyStrokesCommand" + DateTime.Now.ToString("MMddyy.hhmmss");
            v_KeyUpDefault = "Yes";
        }

        public override void RunCommand(object sender)
        {
            var targetWindow = v_WindowName.ConvertToUserVariable(sender);
            var searchMethod = v_SearchMethod.ConvertToUserVariable(sender);
            if (String.IsNullOrEmpty(searchMethod))
            {
                searchMethod = "Contains";
            }

            //activate anything except current window
            if (targetWindow != ((Engine.AutomationEngineInstance)sender).engineSettings.CurrentWindowKeyword)
            {
                ActivateWindowCommand activateWindow = new ActivateWindowCommand
                {
                    v_WindowName = targetWindow,
                    v_SearchMethod = searchMethod
                };
                activateWindow.RunCommand(sender);
            }

            //track all keys down
            var keysDown = new List<Keys>();

            //run each selected item
            foreach (DataRow rw in v_KeyActions.Rows)
            {
                //get key name
                var keyName = rw.Field<string>("Key");

                //get key action
                var action = rw.Field<string>("Action");

                //parse OEM key name
                string oemKeyString = keyName.Split('[', ']')[1];

                var oemKeyName = (Keys)Enum.Parse(typeof(Keys), oemKeyString);

            
                //"Key Press (Down + Up)", "Key Down", "Key Up"
                switch (action)
                {
                    case "Key Press (Down + Up)":
                        //simulate press
                        User32Functions.KeyDown(oemKeyName);
                        User32Functions.KeyUp(oemKeyName);
                        
                        //key returned to UP position so remove if we added it to the keys down list
                        if (keysDown.Contains(oemKeyName))
                        {
                            keysDown.Remove(oemKeyName);
                        }
                        break;
                    case "Key Down":
                        //simulate down
                        User32Functions.KeyDown(oemKeyName);

                        //track via keys down list
                        if (!keysDown.Contains(oemKeyName))
                        {
                            keysDown.Add(oemKeyName);
                        }
                    
                        break;
                    case "Key Up":
                        //simulate up
                        User32Functions.KeyUp(oemKeyName);

                        //remove from key down
                        if (keysDown.Contains(oemKeyName))
                        {
                            keysDown.Remove(oemKeyName);
                        }

                        break;
                    default:
                        break;
                }

            }

            //return key to up position if requested
            if (v_KeyUpDefault.ConvertToUserVariable(sender) == "Yes")
            {
                foreach (var key in keysDown)
                {
                    User32Functions.KeyUp(key);
                }
            }
        }

        public override List<Control> Render(frmCommandEditor editor)
        {
            base.Render(editor);

            RenderedControls.Add(CommandControls.CreateDefaultLabelFor("v_WindowName", this));
            var WindowNameControl = CommandControls.CreateStandardComboboxFor("v_WindowName", this).AddWindowNames(editor);
            RenderedControls.AddRange(CommandControls.CreateDefaultUIHelpersFor("v_WindowName", this, WindowNameControl, editor));
            RenderedControls.Add(WindowNameControl);

            RenderedControls.AddRange(CommandControls.CreateDefaultDropdownGroupFor("v_SearchMethod", this, editor));

            KeystrokeGridHelper = CommandControls.CreateDefaultDataGridViewFor("v_KeyActions", this, true, true, false, 500, 140, false);
            KeystrokeGridHelper.CellClick += DataTableControls.AllEditableDataGridView_CellClick;

            DataGridViewComboBoxColumn propertyName = new DataGridViewComboBoxColumn();
            propertyName.DataSource = Common.GetAvailableKeys();
            propertyName.HeaderText = "Selected Key";
            propertyName.DataPropertyName = "Key";
            KeystrokeGridHelper.Columns.Add(propertyName);

            DataGridViewComboBoxColumn propertyValue = new DataGridViewComboBoxColumn();
            propertyValue.DataSource = new List<string> { "Key Press (Down + Up)", "Key Down", "Key Up" };
            propertyValue.HeaderText = "Selected Action";
            propertyValue.DataPropertyName = "Action";
            KeystrokeGridHelper.Columns.Add(propertyValue);

            RenderedControls.Add(CommandControls.CreateDefaultLabelFor("v_KeyActions", this));
            RenderedControls.Add(KeystrokeGridHelper);

            RenderedControls.AddRange(CommandControls.CreateDefaultDropdownGroupFor("v_KeyUpDefault", this, editor));

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + " [Send To Window '" + v_WindowName + "']";
        }

        public override void BeforeValidate()
        {
            base.BeforeValidate();

            DataTableControls.BeforeValidate((DataGridView)ControlsList[nameof(v_KeyActions)], v_KeyActions);
        }

        public override bool IsValidate(frmCommandEditor editor)
        {
            base.IsValidate(editor);

            if (String.IsNullOrEmpty(this.v_WindowName))
            {
                this.validationResult += "Windows name is empty.\n";
                this.IsValid = false;
            }
            for (int i = 0; i < v_KeyActions.Rows.Count; i++)
            {
                var row = v_KeyActions.Rows[i];
                if (String.IsNullOrEmpty(row["Key"].ToString()))
                {
                    this.validationResult += "Selected Key #" + (i + 1) + " is empty.\n";
                    this.IsValid = false;
                }
                else if (String.IsNullOrEmpty(row["Action"].ToString()))
                {
                    this.validationResult += "Selected Action #" + (i + 1) + " is empty.\n";
                    this.IsValid = false;
                }
            }

            return this.IsValid;
        }
        public override void ConvertToIntermediate(EngineSettings settings, List<Script.ScriptVariable> variables)
        {
            var cnv = new Dictionary<string, string>();
            cnv.Add("v_WindowName", "convertToIntermediateWindowName");
            ConvertToIntermediate(settings, cnv, variables);
        }

        public override void ConvertToRaw(EngineSettings settings)
        {
            var cnv = new Dictionary<string, string>();
            cnv.Add("v_WindowName", "convertToRawWindowName");
            ConvertToRaw(settings, cnv);
        }
    }
}