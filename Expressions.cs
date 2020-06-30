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
        public override IdentType ExpOutType => IdentType.Int;
        public override void СheckType()
        {
            base.СheckType();

            if (left.ExpOutType != IdentType.Int || right.ExpOutType != IdentType.Int)
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

            if (right_convert)
                right.GenDoubleCode();
            else
                right.GenCode();

            Compiler.EmitCode(op);
        }

        public override void СheckType()
        {
            base.СheckType();

            if (left.ExpOutType == IdentType.Bool || right.ExpOutType == IdentType.Bool)
                ReportError();

            else if (left.ExpOutType == IdentType.Double || right.ExpOutType == IdentType.Double)
            {
                exp_out_type = IdentType.Double;
                if (left.ExpOutType == IdentType.Int)
                    left_convert = true;
                if (right.ExpOutType == IdentType.Int)
                    right_convert = true;
            }
            else
                exp_out_type = IdentType.Int;
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

        public override IdentType ExpOutType => IdentType.Bool;

        public override void СheckType()
        {
            base.СheckType();

            if ((left.ExpOutType == IdentType.Bool && right.ExpOutType != IdentType.Bool) || (left.ExpOutType != IdentType.Bool && right.ExpOutType == IdentType.Bool))
                ReportError();

            if ((left.ExpOutType == IdentType.Bool || right.ExpOutType == IdentType.Bool) && (op != "ceq"))
                ReportError();
            else if (left.ExpOutType == IdentType.Double || right.ExpOutType == IdentType.Double)
            {
                exp_out_type = IdentType.Double;
                if (left.ExpOutType == IdentType.Int)
                    left_convert = true;
                if (right.ExpOutType == IdentType.Int)
                    right_convert = true;
            }
            else
                exp_out_type = IdentType.Int;
        }
    }

    public class LogicalExpNode : BinaryNode
    {
        public LogicalExpNode(Node left, string op, Node right) : base(left, op, right) { }
        public override IdentType ExpOutType => IdentType.Bool;
        public override void СheckType()
        {
            base.СheckType();

            if (left.ExpOutType != IdentType.Bool || right.ExpOutType != IdentType.Bool)
                ReportError();
        }
        public override void GenCode()
        {
            var label = Compiler.GetLabel; //$"L{Compiler.label_num++}";

            left.GenCode();
            Compiler.EmitCode("dup");
            if (op == "and")
                Compiler.EmitCode($"brfalse {label}");
            else if (op == "or")
                Compiler.EmitCode($"brtrue {label}");
            right.GenCode();
            Compiler.EmitCode(op);
            Compiler.EmitCode($"{label}:");
        }
    }

    #endregion

    public class Assign : Node
    {
        string left_ident;
        Node right_node;
        bool covert = false;
        public Assign(string to, Node node)
        {
            left_ident = to;
            right_node = node;
        }

        public override IdentType ExpOutType => Compiler.SymbolTable[left_ident];

        public override void GenCode()
        {
            if (covert)
                right_node.GenDoubleCode();
            else
                right_node.GenCode();

            Compiler.EmitCode("dup");
            Compiler.PullStack(left_ident);
        }

        public override void СheckType()
        {
            right_node.СheckType();

            if (!Compiler.SymbolTable.ContainsKey(left_ident))
            {
                Compiler.errors += 1;
                Console.WriteLine("undeclared variable");
                return;
            }

            IdentType assigntTo = Compiler.SymbolTable[left_ident];
            var assigntFrom = right_node.ExpOutType;
            if (assigntTo == IdentType.Double)
            {
                if (assigntFrom == IdentType.Bool)
                {
                    Compiler.errors += 1;
                    Console.WriteLine("Semantic Error: Expected int or double for assigment, got bool");
                }
                else
                    covert = true;
            }
            else if (assigntTo == IdentType.Int && assigntFrom != IdentType.Int)
            {
                Compiler.errors += 1;
                Console.WriteLine($"Semantic Error: Expected int for assigment, got {assigntFrom}");
            }
            else if (assigntTo == IdentType.Bool && assigntFrom != IdentType.Bool)
            {
                Compiler.errors += 1;
                Console.WriteLine($"Semantic Error: Expected bool for assigment, got {assigntFrom}");
            }
        }
    }
    public class Convert : Node
    {
        Node exp;
        IdentType typeTo;
        public Convert(Node exp, IdentType type_to)
        {
            this.exp = exp;
            typeTo = type_to;
        }

        public override IdentType ExpOutType => typeTo;

        public override void GenCode()
        {
            exp.GenCode();
            if (exp.ExpOutType != typeTo)
            {
                if (typeTo == IdentType.Int)
                    Compiler.EmitCode("conv.i4");
                else if (typeTo == IdentType.Double)
                    Compiler.EmitCode("conv.r8");
            }
        }

        public override void СheckType() => exp.СheckType();
    }

    #region Unary Operators
    public class UnaryMinus : UnaryNode
    {
        public UnaryMinus(Node exp, string op) : base(exp, op) { }

        public override void СheckType()
        {
            base.СheckType();
            if (exp.ExpOutType == IdentType.Bool)
                ReportError();

            exp_out_type = exp.ExpOutType;
        }
        public override IdentType ExpOutType => exp.ExpOutType;
    }

    public class BitNegation : UnaryNode
    {
        public BitNegation(Node exp, string op) : base(exp, op) { }

        public override void СheckType()
        {
            base.СheckType();
            if (exp.ExpOutType != IdentType.Int)
                ReportError();
        }

        public override IdentType ExpOutType => IdentType.Int;
    }

    public class LogicNegation : UnaryNode
    {
        public LogicNegation(Node exp, string op) : base(exp, op) { }

        public override void СheckType()
        {
            base.СheckType();
            if (exp.ExpOutType != IdentType.Bool)
                ReportError();
        }

        public override void GenCode()
        {
            exp.GenCode();
            Compiler.EmitCode("ldc.i4 0");
            Compiler.EmitCode(op);
        }

        public override IdentType ExpOutType => IdentType.Bool;
    }

    #endregion
}
