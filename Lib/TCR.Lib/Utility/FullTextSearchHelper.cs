#region Usings

using System;
using System.Linq;

#endregion

namespace TCR.Lib.Utility
{
    //http://www.blackbeltcoder.com/Articles/data/easy-full-text-search-queries

    /*
     SQL Configuration
    
      sp_configure 'show advanced options', 1
      RECONFIGURE
      GO
      sp_configure 'transform noise words', 1
      RECONFIGURE
      GO
     
     * 
     * usage:
     SELECT * FROM Articles WHERE CONTAINS(Articles.*, @FtsQuery)

     */

    public class FullTextSearchHelper
    {
        public static string ConvertSearchQuery(string query)
        {
            var fts = new EasyFts();


            return fts.ToFtsQuery(query);
        }
    }

    internal class EasyFts
    {
        // Characters not allowed in unquoted search terms
        protected const string Punctuation = "~\"`!@#$%^&*()-+=[]{}\\|;:,.<>?/";
        // Class constructor

        /// <summary>
        ///     Converts an "easy" search term to a full-text search term.
        /// </summary>
        /// <param name="query">Search term to convert</param>
        /// <returns>A valid full-text search query</returns>
        public string ToFtsQuery(string query)
        {
            var node = FixUpExpressionTree(ParseNode(query, ConjunctionTypes.And), true);
            return (node != null) ? node.ToString() : string.Empty;
        }

        /// <summary>
        ///     Parses a query segment and converts it to an expression
        ///     tree.
        /// </summary>
        /// <param name="query">Query segment to convert</param>
        /// <param name="defaultConjunction">Implicit conjunction type</param>
        /// <returns>Root node of expression tree</returns>
        private INode ParseNode(string query, ConjunctionTypes defaultConjunction)
        {
            var termForm = TermForms.Inflectional;
            var termExclude = false;
            var conjunction = defaultConjunction;
            var resetState = true;
            INode root = null;
            INode node;
            string term;

            var parser = new TextParser(query);
            while (!parser.EndOfText)
            {
                if (resetState)
                {
                    // Reset modifiers
                    termForm = TermForms.Inflectional;
                    termExclude = false;
                    conjunction = defaultConjunction;
                    resetState = false;
                }

                parser.MovePastWhitespace();
                if (!parser.EndOfText &&
                    !Punctuation.Contains(parser.Peek()))
                {
                    // Extract query term
                    var start = parser.Position;
                    parser.MoveAhead();
                    while (!parser.EndOfText &&
                           !Punctuation.Contains(parser.Peek()) &&
                           !char.IsWhiteSpace(parser.Peek()))
                        parser.MoveAhead();

                    // Allow trailing wildcard
                    if (parser.Peek() == '*')
                    {
                        parser.MoveAhead();
                        termForm = TermForms.Literal;
                    }

                    // Interpret token
                    term = parser.Extract(start, parser.Position);
                    if (string.Compare(term, "AND", true) == 0)
                        conjunction = ConjunctionTypes.And;
                    else if (string.Compare(term, "OR", true) == 0)
                        conjunction = ConjunctionTypes.Or;
                    else if (string.Compare(term, "NEAR", true) == 0)
                        conjunction = ConjunctionTypes.Near;
                    else if (string.Compare(term, "NOT", true) == 0)
                        termExclude = true;
                    else
                    {
                        root = AddNode(root, term, termForm, termExclude, conjunction);
                        resetState = true;
                    }
                    continue;
                }
                if (parser.Peek() == '"')
                {
                    // Match next term exactly
                    termForm = TermForms.Literal;
                    // Extract quoted term
                    term = ExtractQuote(parser);
                    root = AddNode(root, term.Trim(), termForm, termExclude, conjunction);
                    resetState = true;
                }
                else if (parser.Peek() == '(')
                {
                    // Parse parentheses block
                    term = ExtractBlock(parser, '(', ')');
                    node = ParseNode(term, defaultConjunction);
                    root = AddNode(root, node, conjunction, true);
                    resetState = true;
                }
                else if (parser.Peek() == '<')
                {
                    // Parse angle brackets block
                    term = ExtractBlock(parser, '<', '>');
                    node = ParseNode(term, ConjunctionTypes.Near);
                    root = AddNode(root, node, conjunction);
                    resetState = true;
                }
                else if (parser.Peek() == '-')
                {
                    // Match when next term is not present
                    termExclude = true;
                }
                else if (parser.Peek() == '+')
                {
                    // Match next term exactly
                    termForm = TermForms.Literal;
                }
                else if (parser.Peek() == '~')
                {
                    // Match synonyms of next term
                    termForm = TermForms.Thesaurus;
                }
                // Advance to next character
                parser.MoveAhead();
            }
            return root;
        }

