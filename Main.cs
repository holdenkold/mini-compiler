using mini_compiler;
using System;
using System.Collections.Generic;
using System.IO;

namespace mini_compiler
{
    public class Compiler
    {
        public static int errors = 0;

        public static List<string> source;
        public static List<AST> syntaxTree = new List<AST>();
        public static Dictionary<string, IdentType> SymbolTable = new Dictionary<string, IdentType>();

        static int label_num = 0;
        public static string GetLabel => $"L{label_num++}";

        public static void ReportError(int linenum, string msg)
        {
            errors++;
            Console.WriteLine($"line {linenum} : {msg}");
        }

        public static int Main(string[] args)
        {
            string file;
            FileStream source;
            Console.WriteLine("\nMini compiler");
            if (args.Length >= 1)
                file = args[0];
            else
            {
                Console.Write("\nsource file:  ");
                file = Console.ReadLine();
            }
            try
            {
                var sr = new StreamReader(file);
                string str = sr.ReadToEnd();
                sr.Close();
                Compiler.source = new System.Collections.Generic.List<string>(str.Split(new string[] { "\r\n" }, System.StringSplitOptions.None));
                source = new FileStream(file, FileMode.Open);
            }
            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
                return 1;
            }

            Scanner scanner = new Scanner(source);
            Parser parser = new Parser(scanner);
            Console.WriteLine();
            sw = new StreamWriter(file + ".il");
            GenProlog();

            try
            {
                parser.Parse();
                syntaxTree.RemoveAll(el => el == null);
                syntaxTree.ForEach(n => n.СheckType());
                if (errors == 0)
                {
                    syntaxTree.ForEach(n => n.GenCode());
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"Parsing Failed\n {exc.Message}");
            }
            GenEpilog();
            sw.Close();
            source.Close();

            if (errors == 0)
                Console.WriteLine("COMPILATION SUCCESFUL\n");
            else
            {
                Console.WriteLine($"\n  {errors} errors detected\n");
                File.Delete(file + ".il");
            }
            return errors == 0 ? 0 : 2;
        }

        public static void PullStack(int linenum, string var_name)
        {
            if (!SymbolTable.ContainsKey(var_name))
                ReportError(linenum, "undeclared variable");

            EmitCode($"stloc v_{var_name}");
        }

        public static void PushStack(int linenum, string var_name)
        {
            if (!SymbolTable.ContainsKey(var_name))
                ReportError(linenum, "undeclared variable");

            EmitCode($"ldloc v_{var_name}");
        }

        public static void EmitCode(string instr = null) => sw.WriteLine(instr);

        public static void EmitCode(string instr, params object[] args) => sw.WriteLine(instr, args);

        private static StreamWriter sw;

        private static void GenProlog()
        {
            EmitCode(".assembly extern mscorlib { }");
            EmitCode(".assembly calculator { }");
            EmitCode(".method static void main()");
            EmitCode("{");
            EmitCode(".entrypoint");
            EmitCode(".try");
            EmitCode("{");
            EmitCode(".maxstack 64");
            EmitCode();
        }

        private static void GenEpilog()
        {
            EmitCode("leave EndMain");
            EmitCode("}");
            EmitCode("catch [mscorlib]System.Exception");
            EmitCode("{");
            EmitCode("callvirt instance string [mscorlib]System.Exception::get_Message()");
            EmitCode("call void [mscorlib]System.Console::WriteLine(string)");
            EmitCode("leave EndMain");
            EmitCode("}");
            EmitCode("EndMain: ret");
            EmitCode("}");
        }


        #region Base Nodes
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
        #endregion

        #region Statements and Instructions
        public abstract class AST
        {
            protected int linenum = -1;
            public abstract void GenCode();
            public abstract void СheckType();
        }

        public class Declare : AST
        {
            IdentType type;
            string varName;
            public Declare(string name, IdentType type, int linenum)
            {
                base.linenum = linenum;
                this.type = type;
                varName = name;
            }

            public override void GenCode()
            {
                switch (type)
                {
                    case IdentType.Int:
                    case IdentType.Bool:
                        Compiler.EmitCode($".locals init ( int32 v_{varName} )"); //declare
                        Compiler.EmitCode("ldc.i4.0"); //pushing 0 on stack
                        Compiler.PullStack(linenum, varName); //initialisation variable with 0 (linking varName with last value on stack)
                        break;
                    case IdentType.Double:
                        Compiler.EmitCode($".locals init ( float64 v_{varName} )"); //declare
                        Compiler.EmitCode("ldc.r8 0.0"); //pushing 0 on stack
                        Compiler.PullStack(linenum, varName); //initialisation variable with 0 (linking varName with last value on stack)
                        break;
                }
            }

