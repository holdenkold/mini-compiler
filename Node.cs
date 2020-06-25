using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mini_compiler
{

    public abstract class Node : AST 
    {
        public abstract string ExpType { get; }
    }
    public class BinaryNode : Node
    {
        Node right;
        Node left;
        string op;

        public BinaryNode(Node left, string op, Node right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
            Compiler.syntaxTree.Add(this);
        }

        public override void GenCode()
        {
            left.GenCode();
            right.GenCode();
            Compiler.EmitCode(op);
        }

        public override string ExpType => right.ExpType;

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
            Compiler.syntaxTree.Add(this);
        }

        public override void GenCode() => value.PushStack();

        public override string ExpType => Compiler.IdentTypeMap[value.type];

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
            Compiler.syntaxTree.Add(this);
        }

        public override void GenCode() => Compiler.EmitCode($"ldloc {name}");

        public override string ExpType => Compiler.IdentTypeMap[Compiler.SymbolTable[name]];

        public override void СheckType()
        {
            return;
        }
    }

    public class UnaryNode : Node
    {
        Node exp;
        string op;
        public UnaryNode(Node exp, string op)
        {
            this.exp = exp;
            this.op = op;
            Compiler.syntaxTree.Add(this);
        }

        public override void GenCode() 
        {
            exp.GenCode();
            Compiler.EmitCode(op);
        }

        public override string ExpType => exp.ExpType;

        public override void СheckType()
        {
            return;
        }
    }
}
