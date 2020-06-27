using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace mini_compiler
{
    #region Binary Operators
    public class BitExpNode : BinaryNode
    {
        public BitExpNode(Node left, string op, Node right) : base(left, op, right) { }
        public override string ExpOutType => "int32";
        public override void СheckType()
        {
            if (left.ExpOutType != "int32" || right.ExpOutType != "int32")
                ReportError();
        }
    }

    public class ArithmeticExpNode : BinaryNode
    {
        bool right_convert = false;
        bool left_convert = false;
        public ArithmeticExpNode(Node left, string op, Node right) : base(left, op, right) { }
        public override void GenCode()
        {
            left.GenCode();
            if (left_convert)
                Compiler.EmitCode("conv.r8");
            right.GenCode();

            if(right_convert)
                Compiler.EmitCode("conv.r8");

            Compiler.EmitCode(op);
        }
        public override void СheckType()
        {
            if (left.ExpOutType == "bool" || right.ExpOutType == "bool")
                ReportError();
            else if (left.ExpOutType == "float32" || right.ExpOutType == "float32")
            {
                exp_out_type = "float32";
                if (left.ExpOutType == "int32")
                    left_convert = true;
                if (right.ExpOutType == "int32")
                    right_convert = true;
            }
            else
                exp_out_type = "int32";
        }
    }

    public class RelationalExpNode : BinaryNode
    {
        bool eq;
        public RelationalExpNode(Node left, string op, Node right, bool eq = false) : base(left, op, right) { this.eq = eq; }
        public override void GenCode()
        {
            left.GenCode();
            right.GenCode();
            Compiler.EmitCode(op);
            if (eq)
            {
                Compiler.EmitCode("ldc.i4.0");
                Compiler.EmitCode("ceq");
            }
        }

        public override string ExpOutType => "bool";
        public override void СheckType()
        {
            if (left.ExpOutType == "bool" || right.ExpOutType == "bool")
                ReportError();
        }
    }

    public class LogicalExpNode : BinaryNode
    {
        public LogicalExpNode(Node left, string op, Node right) : base(left, op, right) { }
        public override string ExpOutType => "bool";
        public override void СheckType()
        {
            if (left.ExpOutType != "bool" || right.ExpOutType != "bool")
                ReportError();
        }
    }

    #endregion


    #region Unary Operators

    public class UnaryMinus : UnaryNode
    {
        public UnaryMinus(Node exp, string op) : base(exp, op) { }
        public override void СheckType()
        {
            base.СheckType();

            if (exp.ExpOutType == "bool")
                ReportError();
            exp_out_type = exp.ExpOutType;
        }
    }

    public class BitNegation : UnaryNode
    {
        public BitNegation(Node exp, string op) : base(exp, op) { }
        public override void СheckType()
        {
            base.СheckType();
            if (exp.ExpOutType != "int32")
                ReportError();
        }

        public override string ExpOutType => "int32";
    }

    public class LogicNegation : UnaryNode
    {
        public LogicNegation(Node exp, string op) : base(exp, op) { }
        public override void СheckType()
        {
            base.СheckType();
            if (exp.ExpOutType != "bool")
                ReportError();
        }

        public override void GenCode()
        {
            exp.GenCode();
            Compiler.EmitCode("ldc.i4 0");
            Compiler.EmitCode(op);
        }
        public override string ExpOutType => "bool";
    }

    public class ConvertToInt : UnaryNode
    {
        public ConvertToInt(Node exp, string op) : base(exp, op) { }

        public override string ExpOutType => "int32";

        public override void GenCode()
        {
            switch (exp.ExpOutType)
            {
                case "int32":

                    break;
                case "bool":
                   
                    break;
                case "float64":
                    break;
            }
        }
    }

    public class ConvertToDouble : UnaryNode
    {
        public ConvertToDouble(Node exp, string op) : base(exp, op) { }

        public override string ExpOutType => "float64";

        public override void GenCode()
        {
            switch (exp.ExpOutType)
            {
                case "int32":

                    break;
                case "bool":
                    break;
                case "float64":
                    break;
            }
        }
    }

    #endregion


}
