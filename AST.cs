using System;
using System.Data;
using System.IO;
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
        }

        public override void GenCode()
        {
            switch (type)
            {
                case IdentType.Int:
                case IdentType.Bool:
                    Compiler.EmitCode($".locals init ( int32 _i{varName} )"); //declare
                    Compiler.EmitCode("ldc.i4.0"); //pushing 0 on stack
                    Compiler.EmitCode($"stloc {varName}"); //initialisation variable with 0 (linking varName with last value on stack)
                    break;

                case IdentType.Double:
                    Compiler.EmitCode($".locals init ( float32 _i{varName} )"); //declare
                    Compiler.EmitCode("ldc.r4.0"); //pushing 0 on stack
                    Compiler.EmitCode($"stloc {varName}"); //initialisation variable with 0 (linking varName with last value on stack)
                    break;
            }
        }
    }

    public class Assign : AST
    {
        string left_ident;
        string right_value;
        public Assign(string to, string value)
        {
            left_ident = to;
            right_value = value;
        }

        public override void GenCode()
        {
            Compiler.EmitCode($"ldc.r4 {right_value}");
            Compiler.EmitCode($"stloc {left_ident}");
        }
    }

   
}



