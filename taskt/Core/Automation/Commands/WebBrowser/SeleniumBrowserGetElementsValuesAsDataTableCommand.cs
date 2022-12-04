﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using taskt.UI.CustomControls;
using taskt.UI.Forms;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("Web Browser Commands")]
    [Attributes.ClassAttributes.SubGruop("Scraping")]
    [Attributes.ClassAttributes.Description("This command allows you to get Attributes value for Elements As DataTable.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to get Attributes value for Elements As DataTable.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    public class SeleniumBrowserGetElementsValuesAsDataTableCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Please Enter the instance name")]
        [InputSpecification("Enter the unique instance name that was specified in the **Create Browser** command")]
        [SampleUsage("**myInstance** or **{{{vInstance}}}**")]
        [Remarks("Failure to enter the correct instance name or failure to first call **Create Browser** command will cause an error")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyInstanceType(PropertyInstanceType.InstanceType.WebBrowser)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyFirstValue("%kwd_default_browser_instance%")]
        [PropertyValidationRule("Instance Name", PropertyValidationRule.ValidationRuleFlags.Empty)]
        public string v_InstanceName { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please Specify Element Search Method")]
        [PropertyUISelectionOption("Find Element By XPath")]
        [PropertyUISelectionOption("Find Element By ID")]
        [PropertyUISelectionOption("Find Element By Name")]
        [PropertyUISelectionOption("Find Element By Tag Name")]
        [PropertyUISelectionOption("Find Element By Class Name")]
        [PropertyUISelectionOption("Find Element By CSS Selector")]
        [PropertyUISelectionOption("Find Element By Link Text")]
        [PropertyUISelectionOption("Find Elements By XPath")]
        [PropertyUISelectionOption("Find Elements By ID")]
        [PropertyUISelectionOption("Find Elements By Name")]
        [PropertyUISelectionOption("Find Elements By Tag Name")]
        [PropertyUISelectionOption("Find Elements By Class Name")]
        [PropertyUISelectionOption("Find Elements By CSS Selector")]
        [PropertyUISelectionOption("Find Elements By Link Text")]
        [InputSpecification("Select the specific search type that you want to use to isolate the element in the web page.")]
        [SampleUsage("Select **Find Element By XPath**, **Find Element By ID**, **Find Element By Name**, **Find Element By Tag Name**, **Find Element By Class Name**, **Find Element By CSS Selector**, **Find Element By Link Text**")]
        [Remarks("")]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.ComboBox)]
        [PropertyValidationRule("Search Method", PropertyValidationRule.ValidationRuleFlags.Empty)]
        public string v_SeleniumSearchType { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please Specify Element Search Parameter")]
        [InputSpecification("Specifies the parameter text that matches to the element based on the previously selected search type.")]
        [SampleUsage("If search type **Find Element By ID** was specified, for example, given <div id='name'></div>, the value of this field would be **name**")]
        [Remarks("")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyValidationRule("Search Parameter", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyTextBoxSetting(1, false)]
        public string v_SeleniumSearchParameter { get; set; }

        [XmlElement]
        [PropertyDescription("Please specify Attributes Name to Get")]
        [InputSpecification("")]
        [SampleUsage("**id** or **Text** or **textContent** or **{{{vAttribute}}}**")]
        [Remarks("")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyValidationRule("Attribute", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyRecommendedUIControl(PropertyRecommendedUIControl.RecommendeUIControlType.DataGridView)]
        [PropertyDataGridViewSetting(true, true, true)]
        [PropertyDataGridViewColumnSettings("AttributeName", "Attribute Name")]
        [PropertyDataGridViewCellEditEvent("AttributesNameGridHelper_CellClick", PropertyDataGridViewCellEditEvent.DataGridViewCellEvent.CellClick)]
        [PropertyControlIntoCommandField("AttributesNameGridHelper")]
        public DataTable v_AttributesName { get; set; }

        [XmlAttribute]
        [PropertyDescription("Please Specify DataTable Variable Name to store result")]
        [InputSpecification("")]
        [SampleUsage("")]
        [Remarks("")]
        [PropertyIsVariablesList(true)]
        [PropertyParameterDirection(PropertyParameterDirection.ParameterDirection.Output)]
        [PropertyInstanceType(PropertyInstanceType.InstanceType.DataTable)]
        [PropertyValidationRule("DataTable Variable", PropertyValidationRule.ValidationRuleFlags.Empty)]
        public string v_DataTableVariableName { get; set; }

        [XmlIgnore]
        [NonSerialized]
        private DataGridView AttributesNameGridHelper;

        public SeleniumBrowserGetElementsValuesAsDataTableCommand()
        {
            this.CommandName = "SeleniumBrowserGetElementsValuesAsDataTableCommand";
            this.SelectionName = "Get Elements Values As DataTable";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            var seleniumInstance = SeleniumControls.getWebBrowserInstance(engine, v_InstanceName.ConvertToUserVariable(engine));

            string searchMethod = v_SeleniumSearchType.GetUISelectionValue("v_SeleniumSearchType", this, engine);
            string seleniumSearchParam = v_SeleniumSearchParameter.ConvertToUserVariable(sender);
            var elements = SeleniumControls.findElement(seleniumInstance, seleniumSearchParam, searchMethod);

            DataTable newDT = new DataTable();

            int rows = v_AttributesName.Rows.Count;
            var attrList = new List<string>();
            for (int i = 0; i < rows; i++)
            {
                string attrName = (v_AttributesName.Rows[i][0] == null) ? "" : v_AttributesName.Rows[i][0].ToString();
                if (attrName != "")
                {
                    attrList.Add(attrName);
                }
            }
            foreach(string attr in attrList)
            {
                newDT.Columns.Add(attr);
            }

            if (elements is IWebElement)
            {
                newDT.Rows.Add();
                var trgElem = (IWebElement)elements;
                for (int i = 0; i < attrList.Count; i++)
                {
                    newDT.Rows[0][i] = SeleniumControls.getAttribute(trgElem, attrList[i]);
                }
            }
            else
            {
                var trgElems = (ReadOnlyCollection<IWebElement>)elements;
                for (int i = 0; i < trgElems.Count; i++)
                {
                    newDT.Rows.Add();
                    for (int j = 0; j < attrList.Count; j++)
                    {
                        newDT.Rows[i][j] = SeleniumControls.getAttribute(trgElems[i], attrList[j]);
                    }
                }
            }

            newDT.StoreInUserVariable(engine, v_DataTableVariableName);
        }

        private void AttributesNameGridHelper_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0)
            {
                AttributesNameGridHelper.BeginEdit(false);
            }
        }
        public override void BeforeValidate()
        {
            base.BeforeValidate();
            //if (AttributesNameGridHelper.IsCurrentCellDirty || AttributesNameGridHelper.IsCurrentRowDirty)
            //{
            //    AttributesNameGridHelper.CommitEdit(DataGridViewDataErrorContexts.Commit);
            //    var newRow = v_AttributesName.NewRow();
            //    v_AttributesName.Rows.Add(newRow);
            //    for (var i = v_AttributesName.Rows.Count - 1; i >= 0; i--)
            //    {
            //        if (v_AttributesName.Rows[i][0].ToString() == "")
            //        {
            //            v_AttributesName.Rows[i].Delete();
            //        }
            //    }
            //}
            DataTableControls.BeforeValidate((DataGridView)ControlsList[nameof(v_AttributesName)], v_AttributesName);
        }

        public override List<Control> Render(frmCommandEditor editor)
        {
            base.Render(editor);

            var ctrls = CommandControls.MultiCreateInferenceDefaultControlGroupFor(this, editor);
            RenderedControls.AddRange(ctrls);

            return RenderedControls;
        }

        public override string GetDisplayValue()
        {
            return base.GetDisplayValue() + " [Get " + v_SeleniumSearchType + " element " + v_AttributesName.Rows.Count + " Attributes to store " + v_DataTableVariableName + ", Instance Name: '" + v_InstanceName + "']";
        }
    }
}