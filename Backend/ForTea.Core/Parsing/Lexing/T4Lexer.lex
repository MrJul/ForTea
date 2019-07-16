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

RAW_CODE=([^#]|(#+[^#>]))*
RAW_TEXT=([^<\r\n]|(<+[^<#\r\n])|(\\<#))*
RAW_ATTRIBUTE_VALUE=[^\"]*

%state IN_DIRECTIVE
%state IN_BLOCK
%state IN_ATTRIBUTE_VALUE

%%
<YYINITIAL> "<#@"                           { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.DIRECTIVE_START); return myCurrentTokenType; }
<YYINITIAL> "<#="                           { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.EXPRESSION_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "<#+"                           { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.FEATURE_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "<#"                            { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(T4TokenNodeTypes.STATEMENT_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "#>"                            { myCurrentTokenType = makeToken(T4TokenNodeTypes.BLOCK_END); return myCurrentTokenType; }
<YYINITIAL> (\r|\n|\r\n)                    { myCurrentTokenType = makeToken(T4TokenNodeTypes.NEW_LINE); return myCurrentTokenType; }
<YYINITIAL> {RAW_TEXT}                      { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_TEXT); return myCurrentTokenType; }
<YYINITIAL> [^]                             { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_TEXT); return myCurrentTokenType; }

<IN_DIRECTIVE> {WHITESPACE}                 { myCurrentTokenType = makeToken(T4TokenNodeTypes.WHITE_SPACE); return myCurrentTokenType; }
<IN_DIRECTIVE> {TOKEN}                      { myCurrentTokenType = makeToken(T4TokenNodeTypes.TOKEN); return myCurrentTokenType; }
<IN_DIRECTIVE> "="                          { myCurrentTokenType = makeToken(T4TokenNodeTypes.EQUAL); return myCurrentTokenType; }
<IN_DIRECTIVE> "\""                         { yybegin(IN_ATTRIBUTE_VALUE); myCurrentTokenType = makeToken(T4TokenNodeTypes.QUOTE); return myCurrentTokenType; }
<IN_DIRECTIVE> "#>"                         { yybegin(YYINITIAL); myCurrentTokenType = makeToken(T4TokenNodeTypes.BLOCK_END); return myCurrentTokenType; }
<IN_DIRECTIVE> [^]                          { myCurrentTokenType = makeToken(T4TokenNodeTypes.BAD_TOKEN); return myCurrentTokenType; }

<IN_ATTRIBUTE_VALUE> "\""                   { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(T4TokenNodeTypes.QUOTE); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> {RAW_ATTRIBUTE_VALUE}  { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> [^]                    { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_ATTRIBUTE_VALUE); return myCurrentTokenType; }

<IN_BLOCK> "#>"                             { yybegin(YYINITIAL); myCurrentTokenType = makeToken(T4TokenNodeTypes.BLOCK_END); return myCurrentTokenType; }
<IN_BLOCK> {RAW_CODE}                       { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_CODE); return myCurrentTokenType; }
<IN_BLOCK> [^]                              { myCurrentTokenType = makeToken(T4TokenNodeTypes.RAW_CODE); return myCurrentTokenType; }
