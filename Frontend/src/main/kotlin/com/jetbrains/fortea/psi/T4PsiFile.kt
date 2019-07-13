package com.jetbrains.fortea.psi

import com.intellij.extapi.psi.PsiFileBase
import com.intellij.psi.FileViewProvider
import com.jetbrains.fortea.language.T4FileType
import com.jetbrains.fortea.language.T4Language

class T4PsiFile(viewProvider: FileViewProvider) : PsiFileBase(viewProvider, T4Language) {
  override fun getFileType() = T4FileType
}
