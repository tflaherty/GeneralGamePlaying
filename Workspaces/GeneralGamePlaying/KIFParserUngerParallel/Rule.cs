using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TFStreamTokenizer;

namespace API.Parsing.KIFParserUngerParallel
{
    // ToDo:  Create a common constructor
    public class Rule
    {
        // Since this is a parser for Context Free Grammars
        // we only need one symbol on the left hand side.
        public Symbol LHS { get; private set; }
        public List<Symbol> RHS;

        public Rule(Symbol lhs, RuleCollection ruleCollection)
        {
            LHS = lhs;
            RHS = new List<Symbol>();
            ruleCollection.Add(this);
        }

        public Rule(Symbol lhs, IEnumerable<object> rhsObjects, RuleCollection ruleCollection)
        {
            LHS = lhs;
            RHS = new List<Symbol>();
            foreach (object rhsObject in rhsObjects)
            {
                RHS.Add(new Symbol(rhsObject));
            }
            ruleCollection.Add(this);
        }

        public Rule(Symbol lhs, object rhsObject, RuleCollection ruleCollection)
        {
            LHS = lhs;
            RHS = new List<Symbol>();
            RHS.Add(new Symbol(rhsObject));
            ruleCollection.Add(this);
        }

        public Rule(Symbol lhs, List<Symbol> rhs, RuleCollection ruleCollection)
        {
            LHS = lhs;
            RHS = rhs;
            ruleCollection.Add(this);
        }

        public Rule(Symbol lhs, Symbol rhs, RuleCollection ruleCollection)
        {
            LHS = lhs;
            RHS = new List<Symbol>() { rhs };
            ruleCollection.Add(this);
        }

        static public void CreateDisjunctiveRules(Symbol lhs, char lowCharInRange, char highCharInRange, bool? rhsSymbolsAreTerminals, RuleCollection ruleCollection)
        {
            char theChar = lowCharInRange;
            while (theChar <= highCharInRange)
            {
                var newSymbol = new Symbol(theChar.ToString(), rhsSymbolsAreTerminals);
                new Rule(lhs, newSymbol, ruleCollection);
                theChar++;
            }
        }

        static public void CreateDisjunctiveRules(Symbol lhs, IEnumerable<object > rhsObjects, RuleCollection ruleCollection, bool? isTerminal = null)
        {
            foreach (object rhsObject in rhsObjects)
            {
                new Rule(lhs, new Symbol(rhsObject, isTerminal), ruleCollection);
            }
        }

        public override string ToString()
        {
            if (RHS.Any())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(LHS.TheSymbol);
                sb.Append(" --> ");
                foreach (Symbol symbol in RHS)
                {
                    sb.Append(symbol.TheSymbol);
                    sb.Append(" ");
                }

                return sb.ToString();
            }
            else
            {
                return LHS.ToString() + " --> " + "\u0190";
            }
        }
    }

    public class RuleRange
    {
        public Rule TheRule { get; private set; }
        public int InputPos { get; private set; }
        public int Length { get; private set; }

        public RuleRange(RuleInputMap ruleInputMap)
        {
            InputPos = ruleInputMap.InputPos;
        }

        public RuleRange(Rule rule, int inputPos, int length)
        {
            TheRule = rule;
            InputPos = inputPos;
            Length = length;
        }

        public override string ToString()
        {
            return TheRule.ToString() + String.Format(" {0}-{1}", InputPos, InputPos + Length - 1);
        }
    }

    public class RuleInputMap
    {
        public List<int> Lengths = null;
        public int InputPos;

        public RuleInputMap(List<int> lengths, int inputPos)
        {
            Lengths = lengths;
            InputPos = inputPos;
        }

        public string ToString(string input)
        {
            StringBuilder sb = new StringBuilder();
            int currentPos = InputPos;
            foreach (int u in Lengths)
            {
                if (u == 0)
                {
                    sb.Append("\u0190" + ", ");
                }
                else
                {
                    sb.Append(input.Substring(currentPos, u) + ", ");
                    currentPos += u;
                }
            }
            return sb.ToString().Substring(0, sb.ToString().Count() - 2);
        }

        public string ToString(List<Token> input)
        {
            StringBuilder sb = new StringBuilder();
            int currentPos = InputPos;
            foreach (int u in Lengths)
            {
                if (u == 0)
                {
                    sb.Append("\u0190" + ", ");
                }
                else
                {
                    sb.Append(String.Join(", ", input.GetRange(currentPos, u)));
                    currentPos += u;
                }
            }
            return sb.ToString().Substring(0, sb.ToString().Count() - 2);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            int currentPos = InputPos;
            foreach (int u in Lengths)
            {
                if (u == 0)
                {
                    sb.Append("\u0190" + ", ");
                }
                else
                {
                    sb.Append(currentPos + ", ");
                    currentPos += u;
                }
            }
            return sb.ToString().Substring(0, sb.ToString().Count() - 2);
        }
    }

}
