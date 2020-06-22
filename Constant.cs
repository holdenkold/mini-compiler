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
                    Compiler.EmitCode($"ldstr \"{value}\"");
                    break;
                case IdentType.Double:
                    Compiler.EmitCode($"ldc.r4 {value}");
                    break;
            }           
        }
    }
}
