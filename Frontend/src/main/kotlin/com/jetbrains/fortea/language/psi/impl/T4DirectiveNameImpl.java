// This is a generated file. Not intended for manual editing.
package com.jetbrains.fortea.language.psi.impl;

import com.jetbrains.fortea.language.psi.T4DirectiveName;
import com.jetbrains.fortea.language.psi.T4Visitor;
import org.jetbrains.annotations.*;
import com.intellij.lang.ASTNode;
import com.intellij.psi.PsiElementVisitor;
import com.intellij.extapi.psi.ASTWrapperPsiElement;

public class T4DirectiveNameImpl extends ASTWrapperPsiElement implements T4DirectiveName {

  public T4DirectiveNameImpl(@NotNull ASTNode node) {
    super(node);
  }

  public void accept(@NotNull T4Visitor visitor) {
    visitor.visitDirectiveName(this);
  }

  public void accept(@NotNull PsiElementVisitor visitor) {
    if (visitor instanceof T4Visitor) accept((T4Visitor)visitor);
    else super.accept(visitor);
  }

}
