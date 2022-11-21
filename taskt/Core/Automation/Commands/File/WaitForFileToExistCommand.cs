﻿using System;
using System.Xml.Serialization;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("File Operation Commands")]
    [Attributes.ClassAttributes.Description("This command waits for a file to exist at a specified destination")]
    [Attributes.ClassAttributes.UsesDescription("Use this command to wait for a file to exist before proceeding.")]
    [Attributes.ClassAttributes.ImplementationDescription("This command implements '' to achieve automation.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    public class WaitForFileToExistCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Please indicate the directory of the file")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowFileSelectionHelper)]
        [InputSpecification("Enter or Select the path to the file.")]
        [SampleUsage("**C:\\temp\\myfile.txt** or **{{{vTextFilePath}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyTextBoxSetting(1, false)]
        [PropertyValidationRule("File", PropertyValidationRule.ValidationRuleFlags.Empty)]
        [PropertyDisplayText(true, "File")]
        public string v_FileName { get; set; }


        [XmlAttribute]
        [PropertyDescription("Indicate how many seconds to wait for the file to exist")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("Specify how long to wait before an error will occur because the file is not found.")]
        [SampleUsage("**10** or **20** or **{{{vWaitTime}}}**")]
        [Remarks("")]
        [PropertyShowSampleUsageInDescription(true)]
        [PropertyTextBoxSetting(1, false)]
        [PropertyValidationRule("Wait Time", PropertyValidationRule.ValidationRuleFlags.Empty | PropertyValidationRule.ValidationRuleFlags.EqualsZero | PropertyValidationRule.ValidationRuleFlags.LessThanZero)]
        [PropertyDisplayText(true, "Wait", "s")]
        public string v_WaitTime { get; set; }

        public WaitForFileToExistCommand()
        {
            this.CommandName = "WaitForFileToExistCommand";
            this.SelectionName = "Wait For File";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            var engine = (Engine.AutomationEngineInstance)sender;

            //convert items to variables
            var fileName = v_FileName.ConvertToUserVariable(sender);
            //var pauseTime = int.Parse(v_WaitTime.ConvertToUserVariable(sender));
            //int pauseTime = v_WaitTime.ConvertToUserVariableAsInteger("Wait Time", engine);
            //int pauseTime = this.ConvertToUserVariableAsInteger(nameof(v_WaitTime), "Wait Time", engine);

            ////determine when to stop waiting based on user config
            //var stopWaiting = DateTime.Now.AddSeconds(pauseTime);

            ////initialize flag for file found
            //var fileFound = false;


            ////while file has not been found
            //while (!fileFound)
            //{
            //    //if file exists at the file path
            //    if (System.IO.File.Exists(fileName))
            //    {
            //        fileFound = true;
            //    }

            //    //test if we should exit and throw exception
            //    if (DateTime.Now > stopWaiting)
            //    {
            //        throw new Exception("File was not found in time!");
            //    }

            //    //put thread to sleep before iterating
            //    engine.ReportProgress("File Not Yet Found... " + (int)((stopWaiting - DateTime.Now).TotalSeconds) + "s remain");
            //    System.Threading.Thread.Sleep(1000);
            //}

            Func<bool> fileCheckFunc = new Func<bool>(() =>
            {
                return System.IO.File.Exists(fileName);
            });
            this.WaitProcess(nameof(v_WaitTime), "File", fileCheckFunc, engine);
        }
    }
}