        /// <summary>
        ///     Fixes any portions of the expression tree that would produce an invalid SQL Server full-text
        ///     query.
        /// </summary>
        /// <remarks>
        ///     While our expression tree may be properly constructed, it may represent a query that
        ///     is not supported by SQL Server. This method traverses the expression tree and corrects
        ///     problem expressions as described below.
        ///     NOT term1 AND term2         Subexpressions swapped.
        ///     NOT term1                   Expression discarded.
        ///     NOT term1 AND NOT term2     Expression discarded if node is grouped (parenthesized)
        ///     or is the root node; otherwise, the parent node may
        ///     contain another subexpression that will make this one
        ///     valid.
        ///     term1 OR NOT term2          Expression discarded.
        ///     term1 NEAR NOT term2        NEAR conjunction changed to AND.*
        ///     * This method converts all NEAR conjunctions to AND when either subexpression is not
        ///     an InternalNode with the form TermForms.Literal.
        /// </remarks>
        /// <param name="node">Node to fix up</param>
        /// <param name="isRoot">True if node is the tree's root node</param>
        protected INode FixUpExpressionTree(INode node, bool isRoot = false)
        {
            // Test for empty expression tree
            if (node == null) return null;

            // Special handling for internal nodes
            if (node is InternalNode)
            {
                // Fix up child nodes
                var internalNode = node as InternalNode;
                internalNode.Child1 = FixUpExpressionTree(internalNode.Child1);
                internalNode.Child2 = FixUpExpressionTree(internalNode.Child2);

                // Correct subexpressions incompatible with conjunction type
                if (internalNode.Conjunction == ConjunctionTypes.Near)
                {
                    // If either subexpression is incompatible with NEAR conjunction then change to AND
                    if (IsInvalidWithNear(internalNode.Child1) || IsInvalidWithNear(internalNode.Child2))
                        internalNode.Conjunction = ConjunctionTypes.And;
                }
                else if (internalNode.Conjunction == ConjunctionTypes.Or)
                {
                    // Eliminate subexpressions not valid with OR conjunction
                    if (IsInvalidWithOr(internalNode.Child1))
                        internalNode.Child1 = null;
                    if (IsInvalidWithOr(internalNode.Child2))
                        internalNode.Child1 = null;
                }

                // Handle eliminated child expressions
                if (internalNode.Child1 == null && internalNode.Child2 == null)
                {
                    // Eliminate parent node if both child nodes were eliminated
                    return null;
                }
                if (internalNode.Child1 == null)
                {
                    // Child1 eliminated so return only Child2
                    node = internalNode.Child2;
                }
                else if (internalNode.Child2 == null)
                {
                    // Child2 eliminated so return only Child1
                    node = internalNode.Child1;
                }
                else
                {
                    // Determine if entire expression is an exclude expression
                    internalNode.Exclude = (internalNode.Child1.Exclude && internalNode.Child2.Exclude);
                    // If only first child expression is an exclude expression,
                    // then simply swap child expressions
                    if (!internalNode.Exclude && internalNode.Child1.Exclude)
                    {
                        var temp = internalNode.Child1;
                        internalNode.Child1 = internalNode.Child2;
                        internalNode.Child2 = temp;
                    }
                }
            }
            // Eliminate expression group if it contains only exclude expressions
            return ((node.Grouped || isRoot) && node.Exclude) ? null : node;
        }

        /// <summary>
        ///     Determines if the specified node is invalid on either side of a NEAR conjuction.
        /// </summary>
        /// <param name="node">Node to test</param>
        protected bool IsInvalidWithNear(INode node)
        {
            // NEAR is only valid with TerminalNodes with form TermForms.Literal
            return !(node is TerminalNode) || ((TerminalNode) node).TermForm != TermForms.Literal;
        }

        /// <summary>
        ///     Determines if the specified node is invalid on either side of an OR conjunction.
        /// </summary>
        /// <param name="node">Node to test</param>
        protected bool IsInvalidWithOr(INode node)
        {
            // OR is only valid with non-null, non-excluded (NOT) subexpressions
            return node == null || node.Exclude;
        }

        /// <summary>
        ///     Creates an expression node and adds it to the
        ///     give tree.
        /// </summary>
        /// <param name="root">Root node of expression tree</param>
        /// <param name="term">Term for this node</param>
        /// <param name="termForm">Indicates form of this term</param>
        /// <param name="termExclude">Indicates if this is an excluded term</param>
        /// <param name="conjunction">Conjunction used to join with other nodes</param>
        /// <returns>The new root node</returns>
        protected INode AddNode(INode root, string term, TermForms termForm, bool termExclude,
            ConjunctionTypes conjunction)
        {
            if (term.Length > 0)
            {
                INode node = new TerminalNode
                {
                    Term = term,
                    TermForm = termForm,
                    Exclude = termExclude
                };
                root = AddNode(root, node, conjunction);
            }
            return root;
        }

