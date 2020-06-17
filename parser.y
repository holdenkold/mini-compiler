
// Uwaga: W wywołaniu generatora gppg należy użyć opcji /gplex

%namespace GardensPoint

%union
{
public string  val;
public char    type;
}

%token Program If Else While Read Write Return
%token Assign LogicalSum LogicalProduct BitSum BitProduct Equals NotEquals Greater GreaterEquals Lesser LesserEquals
%token Plus Minus Multiplies Divides LogicalNeg BitNeg OpenPar ClosePar OpenBr CloseBr Semicolon
%token <val> Ident IntNumber RealNumber BoolValue StringValue Ident 

%type <type> Int Double Bool String

%%

start     : start line { ++lineno; }
          | line { ++lineno; }
          ;

line      : Print
               {
               Compiler.EmitCode("// linia {0,3} :  "+Compiler.source[lineno-1],lineno);
               Compiler.EmitCode("ldstr \"  Result: {0}{1}\"");
               }
            exp end
               {
               Compiler.EmitCode("box [mscorlib]System.{0}",$3=='i'?"Int32":"Double");
               Compiler.EmitCode("ldstr \"{0}\"",$3=='i'?"i":"r");
               Compiler.EmitCode("call void [mscorlib]System.Console::WriteLine(string,object,object)");
               Compiler.EmitCode("");
               }
          | Ident Assign
               {
               Compiler.EmitCode("// linia {0,3} :  "+Compiler.source[lineno-1],lineno);
               }
            exp end
               {
               if ( $1[0]=='@' && $4!='i' )
                   {
                   Console.WriteLine("  line {0,3}:  semantic error - cannot convert real to int",lineno);
                   ++Compiler.errors;
                   }
               else
                   {
                   if ( $1[0]=='$' && $4!='r' )
                       Compiler.EmitCode("conv.r8");
                   Compiler.EmitCode("stloc _{0}{1}", $1[0]=='@'?'i':'r', $1[1]);
                   Compiler.EmitCode("");
                   }
               }
          | Exit Endl
               {
               Compiler.EmitCode("// linia {0,3} :  "+Compiler.source[lineno-1],lineno);
               Compiler.EmitCode("ldstr \"\\nEnd of execution\\n\"");
               Compiler.EmitCode("call void [mscorlib]System.Console::WriteLine(string)");
               Compiler.EmitCode("");
               YYACCEPT;
               }
          | Exit Eof
               {
               Compiler.EmitCode("// linia {0,3} :  "+Compiler.source[lineno-1],lineno);
               Compiler.EmitCode("ldstr \"\\nEnd of execution\\n\"");
               Compiler.EmitCode("call void [mscorlib]System.Console::WriteLine(string)");
               Compiler.EmitCode("");
               YYACCEPT;
               }
          | error Endl
               {
               Console.WriteLine("  line {0,3}:  syntax error",lineno);
               ++Compiler.errors;
               yyerrok();
               }
          | error Eof
               {
               Console.WriteLine("  line {0,3}:  syntax error",lineno);
               ++Compiler.errors;
               yyerrok();
               YYACCEPT;
               }
          | Eof
               {
               Console.WriteLine("  line {0,3}:  syntax error - unexpected symbol Eof",lineno);
               ++Compiler.errors;
               yyerrok();
               YYACCEPT;
               }
          ;

end       : Endl
          | Eof
               {
               Console.WriteLine("  line {0,3}:  syntax error - unexpected symbol Eof",lineno);
               ++Compiler.errors;
               yyerrok();
               YYACCEPT;
               }
          ;

exp       : exp Plus term
               { $$ = BinaryOpGenCode(Tokens.Plus, $1, $3); }
          | exp Minus term
               { $$ = BinaryOpGenCode(Tokens.Minus, $1, $3); }
          | term
               { $$ = $1; }
          ;

term      : term Multiplies factor
               { $$ = BinaryOpGenCode(Tokens.Multiplies, $1, $3); }
          | term Divides factor
               { $$ = BinaryOpGenCode(Tokens.Divides, $1, $3); }
          | factor
               { $$ = $1; }
          ;

factor    : OpenPar exp ClosePar
               { $$ = $2; }
          | IntNumber
               {
               Compiler.EmitCode("ldc.i4 {0}",int.Parse($1));
               $$ = 'i'; 
               }
          | RealNumber
               {
               double d = double.Parse($1,System.Globalization.CultureInfo.InvariantCulture) ;
               Compiler.EmitCode(string.Format(System.Globalization.CultureInfo.InvariantCulture,"ldc.r8 {0}",d));
               $$ = 'r'; 
               }
          | Ident
               {
               Compiler.EmitCode("ldloc _{0}{1}", $1[0]=='@'?'i':'r', $1[1]);
               $$ = $1[0]=='@'?'i':'r';
               }
          ;

%%

int lineno=1;

public Parser(Scanner scanner) : base(scanner) { }

private char BinaryOpGenCode(Tokens t, char type1, char type2)
    {
    char type = ( type1=='i' && type2=='i' ) ? 'i' : 'r' ;
    if ( type1!=type )
        {
        Compiler.EmitCode("stloc temp");
        Compiler.EmitCode("conv.r8");
        Compiler.EmitCode("ldloc temp");
        }
    if ( type2!=type )
        Compiler.EmitCode("conv.r8");
    switch ( t )
        {
        case Tokens.Plus:
            Compiler.EmitCode("add");
            break;
        case Tokens.Minus:
            Compiler.EmitCode("sub");
            break;
        case Tokens.Multiplies:
            Compiler.EmitCode("mul");
            break;
        case Tokens.Divides:
            Compiler.EmitCode("div");
            break;
        default:
            Console.WriteLine($"  line {lineno,3}:  internal gencode error");
            ++Compiler.errors;
            break;
        }
    return type;
    }