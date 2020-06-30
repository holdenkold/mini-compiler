
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
%type <node_list> declaration_list statement_list block_statement_list
%type <bnode> expression expression_statement primary_expression multiplicative_exp factor unary_exp relation_exp additive_exp bit_exp
%type <con> values


%%

start 
	: Program "{" declaration_list statement_list"}" {Compiler.syntaxTree.AddRange($3); Compiler.syntaxTree.AddRange($4);}
	| error  {Console.Write("line : "); Console.WriteLine(@1.StartLine);} ";"  start
	| start error  {Console.Write("line : "); Console.WriteLine(@1.StartLine);} ";" 
	;

declaration_list
	: declaration_list declaration {$1.Add($2);  $$ = $1;}
	| { $$ = new List<AST>();}
	;

declaration 
	: types Ident ";" {$$ = new Declare($2, $1.type, @1.StartLine);}
	| error  {Console.Write("line : "); Console.WriteLine(@1.StartLine);} ";" 
	;

statement_list
	: statement_list statement {$1.Add($2); $$ = $1;}
	| { $$ = new List<AST>();}
	;

expression_statement
	: Ident "=" expression_statement {$$ = new Assign($1, $3, @1.StartLine);}
	| expression  {$$ = $1;}
	;

statement
	: expression_statement ";" {$$ = new ExpStmt($1);}
	| write_expression ";" 
	| read_expression ";" 
	| "{" block_statement_list "}" {$$ = new Block($2==null? null : $2);}
	| Return ";" {$$ = new ReturnNode();}
	| If "(" expression_statement ")" statement {$$ = new IfNode($3, $5, null, @1.StartLine); }
	| If "(" expression_statement ")" statement Else statement  {$$ = new IfNode($3, $5, $7, @1.StartLine); }
	| While "(" expression_statement ")" statement {$$ = new WhileNode($3, $5, @1.StartLine);}
	| error  {Console.Write("line : "); Console.WriteLine(@1.StartLine);} ";"
	;

block_statement_list
	: block_statement_list statement {$1.Add($2); $$ = $1;}
	| { $$ = new List<AST>();}
	;

expression
	: relation_exp
	| expression "&&" relation_exp { $$ = new LogicalExpNode($1, "and", $3, @1.StartLine);}
	| expression "||" relation_exp { $$ = new LogicalExpNode($1, "or", $3, @1.StartLine);}
	;

relation_exp
	: additive_exp
	| relation_exp "==" additive_exp { $$ = new RelationalExpNode($1, "ceq", $3, false, @1.StartLine);}
	| relation_exp "!=" additive_exp { $$ = new RelationalExpNode($1, "ceq", $3, true, @1.StartLine);}
	| relation_exp ">" additive_exp { $$ = new RelationalExpNode($1, "cgt", $3, false, @1.StartLine);}
	| relation_exp ">=" additive_exp { $$ = new RelationalExpNode($1, "clt", $3, true, @1.StartLine);}
	| relation_exp "<" additive_exp { $$ = new RelationalExpNode($1, "clt", $3, false, @1.StartLine);}
	| relation_exp "<=" additive_exp { $$ = new RelationalExpNode($1, "cgt", $3, true, @1.StartLine);}
	;

additive_exp
	: multiplicative_exp 
	| additive_exp "+" multiplicative_exp { $$ = new ArithmeticExpNode($1, "add", $3, @1.StartLine);}
	| additive_exp "-" multiplicative_exp { $$ = new ArithmeticExpNode($1, "sub", $3, @1.StartLine);}
	;


multiplicative_exp
	: bit_exp
	| multiplicative_exp "*" bit_exp {$$ = new ArithmeticExpNode($1, "mul", $3, @1.StartLine);}
	| multiplicative_exp "/" bit_exp {$$ = new ArithmeticExpNode($1, "div", $3, @1.StartLine);}
	;

bit_exp
	: unary_exp
	| bit_exp "&" unary_exp {$$ = new BitExpNode($1, "and", $3, @1.StartLine);}
	| bit_exp "|" unary_exp {$$ = new BitExpNode($1, "or", $3, @1.StartLine);}
	;

unary_exp  
	: "-" unary_exp {$$ = new UnaryMinus($2, "neg", @1.StartLine);}
	| "~" unary_exp {$$ = new BitNegation($2, "not", @1.StartLine);}
	| "!" unary_exp {$$ = new LogicNegation($2, "ceq", @1.StartLine);}
	| "(" Int ")" unary_exp {$$ = new Convert($4, IdentType.Int, @1.StartLine);}
	| "(" Double ")" unary_exp {$$ = new Convert($4, IdentType.Double, @1.StartLine);}
	| factor
	;

factor
	: primary_expression
	| "(" expression_statement ")" {$$ = $2;}
	;

primary_expression
	: Ident {$$ = new LeafVarNode($1, @1.StartLine);}
	| values {$$ = new LeafValNode($1, @1.StartLine);}
	;

values  
	: IntNumber {$$ = new Constant(IdentType.Int, $1);}
	| RealNumber {$$ = new Constant(IdentType.Double, $1);}
	| BoolValue {$$ = new Constant(IdentType.Bool, $1);}
	;
	

write_expression 
	: Write expression_statement {$$ = new Write($2);}
	| Write String {$$ = new WriteString($2);}
	; 

read_expression
	: Read Ident {$$ = new Read(new LeafVarNode($2, @1.StartLine));}
	;

types 
	: Int {$$.type = IdentType.Int;}
	| Double {$$.type = IdentType.Double;}
	| Bool {$$.type = IdentType.Bool;}
	;

%%

public Parser(Scanner scanner) : base(scanner) { }