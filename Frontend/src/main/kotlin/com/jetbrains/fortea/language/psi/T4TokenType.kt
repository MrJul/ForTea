package com.jetbrains.fortea.language.psi

import com.intellij.psi.tree.IElementType
import com.jetbrains.fortea.language.T4Language
import org.jetbrains.annotations.NonNls

class T4TokenType(@NonNls debugName: String) : IElementType(debugName, T4Language) {
  override fun toString() = "T4TokenType." + super.toString()
}
