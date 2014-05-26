using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using API.GGP.PredicateLogic;
using API.Utilities.TFTree;
using API.GGP.GGPInterfacesNS;
using TFStreamTokenizer;
using API.UtilitiesAndExtensions;

namespace API.Parsing.KIFParserUngerParallel
{
    // ToDo:  Put in exception handling!
    public class KIFParserUngerParallel : IKIFParser
    {
#region Fields, Properties, Events and Delegates
#region Events and Delegates
        // static public
        // instance public
        // overrides public

        // static non-public
        // instance non-public
        // overrides non-public
#endregion

#region Fields (Except Those Wrapped by Properties)
        // static public
        // instance public
        // overrides public

        // static non-public
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<int, TFTree<ParseTreeNodeData>>> parseTreeCache = new ConcurrentDictionary<string, ConcurrentDictionary<int, TFTree<ParseTreeNodeData>>>();
        private static readonly Dictionary<string, TFTree<ParseTreeNodeData>> collapsedParseTreeCache = new Dictionary<string, TFTree<ParseTreeNodeData>>();
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<int, HornClause>> hornClauseCache = new ConcurrentDictionary<string, ConcurrentDictionary<int, HornClause>>();

        // instance non-public
        private string kifFilePath;
        private string kifString;
        private readonly List<List<TFStreamTokenizer.Token>> sentences = new List<List<TFStreamTokenizer.Token>>();
        private ConcurrentDictionary<int, TFTree<ParseTreeNodeData>> parseTrees = new ConcurrentDictionary<int, TFTree<ParseTreeNodeData>>();
        private readonly KIFRuleCollection ruleCollection = KIFRuleCollection.Instance;

        // overrides non-public
#endregion

#region Properties
        // static public
        // instance public
        public long ParseElapsedTimeInUS { get; private set; }
        // overrides public

        // static non-public
        // instance non-public
        // overrides non-public
#endregion
#endregion

#region Constructors and Methods
#region Constructors and Finalizers
        // static public
        // instance public

