package com.jetbrains.fortea.parser

import com.intellij.lang.ASTNode
import com.intellij.lang.ParserDefinition
import com.intellij.openapi.project.Project
import com.intellij.psi.FileViewProvider
import com.intellij.psi.PsiElement
import com.intellij.psi.tree.IFileElementType
import com.intellij.psi.tree.TokenSet
import com.jetbrains.fortea.language.T4Language
import com.jetbrains.fortea.lexer.T4Lexer
import com.jetbrains.fortea.psi.T4ElementTypes
import com.jetbrains.fortea.psi.T4PsiFile

class T4ParserDefinition : ParserDefinition {
  override fun createParser(project: Project) = T4Parser()
  override fun createLexer(project: Project) = T4Lexer()

  override fun createFile(provider: FileViewProvider) = T4PsiFile(provider)
  override fun createElement(node: ASTNode): PsiElement = T4ElementTypes.Factory.createElement(node)

  override fun getWhitespaceTokens(): TokenSet = TokenSet.EMPTY
  override fun getStringLiteralElements(): TokenSet = TokenSet.EMPTY
  override fun getCommentTokens(): TokenSet = TokenSet.EMPTY
  override fun getFileNodeType() = FILE


  private companion object {
    val FILE = IFileElementType(T4Language)
  }
}
