package com.jetbrains.fortea.language.lexer;

import com.intellij.lexer.FlexLexer;
import com.jetbrains.fortea.language.psi.T4ElementType;import com.jetbrains.fortea.language.psi.tree.IElementType;
import com.jetbrains.fortea.language.psi.T4ElementTypes;

%%

%class _T4Lexer
%implements FlexLexer
%unicode
%function advance
%type IElementType

ESCAPE=\\<#
BLOCK_END=#>
DIRECTIVE_START=<#@
CODE_BLOCK_START=<#
EXPRESSION_BLOCK_START=<#=
FEATURE_BLOCK_START=<#\+

TOKEN=\s(\s|\d)*
CODE=sdvasd(?=a)
/*
WHITE_SPACE_CHAR=[\ \n\r\t\f]
VALUE_CHARACTER=[^\n\r\f\\] | "\\"{CRLF} | "\\".
END_OF_LINE_COMMENT=("#"|"!")[^\r\n]*
KEY_SEPARATOR=[:=]
KEY_SEPARATOR_SPACE=\ \t
KEY_CHARACTER=[^:=\ \n\r\t\f\\] | "\\"{CRLF} | "\\".
FIRST_VALUE_CHARACTER_BEFORE_SEP={VALUE_CHARACTER}
VALUE_CHARACTERS_BEFORE_SEP=([^:=\ \t\n\r\f\\] | "\\"{CRLF} | "\\".)({VALUE_CHARACTER}*)
VALUE_CHARACTERS_AFTER_SEP=([^\ \t\n\r\f\\] | "\\"{CRLF} | "\\".)({VALUE_CHARACTER}*)
*/

%state IN_DIRECTIVE
%state IN_BLOCK

%%

<YYINITIAL> {ESCAPE}                     { yybegin(YYINITIAL); return T4ElementTypes.TEXT; }
<YYINITIAL> {DIRECTIVE_START}            { yybegin(IN_DIRECTIVE); return T4ElementTypes.DIRECTIVE_START; }
<YYINITIAL> {CODE_BLOCK_START}           { yybegin(IN_BLOCK); return T4ElementTypes.CODE_BLOCK_START; }
<YYINITIAL> {EXPRESSION_BLOCK_START}     { yybegin(IN_BLOCK); return T4ElementTypes.EXPRESSION_BLOCK_START; }
<YYINITIAL> {FEATURE_BLOCK_START}        { yybegin(IN_BLOCK); return T4ElementTypes.FEATURE_BLOCK_START; }


[^]                                      { return PropertiesTokenTypes.BAD_CHARACTER; }
