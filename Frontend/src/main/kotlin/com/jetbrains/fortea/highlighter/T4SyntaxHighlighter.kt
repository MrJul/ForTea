package com.jetbrains.fortea.highlighter

import com.intellij.openapi.editor.DefaultLanguageHighlighterColors
import com.intellij.openapi.editor.colors.TextAttributesKey
import com.intellij.openapi.editor.colors.TextAttributesKey.createTextAttributesKey
import com.intellij.openapi.fileTypes.SyntaxHighlighterBase
import com.intellij.psi.tree.IElementType
import com.jetbrains.fortea.lexer.T4Lexer
import com.jetbrains.fortea.psi.T4ElementTypes

object T4SyntaxHighlighter : SyntaxHighlighterBase() {
  val ATTRIBUTE_VALUE = createTextAttributesKey("T4_ATTRIBUTE_VALUE", DefaultLanguageHighlighterColors.STRING)
  val BLOCK_MARKER = createTextAttributesKey("T4_BLOCK_MARKER", DefaultLanguageHighlighterColors.METADATA) // ?
  val EQUAL = createTextAttributesKey("T4_DIRECTIVE_EQ_SIGN", DefaultLanguageHighlighterColors.OPERATION_SIGN)

  val QUOTE = createTextAttributesKey("T4_DIRECTIVE_QUOTE", DefaultLanguageHighlighterColors.STRING)
  // Added by annotators
  val ATTRIBUTE = createTextAttributesKey("T4_ATTRIBUTE", DefaultLanguageHighlighterColors.FUNCTION_DECLARATION)
  val ATTRIBUTE_KEY = createTextAttributesKey("T4_ATTRIBUTE_KEY", DefaultLanguageHighlighterColors.PARAMETER)

  private val ATTRIBUTE_VALUE_KEYS = arrayOf(ATTRIBUTE_VALUE)
  private val BLOCK_MARKER_KEYS = arrayOf(BLOCK_MARKER)
  private val EQUAL_KEYS = arrayOf(EQUAL)
  private val QUOTE_KEYS = arrayOf(QUOTE)
  // ---- ---- ---- ----
  override fun getTokenHighlights(elementType: IElementType?): Array<TextAttributesKey> = when (elementType) {
    T4ElementTypes.ATTRIBUTE_VALUE -> ATTRIBUTE_VALUE_KEYS
    T4ElementTypes.BLOCK_END -> BLOCK_MARKER_KEYS
    T4ElementTypes.STATEMENT_BLOCK_START -> BLOCK_MARKER_KEYS
    T4ElementTypes.DIRECTIVE_START -> BLOCK_MARKER_KEYS
    T4ElementTypes.EQUAL -> EQUAL_KEYS
    T4ElementTypes.EXPRESSION_BLOCK_START -> BLOCK_MARKER_KEYS
    T4ElementTypes.FEATURE_BLOCK_START -> BLOCK_MARKER_KEYS
    T4ElementTypes.QUOTE -> QUOTE_KEYS
    T4ElementTypes.TOKEN -> TextAttributesKey.EMPTY_ARRAY
    else -> TextAttributesKey.EMPTY_ARRAY
  }

  override fun getHighlightingLexer() = T4Lexer()
}
