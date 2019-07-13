package com.jetbrains.fortea.highlighter

import com.intellij.openapi.fileTypes.FileType
import com.intellij.openapi.fileTypes.SyntaxHighlighter
import com.intellij.openapi.fileTypes.SyntaxHighlighterFactory
import com.intellij.openapi.fileTypes.SyntaxHighlighterProvider
import com.intellij.openapi.project.Project
import com.intellij.openapi.vfs.VirtualFile
import com.jetbrains.fortea.language.T4FileType

class T4SyntaxHighlighterProvider : SyntaxHighlighterFactory(), SyntaxHighlighterProvider {
  override fun getSyntaxHighlighter(project: Project?, virtualFile: VirtualFile?) = T4SyntaxHighlighter

  override fun create(fileType: FileType, project: Project?, file: VirtualFile?): SyntaxHighlighter? {
    if (fileType !is T4FileType) return null
    return T4SyntaxHighlighter
  }
}