        /// <summary>
        ///     Adds an expression node to the given tree.
        /// </summary>
        /// <param name="root">Root node of expression tree</param>
        /// <param name="node">Node to add</param>
        /// <param name="conjunction">Conjunction used to join with other nodes</param>
        /// <returns>The new root node</returns>
        protected INode AddNode(INode root, INode node, ConjunctionTypes conjunction, bool group = false)
        {
            if (node != null)
            {
                node.Grouped = group;
                if (root != null)
                    root = new InternalNode
                    {
                        Child1 = root,
                        Child2 = node,
                        Conjunction = conjunction
                    };
                else
                    root = node;
            }
            return root;
        }

        /// <summary>
        ///     Extracts a block of text delimited by the specified open and close
        ///     characters. It is assumed the parser is positioned at an
        ///     occurrence of the open character. The open and closing characters
        ///     are not included in the returned string. On return, the parser is
        ///     positioned at the closing character or at the end of the text if
        ///     the closing character was not found.
        /// </summary>
        /// <param name="parser">TextParser object</param>
        /// <param name="openChar">Start-of-block delimiter</param>
        /// <param name="closeChar">End-of-block delimiter</param>
        /// <returns>The extracted text</returns>
        private string ExtractBlock(TextParser parser, char openChar, char closeChar)
        {
            // Track delimiter depth
            var depth = 1;

            // Extract characters between delimiters
            parser.MoveAhead();
            var start = parser.Position;
            while (!parser.EndOfText)
            {
                if (parser.Peek() == openChar)
                {
                    // Increase block depth
                    depth++;
                }
                else if (parser.Peek() == closeChar)
                {
                    // Decrease block depth
                    depth--;
                    // Test for end of block
                    if (depth == 0)
                        break;
                }
                else if (parser.Peek() == '"')
                {
                    // Don't count delimiters within quoted text
                    ExtractQuote(parser);
                }
                // Move to next character
                parser.MoveAhead();
            }
            return parser.Extract(start, parser.Position);
        }

        /// <summary>
        ///     Extracts a block of text delimited by double quotes. It is
        ///     assumed the parser is positioned at the first quote. The
        ///     quotes are not included in the returned string. On return,
        ///     the parser is positioned at the closing quote or at the end of
        ///     the text if the closing quote was not found.
        /// </summary>
        /// <param name="parser">TextParser object</param>
        /// <returns>The extracted text</returns>
        private string ExtractQuote(TextParser parser)
        {
            // Extract contents of quote
            parser.MoveAhead();
            var start = parser.Position;
            while (!parser.EndOfText && parser.Peek() != '"')
                parser.MoveAhead();
            return parser.Extract(start, parser.Position);
        }

        /// <summary>
        ///     Query term forms.
        /// </summary>
        protected enum TermForms
        {
            Inflectional,
            Thesaurus,
            Literal
        }

        /// <summary>
        ///     Term conjunction types.
        /// </summary>
        protected enum ConjunctionTypes
        {
            And,
            Or,
            Near
        }

        /// <summary>
        ///     Common interface for expression nodes
        /// </summary>
        protected interface INode
        {
            /// <summary>
            ///     Indicates this term (or both child terms) should be excluded from
            ///     the results
            /// </summary>
            bool Exclude { get; set; }

            /// <summary>
            ///     Indicates this term is enclosed in parentheses
            /// </summary>
            bool Grouped { get; set; }
        }

        /// <summary>
        ///     Terminal (leaf) expression node class.
        /// </summary>
        private class TerminalNode : INode
        {
            // Class members
            public string Term { get; set; }
            public TermForms TermForm { get; set; }
            // Interface members
            public bool Exclude { get; set; }
            public bool Grouped { get; set; }
            // Convert node to string
            public override string ToString()
            {
                var fmt = string.Empty;
                if (TermForm == TermForms.Inflectional)
                    fmt = "{0}FORMSOF(INFLECTIONAL, {1})";
                else if (TermForm == TermForms.Thesaurus)
                    fmt = "{0}FORMSOF(THESAURUS, {1})";
                else if (TermForm == TermForms.Literal)
                    fmt = "{0}\"{1}\"";
                return string.Format(fmt,
                    Exclude ? "NOT " : string.Empty,
                    Term);
            }
        }

        /// <summary>
        ///     Internal (non-leaf) expression node class
        /// </summary>
        private class InternalNode : INode
        {
            // Class members
            public INode Child1 { get; set; }
            public INode Child2 { get; set; }
            public ConjunctionTypes Conjunction { get; set; }
            // Interface members
            public bool Exclude { get; set; }
            public bool Grouped { get; set; }
            // Convert node to string
            public override string ToString()
            {
                return string.Format(Grouped ? "({0} {1} {2})" : "{0} {1} {2}",
                    Child1,
                    Conjunction.ToString().ToUpper(),
                    Child2);
            }
        }
    }

