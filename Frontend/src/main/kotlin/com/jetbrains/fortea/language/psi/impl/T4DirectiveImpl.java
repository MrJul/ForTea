// This is a generated file. Not intended for manual editing.
package com.jetbrains.fortea.language.psi.impl;

import java.util.List;

import com.jetbrains.fortea.language.psi.T4Attribute;
import com.jetbrains.fortea.language.psi.T4Directive;
import com.jetbrains.fortea.language.psi.T4DirectiveName;
import com.jetbrains.fortea.language.psi.T4Visitor;
import org.jetbrains.annotations.*;
import com.intellij.lang.ASTNode;
import com.intellij.psi.PsiElementVisitor;
import com.intellij.psi.util.PsiTreeUtil;
import com.intellij.extapi.psi.ASTWrapperPsiElement;

public class T4DirectiveImpl extends ASTWrapperPsiElement implements T4Directive {

  public T4DirectiveImpl(@NotNull ASTNode node) {
    super(node);
  }

  public void accept(@NotNull T4Visitor visitor) {
    visitor.visitDirective(this);
  }

  public void accept(@NotNull PsiElementVisitor visitor) {
    if (visitor instanceof T4Visitor) accept((T4Visitor)visitor);
    else super.accept(visitor);
  }

  @Override
  @NotNull
  public List<T4Attribute> getAttributeList() {
    return PsiTreeUtil.getChildrenOfTypeAsList(this, T4Attribute.class);
  }

  @Override
  @Nullable
  public T4DirectiveName getDirectiveName() {
    return findChildByClass(T4DirectiveName.class);
  }

}
