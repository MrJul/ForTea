package com.jetbrains.fortea.language.lexer;

import com.intellij.psi.tree.IElementType;
import com.intellij.lexer.FlexLexer;

import static com.intellij.psi.TokenType.WHITE_SPACE;
import static com.intellij.psi.TokenType.BAD_CHARACTER;
import static com.jetbrains.fortea.language.psi.T4ElementTypes.*;

%%

%{

public _T4Lexer() {
  this(null);
}

private boolean isBlockEndAhead() {
  // TODO: handle invalid block ends as well
  return (yycharat(zzCurrentPos) == '#' &&  yycharat(zzCurrentPos + 1) == '>');
}
%}

%class _T4Lexer
%implements FlexLexer
%unicode
%function advance
%type IElementType

WHITESPACE=[\ \n\r\t\f]

ESCAPE=\\<#
BLOCK_END=#>
DIRECTIVE_START=<#@
CODE_BLOCK_START=<#
EXPRESSION_BLOCK_START=<#=
FEATURE_BLOCK_START=<#\+

LETTER=[A-Za-z_]
QUOTE=\"
TOKEN={LETTER}+
ATTRIBUTE_VALUE=([^\"#>]|#[^>\"])*
EQ==

%state IN_DIRECTIVE
%state IN_BLOCK
%state IN_ATTRIBUTE_VALUE

%%

<YYINITIAL> {ESCAPE}                     { return TEXT; }
<YYINITIAL> {DIRECTIVE_START}            { yybegin(IN_DIRECTIVE); return DIRECTIVE_START; }
<YYINITIAL> {CODE_BLOCK_START}           { yybegin(IN_BLOCK); return CODE_BLOCK_START; }
<YYINITIAL> {EXPRESSION_BLOCK_START}     { yybegin(IN_BLOCK); return EXPRESSION_BLOCK_START; }
<YYINITIAL> {FEATURE_BLOCK_START}        { yybegin(IN_BLOCK); return FEATURE_BLOCK_START; }

<IN_DIRECTIVE> {WHITESPACE}              { return WHITE_SPACE; }
<IN_DIRECTIVE> {TOKEN}                   { return TOKEN; }
<IN_DIRECTIVE> {EQ}                      { return EQ; }
<IN_DIRECTIVE> {QUOTE}                   { yybegin(IN_ATTRIBUTE_VALUE); return QUOTE; }
<IN_DIRECTIVE> {BLOCK_END}               { yybegin(YYINITIAL); return BLOCK_END; }

<IN_DIRECTIVE> {ATTRIBUTE_VALUE}         { yybegin(IN_DIRECTIVE); return ATTRIBUTE_VALUE; }

<IN_BLOCK> ""
      {
        while (!this.isBlockEndAhead()) {
          zzCurrentPos += 1;
        }
        return CODE;
      }
<IN_BLOCK>  {BLOCK_END}                  { yybegin(YYINITIAL); return BLOCK_END; }

[^]                                      { return BAD_CHARACTER; }
