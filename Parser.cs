// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  POLINAGRINK1DDB
// DateTime: 6/20/2020 9:45:22 PM
// UserName: polinagrinko
// Input file <parser.y - 6/20/2020 9:45:19 PM>

// options: lines gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace mini_compiler
{
public enum Tokens {error=2,EOF=3,Program=4,If=5,Else=6,
    While=7,Read=8,Write=9,Return=10,Assign=11,LogicalSum=12,
    LogicalProduct=13,BitSum=14,BitProduct=15,LogicalNeg=16,BitNeg=17,Equals=18,
    NotEquals=19,Greater=20,GreaterEquals=21,Lesser=22,LesserEquals=23,Plus=24,
    Minus=25,Multiplies=26,Divides=27,OpenPar=28,ClosePar=29,OpenBr=30,
    CloseBr=31,Semicolon=32,Ident=33,IntNumber=34,RealNumber=35,BoolValue=36,
    String=37,Int=38,Double=39,Bool=40};

public struct ValueType
#line 5 "parser.y"
{
	public string  val;
	public IdentType type;
	public List<AST> node_list;
	public AST node;
}
#line default
// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

// Utility class for encapsulating token information
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public class ScanObj {
  public int token;
  public ValueType yylval;
  public LexLocation yylloc;
  public ScanObj( int t, ValueType val, LexLocation loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public class Parser: ShiftReduceParser<ValueType, LexLocation>
{
#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[26];
  private static State[] states = new State[52];
  private static string[] nonTerms = new string[] {
      "declaration", "statement", "declaration_list", "statement_list", "start", 
      "$accept", "compound_statement", "types", "expression", "values", };

  static Parser() {
    states[0] = new State(new int[]{4,3},new int[]{-5,1});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{30,5},new int[]{-7,4});
    states[4] = new State(-2);
    states[5] = new State(-5,new int[]{-3,6});
    states[6] = new State(new int[]{38,49,39,50,40,51,31,-8,33,-8,30,-8,34,-8,35,-8,36,-8,10,-8,5,-8,7,-8,9,-8,32,-8},new int[]{-4,7,-1,45,-8,46});
    states[7] = new State(new int[]{31,8,33,10,30,19,34,16,35,17,36,18,10,24,5,26,7,33,9,40,32,-18},new int[]{-2,9,-9,22,-10,14});
    states[8] = new State(-3);
    states[9] = new State(-7);
    states[10] = new State(new int[]{11,11,32,-22});
    states[11] = new State(new int[]{33,15,34,16,35,17,36,18,32,-18},new int[]{-9,12,-10,14});
    states[12] = new State(new int[]{32,13});
    states[13] = new State(-9);
    states[14] = new State(-17);
    states[15] = new State(-22);
    states[16] = new State(-23);
    states[17] = new State(-24);
    states[18] = new State(-25);
    states[19] = new State(-8,new int[]{-4,20});
    states[20] = new State(new int[]{31,21,33,10,30,19,34,16,35,17,36,18,10,24,5,26,7,33,9,40,32,-18},new int[]{-2,9,-9,22,-10,14});
    states[21] = new State(-10);
    states[22] = new State(new int[]{32,23});
    states[23] = new State(-11);
    states[24] = new State(new int[]{32,25});
    states[25] = new State(-12);
    states[26] = new State(new int[]{28,27});
    states[27] = new State(new int[]{33,15,34,16,35,17,36,18,29,-18},new int[]{-9,28,-10,14});
    states[28] = new State(new int[]{29,29});
    states[29] = new State(new int[]{30,30});
    states[30] = new State(-8,new int[]{-4,31});
    states[31] = new State(new int[]{31,32,33,10,30,19,34,16,35,17,36,18,10,24,5,26,7,33,9,40,32,-18},new int[]{-2,9,-9,22,-10,14});
    states[32] = new State(-13);
    states[33] = new State(new int[]{28,34});
    states[34] = new State(new int[]{33,15,34,16,35,17,36,18,29,-18},new int[]{-9,35,-10,14});
    states[35] = new State(new int[]{29,36});
    states[36] = new State(new int[]{30,37});
    states[37] = new State(-8,new int[]{-4,38});
    states[38] = new State(new int[]{31,39,33,10,30,19,34,16,35,17,36,18,10,24,5,26,7,33,9,40,32,-18},new int[]{-2,9,-9,22,-10,14});
    states[39] = new State(-14);
    states[40] = new State(new int[]{37,43,33,15,34,16,35,17,36,18},new int[]{-10,41});
    states[41] = new State(new int[]{32,42});
    states[42] = new State(-15);
    states[43] = new State(new int[]{32,44});
    states[44] = new State(-16);
    states[45] = new State(-4);
    states[46] = new State(new int[]{33,47});
    states[47] = new State(new int[]{32,48});
    states[48] = new State(-6);
    states[49] = new State(-19);
    states[50] = new State(-20);
    states[51] = new State(-21);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-6, new int[]{-5,3});
    rules[2] = new Rule(-5, new int[]{4,-7});
    rules[3] = new Rule(-7, new int[]{30,-3,-4,31});
    rules[4] = new Rule(-3, new int[]{-3,-1});
    rules[5] = new Rule(-3, new int[]{});
    rules[6] = new Rule(-1, new int[]{-8,33,32});
    rules[7] = new Rule(-4, new int[]{-4,-2});
    rules[8] = new Rule(-4, new int[]{});
    rules[9] = new Rule(-2, new int[]{33,11,-9,32});
    rules[10] = new Rule(-2, new int[]{30,-4,31});
    rules[11] = new Rule(-2, new int[]{-9,32});
    rules[12] = new Rule(-2, new int[]{10,32});
    rules[13] = new Rule(-2, new int[]{5,28,-9,29,30,-4,31});
    rules[14] = new Rule(-2, new int[]{7,28,-9,29,30,-4,31});
    rules[15] = new Rule(-2, new int[]{9,-10,32});
    rules[16] = new Rule(-2, new int[]{9,37,32});
    rules[17] = new Rule(-9, new int[]{-10});
    rules[18] = new Rule(-9, new int[]{});
    rules[19] = new Rule(-8, new int[]{38});
    rules[20] = new Rule(-8, new int[]{39});
    rules[21] = new Rule(-8, new int[]{40});
    rules[22] = new Rule(-10, new int[]{33});
    rules[23] = new Rule(-10, new int[]{34});
    rules[24] = new Rule(-10, new int[]{35});
    rules[25] = new Rule(-10, new int[]{36});

    aliases = new Dictionary<int, string>();
    aliases.Add(11, "=");
    aliases.Add(12, "||");
    aliases.Add(13, "&&");
    aliases.Add(14, "|");
    aliases.Add(15, "&");
    aliases.Add(16, "!");
    aliases.Add(17, "~");
    aliases.Add(18, "==");
    aliases.Add(19, "!=");
    aliases.Add(20, ">");
    aliases.Add(21, ">=");
    aliases.Add(22, "<");
    aliases.Add(23, "<=");
    aliases.Add(24, "+");
    aliases.Add(25, "-");
    aliases.Add(26, "*");
    aliases.Add(27, "/");
    aliases.Add(28, "(");
    aliases.Add(29, ")");
    aliases.Add(30, "{");
    aliases.Add(31, "}");
    aliases.Add(32, ";");
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 2: // start -> Program, compound_statement
#line 27 "parser.y"
                               {Console.WriteLine("Program started");}
#line default
        break;
      case 4: // declaration_list -> declaration_list, declaration
#line 35 "parser.y"
                                {ValueStack[ValueStack.Depth-2].node_list.Add(ValueStack[ValueStack.Depth-1].node); CurrentSemanticValue.node_list = ValueStack[ValueStack.Depth-2].node_list;}
#line default
        break;
      case 5: // declaration_list -> /* empty */
#line 36 "parser.y"
   { CurrentSemanticValue.node_list = new List<AST>();}
#line default
        break;
      case 6: // declaration -> types, Ident, ";"
#line 40 "parser.y"
                   {CurrentSemanticValue.node = new Declare(ValueStack[ValueStack.Depth-2].val, ValueStack[ValueStack.Depth-3].type); Console.WriteLine("declared: " + ValueStack[ValueStack.Depth-2].val);}
#line default
        break;
      case 7: // statement_list -> statement_list, statement
#line 44 "parser.y"
                            {ValueStack[ValueStack.Depth-2].node_list.Add(ValueStack[ValueStack.Depth-1].node); CurrentSemanticValue.node_list = ValueStack[ValueStack.Depth-2].node_list;}
#line default
        break;
      case 8: // statement_list -> /* empty */
#line 45 "parser.y"
   { CurrentSemanticValue.node_list = new List<AST>();}
#line default
        break;
      case 9: // statement -> Ident, "=", expression, ";"
#line 49 "parser.y"
                            {var nd = new LeafNode(ValueStack[ValueStack.Depth-2].val); CurrentSemanticValue.node = new Assign(ValueStack[ValueStack.Depth-4].val, nd.Evaluate()); Console.WriteLine(ValueStack[ValueStack.Depth-4].val); Console.WriteLine("="); Console.WriteLine(nd.Evaluate()); Console.WriteLine("\n");}
#line default
        break;
      case 15: // statement -> Write, values, ";"
#line 55 "parser.y"
                    {CurrentSemanticValue.node = new Write(ValueStack[ValueStack.Depth-3].val); }
#line default
        break;
      case 16: // statement -> Write, String, ";"
#line 56 "parser.y"
                    {CurrentSemanticValue.node = new Write(ValueStack[ValueStack.Depth-3].val); }
#line default
        break;
      case 19: // types -> Int
#line 66 "parser.y"
       {CurrentSemanticValue.type = IdentType.Int;}
#line default
        break;
      case 20: // types -> Double
#line 67 "parser.y"
          {CurrentSemanticValue.type = IdentType.Double;}
#line default
        break;
      case 21: // types -> Bool
#line 68 "parser.y"
        {CurrentSemanticValue.type = IdentType.Bool;}
#line default
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

#line 79 "parser.y"

public Parser(Scanner scanner) : base(scanner) { }
#line default
}
}
