using System;
using JetBrains.ReSharper.Psi.Parsing;
using JetBrains.Text;
using JetBrains.Util;

%%

%unicode

%init{
  myCurrentTokenType = null;
%init}

%namespace GammaJul.ForTea.Core.Parsing.Lexing
%class T4LexerGenerated
%implements IIncrementalLexer
%function advance
%type TokenNodeType

%eofval{
  myCurrentTokenType = null;
  return myCurrentTokenType;
%eofval}

WHITESPACE=[\ \n\r\t\f]
LETTER=[A-Za-z_]
TOKEN={LETTER}+

%state IN_DIRECTIVE
%state IN_BLOCK
%state IN_ATTRIBUTE_VALUE

%%
<YYINITIAL> "<#@"               { yybegin(IN_DIRECTIVE); return T4TokenNodeTypes.DIRECTIVE_START; }
<YYINITIAL> "<#="               { yybegin(IN_BLOCK); return T4TokenNodeTypes.EXPRESSION_BLOCK_START; }
<YYINITIAL> "<#+"               { yybegin(IN_BLOCK); return T4TokenNodeTypes.FEATURE_BLOCK_START; }
<YYINITIAL> "<#"                { yybegin(IN_BLOCK); return T4TokenNodeTypes.STATEMENT_BLOCK_START; }
<YYINITIAL> "#>"                { return T4TokenNodeTypes.BLOCK_END; }
<YYINITIAL> [^]                 { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_TEXT); return myCurrentTokenType; }

<IN_DIRECTIVE> {WHITESPACE}     { return T4TokenNodeTypes.WHITE_SPACE; }
<IN_DIRECTIVE> {TOKEN}          { return T4TokenNodeTypes.TOKEN; }
<IN_DIRECTIVE> "="              { return T4TokenNodeTypes.EQUAL; }
<IN_DIRECTIVE> "\""             { yybegin(IN_ATTRIBUTE_VALUE); return T4TokenNodeTypes.QUOTE; }
<IN_DIRECTIVE> "#>"             { yybegin(YYINITIAL); return T4TokenNodeTypes.BLOCK_END; }

<IN_ATTRIBUTE_VALUE> "\""       { yybegin(IN_DIRECTIVE); return T4TokenNodeTypes.QUOTE; }
<IN_ATTRIBUTE_VALUE> .          { return T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE; }