    internal class TextParser
    {
        public static char NullChar = (char) 0;

        public TextParser()
        {
            Reset(null);
        }

        public TextParser(string text)
        {
            Reset(text);
        }

        public string Text { get; private set; }
        public int Position { get; private set; }

        public int Remaining
        {
            get { return Text.Length - Position; }
        }

        /// <summary>
        ///     Indicates if the current position is at the end of the current document
        /// </summary>
        public bool EndOfText
        {
            get { return (Position >= Text.Length); }
        }

        /// <summary>
        ///     Resets the current position to the start of the current document
        /// </summary>
        public void Reset()
        {
            Position = 0;
        }

        /// <summary>
        ///     Sets the current document and resets the current position to the start of it
        /// </summary>
        /// <param name="html"></param>
        public void Reset(string text)
        {
            Text = (text != null) ? text : string.Empty;
            Position = 0;
        }

        /// <summary>
        ///     Returns the character at the current position, or a null character if we're
        ///     at the end of the document
        /// </summary>
        /// <returns>The character at the current position</returns>
        public char Peek()
        {
            return Peek(0);
        }

        /// <summary>
        ///     Returns the character at the specified number of characters beyond the current
        ///     position, or a null character if the specified position is at the end of the
        ///     document
        /// </summary>
        /// <param name="ahead">The number of characters beyond the current position</param>
        /// <returns>The character at the specified position</returns>
        public char Peek(int ahead)
        {
            var pos = (Position + ahead);
            if (pos < Text.Length)
                return Text[pos];
            return NullChar;
        }

        /// <summary>
        ///     Extracts a substring from the specified position to the end of the text
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public string Extract(int start)
        {
            return Extract(start, Text.Length);
        }

        /// <summary>
        ///     Extracts a substring from the specified range of the current text
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public string Extract(int start, int end)
        {
            return Text.Substring(start, end - start);
        }

        /// <summary>
        ///     Moves the current position ahead one character
        /// </summary>
        public void MoveAhead()
        {
            MoveAhead(1);
        }

        /// <summary>
        ///     Moves the current position ahead the specified number of characters
        /// </summary>
        /// <param name="ahead">The number of characters to move ahead</param>
        public void MoveAhead(int ahead)
        {
            Position = Math.Min(Position + ahead, Text.Length);
        }

        /// <summary>
        ///     Moves to the next occurrence of the specified string. Comparisons
        ///     are case-sensitive.
        /// </summary>
        /// <param name="s">String to find</param>
        public void MoveTo(string s)
        {
            MoveTo(s, false);
        }

        /// <summary>
        ///     Moves to the next occurrence of the specified string
        /// </summary>
        /// <param name="s">String to find</param>
        /// <param name="ignoreCase">Indicates if case-insensitive comparisons are used</param>
        public void MoveTo(string s, bool ignoreCase)
        {
            Position = Text.IndexOf(s, Position,
                ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            if (Position < 0)
                Position = Text.Length;
        }

        /// <summary>
        ///     Moves to the next occurrence of the specified character
        /// </summary>
        /// <param name="c">Character to find</param>
        public void MoveTo(char c)
        {
            Position = Text.IndexOf(c, Position);
            if (Position < 0)
                Position = Text.Length;
        }

        /// <summary>
        ///     Moves to the next occurrence of any one of the specified
        ///     characters
        /// </summary>
        /// <param name="chars">Array of characters to find</param>
        public void MoveTo(char[] chars)
        {
            Position = Text.IndexOfAny(chars, Position);
            if (Position < 0)
                Position = Text.Length;
        }

        /// <summary>
        ///     Moves to the next occurrence of any character that is not one
        ///     of the specified characters
        /// </summary>
        /// <param name="chars">Array of characters to move past</param>
        public void MovePast(char[] chars)
        {
            while (IsInArray(Peek(), chars))
                MoveAhead();
        }

        /// <summary>
        ///     Determines if the specified character exists in the specified
        ///     character array.
        /// </summary>
        /// <param name="c">Character to find</param>
        /// <param name="chars">Character array to search</param>
        /// <returns></returns>
        protected bool IsInArray(char c, char[] chars)
        {
            foreach (var ch in chars)
            {
                if (c == ch)
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Moves the current position to the first character that is part of a newline
        /// </summary>
        public void MoveToEndOfLine()
        {
            var c = Peek();
            while (c != '\r' && c != '\n' && !EndOfText)
            {
                MoveAhead();
                c = Peek();
            }
        }

        /// <summary>
        ///     Moves the current position to the next character that is not whitespace
        /// </summary>
        public void MovePastWhitespace()
        {
            while (char.IsWhiteSpace(Peek()))
                MoveAhead();
        }
    }
}