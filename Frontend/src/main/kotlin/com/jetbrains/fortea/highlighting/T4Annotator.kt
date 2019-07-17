package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.AnnotationHolder
import com.intellij.lang.annotation.Annotator
import com.intellij.psi.PsiElement

class T4Annotator : Annotator {
  override fun annotate(element: PsiElement, holder: AnnotationHolder) =
    element.accept(T4AnnotatorVisitor(holder))
}