        // static non-public
        // instance non-public
#endregion

#region Methods
        // static public
        // instance public
        // interface public
#region IKIFParser
        // ToDo:  Find out what parts of this code only need to be 
        // initialized once no matter how many times we want to parse
        // different files with the same KIFParserUngerParallel instance
        public void ParseFromFilePath(string filePath)
        {
            long startTime = Stopwatch.GetTimestamp();

            kifFilePath = filePath;
            kifString = null;

            ConcurrentDictionary<int, TFTree<ParseTreeNodeData>> tmpParseTrees;
            if (parseTreeCache.TryGetValue(kifFilePath, out tmpParseTrees))
            {
                parseTrees = tmpParseTrees;
            }
            else
            {
                sentences.Clear();
                parseTrees.Clear();

                if (!ParseSentencesFromKIFFile())
                {
                    var ex = new FormatException("ParseSentencesFromKIFFile() error");
                    throw ex;
                }

                try
                {
                    if (!DoParse())
                    {
                        var ex = new FormatException("DoParse() error");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    var newEx = new Exception("DoParse() exception", ex);
                    throw newEx;
                }
                
                parseTreeCache.TryAdd(kifFilePath, parseTrees);

                StreamReader streamReader = new StreamReader(kifFilePath);
                string kifDescription = streamReader.ReadToEnd();
                streamReader.Close();

                parseTreeCache.TryAdd(kifDescription, parseTrees);
                var hornClauses = GetHornClauses(); // this caches the horn clauses by file name
                hornClauseCache.TryAdd(kifDescription, hornClauses);

            }

            ParseElapsedTimeInUS = (long)Math.Truncate(((decimal)Stopwatch.GetTimestamp() - (decimal)startTime) * 1000000.0m / Stopwatch.Frequency);

            return;
        }

        public void ParseFromString(string theStringToParse)
        {
            long startTime = Stopwatch.GetTimestamp();

            kifFilePath = null;
            kifString = theStringToParse;

            ConcurrentDictionary<int, TFTree<ParseTreeNodeData>> tmpParseTrees;
            if (parseTreeCache.TryGetValue(kifString, out tmpParseTrees))
            {
                parseTrees = tmpParseTrees;
            }
            else
            {
                sentences.Clear();
                parseTrees.Clear();

                if (!ParseSentencesFromKIFString(theStringToParse))
                {
                    var ex = new FormatException("ParseSentencesFromKIFString() error");
                    throw ex;
                }

                try
                {
                    if (!DoParse())
                    {
                        var ex = new FormatException("DoParse() error");
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    var newEx = new Exception("DoParse() exception", ex);
                    throw newEx;
                }

                parseTreeCache.TryAdd(kifString, parseTrees);
                GetHornClauses(); // this caches the horn clauses by description
            }

            ParseElapsedTimeInUS = (long)Math.Truncate(((decimal)Stopwatch.GetTimestamp() - (decimal)startTime) * 1000000.0m / Stopwatch.Frequency);

            return;
        }

        public void SaveHornClauses(string filePath, string baseName, FileMode mode)
        {
            throw new NotImplementedException();
        }

        public ConcurrentDictionary<int, HornClause> GetHornClauses()
        {
            ConcurrentDictionary<int, HornClause> hornClauses;

            if (hornClauseCache.TryGetValue(kifFilePath ?? kifString, out hornClauses))
            {                
            }
            else
            {
                // it's not in the in-memory cache so lets see if it's been serialized to disk
                var binFilePath = Path.GetDirectoryName(kifFilePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(kifFilePath) + ".bin";
                if (File.Exists(binFilePath) && (File.GetLastWriteTime(kifFilePath ?? kifString).CompareTo(File.GetLastWriteTime(binFilePath)) < 0))
                {
                    return HornClause.SerializeFromFile(binFilePath);
                }

                hornClauses = new ConcurrentDictionary<int, HornClause>();
                string error;

                foreach (KeyValuePair<int, TFTree<ParseTreeNodeData>> keyValuePair in parseTrees.OrderBy(n => n.Key))
                {
                    var collapsedTree =
                        keyValuePair.Value.Root.CreateCollapsedTree(
                            (n =>
                                n.Data.TheGoal.Symbol.TheSymbol == "Sentences" ||
                                n.Data.TheGoal.Symbol.TheSymbol == "Sentence" ||
                                n.Data.TheGoal.Symbol.TheSymbol == "Terms" ||
                                n.Data.TheGoal.Symbol.TheSymbol == "Term" ||
                                n.Data.TheGoal.Symbol.TheSymbol == "RelWord" ||
                                n.Data.TheGoal.Symbol.TheSymbol == "LeftParen" ||
                                n.Data.TheGoal.Symbol.TheSymbol == "RightParen"));

                    var hornClause = ParseTreeNodeData.CreateHornClause(collapsedTree, out error);
                    if (hornClause != null)
                    {
                        hornClauses[keyValuePair.Key] = hornClause;
                    }
                }

                hornClauseCache.TryAdd(kifFilePath ?? kifString, hornClauses);
                HornClause.SerializeToFile(hornClauses, binFilePath);
            }

            return hornClauses;
        }

        public void SaveParseTrees(string filePath, string baseName, FileMode mode)
        {
            foreach (KeyValuePair<int, TFTree<ParseTreeNodeData>> keyValuePair in parseTrees.OrderBy(n => n.Key))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(String.Format(filePath + @"\{0}_{1}.xml", baseName, keyValuePair.Key)))
                {
                    keyValuePair.Value.WriteXml(xmlWriter);
                }
            }
        }

        public void SaveCollapsedParseTrees(string filePath, string baseName, FileMode mode)
        {
            foreach (KeyValuePair<int, TFTree<ParseTreeNodeData>> keyValuePair in parseTrees.OrderBy(n => n.Key))
            {
                var collapsedTree =
                    keyValuePair.Value.Root.CreateCollapsedTree(
                        (n =>
                            n.Data.TheGoal.Symbol.TheSymbol == "Sentences" ||
                            n.Data.TheGoal.Symbol.TheSymbol == "Sentence" ||
                            n.Data.TheGoal.Symbol.TheSymbol == "Terms" ||
                            n.Data.TheGoal.Symbol.TheSymbol == "Term" ||
                            n.Data.TheGoal.Symbol.TheSymbol == "RelWord" ||
                            n.Data.TheGoal.Symbol.TheSymbol == "LeftParen" ||
                            n.Data.TheGoal.Symbol.TheSymbol == "RightParen"));

                using (XmlWriter xmlWriter = XmlWriter.Create(String.Format(filePath + @"\{0}_collapsed{1}.xml", baseName, keyValuePair.Key)))
                {
                    collapsedTree.WriteXml(xmlWriter);
                }
            }
        }

#endregion  // IKIFParser

        // overrides public

        // static non-public
        // instance non-public

        private bool ParseSentencesFromKIFString(string theString)
        {
            try
            {
                using(var sr = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(theString))))
                {
                    var st = new StreamTokenizer(sr) { EOLIsSignificant = true };
                    Token nextToken = null;
                    int parenCount;

                    while ((nextToken = st.NextToken()).TheTokenType != TokenType.EndOfFile)
                    {
                        var input = new List<Token>();
                        parenCount = 0;

                        while (nextToken.TheTokenType != TokenType.EndOfFile)
                        {
                            input.Add(nextToken);

                            if (nextToken.TheTokenType == TokenType.LeftParen)
                            {
                                parenCount++;
                            }
                            else if (nextToken.TheTokenType == TokenType.RightParen)
                            {
                                if (--parenCount == 0) goto EndOfSentence;
                            }

                            nextToken = st.NextToken();
                        }

                        EndOfSentence:
                        sentences.Add(input);
                    }
                }

                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private bool ParseSentencesFromKIFFile()
        {
            using (var sr = new StreamReader(kifFilePath))
            {
                try
                {
                    var st = new StreamTokenizer(sr) { EOLIsSignificant = true };
                    Token nextToken = null;
                    int parenCount;

                    while ((nextToken = st.NextToken()).TheTokenType != TokenType.EndOfFile)
                    {
                        var input = new List<Token>();
                        parenCount = 0;

                        while (nextToken.TheTokenType != TokenType.EndOfFile)
                        {
                            input.Add(nextToken);

                            if (nextToken.TheTokenType == TokenType.LeftParen)
                            {
                                parenCount++;
                            }
                            else if (nextToken.TheTokenType == TokenType.RightParen)
                            {
                                if (--parenCount == 0) goto EndOfSentence;
                            }

                            nextToken = st.NextToken();
                        }

                    EndOfSentence:
                        sentences.Add(input);
                    }

                }
                catch (Exception ex)
                {                    
                    throw;
                }
            }

            return true;
        }

        private bool DoParse(bool useParallelTasks = true)
        {
            // put this in for debugging so we aren't running parallel "tasks"
            //sentences.RemoveRange(0, 2);
            //sentences.RemoveRange(1, sentences.Count-1);

            try
            {
                if (useParallelTasks)
                {
                    Parallel.For(0, sentences.Count, new ParallelOptions() { MaxDegreeOfParallelism = -1 }, i =>
                                                                                                               {
                                                                                                                   var goal1 = new Goal(new Symbol("Sentence"), 0, sentences[i].Count);
                                                                                                                   var ruleTree = new TFTree<Rule>();
                                                                                                                   ruleTree.Root =
                                                                                                                       new TFTreeNode<Rule>(
                                                                                                                           ruleCollection.Rules.ElementAt(0).Value as Rule);
                                                                                                                   var ruleRangeStack = new Stack<RuleRange>();
                                                                                                                   var goalStack = new Stack<Goal>();
                                                                                                                   var ruleInputMapStack = new Stack<RuleInputMap>();
                                                                                                                   var parseTreeTopNode =
                                                                                                                       new TFTreeNode<ParseTreeNodeData>(new ParseTreeNodeData(goal1, null));
                                                                                                                   var parseTree = new TFTree<ParseTreeNodeData>()
                                                                                                                                       {
                                                                                                                                           Root = parseTreeTopNode,
                                                                                                                                           XSLTFile = @"file:///D:/Temp/XMLPrettyPrint.xsl"
                                                                                                                                       };
                                                                                                                   if (goal1.NewProcess(sentences[i], 0, goalStack, ruleRangeStack,
                                                                                                                                        ruleInputMapStack, parseTreeTopNode,
                                                                                                                                        ruleTree.Root,
                                                                                                                                        null, ruleCollection))
                                                                                                                   {

                                                                                                                       parseTrees[i] = parseTree;

#if DEBUG_PARSER
                    textBox1.AppendText(
                        " \n---- rule input map stack ----------------------\n");
                    foreach (RuleInputMap ruleInputMap in ruleInputMapStack)
                    {
                        textBox1.AppendText(ruleInputMap.ToString(sentence) +
                                            Environment.NewLine);
                    }

                    textBox1.AppendText(
                        " \n---- rule range stack ----------------------\n");
                    foreach (RuleRange ruleRange in ruleRangeStack)
                    {
                        textBox1.AppendText(ruleRange.TheRule + " (" +
                                            String.Join(", ",
                                                        sentence.GetRange(
                                                            ruleRange.InputPos,
                                                            ruleRange.Length)) +
                                            ")" + Environment.NewLine);
                    }

                    textBox1.AppendText(" \n---- goal stack ----------------------\n");
                    foreach (Goal goal in goalStack)
                    {
                        textBox1.AppendText(goal.Symbol.TheSymbol + " (" +
                                            String.Join(", ",
                                                        sentence.GetRange(
                                                            goal.InputPos, goal.Length)) +
                                            ")" +
                                            Environment.NewLine);
                    }

                    textBox1.AppendText(" \n--------------------------\n");
                    long endTime = Stopwatch.GetTimestamp();
                                                                                                                       //var numNodes = parseTreeTopNode.GetDepthFirstEnumerable(TreeTraversalDirection.TopDown).Count();
                                                                                                                       //textBox1.AppendText(String.Format("Elapsed Time: {0}ms, # Nodes: {1}", 1000*(endTime-startTime)/Stopwatch.Frequency, numNodes));
                                                                                                                       //textBox1.Text = topNode.ToString(true);
#endif
                                                                                                                   }
                                                                                                                   else
                                                                                                                   {
                                                                                                                        DebugAndTraceHelper.WriteTraceLine(String.Format("Could not parse {0}", String.Join(" ", sentences[i].Select(n => n.Lexeme.ToString()))));
                                                                                                                        DebugAndTraceHelper.WriteTraceLine(String.Format("                {0}", String.Join(" ", sentences[i].Select(n => n.ToString()))));
                                                                                                                   }
                                                                                                               });                    
                }
                else  // do a serial parsing
                {
                    int sentenceCount = 0;
                    foreach (List<Token> sentence in sentences)
                    {
                        var goal1 = new Goal(new Symbol("Sentence"), 0, sentences[sentenceCount].Count);
                        var ruleTree = new TFTree<Rule>();
                        ruleTree.Root =
                            new TFTreeNode<Rule>(
                                ruleCollection.Rules.ElementAt(0).Value as Rule);
                        var ruleRangeStack = new Stack<RuleRange>();
                        var goalStack = new Stack<Goal>();
                        var ruleInputMapStack = new Stack<RuleInputMap>();
                        var parseTreeTopNode =
                            new TFTreeNode<ParseTreeNodeData>(new ParseTreeNodeData(goal1, null));
                        var parseTree = new TFTree<ParseTreeNodeData>()
                                            {
                                                Root = parseTreeTopNode,
                                                XSLTFile = @"file:///D:/Temp/XMLPrettyPrint.xsl"
                                            };
                        if (goal1.NewProcess(sentences[sentenceCount], 0, goalStack, ruleRangeStack,
                                             ruleInputMapStack, parseTreeTopNode,
                                             ruleTree.Root,
                                             null, ruleCollection))
                        {

                            parseTrees[sentenceCount] = parseTree;
                        }

                        sentenceCount++;
                    }
                }

                return true;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        // interface non-public
        // overrides non-public

#endregion
#endregion

        // ToDo:  return false if there are errors
    }
}
