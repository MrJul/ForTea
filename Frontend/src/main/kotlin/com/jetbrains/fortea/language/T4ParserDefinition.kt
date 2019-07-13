package com.jetbrains.fortea.language

import com.intellij.lexer.Lexer
import com.intellij.openapi.project.Project
import com.intellij.psi.tree.IElementType
import com.jetbrains.fortea.highlighter.T4SyntaxHighlighter
import com.jetbrains.rider.ideaInterop.fileTypes.RiderFileElementType
import com.jetbrains.rider.ideaInterop.fileTypes.RiderParserDefinitionBase

class T4ParserDefinition : RiderParserDefinitionBase(t4FileElementType, T4FileType) {
  companion object {
    private val t4ElementType = IElementType("T4", T4Language)
    val t4FileElementType = RiderFileElementType(T4Language, t4ElementType)
  }

  private val lexer = T4SyntaxHighlighter.highlightingLexer

  override fun createLexer(project: Project): Lexer = lexer
}
