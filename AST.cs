using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml.Schema;

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

            Compiler.SymbolTable[name] = type;
            Compiler.syntaxTree.Add(this);
            //GenCode();
        }

        public override void GenCode()
        {
            switch (type)
            {
                case IdentType.Int:
                    Compiler.EmitCode($".locals init ( int32 {varName} )"); //declare
                    Compiler.EmitCode("ldc.i4.0"); //pushing 0 on stack
                    Compiler.EmitCode($"stloc {varName}"); //initialisation variable with 0 (linking varName with last value on stack)
                    break;
                case IdentType.Bool:
                    Compiler.EmitCode($".locals init ( int32 {varName} )"); //declare
                    Compiler.EmitCode("ldc.i4.0"); //pushing 0 on stack
                    Compiler.EmitCode($"stloc {varName}"); //initialisation variable with 0 (linking varName with last value on stack)
                    break;
                case IdentType.Double:
                    Compiler.EmitCode($".locals init ( float64 {varName} )"); //declare
                    Compiler.EmitCode("ldc.r8 0.0"); //pushing 0 on stack
                    Compiler.EmitCode($"stloc {varName}"); //initialisation variable with 0 (linking varName with last value on stack)
                    break;
            }
        }

        public override void СheckType()
        {
            return;
        }
    }

    public class Assign : AST
    {
        string left_ident;
        AST right_node;
        public Assign(string to, AST node)
        {
            left_ident = to;
            right_node = node;
            Compiler.syntaxTree.Add(this);
            //GenCode();
        }

        public override void GenCode()
        {
            right_node.GenCode();
            Compiler.EmitCode($"stloc {left_ident}");
        }

        public override void СheckType()
        {
            return;
        }
    }

    public class Write : AST
    {
        Node value;
        public Write(Node value)
        {
            Console.WriteLine($"printing var:");
            this.value = value;
            Compiler.syntaxTree.Add(this);
            //GenCode();
        }

        public override void GenCode()
        {
            value.GenCode();
            Compiler.EmitCode($"call void [mscorlib]System.Console::Write({value.ExpType})");
        }

        public override void СheckType()
        {
            return;
        }
    }

    public class WriteString : AST
    {
        string str;
        public WriteString(string str)
        {
            Console.WriteLine($"printing str: {str}");
            this.str = str;
            Compiler.syntaxTree.Add(this);
            //GenCode();
        }

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

}