            public override void СheckType()
            {
                if (Compiler.SymbolTable.ContainsKey(varName))
                {
                    Compiler.ReportError(linenum, "variable already declared");
                    return;
                }
                Compiler.SymbolTable[varName] = type;
            }
        }

        public class ExpStmt : AST
        {
            AST stmt;
            public ExpStmt(AST stmt) => this.stmt = stmt;

            public override void GenCode()
            {
                stmt.GenCode();
                Compiler.EmitCode("pop");
            }

            public override void СheckType() => stmt.СheckType();
        }

        public class Block : AST
        {
            List<AST> statements;
            public Block(List<AST> statements) => this.statements = statements;

            public override void GenCode() => statements?.ForEach(s => s.GenCode());

            public override void СheckType() => statements?.ForEach(s => s.СheckType());
        }

        public class Write : AST
        {
            Node value;
            public Write(Node value) => this.value = value;

            public override void GenCode()
            {
                switch (value.ExpOutType)
                {
                    case IdentType.Double:
                        Compiler.EmitCode("call class [mscorlib]System.Globalization.CultureInfo [mscorlib]System.Globalization.CultureInfo::get_InvariantCulture()");
                        Compiler.EmitCode("ldstr \"{0:0.000000}\"");
                        value.GenCode();
                        Compiler.EmitCode("box [mscorlib]System.Double");
                        Compiler.EmitCode("call string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider, string, object)");
                        Compiler.EmitCode("call void [mscorlib]System.Console::Write(string)");
                        break;

                    case IdentType.Int:
                        value.GenCode();
                        Compiler.EmitCode($"call void [mscorlib]System.Console::Write(int32)");
                        break;

                    case IdentType.Bool:
                        value.GenCode();
                        Compiler.EmitCode($"call void [mscorlib]System.Console::Write(bool)");
                        break;
                }
            }

            public override void СheckType() => value.СheckType();
        }

        public class WriteString : AST
        {
            string str;
            public WriteString(string str) => this.str = str;

            public override void GenCode()
            {
                Compiler.EmitCode($"ldstr {str}");
                Compiler.EmitCode($"call void [mscorlib]System.Console::Write(string)");
            }

            public override void СheckType()
            {
                return;
            }
        }

        public class Read : AST
        {
            LeafVarNode node;
            public Read(LeafVarNode node) => this.node = node;

            public override void GenCode()
            {
                Compiler.EmitCode($"call string [mscorlib]System.Console::ReadLine()");

                switch (node.ExpOutType)
                {
                    case IdentType.Int:
                        Compiler.EmitCode($"call int32 [mscorlib]System.Int32::Parse(string)");
                        break;
                    case IdentType.Bool:
                        Compiler.EmitCode($"call bool [mscorlib]System.Boolean::Parse(string)");
                        break;
                    case IdentType.Double:
                        Compiler.EmitCode($"call float64 [mscorlib]System.Double::Parse(string)");
                        break;
                    default:
                        Console.WriteLine($"unrecognized type {node.ExpOutType}");
                        return;
                }

                Compiler.PullStack(linenum, node.name);
            }

            public override void СheckType() => node.СheckType();
        }

        public class IfNode : AST
        {
            Node condition;
            AST body;
            AST elsebody;
            public IfNode(Node condition, AST body, AST elsebody = null, int linenum = -1)
            {
                this.condition = condition;
                this.body = body;
                this.elsebody = elsebody;
                base.linenum = linenum;
            }

            public override void GenCode()
            {
                var else_label = Compiler.GetLabel;
                var end_if_label = Compiler.GetLabel;

                condition.GenCode();                            // pushing to stack condition result
                Compiler.EmitCode($"brfalse {else_label}");     // IF
                body.GenCode();                                 // IF BODY
                Compiler.EmitCode($"br {end_if_label}");
                Compiler.EmitCode($"{else_label}:");
                elsebody?.GenCode();                            // ELSE
                Compiler.EmitCode($"{end_if_label}:");
            }

            public override void СheckType()
            {
                condition.СheckType();
                body.СheckType();
                elsebody?.СheckType();

                if (condition.ExpOutType != IdentType.Bool)
                    Compiler.ReportError(linenum, $"Semantic Error: Expected bool expression, got {condition.ExpOutType}");

            }
        }

        public class WhileNode : AST
        {
            Node condition;
            AST body;
            public WhileNode(Node condition, AST body, int linenum)
            {
                this.condition = condition;
                this.body = body;
                base.linenum = linenum;
            }

