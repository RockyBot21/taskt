﻿using System;
using System.Linq;
using System.Xml.Serialization;
using System.Data;
using System.Windows.Forms;
using System.Collections.Generic;
using taskt.UI.Forms;
using taskt.UI.CustomControls;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("Dictionary Commands")]
    [Attributes.ClassAttributes.SubGruop("Dictionary Item")]
    [Attributes.ClassAttributes.Description("This command allows you to get value in Dictionary")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to get value in Dictionary.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class GetDictionaryValueCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Please input The Dictionary Variable")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("")]
        [SampleUsage("**myDictionary** or **{{{vMyDic}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyInstanceType(PropertyInstanceType.InstanceType.Dictionary)]
        [PropertyValidationRule("Dictionary", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "Dictionary")]
        public string v_InputData { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please indicate the key for the Dictionary")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("")]
        [SampleUsage("**key1** or **{{{vKeyName}}}**")]
        [Remarks("If it is empty, it will be the value of Current Position, which can be used for Loop List command.")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.TextBox)]
        [PropertyTextBoxSetting(1, false)]
        [PropertyIsOptional(true, "Current Position")]
        [PropertyDisplayText(true, "Key")]
        public string v_Key { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please indicate the variable to apply result")]
        [InputSpecification("Enter a unique dataset name that will be used later to traverse over the data")]
        [SampleUsage("**vMyData** or **{{{myData}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsVariablesList(true)]
        [PropertyValidationRule("Result", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "Result")]
        public string v_OutputVariable { get; set; }

        public GetDictionaryValueCommand()
        {
            this.CommandName = "GetDictionaryValueCommand";
            this.SelectionName = "Get Dictionary Item";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            //Retrieve Dictionary by name
            var engine = (Engine.AutomationEngineInstance)sender;
            
            Dictionary<string, string> dic = v_InputData.GetDictionaryVariable(engine);

            string vKey = "";
            if (String.IsNullOrEmpty(v_Key))
            {
                var variable = v_InputData.GetRawVariable(engine);
                int pos = variable.CurrentPosition;
                string[] keys = dic.Keys.ToArray();
                if ((pos >= 0) && (pos < keys.Length))
                {
                    vKey = keys[pos];
                }
                else
                {
                    throw new Exception("Strange Current Position value in Dictionary " + pos);
                }
            }
            else
            {
                vKey = v_Key.ConvertToUserVariable(sender);
            }

            if (dic.ContainsKey(vKey))
            {
                dic[vKey].StoreInUserVariable(engine, v_OutputVariable);
            }
            else
            {
                throw new Exception("Key " + v_Key + " does not exists in the Dictionary");
            }
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
        //    return base.GetDisplayValue() + $" [From: {v_InputData}, Get: {v_Key}, Store In: {v_OutputVariable}]";
        //}

        //public override bool IsValidate(frmCommandEditor editor)
        //{
        //    base.IsValidate(editor);

        //    if (String.IsNullOrEmpty(v_OutputVariable))
        //    {
        //        this.IsValid = false;
        //        this.validationResult += "Variable is empty.\n";
        //    }
        //    if (String.IsNullOrEmpty(v_InputData))
        //    {
        //        this.IsValid = false;
        //        this.validationResult += "Dictionary Variable Name is empty.\n";
        //    }
        //    if (String.IsNullOrEmpty(v_Key))
        //    {
        //        this.IsValid = false;
        //        this.validationResult += "Key is empty.\n";
        //    }

        //    return this.IsValid;
        //}
    }
}