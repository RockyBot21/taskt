﻿using System;
using System.Data;
using System.Xml.Serialization;
using taskt.Core.Automation.Attributes.PropertyAttributes;
using taskt.UI.Forms;
using System.Windows.Forms;
using taskt.UI.CustomControls;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("UIAutomation Commands")]
    [Attributes.ClassAttributes.SubGruop("Search Element & Window")]
    [Attributes.ClassAttributes.CommandSettings("Search Element And Window")]
    [Attributes.ClassAttributes.Description("This command allows you to get AutomationElement from Window Name using by XPath.")]
    [Attributes.ClassAttributes.ImplementationDescription("Use this command when you want to get AutomationElement from Window Name. XPath does not support to use parent and sibling for root element.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class UIAutomationSearchElementAndWindowCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowNameControls), nameof(WindowNameControls.v_WindowName))]
        public string v_WindowName { get; set; }

        [XmlElement]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_SearchParameters))]
        public DataTable v_SearchParameters { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_OutputAutomationElementName))]
        public string v_AutomationElementVariable { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowNameControls), nameof(WindowNameControls.v_CompareMethod))]
        public string v_SearchMethod { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowNameControls), nameof(WindowNameControls.v_MatchMethod_Single))]
        [PropertySelectionChangeEvent(nameof(MatchMethodComboBox_SelectionChangeCommitted))]
        public string v_MatchMethod { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowNameControls), nameof(WindowNameControls.v_TargetWindowIndex))]
        public string v_TargetWindowIndex { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(WindowNameControls), nameof(WindowNameControls.v_WaitTime))]
        public string v_WindowWaitTime { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_WaitTime))]
        public string v_ElementWaitTime { get; set; }

        public UIAutomationSearchElementAndWindowCommand()
        {
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var winElem = AutomationElementControls.GetWindowAutomationElement(this, engine);

            var waitTile = this.ConvertToUserVariableAsInteger(nameof(v_ElementWaitTime), engine);

            var elem = AutomationElementControls.SearchGUIElement(winElem, v_SearchParameters, waitTile, engine);

            elem.StoreInUserVariable(engine, v_AutomationElementVariable);
        }

        private void MatchMethodComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            WindowNameControls.MatchMethodComboBox_SelectionChangeCommitted(ControlsList, (ComboBox)sender, nameof(v_TargetWindowIndex));
        }

        public override void Refresh(frmCommandEditor editor)
        {
            base.Refresh();
            var cmb = (ComboBox)ControlsList[nameof(v_WindowName)];
            cmb.AddWindowNames();
        }
    }
}