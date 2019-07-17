package com.jetbrains.fortea.highlighting

import com.intellij.lang.annotation.AnnotationHolder
import com.jetbrains.fortea.psi.T4AttributeName
import com.jetbrains.fortea.psi.T4AttributeValue
import com.jetbrains.fortea.psi.T4DirectiveName
import com.jetbrains.fortea.psi.T4Visitor

class T4AnnotatorVisitor(private val holder: AnnotationHolder) : T4Visitor() {
  override fun visitDirectiveName(directiveName: T4DirectiveName) {
    holder.createInfoAnnotation(directiveName, null).textAttributes = T4SyntaxHighlighter.DIRECTIVE_NAME
  }

  override fun visitAttributeName(attributeName: T4AttributeName) {
    holder.createInfoAnnotation(attributeName, null).textAttributes = T4SyntaxHighlighter.ATTRIBUTE_KEY
  }

  override fun visitAttributeValue(attributeValue: T4AttributeValue) {
    holder.createInfoAnnotation(attributeValue, null).textAttributes = T4SyntaxHighlighter.ATTRIBUTE_VALUE
  }
}
