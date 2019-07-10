package com.jetbrains.fortea.language.parser;

import com.intellij.lexer.FlexLexer;
import com.intellij.psi.tree.IElementType;

import static com.intellij.psi.TokenType.BAD_CHARACTER;
import static com.intellij.psi.TokenType.WHITE_SPACE;
import static com.jetbrains.fortea.language.psi.T4ElementTypes.*;

%%

%{
  public _T4Lexer() {
    this((java.io.Reader)null);
  }
%}

%public
%class _T4Lexer
%implements FlexLexer
%function advance
%type IElementType
%unicode

EOL=\R
WHITE_SPACE=\s+

CODE=foo(?=s)

%%
<YYINITIAL> {
  {WHITE_SPACE}               { return WHITE_SPACE; }

  "TEXT"                      { return TEXT; }
  "BLOCK_END"                 { return BLOCK_END; }
  "DIRECTIVE_START"           { return DIRECTIVE_START; }
  "TOKEN"                     { return TOKEN; }
  "EQ"                        { return EQ; }
  "QUOTE"                     { return QUOTE; }
  "CODE_BLOCK_START"          { return CODE_BLOCK_START; }
  "EXPRESSION_BLOCK_START"    { return EXPRESSION_BLOCK_START; }
  "FEATURE_BLOCK_START"       { return FEATURE_BLOCK_START; }

  {CODE}                      { return CODE; }

}

[^] { return BAD_CHARACTER; }
