
%namespace mini_compiler

%union
{
	public string  val;
	public IdentType type;
	public List<AST> node_list;
	public AST node;
}


%token Program If Else While Read Write Return
%token Assign "=" LogicalSum "||" LogicalProduct "&&" BitSum "|" BitProduct "&" LogicalNeg "!" BitNeg "~"
%token Equals "==" NotEquals "!=" Greater ">" GreaterEquals ">=" Lesser "<" LesserEquals "<="
%token Plus "+" Minus "-" Multiplies "*" Divides "/"  OpenPar "(" ClosePar ")" OpenBr "{" CloseBr "}" Semicolon ";"

%token <val> Ident IntNumber RealNumber BoolValue String 
%token Int Double Bool

%type <node> declaration
%type <node_list> declaration_list

%%

start  
	:  Program compound_statement {Console.WriteLine("Program started");}
	;

compound_statement
	: "{" declaration_list "}" 
	;

declaration_list
	: declaration_list declaration {$1.Add($2); $$ = $1;}
	| { $$ = new List<AST>();}
	;

declaration 
	: types Ident ";" {$$ = new Declare($2, $1.type); Console.WriteLine($2);}
	;


types 
	: Int {$$.type = IdentType.Int;}
	| Double {$$.type = IdentType.Double;}
	| Bool {$$.type = IdentType.Bool;}
	;

%%

public Parser(Scanner scanner) : base(scanner) { }