            public override void GenCode()
            {
                var while_start = Compiler.GetLabel;
                var while_end = Compiler.GetLabel;

                Compiler.EmitCode($"{while_start}:");
                condition.GenCode();                            // pushing to stack condition result
                Compiler.EmitCode($"brfalse {while_end}");     // WHILE
                body.GenCode();                                 // BODY
                Compiler.EmitCode($"br {while_start}");        // checking the condition one more time
                Compiler.EmitCode($"{while_end}:");
            }

            public override void СheckType()
            {
                condition.СheckType();
                body.СheckType();

                if (condition.ExpOutType != IdentType.Bool)
                    Compiler.ReportError(linenum, $"Semantic Error: Expected bool expression, got {condition.ExpOutType}");
            }
        }

        public class ReturnNode : AST
        {
            public override void GenCode() => Compiler.EmitCode("leave EndMain");

            public override void СheckType()
            {
                return;
            }
        }

        #endregion

        #region Expressions

        #region Binary Operators
        public class BitExpNode : BinaryNode
        {
            public BitExpNode(Node left, string op, Node right, int linenum) : base(left, op, right, linenum) { }
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
            public ArithmeticExpNode(Node left, string op, Node right, int linenum) : base(left, op, right, linenum) { }
            public override void СheckType()
            {
                base.СheckType();

                if (left.ExpOutType == IdentType.Bool || right.ExpOutType == IdentType.Bool)
                    ReportError();

                else if (left.ExpOutType == IdentType.Double || right.ExpOutType == IdentType.Double)
                {
                    exp_out_type = IdentType.Double;
                    if (left.ExpOutType == IdentType.Int)
                        left = new Convert(left, IdentType.Double, linenum);
                    if (right.ExpOutType == IdentType.Int)
                        right = new Convert(right, IdentType.Double, linenum);
                }
                else
                    exp_out_type = IdentType.Int;
            }
        }

        public class RelationalExpNode : BinaryNode
        {
            bool eq;
            public RelationalExpNode(Node left, string op, Node right, bool eq = false, int linenum = -1) : base(left, op, right, linenum) { this.eq = eq; }
            public override void GenCode()
            {
                base.GenCode();
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
                        left = new Convert(left, IdentType.Double, linenum);
                    if (right.ExpOutType == IdentType.Int)
                        right = new Convert(right, IdentType.Double, linenum);
                }
                else
                    exp_out_type = IdentType.Int;
            }
        }

        public class LogicalExpNode : BinaryNode
        {
            public LogicalExpNode(Node left, string op, Node right, int linenum) : base(left, op, right, linenum) { }
            public override IdentType ExpOutType => IdentType.Bool;
            public override void СheckType()
            {
                base.СheckType();

                if (left.ExpOutType != IdentType.Bool || right.ExpOutType != IdentType.Bool)
                    ReportError();
            }
            public override void GenCode()
            {
                var label = Compiler.GetLabel;

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
            public Assign(string to, Node node, int linenum) : base(linenum)
            {
                left_ident = to;
                right_node = node;
            }

            public override IdentType ExpOutType => Compiler.SymbolTable[left_ident];

            public override void GenCode()
            {
                right_node.GenCode();

                Compiler.EmitCode("dup");
                Compiler.PullStack(linenum, left_ident);
            }

            public override void СheckType()
            {
                right_node.СheckType();

                if (!Compiler.SymbolTable.ContainsKey(left_ident))
                {
                    Compiler.ReportError(linenum, "undeclared variable");
                    return; //?
                }

                IdentType assigntTo = Compiler.SymbolTable[left_ident];
                var assigntFrom = right_node.ExpOutType;
                if (assigntTo == IdentType.Double)
                {
                    if (assigntFrom == IdentType.Bool)
                        Compiler.ReportError(linenum, "Semantic Error: Expected int or double for assigment, got bool");
                    else
                        right_node = new Convert(right_node, IdentType.Double, linenum);
                }
                else if (assigntTo == IdentType.Int && assigntFrom != IdentType.Int)
                    Compiler.ReportError(linenum, $"Semantic Error: Expected int for assigment, got {assigntFrom}");
                else if (assigntTo == IdentType.Bool && assigntFrom != IdentType.Bool)
                    Compiler.ReportError(linenum, $"Semantic Error: Expected bool for assigment, got {assigntFrom}");

            }
        }

        public class Convert : Node
        {
            Node exp;
            IdentType typeTo;
            public Convert(Node exp, IdentType type_to, int linenum) : base(linenum)
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
            public UnaryMinus(Node exp, string op, int linenum) : base(exp, op, linenum) { }

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
            public BitNegation(Node exp, string op, int linenum) : base(exp, op, linenum) { }

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
            public LogicNegation(Node exp, string op, int linenum) : base(exp, op, linenum) { }

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


        #endregion
    }
}

