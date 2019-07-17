package com.jetbrains.fortea.lexer;

import com.intellij.psi.tree.IElementType;
import com.intellij.lexer.FlexLexer;

import static com.intellij.psi.TokenType.WHITE_SPACE;
import static com.intellij.psi.TokenType.BAD_CHARACTER;
import static com.jetbrains.fortea.psi.T4ElementTypes.*;

%%

%{
  private IElementType myCurrentTokenType;
  private IElementType makeToken(IElementType token) {
    myCurrentTokenType = token;
    return myCurrentTokenType;
  }
%}

%class _T4Lexer
%implements FlexLexer
%type IElementType
%init{
  myCurrentTokenType = null;
%init}

%function advance
%unicode

%eofval{
    myCurrentTokenType = null;
    return null;
%eofval}

WHITESPACE=[\ \n\r\t\f]
LETTER=[A-Za-z_]
TOKEN={LETTER}+

RAW_CODE=([^#]|(#+[^#>]))+
RAW_TEXT=([^<\r\n]|(<+[^<#\r\n])|(\\<#))+
RAW_ATTRIBUTE_VALUE=[^\"]+

%state IN_DIRECTIVE
%state IN_BLOCK
%state IN_ATTRIBUTE_VALUE

%%
<YYINITIAL> "<#@"                           { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(DIRECTIVE_START); return myCurrentTokenType; }
<YYINITIAL> "<#="                           { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(EXPRESSION_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "<#+"                           { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(FEATURE_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "<#"                            { yybegin(IN_BLOCK); myCurrentTokenType = makeToken(STATEMENT_BLOCK_START); return myCurrentTokenType; }
<YYINITIAL> "#>"                            { myCurrentTokenType = makeToken(BLOCK_END); return myCurrentTokenType; }
<YYINITIAL> (\r|\n|\r\n)                    { myCurrentTokenType = makeToken(NEW_LINE); return myCurrentTokenType; }
<YYINITIAL> {RAW_TEXT}                      { myCurrentTokenType = makeToken(RAW_TEXT); return myCurrentTokenType; }
<YYINITIAL> [^]                             { myCurrentTokenType = makeToken(RAW_TEXT); return myCurrentTokenType; }

<IN_DIRECTIVE> {WHITESPACE}                 { myCurrentTokenType = makeToken(WHITE_SPACE); return myCurrentTokenType; }
<IN_DIRECTIVE> {TOKEN}                      { myCurrentTokenType = makeToken(TOKEN); return myCurrentTokenType; }
<IN_DIRECTIVE> "="                          { myCurrentTokenType = makeToken(EQUAL); return myCurrentTokenType; }
<IN_DIRECTIVE> "\""                         { yybegin(IN_ATTRIBUTE_VALUE); myCurrentTokenType = makeToken(QUOTE); return myCurrentTokenType; }
<IN_DIRECTIVE> "#>"                         { yybegin(YYINITIAL); myCurrentTokenType = makeToken(BLOCK_END); return myCurrentTokenType; }
<IN_DIRECTIVE> [^]                          { myCurrentTokenType = makeToken(BAD_CHARACTER); return myCurrentTokenType; }

<IN_ATTRIBUTE_VALUE> "\""                   { yybegin(IN_DIRECTIVE); myCurrentTokenType = makeToken(QUOTE); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> {RAW_ATTRIBUTE_VALUE}  { myCurrentTokenType = makeToken(RAW_ATTRIBUTE_VALUE); return myCurrentTokenType; }
<IN_ATTRIBUTE_VALUE> [^]                    { myCurrentTokenType = makeToken(RAW_ATTRIBUTE_VALUE); return myCurrentTokenType; }

<IN_BLOCK> "#>"                             { yybegin(YYINITIAL); myCurrentTokenType = makeToken(BLOCK_END); return myCurrentTokenType; }
<IN_BLOCK> {RAW_CODE}                       { myCurrentTokenType = makeToken(RAW_CODE); return myCurrentTokenType; }
<IN_BLOCK> [^]                              { myCurrentTokenType = makeToken(RAW_CODE); return myCurrentTokenType; }
