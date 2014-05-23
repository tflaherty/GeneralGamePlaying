using System.Collections.Generic;
using System.Linq;
using Wintellect.PowerCollections;

namespace API.Parsing.KIFParserUngerParallel
{
    public class RuleCollection
    {
        public MultiDictionary<string, Rule> Rules = new MultiDictionary<string, Rule>(false);

        public void Add(Rule rule)
        {
            Rules.Add(rule.LHS.TheSymbol, rule);
        }

        public void Clear()
        {
            Rules.Clear();
        }

        public IEnumerable<Rule> FindByLHS(Symbol symbol)
        {
            var rules = Rules[symbol.TheSymbol];
            // ToDo: Check PowerCollections code to see if a null is ever returned
            if (rules == null)
            {
                return Enumerable.Empty<Rule>();
            }
            return rules;
        }

        public IEnumerable<Rule> FindByLHS(string lhs)
        {
            var rules = Rules[lhs];
            // ToDo: Check PowerCollections code to see if a null is ever returned
            if (rules == null)
            {
                return Enumerable.Empty<Rule>();
            }
            return rules;
        }
    }
}
