﻿using System;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("Math Commands")]
    [Attributes.ClassAttributes.CommandSettings("Get Arcsin")]
    [Attributes.ClassAttributes.Description("This command allows you to get arcsin.")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to get arcsin.")]
    [Attributes.ClassAttributes.ImplementationDescription("")]
    [Attributes.ClassAttributes.CommandIcon(nameof(Properties.Resources.command_function))]
    [Attributes.ClassAttributes.EnableAutomateDisplayText(true)]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    public class GetArcsinCommand : AInverseTrignometricCommand
    {
        //[XmlAttribute]
        //[PropertyVirtualProperty(nameof(NumberControls), nameof(NumberControls.v_Value))]
        //public string v_Value { get; set; }

        //[XmlAttribute]
        //[PropertyVirtualProperty(nameof(NumberControls), nameof(NumberControls.v_OutputNumericalVariableName))]
        //public string v_Result { get; set; }

        //[XmlAttribute]
        //[PropertyVirtualProperty(nameof(MathControls), nameof(MathControls.v_AngleType))]
        //public string v_AngleType { get; set; }

        //[XmlAttribute]
        //[PropertyVirtualProperty(nameof(MathControls), nameof(MathControls.v_WhenValueIsOutOfRange))]
        //public string v_WhenValueIsOutOfRange { get; set; }

        public GetArcsinCommand()
        {
        }

        public override void RunCommand(Engine.AutomationEngineInstance engine)
        {
            //var r = MathControls.InverseTrignometicFunctionAction(this, nameof(v_Value), nameof(v_AngleType), nameof(v_WhenValueIsOutOfRange),
            //    Math.Asin, new Func<double, bool>(v =>
            //    {
            //        return (v >= -1 && v <= 1);
            //    }), engine);
            //var r = MathControls.InverseTrignometicFunctionAction(this, Math.Asin,
            //        new Func<double, bool>(v =>
            //        {
            //            return (v >= -1 && v <= 1);
            //        }), engine);
            var r = MathControls.InverseTrignometicFunctionAction(this, Math.Asin, MathControls.CheckAcosAsinRange, engine);
            r.StoreInUserVariable(engine, v_Result);
        }
    }
}