﻿using System;
using System.Xml.Serialization;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{

    [Serializable]
    [Attributes.ClassAttributes.Group("UIAutomation Commands")]
    [Attributes.ClassAttributes.SubGruop("Search Element")]
    [Attributes.ClassAttributes.CommandSettings("Check Element Exist By XPath")]
    [Attributes.ClassAttributes.Description("This command allows you to check AutomationElement existence.")]
    [Attributes.ClassAttributes.ImplementationDescription("Use this command when you want to check AutomationElement existence.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class UIAutomationCheckElementExistByXPathCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_InputAutomationElementName))]
        public string v_TargetElement { get; set; }

        [XmlElement]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_XPath))]
        public string v_SearchXPath { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(BooleanControls), nameof(BooleanControls.v_Result))]
        [Remarks("When the Element exists, Result value is **True**")]
        public string v_Result { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_WaitTime))]
        [PropertyIsOptional(true, "0")]
        [PropertyFirstValue("0")]
        public string v_WaitTime { get; set; }

        public UIAutomationCheckElementExistByXPathCommand()
        {
            //this.CommandName = "UIAutomationCheckElementExistByXPathCommand";
            //this.SelectionName = "Check Element Exist By XPath";
            //this.CommandEnabled = true;
            //this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            try
            {
                AutomationElementControls.SearchGUIElementByXPath(this, engine);
                true.StoreInUserVariable(engine, v_Result);
            }
            catch
            {
                false.StoreInUserVariable(engine, v_Result);
            }
        }
    }
}