%using QUT.Gppg;
%namespace GardensPoint

IntNumber   ([0-9]|[1-9][0-9]*)
RealNumber  ([0-9]\.[0-9]+|[1-9][0-9]*\.[0-9]+)
Ident       ([A-Za-z][0-9]*[A-Za-z]*)*
Comment		(\/\/.*\n)
String		\"[^\n\r]*\"

%%

"program"       { return (int)Tokens.Program; }
"if"            { return (int)Tokens.If; }
"else"          { return (int)Tokens.Else; }
"while"         { return (int)Tokens.While; }
"read"          { return (int)Tokens.Read; }
"write"         { return (int)Tokens.Write; }
"return"        { return (int)Tokens.Return; }
"int"           { return (int)Tokens.Int; }
"double"        { return (int)Tokens.Double; }
"bool"          { return (int)Tokens.Bool; }

"true"          { yylval.val=yytext; return (int)Tokens.BoolValue; }
"false"         { yylval.val=yytext; return (int)Tokens.BoolValue; }
{IntNumber}     { yylval.val=yytext; return (int)Tokens.IntNumber; }
{RealNumber}    { yylval.val=yytext; return (int)Tokens.RealNumber; }
{Ident}         { yylval.val=yytext; return (int)Tokens.Ident; }
{String}        { return (int)Tokens.String; }

<<EOF>>         { return (int)Tokens.Eof; }
"\r"            { return (int)Tokens.Endl; }
" "             { }
"\t"            { }

"="             { return (int)Tokens.Assign; }
"||"            { return (int)Tokens.LogicalSum; } 
"&&"            { return (int)Tokens.LogicalProduct; }
"|"             { return (int)Tokens.BitSum; }
"&"             { return (int)Tokens.BitProduct; }
"=="            { return (int)Tokens.Equals; }
"!="            { return (int)Tokens.NotEquals; }
">"             { return (int)Tokens.Greater; }
">="            { return (int)Tokens.GreaterEquals; }
"<"             { return (int)Tokens.Lesser; }
"<="            { return (int)Tokens.LesserEquals; }
"+"             { return (int)Tokens.Plus; }
"-"             { return (int)Tokens.Minus; }
"*"             { return (int)Tokens.Multiplies; }
"/"             { return (int)Tokens.Divides; }
"!"             { return (int)Tokens.LogicalNeg; }
"~"             { return (int)Tokens.BitNeg; }
"("             { return (int)Tokens.OpenPar; }
")"             { return (int)Tokens.ClosePar; }
"{"             { return (int)Tokens.OpenBr; }
"}"             { return (int)Tokens.CloseBr; }
";"             { return (int)Tokens.Semicolon; }
