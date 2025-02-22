﻿using System;
using System.Data;
using System.Xml.Serialization;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("Excel Commands")]
    [Attributes.ClassAttributes.SubGruop("Range")]
    [Attributes.ClassAttributes.CommandSettings("Get Range Values As DataTable")]
    [Attributes.ClassAttributes.Description("This command get Range values as DataTable.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to get Range values as DataTable.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.CommandIcon(nameof(Properties.Resources.command_spreadsheet))]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class ExcelGetRangeValuesAsDataTableCommand : AExcelInstanceCommand
    {
        //[XmlAttribute]
        //[PropertyVirtualProperty(nameof(ExcelControls), nameof(ExcelControls.v_InputInstanceName))]
        //public string v_InstanceName { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(ExcelControls), nameof(ExcelControls.v_ColumnType))]
        [PropertyParameterOrder(6000)]
        public string v_ColumnType { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(ExcelControls), nameof(ExcelControls.v_ColumnStart))]
        [PropertyParameterOrder(6001)]
        public string v_ColumnStart { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(ExcelControls), nameof(ExcelControls.v_ColumnEnd))]
        [PropertyParameterOrder(6002)]
        public string v_ColumnEnd { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(ExcelControls), nameof(ExcelControls.v_RowStart))]
        [PropertyIsOptional(false)]
        [PropertyParameterOrder(6003)]
        public string v_RowStart { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(ExcelControls), nameof(ExcelControls.v_RowEnd))]
        [PropertyParameterOrder(6004)]
        public string v_RowEnd { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(DataTableControls), nameof(DataTableControls.v_OutputDataTableName))]
        [PropertyParameterOrder(6005)]
        public string v_userVariableName { get; set; }

        [XmlAttribute]
        [PropertyVirtualProperty(nameof(ExcelControls), nameof(ExcelControls.v_ValueType))]
        [PropertyParameterOrder(6006)]
        public string v_ValueType { get; set; }

        public ExcelGetRangeValuesAsDataTableCommand()
        {
            //this.CommandName = "ExcelGetRangeValuesAsDataTableCommand";
            //this.SelectionName = "Get Range Values As DataTable";
            //this.CommandEnabled = true;
            //this.CustomRendering = true;
        }

        public override void RunCommand(Engine.AutomationEngineInstance engine)
        {
            (var excelInstance, var excelSheet) = v_InstanceName.ExpandValueOrUserVariableAsExcelInstanceAndWorksheet(engine);

            string valueType = this.ExpandValueOrUserVariableAsSelectionItem(nameof(v_ValueType), "Value Type", engine);

            int rowStartIndex = this.ExpandValueOrUserVariableAsInteger(nameof(v_RowStart), "Start Row", engine);

            int columnStartIndex = 0;
            int columnEndIndex = 0;
            switch(this.ExpandValueOrUserVariableAsSelectionItem(nameof(v_ColumnType), "Column Type", engine))
            {
                case "range":
                    columnStartIndex = ExcelControls.GetColumnIndex(excelSheet, v_ColumnStart.ExpandValueOrUserVariable(engine));
                    if (String.IsNullOrEmpty(v_ColumnEnd))
                    {
                        columnEndIndex = ExcelControls.GetLastColumnIndex(excelSheet, rowStartIndex, columnStartIndex, valueType);
                    }
                    else
                    {
                        columnEndIndex = ExcelControls.GetColumnIndex(excelSheet, v_ColumnEnd.ExpandValueOrUserVariable(engine));
                    }
                    break;

                case "rc":
                    columnStartIndex = this.ExpandValueOrUserVariableAsInteger(nameof(v_ColumnStart), "Column Start", engine);
                    if (String.IsNullOrEmpty(v_ColumnEnd))
                    {
                        columnEndIndex = ExcelControls.GetLastColumnIndex(excelSheet, rowStartIndex, columnStartIndex, valueType);
                    }
                    else
                    {
                        columnEndIndex = this.ExpandValueOrUserVariableAsInteger(nameof(v_ColumnEnd), "Column End", engine);
                    }
                    break;
            }

            if (columnStartIndex > columnEndIndex)
            {
                int t = columnStartIndex;
                columnStartIndex = columnEndIndex;
                columnEndIndex = t;
            }

            int rowEndIndex;
            if (String.IsNullOrEmpty(v_RowEnd))
            {
                rowEndIndex = ExcelControls.GetLastRowIndex(excelSheet, columnStartIndex, rowStartIndex, valueType);
            }
            else
            {
                rowEndIndex = this.ExpandValueOrUserVariableAsInteger(nameof(v_RowEnd), "Row End", engine);
            }

            if (rowStartIndex > rowEndIndex)
            {
                int t = rowStartIndex;
                rowStartIndex = rowEndIndex;
                rowEndIndex = t;
            }

            ExcelControls.CheckCorrectRCRange(rowStartIndex, columnStartIndex, rowEndIndex, columnEndIndex, excelInstance);

            Func<Microsoft.Office.Interop.Excel.Worksheet, int, int, string> getFunc = ExcelControls.GetCellValueFunction(valueType);

            int rowRange = rowEndIndex - rowStartIndex + 1;
            int colRange = columnEndIndex - columnStartIndex + 1;

            DataTable newDT = new DataTable();
            // set columns
            for (int i = 0; i < colRange; i++) 
            {
                newDT.Columns.Add(ExcelControls.GetColumnName(excelSheet, columnStartIndex + i));
            }

            for (int i = 0; i < rowRange; i++)
            {
                newDT.Rows.Add();
                for (int j = 0; j < colRange; j++)
                {
                    newDT.Rows[i][j] = getFunc(excelSheet, columnStartIndex + j, rowStartIndex + i);
                }
            }

            newDT.StoreInUserVariable(engine, v_userVariableName);
        }
    }
}