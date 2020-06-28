
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

%type <node> declaration statement write_expression read_expression expression_statement
%type <node_list> declaration_list statement_list
%type <bnode> expression assign_statement primary_expression multiplicative_exp factor unary_exp relation_exp additive_exp bit_exp
%type <con> values


%%

start 
	:  Program "{" declaration_list statement_list"}"
	;

declaration_list
	: declaration_list declaration {$1.Add($2); $$ = $1;}
	| { $$ = new List<AST>();}
	;

declaration 
	: types Ident ";" {$$ = new Declare($2, $1.type);}
	;

statement_list
	: statement_list statement {$1.Add($2); $$ = $1;}
	| { $$ = new List<AST>();}
	;


expression_statement
	: ";"
	| expression ";" {$$ = $1;}
	;

assign_statement
	: Ident "=" expression { $$ = new Assign($1, $3);}
	| Ident "=" assign_statement {$$ = new Assign($1, $3);}
	;


statement
	: assign_statement ";" {$$ = $1; Compiler.syntaxTree.Add($1); Console.WriteLine("assign:");Console.WriteLine($$);}
	| write_expression ";" {Compiler.syntaxTree.Add($1); Console.WriteLine("write:");Console.WriteLine($$);}
	| read_expression ";" {Compiler.syntaxTree.Add($1); Console.WriteLine("read:");Console.WriteLine($$);}
	| expression_statement  {Compiler.syntaxTree.Add($1); Console.WriteLine("expr:");Console.WriteLine($$);}
	| "{" statement_list "}" {$$ = new Block($2==null? null : $2); Compiler.syntaxTree.Add($$);}
	| Return ";" {$$ = new ReturnNode(); Console.WriteLine("return;");}
	| If "(" expression ")" statement {$$ = new IfNode($3, $5); Compiler.syntaxTree.Add($$);}
	| If "(" expression ")" statement Else statement  {$$ = new IfNode($3, $5, $7); Compiler.syntaxTree.Add($$);}
	| While "(" expression ")" statement {$$ = new WhileNode($3, $5); Compiler.syntaxTree.Add($$); Console.WriteLine("while:");Console.WriteLine($5);}
	;

expression
	: relation_exp
	| expression "&&" relation_exp { $$ = new LogicalExpNode($1, "and", $3);}
	| expression "||" relation_exp { $$ = new LogicalExpNode($1, "or", $3);}
	;

relation_exp
	: additive_exp
	| relation_exp "==" additive_exp { $$ = new RelationalExpNode($1, "ceq", $3);}
	| relation_exp "!=" additive_exp { $$ = new RelationalExpNode($1, "ceq", $3, true);}
	| relation_exp ">" additive_exp { $$ = new RelationalExpNode($1, "cgt", $3);}
	| relation_exp ">=" additive_exp { $$ = new RelationalExpNode($1, "clt", $3, true);}
	| relation_exp "<" additive_exp { $$ = new RelationalExpNode($1, "clt", $3);}
	| relation_exp "<=" additive_exp { $$ = new RelationalExpNode($1, "cgt", $3, true);}
	;

additive_exp
	: multiplicative_exp 
	| additive_exp "+" multiplicative_exp { $$ = new ArithmeticExpNode($1, "add", $3);}
	| additive_exp "-" multiplicative_exp { $$ = new ArithmeticExpNode($1, "sub", $3);}
	;


multiplicative_exp
	: bit_exp
	| multiplicative_exp "*" bit_exp {$$ = new ArithmeticExpNode($1, "mul", $3);}
	| multiplicative_exp "/" bit_exp {$$ = new ArithmeticExpNode($1, "div", $3);}
	;

bit_exp
	: unary_exp
	| bit_exp "&" unary_exp {$$ = new BitExpNode($1, "and", $3);}
	| bit_exp "|" unary_exp {$$ = new BitExpNode($1, "or", $3);}
	;

unary_exp  
	: "-" factor {$$ = new UnaryMinus($2, "neg");}
	| "~" factor {$$ = new BitNegation($2, "not");}
	| "!" factor {$$ = new LogicNegation($2, "ceq");}
	| "(" Int ")" factor {$$ = new ConvertToInt($4);}
	| "(" Double ")" factor {$$ = new ConvertToDouble($4);}
	| factor
	;

factor
	: primary_expression
	| "(" expression ")" {$$ = $2;}
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
	

write_expression 
	: Write expression {$$ = new Write($2);}
	| Write String {$$ = new WriteString($2);}
	; 

read_expression
	: Read Ident {$$ = new Read(new LeafVarNode($2));}
	;

types 
	: Int {$$.type = IdentType.Int;}
	| Double {$$.type = IdentType.Double;}
	| Bool {$$.type = IdentType.Bool;}
	;


%%

public Parser(Scanner scanner) : base(scanner) { }