
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

%type <node> declaration statement
%type <node_list> declaration_list statement_list

%%

start  
	:  Program compound_statement {Console.WriteLine("Program started");}
	;

compound_statement
	: "{" declaration_list statement_list"}" 
	;

declaration_list
	: declaration_list declaration {$1.Add($2); $$ = $1;}
	| { $$ = new List<AST>();}
	;

declaration 
	: types Ident ";" {$$ = new Declare($2, $1.type); Console.WriteLine("declared: " + $2);}
	;

statement_list
	: statement_list statement {$1.Add($2); $$ = $1;}
	| { $$ = new List<AST>();}
	;

statement
	: Ident "=" expression ";" { var nd = new LeafNode($3.val); $$ = new Assign($1, nd.Evaluate()); Console.WriteLine($1); Console.WriteLine("="); Console.WriteLine(nd.Evaluate()); Console.WriteLine("\n");}
	| "{" statement_list "}"
	| expression ";"
	|  Return ";"
	| If "(" expression ")" "{" statement_list "}"
	| While "(" expression ")" "{" statement_list "}"
	;


expression
	: values 
	| 
	;

types 
	: Int {$$.type = IdentType.Int;}
	| Double {$$.type = IdentType.Double;}
	| Bool {$$.type = IdentType.Bool;}
	;

values 
	: Ident 
	| IntNumber
	| RealNumber
	| BoolValue
	;

%%

public Parser(Scanner scanner) : base(scanner) { }