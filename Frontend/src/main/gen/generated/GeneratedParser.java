// This is a generated file. Not intended for manual editing.
package generated;

import com.intellij.lang.PsiBuilder;
import com.intellij.lang.PsiBuilder.Marker;
import static com.jetbrains.fortea.language.psi.T4ElementTypes.*;
import static com.intellij.lang.parser.GeneratedParserUtilBase.*;
import com.intellij.psi.tree.IElementType;
import com.intellij.psi.tree.IFileElementType;
import com.intellij.lang.ASTNode;
import com.intellij.psi.tree.TokenSet;
import com.intellij.lang.PsiParser;
import com.intellij.lang.LightPsiParser;

@SuppressWarnings({"SimplifiableIfStatement", "UnusedAssignment"})
public class GeneratedParser implements PsiParser, LightPsiParser {

  public ASTNode parse(IElementType t, PsiBuilder b) {
    parseLight(t, b);
    return b.getTreeBuilt();
  }

  public void parseLight(IElementType t, PsiBuilder b) {
    boolean r;
    b = adapt_builder_(t, b, this, null);
    Marker m = enter_section_(b, 0, _COLLAPSE_, null);
    if (t instanceof IFileElementType) {
      r = parse_root_(t, b, 0);
    }
    else {
      r = false;
    }
    exit_section_(b, 0, m, t, r, true, TRUE_CONDITION);
  }

  protected boolean parse_root_(IElementType t, PsiBuilder b, int l) {
    return t4File(b, l + 1);
  }

