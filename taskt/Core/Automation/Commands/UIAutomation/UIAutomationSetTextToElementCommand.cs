﻿using System;
using System.Xml.Serialization;
using System.Windows.Automation;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{

    [Serializable]
    [Attributes.ClassAttributes.Group("UIAutomation Commands")]
    [Attributes.ClassAttributes.SubGruop("Element Action")]
    [Attributes.ClassAttributes.CommandSettings("Set Text To Element")]
    [Attributes.ClassAttributes.Description("This command allows you to set Text Value from AutomationElement.")]
    [Attributes.ClassAttributes.ImplementationDescription("Use this command when you want to set Text Value from AutomationElement.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class UIAutomationSetTextToElementCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_InputAutomationElementName))]
        public string v_TargetElement { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(GeneralPropertyControls), nameof(GeneralPropertyControls.v_OneLineTextBox))]
        [PropertyDescription("Text to Set")]
        [InputSpecification("Text", true)]
        [PropertyDetailSampleUsage("**Hello**", PropertyDetailSampleUsage.ValueType.Value)]
        [PropertyDetailSampleUsage("**{{{vText}}}**", PropertyDetailSampleUsage.ValueType.VariableValue)]
        [PropertyDisplayText(true, "Text")]
        public string v_TextVariable { get; set; }

        public UIAutomationSetTextToElementCommand()
        {
            //this.CommandName = "UIAutomationSetTextToElementCommand";
            //this.SelectionName = "Set Text To Element";
            //this.CommandEnabled = true;
            //this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var targetElement = v_TargetElement.GetAutomationElementVariable(engine);

            string textValue = v_TextVariable.ConvertToUserVariable(sender);

            if (targetElement.TryGetCurrentPattern(ValuePattern.Pattern, out object textPtn))
            {
                ((ValuePattern)textPtn).SetValue(textValue);
            }
            else
            {
                throw new Exception("AutomationElement '" + v_TargetElement + "' can not set Text");
            }
        }
    }
}