using QUT.Gppg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
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

            //Compiler.syntaxTree.Add(this);
        }

        public override void GenCode()
        {
            switch (type)
            {
                case IdentType.Int:
                    Compiler.EmitCode($".locals init ( int32 v_{varName} )"); //declare
                    Compiler.EmitCode("ldc.i4.0"); //pushing 0 on stack
                    Compiler.PullStack(varName); //initialisation variable with 0 (linking varName with last value on stack)
                    break;
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

    public class Assign : Node
    {
        string left_ident;
        Node right_node;
        bool covert = false;
        public Assign(string to, Node node)
        {
            left_ident = to;
            right_node = node;
           // Compiler.syntaxTree.Add(this);
        }

        public override string ExpOutType => right_node.ExpOutType;

        public override void GenCode()
        {
            right_node.GenCode();
            if (covert)
                Compiler.EmitCode("conv.r8");
            Compiler.PullStack(left_ident);
        }

        public override void СheckType()
        {
            right_node.СheckType();

            if (!Compiler.SymbolTable.ContainsKey(left_ident))
            {
                Compiler.errors += 1;
                Console.WriteLine("undeclared variable");
                return;
            }

            IdentType assigntTo = Compiler.SymbolTable[left_ident];
            string assigntFrom = right_node.ExpOutType;
            if (assigntTo == IdentType.Double )
            {
                if (assigntFrom == "bool")
                {
                    Compiler.errors += 1;
                    Console.WriteLine("Semantic Error: Expected int or double for assigment, got bool");
                }
                else
                    covert = true;
            }
            else if (assigntTo == IdentType.Int && assigntFrom != "int32")
            {
                Compiler.errors += 1;
                Console.WriteLine($"Semantic Error: Expected int for assigment, got {assigntFrom}");
            }
            else if (assigntTo == IdentType.Bool && assigntFrom != "bool")
            {
                Compiler.errors += 1;
                Console.WriteLine($"Semantic Error: Expected bool for assigment, got {assigntFrom}");
            }
        }
    }

    public class Block : AST
    {
        List<AST> statements;
        public Block(List<AST> statements) => this.statements = statements;

        public override void GenCode()
        {
            if(statements!= null)
                statements.ForEach(s => s.GenCode());
        }

        public override void СheckType()
        {
            if (statements != null)
                statements.ForEach(s => s.СheckType());
        }
    }

    public class Write : AST
    {
        Node value;
        public Write(Node value)
        {
            this.value = value;
            //Compiler.syntaxTree.Add(this);
        }

        public override void GenCode()
        {
            if (value.ExpOutType == "float64")
            {

                Compiler.EmitCode("call class [mscorlib]System.Globalization.CultureInfo [mscorlib]System.Globalization.CultureInfo::get_InvariantCulture()");
                Compiler.EmitCode("ldstr \"{0:0.000000}\"");
                value.GenCode();
                Compiler.EmitCode("box [mscorlib]System.Double");
                Compiler.EmitCode("call string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider, string, object)");
                Compiler.EmitCode("call void [mscorlib]System.Console::Write(string)");
            }
            else
            {
                value.GenCode();
                Compiler.EmitCode($"call void [mscorlib]System.Console::Write({value.ExpOutType})");
            }
        }

        public override void СheckType()
        {
            value.СheckType();
        }
    }

    public class WriteString : AST
    {
        string str;
        public WriteString(string str)
        {
            Console.WriteLine($"printing str: {str}");
            this.str = str;
            //Compiler.syntaxTree.Add(this);
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

    public class Read : AST
    {
        LeafVarNode node;
        public Read(LeafVarNode node)
        { 
            this.node = node;
            //Compiler.syntaxTree.Add(this);
        }

        public override void GenCode()
        {
            var type = char.ToUpper(node.ExpOutType[0]) + node.ExpOutType.Substring(1); //converting type, for ex: int32 -> Int32
            Compiler.EmitCode($"call string [mscorlib]System.Console::ReadLine()");
            Compiler.EmitCode($"call {node.ExpOutType} [mscorlib]System.{type}::Parse(string)");
            //Compiler.EmitCode($"stloc {node.name}");
            Compiler.PullStack(node.name);
        }

        public override void СheckType()
        {
            node.СheckType();
            return;
        }
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
           // Compiler.syntaxTree.Add(this);
        }

        public override void GenCode()
        {
            var else_label = $"L{Compiler.label_num++}";
            var end_if_label = $"L{Compiler.label_num++}";
            condition.GenCode(); //pushing to stack condition result
            Compiler.EmitCode($"brfalse {else_label}");
            body.GenCode();
            Compiler.EmitCode($"br {end_if_label}");

            Compiler.EmitCode($"{else_label}:");
            if (elsebody != null)  // ELSE
                elsebody.GenCode();

            Compiler.EmitCode($"{end_if_label}:");
        }

        public override void СheckType()
        {
            if (condition.ExpOutType != "bool")
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
            //Compiler.syntaxTree.Add(this);
        }

        public override void GenCode()
        {
            int while_start = Compiler.label_num++;
            int while_end = Compiler.label_num++;

            Compiler.EmitCode($"L{while_start}:");
            condition.GenCode(); //pushing to stack condition result
            Compiler.EmitCode($"brfalse L{while_end}");
                body.GenCode();
                Compiler.EmitCode($"br L{while_start}");                

            Compiler.EmitCode($"L{while_end}:");
        }

        public override void СheckType()
        {
            if (condition.ExpOutType != "bool")
            {
                Compiler.errors += 1;
                Console.WriteLine($"Semantic Error: Expected bool expression, got {condition.ExpOutType}");
            }
        }
    }
}



