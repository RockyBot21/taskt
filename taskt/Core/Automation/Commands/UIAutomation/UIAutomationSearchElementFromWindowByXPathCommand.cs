﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Windows.Automation;
using taskt.Core.Automation.Attributes.PropertyAttributes;
using System.Xml.Linq;
using System.Xml.XPath;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("UIAutomation Commands")]
    [Attributes.ClassAttributes.SubGruop("Search")]
    [Attributes.ClassAttributes.CommandSettings("Search Element From Window By XPath")]
    [Attributes.ClassAttributes.Description("This command allows you to get AutomationElement from Window Name using by XPath.")]
    [Attributes.ClassAttributes.ImplementationDescription("Use this command when you want to get AutomationElement from Window Name. XPath does not support to use parent and sibling for root element.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class UIAutomationSearchElementFromWindowByXPathCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Please specify Window Name")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("")]
        [SampleUsage("**{{{vElement}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyIsWindowNamesList(true)]
        [PropertyValidationRule("Window Name", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "Window Name")]
        public string v_WindowName { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please select Window name search method")]
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

        [XmlElement]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_XPath))]
        public string v_SearchXPath { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(AutomationElementControls), nameof(AutomationElementControls.v_OutputAutomationElementName))]
        public string v_AutomationElementVariable { get; set; }

        public UIAutomationSearchElementFromWindowByXPathCommand()
        {
            //this.CommandName = "UIAutomationGetElementFromWindowByXPathCommand";
            //this.SelectionName = "Get Element From Window By XPath";
            //this.CommandEnabled = true;
            //this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            string searchMethod = v_SearchMethod.GetUISelectionValue("v_SearchMethod", this, engine);
            string windowName = WindowNameControls.GetMatchedWindowName(v_WindowName, searchMethod, engine);
            var rootElement = AutomationElementControls.GetFromWindowName(windowName, engine);

            if (rootElement == null)
            {
                throw new Exception("Window Name '" + v_WindowName + "' not found");
            }

            Dictionary<string, AutomationElement> dic;
            XElement xml = AutomationElementControls.GetElementXml(rootElement, out dic);

            string xpath = v_SearchXPath.ConvertToUserVariable(engine);
            if (!xpath.StartsWith("."))
            {
                xpath = "." + xpath;
            }

            XElement resElem = xml.XPathSelectElement(xpath);

            if (resElem == null)
            {
                throw new Exception("AutomationElement not found XPath: " + v_SearchXPath);
            }

            AutomationElement res = dic[resElem.Attribute("Hash").Value];
            res.StoreInUserVariable(engine, v_AutomationElementVariable);
        }
    }
}