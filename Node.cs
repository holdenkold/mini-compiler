using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mini_compiler
{
    public abstract class Node
    {
        public abstract string Evaluate();
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

        public override string Evaluate()
        {
            var left_val = left.Evaluate();
            var right_val = right.Evaluate();
            switch (op)
            {
                case "ADD":
                    return left_val + right_val;
            }

            return null;
        }
    }

    public class LeafNode : Node
    {
        string value;
        public LeafNode(string val) => value = val;

        public override string Evaluate() => value;
    }

}
