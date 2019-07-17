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

public class T4TextImpl extends ASTWrapperPsiElement implements T4Text {

  public T4TextImpl(@NotNull ASTNode node) {
    super(node);
  }

  public void accept(@NotNull T4Visitor visitor) {
    visitor.visitText(this);
  }

  public void accept(@NotNull PsiElementVisitor visitor) {
    if (visitor instanceof T4Visitor) accept((T4Visitor)visitor);
    else super.accept(visitor);
  }

}
