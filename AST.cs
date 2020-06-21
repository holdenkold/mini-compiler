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
    }

    public enum IdentType
    {
        Int,
        Double,
        Bool
    }

    public class Declare : AST
    {
        IdentType type;
        string varName;
        public Declare(string name, IdentType type)
        {
            this.type = type;
            varName = name;
            GenCode();
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
    }

    public class Assign : AST
    {
        string left_ident;
        double right_value;
        public Assign(string to, Node node)
        {
            left_ident = to;
            right_value = node.Evaluate();

            GenCode();
        }

        public override void GenCode()
        {
            Compiler.EmitCode($"ldc.r4 {right_value}");
            //if (right_value.All(Char.IsDigit)) // case when int/double/bool provided
            //{
            //    Compiler.EmitCode($"ldc.i4 {right_value}");
            //}
            //else // case when variable name provided
            //{
            //    Compiler.EmitCode($"ldloc {right_value}");
            //}

            Compiler.EmitCode($"stloc {left_ident}");
        }
    }

    public class Write : AST
    {
        double value;
        public Write(string value)
        {

            this.value = double.Parse(value);
            GenCode();
        }

        public override void GenCode()
        {
            //if (value.All(Char.IsDigit)) // case when int/double/bool provided
            //{
                
                Compiler.EmitCode($"ldc.r4 {value}");
            //}
            //else // case when variable name provided
            //{
            //    Compiler.EmitCode($"ldloc {value}");
            //}
            
            Compiler.EmitCode($"call void [System.Console]System.Console::Write(float32)");
        }
    }

    public class WriteString : AST
    {
        string str;
        public WriteString(string str)
        {
            Console.WriteLine($"printing str: {str}");
            this.str = str;
            GenCode();
        }

        public override void GenCode()
        {
            Compiler.EmitCode($"ldstr {str}");
            Compiler.EmitCode($"call void [System.Console]System.Console::Write(string)");
        }
    }

}



