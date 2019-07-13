// This is a generated file. Not intended for manual editing.
package com.jetbrains.fortea.psi.impl;

import java.util.List;
import org.jetbrains.annotations.*;
import com.intellij.lang.ASTNode;
import com.intellij.psi.PsiElement;
import com.intellij.psi.PsiElementVisitor;
import com.intellij.psi.util.PsiTreeUtil;
import static com.jetbrains.fortea.psi.T4ElementTypes.*;
import com.intellij.extapi.psi.ASTWrapperPsiElement;
import com.jetbrains.fortea.psi.*;

public class T4BlockImpl extends ASTWrapperPsiElement implements T4Block {

  public T4BlockImpl(@NotNull ASTNode node) {
    super(node);
  }

  public void accept(@NotNull T4Visitor visitor) {
    visitor.visitBlock(this);
  }

  public void accept(@NotNull PsiElementVisitor visitor) {
    if (visitor instanceof T4Visitor) accept((T4Visitor)visitor);
    else super.accept(visitor);
  }

  @Override
  @Nullable
  public T4CodeBlock getCodeBlock() {
    return findChildByClass(T4CodeBlock.class);
  }

  @Override
  @Nullable
  public T4ExpressionBlock getExpressionBlock() {
    return findChildByClass(T4ExpressionBlock.class);
  }

  @Override
  @Nullable
  public T4FeatureBlock getFeatureBlock() {
    return findChildByClass(T4FeatureBlock.class);
  }

}