  /* ********************************************************** */
  // attribute_name EQ QUOTE attribute_value QUOTE
  public static boolean attribute(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "attribute")) return false;
    if (!nextTokenIs(b, TOKEN)) return false;
    boolean r;
    Marker m = enter_section_(b);
    r = attribute_name(b, l + 1);
    r = r && consumeTokens(b, 0, EQ, QUOTE);
    r = r && attribute_value(b, l + 1);
    r = r && consumeToken(b, QUOTE);
    exit_section_(b, m, ATTRIBUTE, r);
    return r;
  }

  /* ********************************************************** */
  // TOKEN
  public static boolean attribute_name(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "attribute_name")) return false;
    if (!nextTokenIs(b, TOKEN)) return false;
    boolean r;
    Marker m = enter_section_(b);
    r = consumeToken(b, TOKEN);
    exit_section_(b, m, ATTRIBUTE_NAME, r);
    return r;
  }

  /* ********************************************************** */
  // TOKEN
  public static boolean attribute_value(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "attribute_value")) return false;
    if (!nextTokenIs(b, TOKEN)) return false;
    boolean r;
    Marker m = enter_section_(b);
    r = consumeToken(b, TOKEN);
    exit_section_(b, m, ATTRIBUTE_VALUE, r);
    return r;
  }

  /* ********************************************************** */
  // code_block|expression_block|feature_block
  public static boolean block(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "block")) return false;
    boolean r;
    Marker m = enter_section_(b, l, _NONE_, BLOCK, "<block>");
    r = code_block(b, l + 1);
    if (!r) r = expression_block(b, l + 1);
    if (!r) r = feature_block(b, l + 1);
    exit_section_(b, l, m, r, false, null);
    return r;
  }

  /* ********************************************************** */
  // CODE_BLOCK_START CODE? BLOCK_END
  public static boolean code_block(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "code_block")) return false;
    if (!nextTokenIs(b, CODE_BLOCK_START)) return false;
    boolean r;
    Marker m = enter_section_(b);
    r = consumeToken(b, CODE_BLOCK_START);
    r = r && code_block_1(b, l + 1);
    r = r && consumeToken(b, BLOCK_END);
    exit_section_(b, m, CODE_BLOCK, r);
    return r;
  }

  // CODE?
  private static boolean code_block_1(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "code_block_1")) return false;
    consumeToken(b, CODE);
    return true;
  }

  /* ********************************************************** */
  // directive_main BLOCK_END
  public static boolean directive(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "directive")) return false;
    if (!nextTokenIs(b, DIRECTIVE_START)) return false;
    boolean r;
    Marker m = enter_section_(b);
    r = directive_main(b, l + 1);
    r = r && consumeToken(b, BLOCK_END);
    exit_section_(b, m, DIRECTIVE, r);
    return r;
  }

  /* ********************************************************** */
  // DIRECTIVE_START directive_name attribute*
  static boolean directive_main(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "directive_main")) return false;
    boolean r, p;
    Marker m = enter_section_(b, l, _NONE_);
    r = consumeToken(b, DIRECTIVE_START);
    p = r; // pin = 1
    r = r && report_error_(b, directive_name(b, l + 1));
    r = p && directive_main_2(b, l + 1) && r;
    exit_section_(b, l, m, r, p, not_block_end_parser_);
    return r || p;
  }

  // attribute*
  private static boolean directive_main_2(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "directive_main_2")) return false;
    while (true) {
      int c = current_position_(b);
      if (!attribute(b, l + 1)) break;
      if (!empty_element_parsed_guard_(b, "directive_main_2", c)) break;
    }
    return true;
  }

  /* ********************************************************** */
  // TOKEN
  public static boolean directive_name(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "directive_name")) return false;
    if (!nextTokenIs(b, TOKEN)) return false;
    boolean r;
    Marker m = enter_section_(b);
    r = consumeToken(b, TOKEN);
    exit_section_(b, m, DIRECTIVE_NAME, r);
    return r;
  }

  /* ********************************************************** */
  // EXPRESSION_BLOCK_START CODE? BLOCK_END
  public static boolean expression_block(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "expression_block")) return false;
    if (!nextTokenIs(b, EXPRESSION_BLOCK_START)) return false;
    boolean r;
    Marker m = enter_section_(b);
    r = consumeToken(b, EXPRESSION_BLOCK_START);
    r = r && expression_block_1(b, l + 1);
    r = r && consumeToken(b, BLOCK_END);
    exit_section_(b, m, EXPRESSION_BLOCK, r);
    return r;
  }

  // CODE?
  private static boolean expression_block_1(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "expression_block_1")) return false;
    consumeToken(b, CODE);
    return true;
  }

  /* ********************************************************** */
  // FEATURE_BLOCK_START CODE? BLOCK_END
  public static boolean feature_block(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "feature_block")) return false;
    if (!nextTokenIs(b, FEATURE_BLOCK_START)) return false;
    boolean r;
    Marker m = enter_section_(b);
    r = consumeToken(b, FEATURE_BLOCK_START);
    r = r && feature_block_1(b, l + 1);
    r = r && consumeToken(b, BLOCK_END);
    exit_section_(b, m, FEATURE_BLOCK, r);
    return r;
  }

  // CODE?
  private static boolean feature_block_1(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "feature_block_1")) return false;
    consumeToken(b, CODE);
    return true;
  }

  /* ********************************************************** */
  // !BLOCK_END
  static boolean not_block_end(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "not_block_end")) return false;
    boolean r;
    Marker m = enter_section_(b, l, _NOT_);
    r = !consumeToken(b, BLOCK_END);
    exit_section_(b, l, m, r, false, null);
    return r;
  }

  /* ********************************************************** */
  // (TEXT|directive|block)*
  static boolean t4File(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "t4File")) return false;
    while (true) {
      int c = current_position_(b);
      if (!t4File_0(b, l + 1)) break;
      if (!empty_element_parsed_guard_(b, "t4File", c)) break;
    }
    return true;
  }

  // TEXT|directive|block
  private static boolean t4File_0(PsiBuilder b, int l) {
    if (!recursion_guard_(b, l, "t4File_0")) return false;
    boolean r;
    r = consumeToken(b, TEXT);
    if (!r) r = directive(b, l + 1);
    if (!r) r = block(b, l + 1);
    return r;
  }

  static final Parser not_block_end_parser_ = new Parser() {
    public boolean parse(PsiBuilder b, int l) {
      return not_block_end(b, l + 1);
    }
  };
}
