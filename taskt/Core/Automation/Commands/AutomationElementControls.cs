﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Xml.Linq;
using System.Xml.XPath;

namespace taskt.Core.Automation.Commands
{
    internal class AutomationElementControls
    {
        public static AutomationElement GetFromWindowName(string windowName)
        {
            var windowElement = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, windowName));
            if (windowElement == null)
            {
                // more deep search
                windowElement = AutomationElement.RootElement.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, windowName));
            }

            if (windowElement == null)
            {
                throw new Exception("Window Name '" + windowName + "' not found AutomationElement");
            }
            else
            {
                return windowElement;
            }
        }

        public static void CreateEmptyParamters(DataTable table)
        {
            table.Rows.Clear();
            table.Rows.Add(false, "AcceleratorKey", "");
            table.Rows.Add(false, "AccessKey", "");
            table.Rows.Add(false, "AutomationId", "");
            table.Rows.Add(false, "ControlType", "");
            table.Rows.Add(false, "ClassName", "");
            table.Rows.Add(false, "FrameworkId", "");
            table.Rows.Add(false, "HasKeyboardFocus", "");
            table.Rows.Add(false, "HelpText", "");
            table.Rows.Add(false, "IsContentElement", "");
            table.Rows.Add(false, "IsControlElement", "");
            table.Rows.Add(false, "IsEnabled", "");
            table.Rows.Add(false, "IsKeyboardFocusable", "");
            table.Rows.Add(false, "IsOffscreen", "");
            table.Rows.Add(false, "IsPassword", "");
            table.Rows.Add(false, "IsRequiredForForm", "");
            table.Rows.Add(false, "ItemStatus", "");
            table.Rows.Add(false, "ItemType", "");
            table.Rows.Add(false, "LocalizedControlType", "");
            table.Rows.Add(false, "Name", "");
            table.Rows.Add(false, "NativeWindowHandle", "");
            table.Rows.Add(false, "ProcessID", "");
        }

        private static void parseInspectToolResult(string result, DataTable table, System.Windows.Forms.ComboBox windowNames = null)
        {
            string[] results = result.Split(new[] { "\r\n" }, StringSplitOptions.None);

            if ((results.Length >= 1) && (result != ""))
            {
                CreateEmptyParamters(table);

                List<string> ancestors = new List<string>();
                string currentParam = "";
                foreach (string res in results)
                {
                    string[] spt = res.Split('\t');
                    string value = (spt.Length >= 2) ? spt[1] : "";
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    if (spt[0] != "")
                    {
                        string name = spt[0].Substring(0, spt[0].Length - 1);
                        currentParam = name;

                        switch (name)
                        {
                            case "AcceleratorKey":
                            case "AccessKey":
                            case "AutomationId":
                            case "ClassName":
                            case "FrameworkId":
                            case "HasKeyboardFocus":
                            case "HelpText":
                            case "IsContentElement":
                            case "IsControlElement":
                            case "IsEnabled":
                            case "IsKeyboardFocusable":
                            case "IsOffscreen":
                            case "IsPassword":
                            case "IsRequiredForForm":
                            case "ItemStatus":
                            case "ItemType":
                            case "LocalizedControlType":
                            case "Name":
                            case "NativeWindowHandle":
                            case "ProcessID":
                                DataTableControls.SetParameterValue(table, value, name, "ParameterName", "ParameterValue");
                                break;

                            case "ControlType":
                                DataTableControls.SetParameterValue(table, parseControlTypeInspectToolResult(value), name, "ParameterName", "ParameterValue");
                                break;

                            case "Ancestors":
                                ancestors.Add(value);
                                break;
                        }
                    }
                    else
                    {
                        if (currentParam == "Ancestors")
                        {
                            ancestors.Add(value);
                        }
                    }
                    if (windowNames != null)
                    {
                        setComboBoxWindowNameFromInspectAncestors(ancestors, windowNames);
                    }
                }
            }
            else
            {
                var f = new UI.Forms.Supplemental.frmDialog("No Inspect Tool Results", "Fail Parse", UI.Forms.Supplemental.frmDialog.DialogType.OkOnly, 0);
                f.ShowDialog();
            }
        }

        public static void InspectToolParserClicked(DataTable table, System.Windows.Forms.ComboBox windowNames = null)
        {
            using (UI.Forms.Supplement_Forms.frmInspectParser frm = new UI.Forms.Supplement_Forms.frmInspectParser())
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    parseInspectToolResult(frm.inspectResult, table, windowNames);
                }
            }
        }

        private static string parseControlTypeInspectToolResult(string value)
        {
            var spt = value.Split(' ');
            return spt[0].Replace("UIA_", "").Replace("ControlTypeId", "");
        }
        private static void setComboBoxWindowNameFromInspectAncestors(List<string> ancestors, System.Windows.Forms.ComboBox cmb)
        {
            if (ancestors.Count > 0)
            {
                string[] windows = new string[cmb.Items.Count];
                cmb.Items.CopyTo(windows, 0);

                bool isFound = false;
                foreach (string ancestor in ancestors)
                {
                    // get quoted " " text
                    int pos = ancestor.IndexOf("\"");
                    if (pos < 0)
                    {
                        continue;
                    }

                    string winName = ancestor.Substring(pos + 1);
                    pos = winName.IndexOf("\"");
                    if (pos < 0)
                    {
                        continue;
                    }
                    winName = winName.Substring(0, pos);

                    foreach (string win in windows)
                    {
                        if (winName == win)
                        {
                            cmb.Text = winName;
                            isFound = true;
                            break;
                        }
                    }
                    if (isFound)
                    {
                        break;
                    }
                }
            }
        }

        //private static void SetControlTypeCell(System.Windows.Forms.DataGridView dgv)
        //{
        //    if ((dgv.Columns[1].Name != "Parameter Name") || (dgv.Columns[2].Name != "Parameter Value"))
        //    {
        //        throw new Exception("DataGridView is not UIAutomation SearchParameters");
        //    }

        //    int row = -1;
        //    for (int i = 0; i < dgv.Rows.Count; i++)
        //    {
        //        string paramValue = (dgv.Rows[i].Cells[1].Value != null) ? dgv.Rows[i].Cells[i].Value.ToString() : "";
        //        if (paramValue == "ControlType")
        //        {
        //            row = i;
        //            break;
        //        }
        //    }
        //    if (row < 0)
        //    {
        //        throw new Exception("DataGridView does not have 'ControlType' row");
        //    }

        //    System.Windows.Forms.DataGridViewComboBoxCell cmbCell = new System.Windows.Forms.DataGridViewComboBoxCell();
        //    cmbCell.Items.AddRange(new string[]
        //    {
        //        "Button", "Calendar", "CheckBox", "ComboBox", "Custom",
        //        "DataGrid", "DataItem", "Document", "Edit", "Group",
        //        "Header", "HeaderItem", "Hyperlink", "Image", "List",
        //        "ListItem", "Menu", "MenuBar", "MenuItem", "Pane",
        //        "ProgressBar", "RadioButton", "ScrollBar", "Separator", "Slider",
        //        "Spinner", "SplitButton", "StatusBar", "Tab", "TabItem",
        //        "Table", "Text", "Thumb", "TitleBar", "ToolBar",
        //        "ToolTip", "Tree", "TreeItem", "Window"
        //    });
        //    dgv.Rows[row].Cells[2] = cmbCell;
        //}

        private static ControlType GetControlType(string controlTypeName)
        {
            switch (controlTypeName.ToLower())
            {
                case "button":
                    return ControlType.Button;
                case "calender":
                    return ControlType.Calendar;
                case "checkbox":
                    return ControlType.CheckBox;
                case "combobox":
                    return ControlType.ComboBox;
                case "custom":
                    return ControlType.Custom;
                case "datagrid":
                    return ControlType.DataGrid;
                case "dataitem":
                    return ControlType.DataItem;
                case "document":
                    return ControlType.Document;
                case "edit":
                    return ControlType.Edit;
                case "group":
                    return ControlType.Group;
                case "header":
                    return ControlType.Header;
                case "headeritem":
                    return ControlType.HeaderItem;
                case "hyperlink":
                    return ControlType.Hyperlink;
                case "image":
                    return ControlType.Image;
                case "list":
                    return ControlType.List;
                case "listitem":
                    return ControlType.ListItem;
                case "menu":
                    return ControlType.Menu;
                case "menubar":
                    return ControlType.MenuBar;
                case "menuitem":
                    return ControlType.MenuItem;
                case "pane":
                    return ControlType.Pane;
                case "progressbar":
                    return ControlType.ProgressBar;
                case "radiobutton":
                    return ControlType.RadioButton;
                case "scrollbar":
                    return ControlType.ScrollBar;
                case "separator":
                    return ControlType.Separator;
                case "slider":
                    return ControlType.Slider;
                case "spinner":
                    return ControlType.Spinner;
                case "splitbutton":
                    return ControlType.SplitButton;
                case "statusbar":
                    return ControlType.StatusBar;
                case "tab":
                    return ControlType.Tab;
                case "tabitem":
                    return ControlType.TabItem;
                case "table":
                    return ControlType.Table;
                case "text":
                    return ControlType.Text;
                case "thumb":
                    return ControlType.Thumb;
                case "titlebar":
                    return ControlType.TitleBar;
                case "toolbar":
                    return ControlType.ToolBar;
                case "tooltip":
                    return ControlType.ToolTip;
                case "tree":
                    return ControlType.Tree;
                case "treeitem":
                    return ControlType.TreeItem;
                case "window":
                    return ControlType.Window;
                default:
                    throw new Exception("Strange ControlType '" + controlTypeName + "'");
                    break;
            }
        }

        public static string GetControlTypeText(ControlType control)
        {
            if (control == ControlType.Button)
            {
                return "Button";
            }
            else if (control == ControlType.Calendar)
            {
                return "Calender";
            }
            else if (control == ControlType.CheckBox)
            {
                return "CheckBox";
            }
            else if (control == ControlType.ComboBox)
            {
                return "ComboBox";
            }
            else if (control == ControlType.Custom)
            {
                return "Custom";
            }
            else if (control == ControlType.DataGrid)
            {
                return "DataGrid";
            }
            else if (control == ControlType.DataItem)
            {
                return "DataItem";
            }
            else if (control == ControlType.Document)
            {
                return "Document";
            }
            else if (control == ControlType.Edit)
            {
                return "Edit";
            }
            else if (control == ControlType.Group)
            {
                return "Group";
            }
            else if (control == ControlType.Header)
            {
                return "Header";
            }
            else if (control == ControlType.HeaderItem)
            {
                return "HeaderItem";
            }
            else if (control == ControlType.Hyperlink)
            {
                return "Hyperlink";
            }
            else if (control == ControlType.Image)
            {
                return "Image";
            }
            else if (control == ControlType.List)
            {
                return "List";
            }
            else if (control == ControlType.ListItem)
            {
                return "ListItem";
            }
            else if (control == ControlType.Menu)
            {
                return "Menu";
            }
            else if (control == ControlType.MenuBar)
            {
                return "MenuBar";
            }
            else if (control == ControlType.MenuItem)
            {
                return "MenuItem";
            }
            else if (control == ControlType.Pane)
            {
                return "Pane";
            }
            else if (control == ControlType.ProgressBar)
            {
                return "ProgressBar";
            }
            else if (control == ControlType.RadioButton)
            {
                return "RadioButton";
            }
            else if (control == ControlType.ScrollBar)
            {
                return "ScrollBar";
            }
            else if (control == ControlType.Separator)
            {
                return "Separator";
            }
            else if (control == ControlType.Slider)
            {
                return "Slider";
            }
            else if (control == ControlType.Spinner)
            {
                return "Spinner";
            }
            else if (control == ControlType.SplitButton)
            {
                return "SplitButton";
            }
            else if (control == ControlType.StatusBar)
            {
                return "StatusBar";
            }
            else if (control == ControlType.Tab)
            {
                return "Tab";
            }
            else if (control == ControlType.TabItem)
            {
                return "TabItem";
            }
            else if (control == ControlType.Table)
            {
                return "Table";
            }
            else if (control == ControlType.Text)
            {
                return "Text";
            }
            else if (control == ControlType.Thumb)
            {
                return "Thumb";
            }
            else if (control == ControlType.TitleBar)
            {
                return "TitleBar";
            }
            else if (control == ControlType.ToolBar)
            {
                return "ToolBar";
            }
            else if (control == ControlType.ToolTip)
            {
                return "ToolTip";
            }
            else if (control == ControlType.Tree)
            {
                return "Tree";
            }
            else if (control == ControlType.TreeItem)
            {
                return "TreeItem";
            }
            else if (control == ControlType.Window)
            {
                return "Window";
            }
            else
            {
                throw new Exception("Strange ControlType");
            }
        }

        private static PropertyCondition CreatePropertyCondition(string propertyName, object propertyValue)
        {
            string propName = propertyName + "Property";

            switch (propertyName)
            {
                case "AcceleratorKey":
                    return new PropertyCondition(AutomationElement.AcceleratorKeyProperty, propertyValue);
                case "AccessKey":
                    return new PropertyCondition(AutomationElement.AccessKeyProperty, propertyValue);
                case "AutomationId":
                    return new PropertyCondition(AutomationElement.AutomationIdProperty, propertyValue);
                case "ClassName":
                    return new PropertyCondition(AutomationElement.ClassNameProperty, propertyValue);
                case "ControlType":
                    return new PropertyCondition(AutomationElement.ControlTypeProperty, GetControlType((string)propertyValue));
                case "FrameworkId":
                    return new PropertyCondition(AutomationElement.FrameworkIdProperty, propertyValue);
                case "HasKeyboardFocus":
                    return new PropertyCondition(AutomationElement.HasKeyboardFocusProperty, propertyValue);
                case "HelpText":
                    return new PropertyCondition(AutomationElement.HelpTextProperty, propertyValue);
                case "IsContentElement":
                    return new PropertyCondition(AutomationElement.IsContentElementProperty, propertyValue);
                case "IsControlElement":
                    return new PropertyCondition(AutomationElement.IsControlElementProperty, propertyValue);
                case "IsEnabled":
                    return new PropertyCondition(AutomationElement.IsEnabledProperty, propertyValue);
                case "IsKeyboardFocusable":
                    return new PropertyCondition(AutomationElement.IsKeyboardFocusableProperty, propertyValue);
                case "IsOffscreen":
                    return new PropertyCondition(AutomationElement.IsOffscreenProperty, propertyValue);
                case "IsPassword":
                    return new PropertyCondition(AutomationElement.IsPasswordProperty, propertyValue);
                case "IsRequiredForForm":
                    return new PropertyCondition(AutomationElement.IsRequiredForFormProperty, propertyValue);
                case "ItemStatus":
                    return new PropertyCondition(AutomationElement.ItemStatusProperty, propertyValue);
                case "ItemType":
                    return new PropertyCondition(AutomationElement.ItemTypeProperty, propertyValue);
                case "LocalizedControlType":
                    return new PropertyCondition(AutomationElement.LocalizedControlTypeProperty, propertyValue);
                case "Name":
                    return new PropertyCondition(AutomationElement.NameProperty, propertyValue);
                case "NativeWindowHandle":
                    return new PropertyCondition(AutomationElement.NativeWindowHandleProperty, propertyValue);
                case "ProcessID":
                    return new PropertyCondition(AutomationElement.ProcessIdProperty, propertyValue);
                default:
                    throw new NotImplementedException("Property Type '" + propertyName + "' not implemented");
            }
        }

        private static Condition CreateSearchCondition(DataTable table, taskt.Core.Automation.Engine.AutomationEngineInstance engine)
        {  
            //create search params
            var searchParams = from rw in table.AsEnumerable()
                                where rw.Field<string>("Enabled") == "True"
                                select rw;

            if (searchParams.Count() == 0)
            {
                return null;
            }

            //create and populate condition list
            var conditionList = new List<Condition>();
            foreach (var param in searchParams)
            {
                var parameterName = (string)param["ParameterName"];
                var parameterValue = (string)param["ParameterValue"];

                parameterName = parameterName.ConvertToUserVariable(engine);
                parameterValue = parameterValue.ConvertToUserVariable(engine);

                PropertyCondition propCondition;
                if (bool.TryParse(parameterValue, out bool bValue))
                {
                    propCondition = CreatePropertyCondition(parameterName, bValue);
                }
                else
                {
                    propCondition = CreatePropertyCondition(parameterName, parameterValue);
                }
                conditionList.Add(propCondition);
            }

            //concatenate or take first condition
            Condition searchConditions;
            if (conditionList.Count > 1)
            {
                searchConditions = new AndCondition(conditionList.ToArray());

            }
            else
            {
                searchConditions = conditionList[0];
            }

            return searchConditions;
        }

        public static AutomationElement SearchGUIElement(AutomationElement rootElement, DataTable conditionTable, taskt.Core.Automation.Engine.AutomationEngineInstance engine)
        {
            Condition searchConditions = CreateSearchCondition(conditionTable, engine);

            var element = rootElement.FindFirst(TreeScope.Descendants, searchConditions);
            if (element == null)
            {
                // more deep search
                element = rootElement.FindFirst(TreeScope.Subtree, searchConditions);
                if (element == null)
                {
                    element = DeepSearchGUIElement(rootElement, searchConditions);
                }
            }
            // if element not found, don't throw exception here
            return element;
        }

        private static AutomationElement DeepSearchGUIElement(AutomationElement rootElement, Condition searchCondition)
        {
            TreeWalker walker = TreeWalker.RawViewWalker;

            // format conditions
            PropertyCondition[] conditions;
            if (searchCondition is AndCondition)
            {
                Condition[] conds = ((AndCondition)searchCondition).GetConditions();
                conditions = new PropertyCondition[conds.Length];
                for (int i = 0; i < conds.Length; i++)
                {
                    conditions[i] = (PropertyCondition)conds[i];
                }
            }
            else
            {
                conditions = new PropertyCondition[1];
                conditions[0] = (PropertyCondition)searchCondition;
            }

            var ret = WalkerSearch(rootElement, conditions, walker);
            return ret;
        }

        private static AutomationElement WalkerSearch(AutomationElement rootElement, PropertyCondition[] searchConditions, TreeWalker walker)
        {
            AutomationElement node = walker.GetFirstChild(rootElement);
            AutomationElement ret = null;

            while (node != null)
            {
                bool result = true;
                foreach (PropertyCondition condition in searchConditions)
                {
                    if (node.GetCurrentPropertyValue(condition.Property) != condition.Value)
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                {
                    ret = node;
                    break;
                }

                // search child node
                if (walker.GetFirstChild(node) != null)
                {
                    ret = WalkerSearch(node, searchConditions, walker);
                    if (ret != null)
                    {
                        break;
                    }
                }

                // next sibling
                node = walker.GetNextSibling(node);
            }

            return ret;
        }

        public static List<AutomationElement> GetChildrenElements(AutomationElement rootElement, DataTable conditionTable, taskt.Core.Automation.Engine.AutomationEngineInstance engine)
        {
            Condition searchConditions = CreateSearchCondition(conditionTable, engine);

            if (searchConditions != null)
            {
                AutomationElementCollection elements = rootElement.FindAll(TreeScope.Children, searchConditions);

                // if element not found, don't throw exception here
                List<AutomationElement> ret = new List<AutomationElement>();
                foreach (AutomationElement element in elements)
                {
                    ret.Add(element);
                }
                return ret;
            }
            else
            {
                return GetAllChildrenElements(rootElement);
            }
        }

        private static List<AutomationElement> GetAllChildrenElements(AutomationElement rootElement)
        {
            TreeWalker walker = TreeWalker.RawViewWalker;

            var elems = new List<AutomationElement>();

            var node = walker.GetFirstChild(rootElement);
            while (node != null)
            {
                elems.Add(node);
                node = walker.GetNextSibling(node);
            }

            return elems;
        }

        public static AutomationElement GetParentElement(AutomationElement targetElement)
        {
            TreeWalker walker = TreeWalker.RawViewWalker;

            var parent = walker.GetParent(targetElement);
            if (parent != null)
            {
                return parent;
            }
            else
            {
                throw new Exception("Parent Element not exists");
            }
        }

        public static string GetWindowName(AutomationElement targetElement)
        {
            TreeWalker walker = TreeWalker.RawViewWalker;

            if (targetElement.Current.ControlType == ControlType.Window)
            {
                return targetElement.Current.Name;
            }

            var parent = walker.GetParent(targetElement);
            while(parent.Current.ControlType != ControlType.Window)
            {
                parent = walker.GetParent(parent);
            }
            return parent.Current.Name;
        }

        public static string GetTextValue(AutomationElement targetElement)
        {
            object patternObj;
            if (targetElement.TryGetCurrentPattern(ValuePattern.Pattern, out patternObj))
            {
                // TextBox
                return ((ValuePattern)patternObj).Current.Value;
            }
            else if (targetElement.TryGetCurrentPattern(TextPattern.Pattern, out patternObj))
            {
                // TextBox Multilune
                return ((TextPattern)patternObj).DocumentRange.GetText(-1);
            }
            else if (targetElement.TryGetCurrentPattern(SelectionPattern.Pattern, out patternObj))
            {
                // combobox
                AutomationElement selElem = ((SelectionPattern)patternObj).Current.GetSelection()[0];
                return selElem.Current.Name;
            }
            else
            {
                // others
                return targetElement.Current.Name;
            }
        }

        public static AutomationElement GetTableElement(AutomationElement targetElement, int row, int column)
        {
            object tryObj;
            if (!targetElement.TryGetCurrentPattern(GridPattern.Pattern, out tryObj))
            {
                throw new Exception("AutomationElement is not Table Element");
            }
            GridPattern gridPtn = (GridPattern)tryObj;

            AutomationElement cellElem = gridPtn.GetItem(row, column);
            if (cellElem == null)
            {
                throw new Exception("Table Row: '" + row + "', Column: '" + column + "' does not exists");
            }

            return cellElem;
        }

        public static List<AutomationElement> GetSelectionItems(AutomationElement targetElement, bool collapseAfter = true)
        {
            AutomationElement rootElement = targetElement;

            object ptnResult;

            ptnResult = rootElement.GetCurrentPropertyValue(AutomationElement.IsGridPatternAvailableProperty);
            if ((bool)ptnResult)
            {
                // DataGridView
                var elems = rootElement.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.IsSelectionItemPatternAvailableProperty, true));
                List<AutomationElement> ret = new List<AutomationElement>();
                foreach (AutomationElement elem in elems)
                {
                    ret.Add(elem);
                }
                return ret;
            }
            else
            {
                // ComboBox
                ptnResult = rootElement.GetCurrentPropertyValue(AutomationElement.IsExpandCollapsePatternAvailableProperty);
                if (!(bool)ptnResult)
                {
                    rootElement = GetParentElement(rootElement);
                }

                ptnResult = rootElement.GetCurrentPropertyValue(AutomationElement.IsExpandCollapsePatternAvailableProperty);
                if ((bool)ptnResult)
                {
                    object selPtn = rootElement.GetCurrentPattern(ExpandCollapsePattern.Pattern);

                    ExpandCollapsePattern ecPtn = (ExpandCollapsePattern)selPtn;
                    ecPtn.Expand();

                    var elems = rootElement.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.IsSelectionItemPatternAvailableProperty, true));
                    List<AutomationElement> ret = new List<AutomationElement>();
                    foreach (AutomationElement elem in elems)
                    {
                        ret.Add(elem);
                    }

                    if (collapseAfter)
                    {
                        ecPtn.Collapse();
                    }
                    return ret;
                }
                else
                {
                    throw new Exception("This element does not have Selection Items");
                }
            }
        }

        public static XElement GetElementXml(string windowName, out Dictionary<string, AutomationElement> elemsDic)
        {
            AutomationElement window = GetFromWindowName(windowName);
            var ret = GetElementXml(window, out elemsDic);
            return ret;
        }

        //public static XElement GetElementXml(AutomationElement targetElement, out Dictionary<string, AutomationElement> elemsDic)
        //{
        //    AutomationElement window = GetWindowElement(targetElement);

        //    XElement root = CreateXmlElement(window);

        //    elemsDic = new Dictionary<string, AutomationElement>();
        //    elemsDic.Add(window.GetHashCode().ToString(), window);

        //    TreeWalker walker = TreeWalker.RawViewWalker;

        //    GetChildNodeFromElement(root, window, elemsDic, walker);

        //    return root;
        //}
        public static XElement GetElementXml(AutomationElement targetElement, out Dictionary<string, AutomationElement> elemsDic)
        {
            XElement root = CreateXmlElement(targetElement);

            elemsDic = new Dictionary<string, AutomationElement>();
            elemsDic.Add(targetElement.GetHashCode().ToString(), targetElement);

            TreeWalker walker = TreeWalker.RawViewWalker;

            GetChildNodeFromElement(root, targetElement, elemsDic, walker);

            return root;
        }

        private static AutomationElement GetWindowElement(AutomationElement targetElement)
        {
            TreeWalker walker = TreeWalker.RawViewWalker;

            AutomationElement node = targetElement;
            while(GetControlTypeText(node.Current.ControlType) != "Window")
            {
                node = walker.GetParent(node);
            }

            return node;
        }

        private static XElement GetChildNodeFromElement(XElement rootNode, AutomationElement rootElement, Dictionary<string, AutomationElement> elemsDic, TreeWalker walker)
        {
            var node = walker.GetFirstChild(rootElement);
            while (node != null)
            {
                string hash = node.GetHashCode().ToString();
                if (elemsDic.ContainsKey(hash))
                {
                    int i = 1;
                    while(elemsDic.ContainsKey(hash + "-" + i))
                    {
                        i++;
                    }
                    hash += "-" + i;
                }

                var childNode = CreateXmlElement(node, hash);
                rootNode.Add(childNode);
                elemsDic.Add(hash, node);

                if (walker.GetFirstChild(node) != null)
                {
                    GetChildNodeFromElement(childNode, node, elemsDic, walker);
                }

                node = walker.GetNextSibling(node);
            }

            return rootNode;
        }

        private static XElement CreateXmlElement(AutomationElement targetElement, string hash = "")
        {
            XElement node = new XElement(GetControlTypeText(targetElement.Current.ControlType));
            node.SetAttributeValue("AcceleratorKey", targetElement.Current.AcceleratorKey);
            node.SetAttributeValue("AccessKey", targetElement.Current.AccessKey);
            node.SetAttributeValue("AutomationId", targetElement.Current.AutomationId);
            node.SetAttributeValue("ClassName", targetElement.Current.ClassName);
            node.SetAttributeValue("FrameworkId", targetElement.Current.FrameworkId);
            node.SetAttributeValue("HasKeyboardFocus", targetElement.Current.HasKeyboardFocus);
            node.SetAttributeValue("HelpText", targetElement.Current.HelpText);
            node.SetAttributeValue("IsContentElement", targetElement.Current.IsContentElement);
            node.SetAttributeValue("IsControlElement", targetElement.Current.IsControlElement);
            node.SetAttributeValue("IsEnabled", targetElement.Current.IsEnabled);
            node.SetAttributeValue("IsKeyboardFocusable", targetElement.Current.IsKeyboardFocusable);
            node.SetAttributeValue("IsOffscreen", targetElement.Current.IsOffscreen);
            node.SetAttributeValue("IsPassword", targetElement.Current.IsPassword);
            node.SetAttributeValue("IsRequiredForForm", targetElement.Current.IsRequiredForForm);
            node.SetAttributeValue("ItemStatus", targetElement.Current.ItemStatus);
            node.SetAttributeValue("ItemType", targetElement.Current.ItemType);
            node.SetAttributeValue("LocalizedControlType", targetElement.Current.LocalizedControlType);
            node.SetAttributeValue("Name", targetElement.Current.Name);
            node.SetAttributeValue("NativeWindowHandle", targetElement.Current.NativeWindowHandle);
            node.SetAttributeValue("ProcessID", targetElement.Current.ProcessId);

            if (hash == "") 
            {
                node.SetAttributeValue("Hash", targetElement.GetHashCode());
            }
            else
            {
                node.SetAttributeValue("Hash", hash);
            }
            

            return node;
        }

        public static System.Windows.Forms.TreeNode GetElementTreeNode(string windowName, out XElement xml)
        {
            AutomationElement root = GetFromWindowName(windowName);

            TreeWalker walker = TreeWalker.RawViewWalker;

            var tree = CreateTreeNodeFromAutomationElement(root);
            xml = CreateXmlElement(root);

            GetChildElementTreeNode(tree, xml, root, walker);

            return tree;
        }

        private static void GetChildElementTreeNode(System.Windows.Forms.TreeNode tree, XElement xml, AutomationElement rootElement, TreeWalker walker)
        {
            AutomationElement node = walker.GetFirstChild(rootElement);
            while(node != null)
            {
                var item = CreateTreeNodeFromAutomationElement(node);
                tree.Nodes.Add(item);

                var childXml = CreateXmlElement(node);
                xml.Add(childXml);

                if (walker.GetFirstChild(node) != null)
                {
                    GetChildElementTreeNode(item, childXml, node, walker);
                }

                node = walker.GetNextSibling(node);
            }
        }

        private static System.Windows.Forms.TreeNode CreateTreeNodeFromAutomationElement(AutomationElement element)
        {
            System.Windows.Forms.TreeNode node = new System.Windows.Forms.TreeNode();
            node.Text = "\"" + element.Current.Name + "\" " + element.Current.LocalizedControlType;
            node.Tag = element;
            return node;
        }

        public static string GetInspectResultFromAutomationElement(AutomationElement elem)
        {
            string res = "";

            try
            {
                res += "Name: \"" + elem.Current.Name + "\"\r\n";
                res += "ControlType: " + GetControlTypeText(elem.Current.ControlType) + "\r\n";
                res += "LocalizedControlType: \"" + elem.Current.LocalizedControlType + "\"\r\n";
                res += "IsEnabled: " + elem.Current.IsEnabled.ToString() + "\r\n";
                res += "IsOffscreen: " + elem.Current.IsOffscreen.ToString() + "\r\n";
                res += "IsKeyboardFocusable: " + elem.Current.IsKeyboardFocusable.ToString() + "\r\n";
                res += "HasKeyboardFocusable: " + elem.Current.HasKeyboardFocus.ToString() + "\r\n";
                res += "AccessKey: \"" + elem.Current.AccessKey + "\"\r\n";
                res += "ProcessId: " + elem.Current.ProcessId.ToString() + "\r\n";
                res += "AutomationId: \"" + elem.Current.AutomationId + "\"\r\n";
                res += "FrameworkId: \"" + elem.Current.FrameworkId + "\"\r\n";
                res += "ClassName: \"" + elem.Current.ClassName + "\"\r\n";
                res += "IsContentElement: " + elem.Current.IsContentElement.ToString() + "\r\n";
                res += "IsPassword: " + elem.Current.IsPassword.ToString() + "\r\n";

                res += "AcceleratorKey: \"" + elem.Current.AcceleratorKey + "\"\r\n";
                res += "HelpText: \"" + elem.Current.HelpText + "\"\r\n";
                res += "IsControlElement: " + elem.Current.IsControlElement.ToString() + "\r\n";
                res += "IsRequiredForForm: " + elem.Current.IsRequiredForForm.ToString() + "\r\n";
                res += "ItemStatus: \"" + elem.Current.ItemStatus + "\"\r\n";
                res += "ItemType: \"" + elem.Current.ItemType + "\"\r\n";
                res += "NativeWindowHandle: " + elem.Current.NativeWindowHandle.ToString() + "\r\n";
            }
            catch(Exception ex)
            {
                res += "Error: " + ex.Message;
            }

            return res;
        }

        public static string GetXPath(XElement xml, AutomationElement elem)
        {
            string searchPath = "//" + GetControlTypeText(elem.Current.ControlType) + "[@Hash=\"" + elem.GetHashCode() + "\"]";
            XElement trgElement = xml.XPathSelectElement(searchPath);

            if (trgElement == null)
            {
                return "";
            }

            string xpath = "";
            while (trgElement.Parent != null)
            {
                xpath = CreateXPath(trgElement) + xpath;
                trgElement = trgElement.Parent;
            }

            return xpath;
        }

        private static string CreateXPath(XElement elemNode)
        {
            XElement parentNode = elemNode.Parent;

            string elemType = elemNode.Name.ToString();
            string elemHash = elemNode.Attribute("Hash").Value;
            string xpath;
            
            if (elemNode.Attribute("AutomationId").ToString() != "")
            {
                xpath = "/" + elemType + "[@AutomationId=\"" + elemNode.Attribute("AutomationId").Value + "\"]";
                XElement idNode = parentNode.XPathSelectElement("." + xpath);
                if (idNode != null)
                {
                    if (idNode.Attribute("Hash").Value == elemHash)
                    {
                        return xpath;
                    }
                }
            }

            xpath = "/" + elemType + "[@Name=\"" + elemNode.Attribute("Name").Value + "\"]";
            XElement nameNode = parentNode.XPathSelectElement("." + xpath);
            if (nameNode != null)
            {
                if (nameNode.Attribute("Hash").Value == elemHash)
                {
                    return xpath;
                }
            }

            xpath = "/" + elemType;
            IEnumerable<XElement> typeNodes = parentNode.XPathSelectElements("." + xpath);
            int idx = 1;
            foreach(XElement nd in typeNodes)
            {
                if (nd.Attribute("Hash").Value == elemHash)
                {
                    return xpath + "[" + idx + "]";
                }
                idx++;
            }

            throw new Exception("Fail Create AutomationElement XPath");
        }

        public static void InspectToolClicked(System.Windows.Forms.TextBox txtXPath)
        {
            using(var fm = new taskt.UI.Forms.Supplement_Forms.frmInspect())
            {
                if (fm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtXPath.Text = fm.XPath;
                }
            }
        }
    }
}
