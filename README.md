
# Requirements
- `gplex` and `gppg` parser generators
- C# 7.3

# Launching
1. `gplex` /out:Skaner.cs lexer.lex
2. `gppg` /gplex /out:Parser.cs parser.y

3. source file: `source`
4. ilasm `source`.il
5. `source`.exe

`source` - file in Mini specification [Mini specification](https://github.com/holdenkold/mini-compiler/blob/master/mini_%20specification.pdf)
