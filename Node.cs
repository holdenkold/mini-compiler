using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mini_compiler
{
    public abstract class Node
    {
        public abstract double Evaluate();
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

        public override double Evaluate()
        {
            var left_val = left.Evaluate();
            var right_val = right.Evaluate();
            switch (op)
            {
                case "+":
                    return left_val + right_val;
            }

            return int.MinValue;
        }
    }

    public class LeafNode : Node
    {
        double value;
        public LeafNode(string val)
        {
            value = double.Parse(val);
        }

        public override double Evaluate() => value;
    }

}
