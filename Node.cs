using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mini_compiler
{

    public abstract class Node : AST
    { 
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
        }

        public override void GenCode()
        {
            left.GenCode();
            right.GenCode();
            Compiler.EmitCode(op);
        }
    }

    public class LeafValNode : Node
    {
        Constant value;
        public LeafValNode(Constant con) => value = con;

        public override void GenCode() => value.PushStack();
    }

    public class LeafVarNode : AST
    {
        string name;
        public LeafVarNode(string name) => this.name = name;

        public override void GenCode() => Compiler.EmitCode($"ldloc {name}");
    }
}
