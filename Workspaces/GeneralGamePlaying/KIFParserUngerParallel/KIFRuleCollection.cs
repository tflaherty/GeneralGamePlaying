#define INCLUDE_LESS_THAN_OR_EQUAL_AS_LOGICAL_SENTENCE_OPERATOR

using System;
using System.Collections.Generic;
using TFStreamTokenizer;

namespace API.Parsing.KIFParserUngerParallel
{
    // this is implemented as a thread safe singleton from:
    // http://msdn.microsoft.com/en-us/library/ff650316.aspx
    public sealed class KIFRuleCollection : RuleCollection
    {
        private static object SyncRoot = new Object();

        private static volatile KIFRuleCollection _Instance;
        public static KIFRuleCollection Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_Instance == null)
                        {
                            _Instance = new KIFRuleCollection();
                        }
                    }
                }

                return _Instance;
            }
        }

        private KIFRuleCollection()
        {
            // KIF characters
            // these are all handled by the StreamTokenizer
            //Rule.CreateDisjunctiveRules(new Symbol("Upper"), 'A', 'Z', true);
            //Rule.CreateDisjunctiveRules(new Symbol("Lower"), 'a', 'z', true);
            //Rule.CreateDisjunctiveRules(new Symbol("Digit"), '0', '9', true);
            //Rule.CreateDisjunctiveRules(new Symbol("Special"), new List<string>() { "!", "$", "%", "&", "*", "+", "-", ".", "/", "<", "=", ">", "?", "@", "_", "~" }, true);
            //Rule.CreateDisjunctiveRules(new Symbol("White"), new List<string>() { " ", "\t", "\r", "\n", "\f" }, true);
            //Rule.CreateDisjunctiveRules(new Symbol("InitialChar"), new List<string>() { "Upper", "Lower" }, false);
            //Rule.CreateDisjunctiveRules(new Symbol("WordChar"), new List<string>() { "Upper", "Lower", "Digit", "Special" }, false);
            //Rule.CreateDisjunctiveRules(new Symbol("Character"), new List<string>() { "Upper", "Lower", "Digit", "Special", "White" }, false);

            // KIF lexemes (modified to remove left recursion)
            // ToDo: modify these to use e-rules to test my e-rule functionality (once it's in!) (and if I even need it)
            // string ::= "character*"
            // these are all handled by the StreamTokenizer
            //new Rule(new Symbol("QuotedString"), new List<string>() { "\"", "\"" });
            //new Rule(new Symbol("QuotedString"), new List<string>() { "\"", "Characters", "\"" });
            //new Rule(new Symbol("Characters"), new List<string>() { "Character" });
            //new Rule(new Symbol("Characters"), new List<string>() { "Character", "Characters" });

            // word ::= initialchar wordchar*
            // these are all handled by the StreamTokenizer
            //new Rule(new Symbol("WordTail"), new List<string>() { "WordChar" });
            //new Rule(new Symbol("WordTail"), new List<string>() { "WordChar", "WordTail" });
            //new Rule(new Symbol("Word"), new List<string>() { "InitialChar", "WordTail" });
            //new Rule(new Symbol("Word"), new List<string>() { "InitialChar" });

            // variable ::= ?word | @word
            //new Rule(new Symbol("Variable"), new List<object>() { "?", TokenType.Word });
            //new Rule(new Symbol("Variable"), new List<object>() { "@", TokenType.Word });
            new Rule(new Symbol("Variables"), new List<object>() { TokenType.Variable }, this);
            new Rule(new Symbol("Variables"), new List<object>() { TokenType.Variable, "Variables" }, this);

            // KIF expressions (modified to remove left recursion)
            // number ::= [-] digit+ [. digit+] [exponent]
            // these are all handled by the StreamTokenizer
            //Rule.CreateDisjunctiveRules(new Symbol("Number"), new List<string>() { "Integer", "Real" });
            //new Rule(new Symbol("Integer"), new List<string>() { "Sign", "Digits" });
            //new Rule(new Symbol("Integer"), new List<string>() { "Digits" });
            //new Rule(new Symbol("Real"), new List<string>() { "Integer", "Fraction" });
            //new Rule(new Symbol("Real"), new List<string>() { "Integer", "Fraction", "Exponent" });
            //new Rule(new Symbol("Fraction"), new List<string>() { ".", "Digits" });

            // these are all handled by the StreamTokenizer
            //Rule.CreateDisjunctiveRules(new Symbol("Sign"), new List<string>() { "+", "-" });
            //new Rule(new Symbol("Digits"), new List<string>() { "Digit" });
            //new Rule(new Symbol("Digits"), new List<string>() { "Digit", "Digits" });

            // exponent ::= e [-] digit+
            // these are all handled by the StreamTokenizer
            //new Rule(new Symbol("Exponent"), new List<string>() { "e", "Integer" });

#if USE_FUNCTIONS_IN_PARSER
    //funword ::= initialchar wordchar*
            new Rule(new Symbol("FunWord"), TokenType.Word);
#endif
            //relword ::= initialchar wordchar*
            new Rule(new Symbol("RelWord"), TokenType.Word, this);

            //term ::= variable | word | string | funterm | number | sentence
#if USE_FUNCTIONS_IN_PARSER
            Rule.CreateDisjunctiveRules(new Symbol("Term"), new List<object>() { TokenType.Variable, TokenType.Word, 
                TokenType.QuotedString, "FunTerm", TokenType.Number, "Sentence" });
#else
            Rule.CreateDisjunctiveRules(new Symbol("Term"), new List<object>() { TokenType.Variable, TokenType.Word, 
                                                                                 TokenType.QuotedString, TokenType.Number, "Sentence" }, this);
#endif
            new Rule(new Symbol("Terms"), new List<object>() { "Term" }, this);
            new Rule(new Symbol("Terms"), new List<object>() { "Term", "Terms" }, this);

#if USE_FUNCTIONS_IN_PARSER

    //funterm ::= (funword term+)
            new Rule(new Symbol("FunTerm"), new List<object>() { TokenType.LeftParen, "FunWord", "Terms", TokenType.RightParen });
#endif

            //sentence ::= word | equation | relsent | logsent | quantsent
            // I changed the following line to the line below it on 9/6/13 in hopes it would speed up parsing but it didn't!
            //Rule.CreateDisjunctiveRules(new Symbol("Sentence"), new List<object>() { "LogSent", TokenType.Word, "Equation", "RelSent", "Quantsent", "TermSent" }, this);
            Rule.CreateDisjunctiveRules(new Symbol("Sentence"), new List<object>() { "LogSent", "RelSent", TokenType.Word, "Equation", "Quantsent", "TermSent" }, this);
            new Rule(new Symbol("Sentences"), new List<object>() { "Sentence", "Sentences" }, this);
            new Rule(new Symbol("Sentences"), new List<object>() { "Sentence" }, this);

            // ToDo: See if I can merge the "(" with the "=")
            //equation ::= (= term term)          
            new Rule(new Symbol("Equation"), new List<object>() { TokenType.LeftParen, TokenType.Equal, "Term", "Term", TokenType.RightParen }, this);

            //relsent ::= (relword term+)
            //relsent ::= (relword)
            new Rule(new Symbol("RelSent"), new List<object>() { TokenType.LeftParen, "RelWord", "Terms", TokenType.RightParen }, this);
            new Rule(new Symbol("RelSent"), new List<object>() { TokenType.LeftParen, "RelWord", TokenType.RightParen }, this);

            // ToDo: put this back in if we want to run the same parser for KIF sentences and a list of moves
            //termsent ::= (term+)
            //new Rule(new Symbol("TermSent"), new List<object>() { TokenType.LeftParen, TokenType.LeftParen, "Term", TokenType.RightParen, TokenType.LeftParen, "Term", TokenType.RightParen, TokenType.RightParen }, this);

            //logsent ::= (not sentence) |
            //            (and sentence+) |
            //            (or sentence+) |
            //            (<= sentence sentence) |
            //            (=> sentence sentence) |
            //            (<=> sentence sentence)
#if INCLUDE_LESS_THAN_OR_EQUAL_AS_LOGICAL_SENTENCE_OPERATOR
            // I moved this to be the first rule on 9/6/13 in hopes it would speed up parsing but it didn't!
            new Rule(new Symbol("LogSent"),
                     new List<object>() { TokenType.LeftParen, TokenType.ImplicationToLeft, "Sentences", TokenType.RightParen }, this);
#endif
            new Rule(new Symbol("LogSent"),
                     new List<object>() { TokenType.LeftParen, "not", "Sentence", TokenType.RightParen }, this);
            new Rule(new Symbol("LogSent"),
                     new List<object>() { TokenType.LeftParen, "and", "Sentences", TokenType.RightParen }, this);
            new Rule(new Symbol("LogSent"),
                     new List<object>() { TokenType.LeftParen, "or", "Sentences", TokenType.RightParen }, this);
            new Rule(new Symbol("LogSent"),
                     new List<object>() { TokenType.LeftParen, TokenType.ImplicationToRight, "Sentences", TokenType.RightParen }, this);
            new Rule(new Symbol("LogSent"),
                     new List<object>() { TokenType.LeftParen, TokenType.Equivalence, "Sentence", "Sentence", TokenType.RightParen }, this);

            //quantsent ::= (forall (variable+) sentence) |
            //              (exists (variable+) sentence)
            new Rule(new Symbol("QuantSent"),
                     new List<object>() { TokenType.LeftParen, "forall", TokenType.LeftParen, "Variables", TokenType.RightParen, "Sentence", TokenType.RightParen }, this);
            new Rule(new Symbol("QuantSent"),
                     new List<object>() { TokenType.LeftParen, "exists", TokenType.LeftParen, "Variables", TokenType.RightParen, "Sentence", TokenType.RightParen }, this);

        }
    }
}
