// This is a generated file. Not intended for manual editing.
package com.jetbrains.fortea.psi;

import com.intellij.psi.tree.IElementType;
import com.intellij.psi.PsiElement;
import com.intellij.lang.ASTNode;
import com.jetbrains.fortea.psi.impl.*;

public interface T4ElementTypes {

  IElementType ATTRIBUTE = new T4ElementType("ATTRIBUTE");
  IElementType ATTRIBUTE_NAME = new T4ElementType("ATTRIBUTE_NAME");
  IElementType BLOCK = new T4ElementType("BLOCK");
  IElementType CODE_BLOCK = new T4ElementType("CODE_BLOCK");
  IElementType DIRECTIVE = new T4ElementType("DIRECTIVE");
  IElementType DIRECTIVE_NAME = new T4ElementType("DIRECTIVE_NAME");
  IElementType EXPRESSION_BLOCK = new T4ElementType("EXPRESSION_BLOCK");
  IElementType FEATURE_BLOCK = new T4ElementType("FEATURE_BLOCK");

  IElementType ATTRIBUTE_VALUE = new T4TokenType("ATTRIBUTE_VALUE");
  IElementType BLOCK_END = new T4TokenType("BLOCK_END");
  IElementType CODE = new T4TokenType("CODE");
  IElementType CODE_BLOCK_START = new T4TokenType("CODE_BLOCK_START");
  IElementType DIRECTIVE_START = new T4TokenType("DIRECTIVE_START");
  IElementType EQ = new T4TokenType("EQ");
  IElementType EXPRESSION_BLOCK_START = new T4TokenType("EXPRESSION_BLOCK_START");
  IElementType FEATURE_BLOCK_START = new T4TokenType("FEATURE_BLOCK_START");
  IElementType QUOTE = new T4TokenType("QUOTE");
  IElementType TEXT = new T4TokenType("TEXT");
  IElementType TOKEN = new T4TokenType("TOKEN");

  class Factory {
    public static PsiElement createElement(ASTNode node) {
      IElementType type = node.getElementType();
       if (type == ATTRIBUTE) {
        return new T4AttributeImpl(node);
      }
      else if (type == ATTRIBUTE_NAME) {
        return new T4AttributeNameImpl(node);
      }
      else if (type == BLOCK) {
        return new T4BlockImpl(node);
      }
      else if (type == CODE_BLOCK) {
        return new T4CodeBlockImpl(node);
      }
      else if (type == DIRECTIVE) {
        return new T4DirectiveImpl(node);
      }
      else if (type == DIRECTIVE_NAME) {
        return new T4DirectiveNameImpl(node);
      }
      else if (type == EXPRESSION_BLOCK) {
        return new T4ExpressionBlockImpl(node);
      }
      else if (type == FEATURE_BLOCK) {
        return new T4FeatureBlockImpl(node);
      }
      throw new AssertionError("Unknown element type: " + type);
    }
  }
}
