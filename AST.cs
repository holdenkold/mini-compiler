using System;
using System.Collections.Generic;

namespace mini_compiler
{
    public abstract class AST
    {
        public abstract void GenCode();
        public abstract void СheckType();
    }

    public class Declare : AST
    {
        IdentType type;
        string varName;
        public Declare(string name, IdentType type)
        {
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
                    Compiler.PullStack(varName); //initialisation variable with 0 (linking varName with last value on stack)
                    break;
                case IdentType.Double:
                    Compiler.EmitCode($".locals init ( float64 v_{varName} )"); //declare
                    Compiler.EmitCode("ldc.r8 0.0"); //pushing 0 on stack
                    Compiler.PullStack(varName); //initialisation variable with 0 (linking varName with last value on stack)
                    break;
            }
        }

        public override void СheckType()
        {
            if (Compiler.SymbolTable.ContainsKey(varName))
            {
                Compiler.errors += 1;
                Console.WriteLine("variable already declared");
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
                    //TO DO: should errors++?
                    return;
            }

            Compiler.PullStack(node.name);
        }

        public override void СheckType() => node.СheckType();
    }

    public class IfNode : AST
    {
        Node condition;
        AST body;
        AST elsebody;
        public IfNode(Node condition, AST body, AST elsebody = null)
        {
            this.condition = condition;
            this.body = body;
            this.elsebody = elsebody;
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
            {
                Compiler.errors += 1;
                Console.WriteLine($"Semantic Error: Expected bool expression, got {condition.ExpOutType}");
            }
        }
    }

    public class WhileNode : AST
    {
        Node condition;
        AST body;
        public WhileNode(Node condition, AST body)
        {
            this.condition = condition;
            this.body = body;
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
            {
                Compiler.errors += 1;
                Console.WriteLine($"Semantic Error: Expected bool expression, got {condition.ExpOutType}");
            }
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
}



