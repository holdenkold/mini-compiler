
gplex /out:Skaner.cs lexer.lex
gppg /gplex /out:Parser.cs parser.y

source file: code.txt
ilasm code.txt.il
code.txt.exe
