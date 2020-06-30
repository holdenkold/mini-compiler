using System;

namespace mini_compiler
{
    public enum IdentType
    {
        Int,
        Double,
        Bool
    }

    public class Constant
    {
        public IdentType type;
        string value;

        public Constant(IdentType type, string value)
        {
            this.type = type;
            this.value = value;
        }

        public void PushStack()
        {
            switch (type)
            {
                case IdentType.Int:
                    Compiler.EmitCode($"ldc.i4 {value}");
                    break;
                case IdentType.Bool:
                    var val = value == "true" ? 1 : 0;
                    Compiler.EmitCode($"ldc.i4 {val}");
                    break;
                case IdentType.Double:
                    Compiler.EmitCode($"ldc.r8 {value}");
                    break;
            }
        }
    }

    public abstract class Node : AST
    {
        public Node(int linenum) => base.linenum = linenum;
        public abstract IdentType ExpOutType { get; }
    }

    public class LeafValNode : Node
    {
        Constant value;

        public LeafValNode(Constant con, int linenum) : base(linenum) { value = con; }

        public override void GenCode() => value.PushStack();

        public override IdentType ExpOutType => value.type;

        public override void СheckType()
        {
            return;
        }
    }

    public class LeafVarNode : Node
    {
        public string name;
        public IdentType exp_out_type;

        public LeafVarNode(string name, int linenum) : base(linenum) => this.name = name;

        public override void GenCode() => Compiler.PushStack(linenum, name);

        public override IdentType ExpOutType => exp_out_type;

        public override void СheckType()
        {
            if (!Compiler.SymbolTable.ContainsKey(name))
                Compiler.ReportError(linenum, "undeclared variable");
            else
                exp_out_type = Compiler.SymbolTable[name];
        }
    }

    public abstract class UnaryNode : Node
    {
        protected Node exp;
        protected string op;
        protected IdentType exp_out_type;
        protected string error_msg { get => $"Semantic Error: Invalid type {op} {exp.ExpOutType}"; }

        public UnaryNode(Node exp, string op, int linenum) : base(linenum)
        {
            this.exp = exp;
            this.op = op;
        }

        public void ReportError() => Compiler.ReportError(linenum, error_msg);

        public override void GenCode()
        {
            exp.GenCode();
            Compiler.EmitCode(op);
        }

        public override void СheckType() => exp.СheckType();

        public override IdentType ExpOutType => exp_out_type;
    }

    public abstract class BinaryNode : Node
    {
        protected Node right;
        protected Node left;
        protected string op;
        protected IdentType exp_out_type;
        protected string error_msg { get => $"Semantic Error: Invalid type {left.ExpOutType} {op} {right.ExpOutType}"; }

        public override IdentType ExpOutType => exp_out_type;
        public BinaryNode(Node left, string op, Node right, int linenum) : base(linenum)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
        public void ReportError()
        {
           Compiler.ReportError(linenum, error_msg);
        }

        public override void GenCode()
        {
            left.GenCode();
            right.GenCode();
            Compiler.EmitCode(op);
        }

        public override void СheckType()
        {
            left.СheckType();
            right.СheckType();
        }
    }
}
