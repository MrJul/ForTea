// This is a generated file. Not intended for manual editing.
package com.jetbrains.fortea.language.psi.impl;

import com.jetbrains.fortea.language.psi.T4ExpressionBlock;
import com.jetbrains.fortea.language.psi.T4Visitor;
import org.jetbrains.annotations.*;
import com.intellij.lang.ASTNode;
import com.intellij.psi.PsiElement;
import com.intellij.psi.PsiElementVisitor;

import static com.jetbrains.fortea.language.psi.T4ElementTypes.*;
import com.intellij.extapi.psi.ASTWrapperPsiElement;

public class T4ExpressionBlockImpl extends ASTWrapperPsiElement implements T4ExpressionBlock {

  public T4ExpressionBlockImpl(@NotNull ASTNode node) {
    super(node);
  }

  public void accept(@NotNull T4Visitor visitor) {
    visitor.visitExpressionBlock(this);
  }

  public void accept(@NotNull PsiElementVisitor visitor) {
    if (visitor instanceof T4Visitor) accept((T4Visitor)visitor);
    else super.accept(visitor);
  }

  @Override
  @Nullable
  public PsiElement getCode() {
    return findChildByType(CODE);
  }

}
