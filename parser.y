
%namespace GardensPoint

%union
{
public string  val;
public char    type;
}

%token Program If Else While Read Write Return
%token Assign "=" LogicalSum "||" LogicalProduct "&&" BitSum "|" BitProduct "&" LogicalNeg "!" BitNeg "~"
%token Equals "==" NotEquals "!=" Greater ">" GreaterEquals ">=" Lesser "<" LesserEquals "<="
%token Plus "+" Minus "-" Multiplies "*" Divides "/"  OpenPar "(" ClosePar ")" OpenBr "{" CloseBr "}" Semicolon ";"
%token Eof Endl

%token <val> Ident IntNumber RealNumber BoolValue String Ident 
%token Int Double Bool

%%

start  :  Program compound_statement Eof;

compound_statement
	: "{" "}"
	| "{" declaration_list "}"
	;

declaration_list
	: declaration
	| declaration_list declaration
	;

declaration 
	: types Ident ";"
	;

types 
	: Int
	| Double
	| Bool
	;

%%

public Parser(Scanner scanner) : base(scanner) { }