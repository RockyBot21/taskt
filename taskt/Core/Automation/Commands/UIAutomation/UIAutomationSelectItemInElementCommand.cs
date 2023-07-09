﻿using System;
using System.Xml.Serialization;
using System.Windows.Automation;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{

    [Serializable]
    [Attributes.ClassAttributes.Group("UIAutomation Commands")]
    [Attributes.ClassAttributes.SubGruop("Element Action")]
    [Attributes.ClassAttributes.CommandSettings("Select Item In Element")]
    [Attributes.ClassAttributes.Description("This command allows you to Select a Item in AutomationElement.")]
    [Attributes.ClassAttributes.ImplementationDescription("Use this command when you want to Select a Item in AutomationElement.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class UIAutomationSelectItemInElementCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_InputAutomationElementName))]
        public string v_TargetElement { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(GeneralPropertyControls), nameof(GeneralPropertyControls.v_DisallowNewLine_OneLineTextBox))]
        [PropertyDescription("Item Value to Select")]
        [InputSpecification("Item Value", true)]
        [PropertyDetailSampleUsage("**Yes**", PropertyDetailSampleUsage.ValueType.Value)]
        [PropertyDetailSampleUsage("**Hello**", PropertyDetailSampleUsage.ValueType.Value)]
        [PropertyDetailSampleUsage("**{{{vItem}}}**", PropertyDetailSampleUsage.ValueType.VariableValue)]
        [PropertyDisplayText(true, "Item")]
        public string v_Item { get; set; }

        public UIAutomationSelectItemInElementCommand()
        {
            //this.CommandName = "UIAutomationSelectItemInElementCommand";
            //this.SelectionName = "Select Item In Element";
            //this.CommandEnabled = true;
            //this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var targetElement = v_TargetElement.GetAutomationElementVariable(engine);

            var itemName = v_Item.ConvertToUserVariable(engine);

            var items = AutomationElementControls.GetSelectionItems(targetElement, true);
            bool isSelected = false;
            foreach(var item in items)
            {
                if (item.Current.Name == itemName)
                {
                    SelectionItemPattern selPtn = (SelectionItemPattern)item.GetCurrentPattern(SelectionItemPattern.Pattern);
                    selPtn.Select();
                    isSelected = true;
                    break;
                }
            }

            if (!isSelected)
            {
                throw new Exception("Item '" + v_Item + "' does not exists");
            }
        }
    }
}