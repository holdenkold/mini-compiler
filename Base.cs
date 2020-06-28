using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                    Compiler.EmitCode($"ldc.r4 {value}");
                    break;
            }
        }
    }

    public abstract class Node : AST
    {
        public abstract string ExpOutType { get; }
    }

    public class Program : AST
    {
        List<AST> declarations;
        List<AST> statements;
        public Program(List<AST> decl, List<AST> stmt)
        {
            declarations = decl;
            statements = stmt;
        }

        public override void GenCode()
        {
            declarations.ForEach(d => d.GenCode());
            statements.ForEach(s => s.GenCode());
        }

        public override void СheckType()
        {
            declarations.ForEach(d => d.СheckType());
            statements.ForEach(s => s.СheckType());
        }
    }

    public class ReturnNode : AST
    {
        public override void GenCode()
        {
            Compiler.EmitCode("ret");
        }

        public override void СheckType()
        {
            return;
        }
    }

    public class LeafValNode : Node
    {
        Constant value;
        public LeafValNode(Constant con)
        {
            value = con;
        }

        public override void GenCode() => value.PushStack();

        public override string ExpOutType => Compiler.IdentTypeMap[value.type];

        public override void СheckType()
        {
            return;
        }
    }

    public class LeafVarNode : Node
    {
        public string name;
        public LeafVarNode(string name)
        {
            this.name = name;
        }

        public override void GenCode() => Compiler.PushStack(name); // Compiler.EmitCode($"ldloc {name}");

        public override string ExpOutType => Compiler.IdentTypeMap[Compiler.SymbolTable[name]];

        public override void СheckType()
        {
            if (!Compiler.SymbolTable.ContainsKey(name))
            {
                Compiler.errors++;
                Console.WriteLine("undeclared variable");
            }
        }
    }

    public abstract class UnaryNode : Node
    {
        protected Node exp;
        protected string op;
        protected string exp_out_type;
        protected string error_msg { get => $"Semantic Error: Invalid type {op} {exp.ExpOutType}"; }

        public UnaryNode(Node exp, string op)
        {
            this.exp = exp;
            this.op = op;
        }

        public void ReportError()
        {
            Compiler.errors++;
            Console.WriteLine(error_msg);
        }

        public override void GenCode()
        {
            exp.GenCode();
            //Compiler.EmitCode("ldc.i4 0");
            Compiler.EmitCode(op);
        }

        public override void СheckType()
        {
            exp.СheckType();
        }

        public override string ExpOutType => exp_out_type;

    }

    public abstract class BinaryNode : Node
    {
        protected Node right;
        protected Node left;
        protected string op;
        protected string exp_out_type;
        protected string error_msg { get => $"Semantic Error: Invalid type {left.ExpOutType} {op} {right.ExpOutType}"; }

        public override string ExpOutType => exp_out_type;
        public BinaryNode(Node left, string op, Node right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
        public void ReportError()
        {
            Compiler.errors++;
            Console.WriteLine(error_msg);
        }

        public override void GenCode()
        {
            left.GenCode();
            right.GenCode();
            Compiler.EmitCode(op);
        }
    }
}
