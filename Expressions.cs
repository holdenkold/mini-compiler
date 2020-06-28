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
            if (left_convert)
                left.GenDoubleCode();
            else
                left.GenCode();

            if(right_convert)
                right.GenDoubleCode();
            else
                right.GenCode();

            Compiler.EmitCode(op);
        }
        public override void СheckType()
        {
            if (left.ExpOutType == "bool" || right.ExpOutType == "bool")
                ReportError();
            else if (left.ExpOutType == "float64" || right.ExpOutType == "float64")
            {
                exp_out_type = "float64";
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
        bool right_convert = false;
        bool left_convert = false;
        public RelationalExpNode(Node left, string op, Node right, bool eq = false) : base(left, op, right) { this.eq = eq; }
        public override void GenCode()
        {
            if (left_convert)
                left.GenDoubleCode();
            else
                left.GenCode();

            if (right_convert)
                right.GenDoubleCode();
            else
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
            else if (left.ExpOutType == "float64" || right.ExpOutType == "float64")
            {
                exp_out_type = "float64";
                if (left.ExpOutType == "int32")
                    left_convert = true;
                if (right.ExpOutType == "int32")
                    right_convert = true;
            }
            else
                exp_out_type = "int32";
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
        public override void GenCode()
        {
            var label = $"L{Compiler.label_num++}";
            left.GenCode();
            //Compiler.EmitCode("ldc.i4.1");
            //Compiler.EmitCode("ceq");
            //if (op == "and")
            //    Compiler.EmitCode($"brfalse {label}");
            //else if (op == "or")
            //    Compiler.EmitCode($"brtrue {label}");

            //left.GenCode();
            right.GenCode();
            Compiler.EmitCode(op);
            Compiler.EmitCode($"{label}:");
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
        public override string ExpOutType => exp.ExpOutType;
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

    public class ConvertToInt : Node
    {
        Node exp;
        public ConvertToInt(Node exp) => this.exp = exp;

        public override string ExpOutType => "int32";

        public override void GenCode()
        {
            exp.GenCode();
            switch (exp.ExpOutType)
            {
                case "int32":
                    break;
                case "bool":
                    Compiler.EmitCode("conv.i4");
                    break;
                case "float64":
                    Compiler.EmitCode("conv.i4"); //TO DO:
                    break;
            }
        }

        public override void СheckType() => exp.СheckType();
    }

    public class ConvertToDouble : Node
    {
        Node exp;
        public ConvertToDouble(Node exp) => this.exp = exp;

        public override string ExpOutType => "float64";

        public override void GenCode()
        {
            exp.GenCode();
            switch (exp.ExpOutType)
            {
                case "int32":
                    Compiler.EmitCode("conv.r8"); //TO DO:
                    break;
                case "bool":
                    Compiler.EmitCode("conv.r8"); //TO DO:
                    break;
                case "float64":
                    break;
            }
        }

        public override void СheckType() => exp.СheckType();
    }

    #endregion


}
