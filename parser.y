﻿
%namespace mini_compiler

%union
{
	public string val;
	public IdentType type;
	public List<AST> node_list;
	public AST node;
	public Constant con;
	public Node bnode;
}


%token Program If Else While Read Write Return
%token Assign "=" LogicalSum "||" LogicalProduct "&&" BitSum "|" BitProduct "&" LogicalNeg "!" BitNeg "~"
%token Equals "==" NotEquals "!=" Greater ">" GreaterEquals ">=" Lesser "<" LesserEquals "<="
%token Plus "+" Minus "-" Multiplies "*" Divides "/"  OpenPar "(" ClosePar ")" OpenBr "{" CloseBr "}" Semicolon ";"

%token <val> Ident IntNumber RealNumber BoolValue String
%token Int Double Bool

%type <node> declaration statement write_expression read_expression
%type <node_list> declaration_list statement_list
%type <bnode> expression primary_expression term
%type <con> values


%%

starth  
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
	: Ident "=" expression ";" { $$ = new Assign($1, $3);}
	| write_expression ";"
	| read_expression ";"
	| "{" statement_list "}"
	| expression ";"
	| Return ";" {Console.WriteLine("return;");}
	| If "(" expression ")" "{" statement_list "}" {Console.WriteLine("IF");}
	| While "(" expression ")" "{" statement_list "}" {Console.WriteLine("WHILE");}
	;

expression
	: term 
	| expression "+" term { $$ = new BinaryNode($1, "add", $3);}
	| expression "-" term { $$ = new BinaryNode($1, "sub", $3);}
	;

term
	: primary_expression
	| "(" expression ")" {$$ = $2;}
	| term "*" primary_expression {$$ = new BinaryNode($1, "mul", $3);}
	| term "/" primary_expression {$$ = new BinaryNode($1, "div", $3);}
	;

write_expression 
	: Write expression {$$ = new Write($2);}
	| Write String {$$ = new WriteString($2); Console.WriteLine($2);}
	; 

read_expression
	: Read Ident {$$ = new Read(new LeafVarNode($2));}
	;

types 
	: Int {$$.type = IdentType.Int;}
	| Double {$$.type = IdentType.Double;}
	| Bool {$$.type = IdentType.Bool;}
	;

primary_expression
	: Ident {$$ = new LeafVarNode($1);}
	| values {$$ = new LeafValNode($1);}
	;

values  
	: IntNumber {$$ = new Constant(IdentType.Int, $1);}
	| RealNumber {$$ = new Constant(IdentType.Double, $1);}
	| BoolValue {$$ = new Constant(IdentType.Bool, $1);}
	;

%%

public Parser(Scanner scanner) : base(scanner) { }