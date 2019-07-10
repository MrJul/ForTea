// This is a generated file. Not intended for manual editing.
package com.jetbrains.fortea.language.psi.impl;

import java.util.List;
import org.jetbrains.annotations.*;
import com.intellij.lang.ASTNode;
import com.intellij.psi.PsiElement;
import com.intellij.psi.PsiElementVisitor;
import com.intellij.psi.util.PsiTreeUtil;
import static com.jetbrains.fortea.language.psi.T4ElementTypes.*;
import com.intellij.extapi.psi.ASTWrapperPsiElement;
import com.jetbrains.fortea.language.psi.*;

public class T4AttributeImpl extends ASTWrapperPsiElement implements T4Attribute {

  public T4AttributeImpl(@NotNull ASTNode node) {
    super(node);
  }

  public void accept(@NotNull T4Visitor visitor) {
    visitor.visitAttribute(this);
  }

  public void accept(@NotNull PsiElementVisitor visitor) {
    if (visitor instanceof T4Visitor) accept((T4Visitor)visitor);
    else super.accept(visitor);
  }

  @Override
  @NotNull
  public T4AttributeName getAttributeName() {
    return findNotNullChildByClass(T4AttributeName.class);
  }

  @Override
  @NotNull
  public T4AttributeValue getAttributeValue() {
    return findNotNullChildByClass(T4AttributeValue.class);
  }

}
