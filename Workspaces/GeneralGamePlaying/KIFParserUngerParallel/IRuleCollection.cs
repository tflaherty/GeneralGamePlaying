using System.Collections.Generic;

namespace API.Parsing.KIFParserUngerParallel
{
    public interface IRuleCollection
    {
        void Add(Rule rule);
        void Clear();
        IEnumerable<Rule> FindByLHS(Symbol symbol);
        IEnumerable<Rule> FindByLHS(string lhs);
    }
}