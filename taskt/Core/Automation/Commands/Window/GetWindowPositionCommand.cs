﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using taskt.Core.Automation.User32;
using taskt.UI.Forms;
using taskt.UI.CustomControls;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("Window Commands")]
    [Attributes.ClassAttributes.SubGruop("Window State")]
    [Attributes.ClassAttributes.Description("This command returns window position.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want window position.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    public class GetWindowPositionCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Please enter or select the window position that you want to.")]
        [InputSpecification("Input or Type the name of the window name that you want to.")]
        [SampleUsage("**Untitled - Notepad** or **%kwd_current_window%** or **{{{vWindow}}}**")]
        [Remarks("")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyCustomUIHelper("Up-to-date", "lnkUpToDate_Click")]
        [PropertyIsWindowNamesList(true)]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyValidationRule("Window Name", PropertyValidationRule.ValidationRuleFlags.Empty)]
        public string v_WindowName { get; set; }

        [XmlAttribute]
        [PropertyDescription("Window title search method")]
        [InputSpecification("")]
        [PropertyUISelectionOption("Contains")]
        [PropertyUISelectionOption("Starts with")]
        [PropertyUISelectionOption("Ends with")]
        [PropertyUISelectionOption("Exact match")]
        [SampleUsage("**Contains** or **Starts with** or **Ends with** or **Exact match**")]
        [Remarks("")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsOptional(true, "Contains")]
        public string v_SearchMethod { get; set; }

        [XmlAttribute]
        [PropertyDescription("Specify the variable to recieve the window position X")]
        [InputSpecification("")]
        [SampleUsage("**vSomeVariable**")]
        [Remarks("If you have enabled the setting **Create Missing Variables at Runtime** then you are not required to pre-define your variables, however, it is highly recommended.")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsVariablesList(true)]
        [PropertyIsOptional(true)]
        public string v_VariablePositionX { get; set; }

        [XmlAttribute]
        [PropertyDescription("Specify the variable to recieve the window position Y")]
        [InputSpecification("")]
        [SampleUsage("**vSomeVariable**")]
        [Remarks("If you have enabled the setting **Create Missing Variables at Runtime** then you are not required to pre-define your variables, however, it is highly recommended.")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsVariablesList(true)]
        [PropertyIsOptional(true)]
        public string v_VariablePositionY { get; set; }

        [XmlAttribute]
        [PropertyDescription("Base position")]
        [InputSpecification("")]
        [SampleUsage("")]
        [Remarks("")]
        [PropertyUISelectionOption("Top Left")]
        [PropertyUISelectionOption("Bottom Right")]
        [PropertyUISelectionOption("Top Right")]
        [PropertyUISelectionOption("Bottom Left")]
        [PropertyUISelectionOption("Center")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsOptional(true, "Top Left")]
        public string v_PositionBase { get; set; }

        [XmlIgnore]
        [NonSerialized]
        public ComboBox WindowNameControl;

        public GetWindowPositionCommand()
        {
            this.CommandName = "GetWindowPositionCommand";
            this.SelectionName = "Get Window Position";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }
        public override void RunCommand(object sender)
        {
            //var engine = (Engine.AutomationEngineInstance)sender;

            //string windowName = v_WindowName.ConvertToUserVariable(sender);
            //string searchMethod = v_SearchMethod.ConvertToUserVariable(sender);
            //if (String.IsNullOrEmpty(searchMethod))
            //{
            //    searchMethod = "Contains";
            //}

            //bool targetIsCurrentWindow = windowName == engine.engineSettings.CurrentWindowKeyword;
            //var targetWindows = User32Functions.FindTargetWindows(windowName, targetIsCurrentWindow, true);

            //User32Functions.RECT pos = new User32Functions.RECT();
            //bool isWindowsFound = false;

            ////loop each window
            //if (searchMethod == "Contains" || targetIsCurrentWindow)
            //{
            //    foreach (var targetedWindow in targetWindows)
            //    {
            //        pos = User32Functions.GetWindowPosition(targetedWindow);
            //        isWindowsFound = true;
            //    }
            //}
            //else
            //{
            //    Func<string, bool> searchFunc;
            //    switch(searchMethod)
            //    {
            //        case "Starts with":
            //            searchFunc = (s) => s.StartsWith(windowName);
            //            break;

            //        case "Ends with":
            //            searchFunc = (s) => s.EndsWith(windowName);
            //            break;

            //        case "Exact match":
            //            searchFunc = (s) => (s == windowName);
            //            break;

            //        default:
            //            throw new Exception("Search method " + searchMethod + " is not support.");
            //            break;
            //    }

            //    foreach (var targetedWindow in targetWindows)
            //    {
            //        if (searchFunc(User32Functions.GetWindowTitle(targetedWindow)))
            //        {
            //            pos = User32Functions.GetWindowPosition(targetedWindow);
            //            isWindowsFound = true;
            //        }
            //    }
            //}

            //if (!isWindowsFound)
            //{
            //    throw new Exception("Window name " + windowName + " is not found.");
            //    return;
            //}

            //var basePosition = v_PositionBase.ConvertToUserVariable(sender);
            //if (String.IsNullOrEmpty(basePosition))
            //{
            //    basePosition = "Top Left";
            //}
            //int x, y;
            //switch (basePosition)
            //{
            //    case "Top Left":
            //        x = pos.left;
            //        y = pos.top;
            //        break;
            //    case "Bottom Right":
            //        x = pos.right;
            //        y = pos.bottom;
            //        break;
            //    case "Top Right":
            //        x = pos.right;
            //        y = pos.top;
            //        break;
            //    case "Bottom Left":
            //        x = pos.left;
            //        y = pos.bottom;
            //        break;
            //    case "Center":
            //        x = (pos.right + pos.left) / 2;
            //        y = (pos.top + pos.bottom) / 2;
            //        break;
            //    default:
            //        throw new Exception("Base Position " + basePosition + " is not supported.");
            //        break;
            //}

            //if (!String.IsNullOrEmpty(v_VariablePositionX))
            //{
            //    x.ToString().StoreInUserVariable(sender, v_VariablePositionX);
            //}
            //if (!String.IsNullOrEmpty(v_VariablePositionY))
            //{
            //    y.ToString().StoreInUserVariable(sender, v_VariablePositionY);
            //}

            var engine = (Engine.AutomationEngineInstance)sender;

            string windowName = v_WindowName.ConvertToUserVariable(sender);
            string searchMethod = v_SearchMethod.GetUISelectionValue("v_SearchMethod", this, engine);

            IntPtr hWnd = WindowNameControls.FindWindow(windowName, searchMethod, engine);

            User32Functions.RECT pos = User32Functions.GetWindowPosition(hWnd);

            int x = 0, y = 0;
            switch(v_PositionBase.GetUISelectionValue("v_PositionBase", this, engine))
            {
                case "top left":
                    x = pos.left;
                    y = pos.top;
                    break;
                case "bottom right":
                    x = pos.right;
                    y = pos.bottom;
                    break;
                case "top right":
                    x = pos.right;
                    y = pos.top;
                    break;
                case "bottom left":
                    x = pos.left;
                    y = pos.bottom;
                    break;
                case "center":
                    x = (pos.right + pos.left) / 2;
                    y = (pos.top + pos.bottom) / 2;
                    break;
            }
            if (!String.IsNullOrEmpty(v_VariablePositionX))
            {
                x.ToString().StoreInUserVariable(engine, v_VariablePositionX);
            }
            if (!String.IsNullOrEmpty(v_VariablePositionY))
            {
                y.ToString().StoreInUserVariable(engine, v_VariablePositionY);
            }
        }

        public override List<Control> Render(frmCommandEditor editor)
        {
            base.Render(editor);

            var ctrls = CommandControls.MultiCreateInferenceDefaultControlGroupFor(this, editor);
            RenderedControls.AddRange(ctrls);

            return RenderedControls;

        }
        public override void Refresh(frmCommandEditor editor)
        {
            base.Refresh();
            WindowNameControl.AddWindowNames();
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + " [Target Window: " + v_WindowName + "', Result(X) In: '" + v_VariablePositionX + "', Result(Y) In: '" + v_VariablePositionY + "']";
        }

        private void lnkUpToDate_Click(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)((CommandItemControl)sender).Tag;
            WindowNameControls.UpdateWindowTitleCombobox(cmb);
        }

        //public override bool IsValidate(frmCommandEditor editor)
        //{
        //    base.IsValidate(editor);

        //    if (String.IsNullOrEmpty(this.v_WindowName))
        //    {
        //        this.validationResult += "Window is empty.\n";
        //        this.IsValid = false;
        //    }

        //    return this.IsValid;
        //}

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