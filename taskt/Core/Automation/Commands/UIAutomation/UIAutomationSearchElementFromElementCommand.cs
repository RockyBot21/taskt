﻿using System;
using System.Xml.Serialization;
using System.Data;
using taskt.Core.Automation.Attributes.PropertyAttributes;
using System.Windows.Forms;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("UIAutomation Commands")]
    [Attributes.ClassAttributes.SubGruop("Search Element")]
    [Attributes.ClassAttributes.CommandSettings("Search Element From Element")]
    [Attributes.ClassAttributes.Description("This command allows you to get AutomationElement from AutomationElement.")]
    [Attributes.ClassAttributes.ImplementationDescription("Use this command when you want to get AutomationElement from AutomationElement. Search for Descendants Elements.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class UIAutomationSearchElementFromElementCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_InputAutomationElementName))]
        [PropertyDescription("AutomationElement Variable Name to Search")]
        public string v_TargetElement { get; set; }

        [XmlElement]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_SearchParameters))]
        public DataTable v_SearchParameters { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_NewOutputAutomationElementName))]
        public string v_AutomationElementVariable { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_WaitTime))]
        public string v_WaitTime { get; set; }

        public UIAutomationSearchElementFromElementCommand()
        {
            //this.CommandName = "UIAutomationGetElementFromElementCommand";
            //this.SelectionName = "Get Element From Element";
            //this.CommandEnabled = true;
            //this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var elem = AutomationElementControls.SearchGUIElement(this, engine);
            elem.StoreInUserVariable(engine, v_AutomationElementVariable);
        }

        public override void AfterShown()
        {
            //AutomationElementControls.RenderSearchParameterDataGridView((DataGridView)ControlsList[nameof(v_SearchParameters)]);
            AutomationElementControls.RenderSearchParameterDataGridView(ControlsList.GetPropertyControl<DataGridView>(nameof(v_SearchParameters)));
        }
    }
}