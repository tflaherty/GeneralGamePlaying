#define CACHE_INPUT_MAPS
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace API.Parsing.KIFParserUngerParallel
{
    public class RuleInputMapper
    {
#if CACHE_INPUT_MAPS
        static private readonly ConcurrentDictionary<long, List<List<int>>> InputMapDict = new ConcurrentDictionary<long, List<List<int>>>();
#endif

        public string Input { get; private set; }
        public Rule Rule { get; private set; }
        public int InputPos { get; private set; }
        public int Length { get; private set; }
        public List<int> Lengths;
        public long Key;
        public bool AllowZeroLengthAssignment;
        public RuleInputMapper(Rule rule, int inputPos, int length, bool allowZeroLengthAssignment = false)
        {
            Rule = rule;
            InputPos = inputPos;
            Length = length;
            Lengths = Enumerable.Repeat((int)0, Rule.RHS.Count).ToList();
            Key = this.Rule.RHS.Count + (length << 16);
            AllowZeroLengthAssignment = allowZeroLengthAssignment;
        }

        public IEnumerable<List<int>> GetInputMap()
        {
#if CACHE_INPUT_MAPS
            if (InputMapDict.ContainsKey(Key))
            {
                foreach (List<int> inputMap in InputMapDict[Key])
                {
                    yield return inputMap;
                }
            }
            else
#endif
            {
                var listOfMaps = new List<List<int>>();
                try
                {
                    foreach (var map in AssignInputLength(0, Length))
                    {
                        listOfMaps.Add(new List<int>(map));
                    }
                }
                catch (Exception)
                {
                    
                    throw;
                }

#if CACHE_INPUT_MAPS
                InputMapDict.TryAdd(Key, listOfMaps);
#endif

                foreach (List<int> inputMap in listOfMaps)
                {
                    yield return inputMap;
                }
            }
        }

        private IEnumerable<List<int>> AssignInputLength(int nextListPos, int lengthRemaining)
        {
            if (nextListPos == Lengths.Count - 1)
            {
                try
                {
                    Lengths[nextListPos] = lengthRemaining;
                }
                catch (Exception)
                {
                    
                    throw;
                }
                yield return Lengths;
            }
            else
            {
                int start = AllowZeroLengthAssignment ? 0 : 1;
                for (int i = start; i <= lengthRemaining; i++)
                {
                    try
                    {
                        Lengths[nextListPos] = i;
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                    foreach (var map in AssignInputLength(nextListPos + 1, lengthRemaining - i))
                    {
                        yield return map;
                    }
                }
            }            
        }
    }
}
