using System;
using System.IO;
using System.Collections.Generic;
using mini_compiler;

public class Compiler
{
    public static int errors = 0;

    public static List<string> source;
    public static AST root;
    public static Dictionary<string, IdentType> SymbolTable = new Dictionary<string, IdentType>();

    public static Dictionary<IdentType, string> IdentTypeMap = new Dictionary<IdentType, string> {
        { IdentType.Int, "int32" }, { IdentType.Double, "float64" }, { IdentType.Bool, "bool" }
    };

    public static int label_num = 0;

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
        parser.Parse();

        syntaxTree.RemoveAll(el => el == null);

        syntaxTree.ForEach(n => n.СheckType());
        if (errors == 0)
        {
            root.GenCode();
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


    public static void PullStack(string var_name)
    {
        if (!SymbolTable.ContainsKey(var_name))
        {
            errors++;
            Console.WriteLine("undeclared variable");

        }
        EmitCode($"stloc v_{var_name}");
    }

    public static void PushStack(string var_name)
    {
        if (!SymbolTable.ContainsKey(var_name))
        {
            errors++;
            Console.WriteLine("undeclared variable");

        }
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
}
