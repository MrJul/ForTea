// This is a generated file. Not intended for manual editing.
package com.jetbrains.fortea.psi;

import com.intellij.psi.tree.IElementType;
import com.intellij.psi.PsiElement;
import com.intellij.lang.ASTNode;
import com.jetbrains.fortea.psi.impl.*;

public interface T4ElementTypes {

  IElementType ATTRIBUTE = new T4ElementType("ATTRIBUTE");
  IElementType ATTRIBUTE_NAME = new T4ElementType("ATTRIBUTE_NAME");
  IElementType ATTRIBUTE_VALUE = new T4ElementType("ATTRIBUTE_VALUE");
  IElementType BLOCK = new T4ElementType("BLOCK");
  IElementType CODE = new T4ElementType("CODE");
  IElementType CODE_BLOCK = new T4ElementType("CODE_BLOCK");
  IElementType DIRECTIVE = new T4ElementType("DIRECTIVE");
  IElementType DIRECTIVE_NAME = new T4ElementType("DIRECTIVE_NAME");
  IElementType EXPRESSION_BLOCK = new T4ElementType("EXPRESSION_BLOCK");
  IElementType FEATURE_BLOCK = new T4ElementType("FEATURE_BLOCK");
  IElementType STATEMENT_BLOCK = new T4ElementType("STATEMENT_BLOCK");
  IElementType TEXT = new T4ElementType("TEXT");

  IElementType BLOCK_END = new T4TokenType("BLOCK_END");
  IElementType DIRECTIVE_START = new T4TokenType("DIRECTIVE_START");
  IElementType EQUAL = new T4TokenType("EQUAL");
  IElementType EXPRESSION_BLOCK_START = new T4TokenType("EXPRESSION_BLOCK_START");
  IElementType FEATURE_BLOCK_START = new T4TokenType("FEATURE_BLOCK_START");
  IElementType NEW_LINE = new T4TokenType("NEW_LINE");
  IElementType QUOTE = new T4TokenType("QUOTE");
  IElementType RAW_ATTRIBUTE_VALUE = new T4TokenType("RAW_ATTRIBUTE_VALUE");
  IElementType RAW_CODE = new T4TokenType("RAW_CODE");
  IElementType RAW_TEXT = new T4TokenType("RAW_TEXT");
  IElementType STATEMENT_BLOCK_START = new T4TokenType("STATEMENT_BLOCK_START");
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
      else if (type == ATTRIBUTE_VALUE) {
        return new T4AttributeValueImpl(node);
      }
      else if (type == BLOCK) {
        return new T4BlockImpl(node);
      }
      else if (type == CODE) {
        return new T4CodeImpl(node);
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
      else if (type == STATEMENT_BLOCK) {
        return new T4StatementBlockImpl(node);
      }
      else if (type == TEXT) {
        return new T4TextImpl(node);
      }
      throw new AssertionError("Unknown element type: " + type);
    }
  }
}
