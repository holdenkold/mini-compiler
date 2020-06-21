
%namespace mini_compiler

%union
{
	public string val;
	public IdentType type;
	public List<AST> node_list;
	public AST node;
	public Node exp_node;
}


%token Program If Else While Read Write Return
%token Assign "=" LogicalSum "||" LogicalProduct "&&" BitSum "|" BitProduct "&" LogicalNeg "!" BitNeg "~"
%token Equals "==" NotEquals "!=" Greater ">" GreaterEquals ">=" Lesser "<" LesserEquals "<="
%token Plus "+" Minus "-" Multiplies "*" Divides "/"  OpenPar "(" ClosePar ")" OpenBr "{" CloseBr "}" Semicolon ";"

%token <val> Ident IntNumber RealNumber BoolValue String
%token Int Double Bool

%type <node> declaration statement
%type <node_list> declaration_list statement_list
%type <exp_node> expression


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
	: Ident "=" expression ";" { Console.Write($1); Console.Write("="); Console.Write($3.Evaluate()); Console.WriteLine("\n");}
	| "{" statement_list "}" {Console.WriteLine("{ statement_list }");}
	| expression ";"
	| Return ";" {Console.WriteLine("return;");}
	| If "(" expression ")" "{" statement_list "}" {Console.WriteLine("IF");}
	| While "(" expression ")" "{" statement_list "}" {Console.WriteLine("WHILE");}
	| write_expression ";"
	;

expression
	: primary_expression {$$ = new LeafNode($1.val);}
	| expression "+" expression {$$ = new BinaryNode($1, "+",  $3);}
	;

write_expression 
	: Write primary_expression
	; 

types 
	: Int {$$.type = IdentType.Int;}
	| Double {$$.type = IdentType.Double;}
	| Bool {$$.type = IdentType.Bool;}
	;

primary_expression
	: Ident 
	| values {$$.val = $1.val;}
	;

values  
	: IntNumber
	| RealNumber 
	| BoolValue
	;

%%

public Parser(Scanner scanner) : base(scanner) { }