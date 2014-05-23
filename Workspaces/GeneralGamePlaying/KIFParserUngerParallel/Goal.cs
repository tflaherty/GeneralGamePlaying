//#define DEBUG_OUT
//#define INCLUDE_UNSUCCESSFUL_NODES_IN_PARSE_TREE
#define USE_TOKENTYPE_IN_SYMBOL
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using API.Utilities.TFTree;
using API.UtilitiesAndExtensions;
using TFStreamTokenizer;

namespace API.Parsing.KIFParserUngerParallel
{
    public class Goal
    {
        private static int visitCount = 0;
        public Symbol Symbol { get; private set; }
        public int InputPos { get; private set; }
        public int Length { get; private set; }

        public Goal(Symbol symbol, int inputPos, int length)
        {
            Symbol = symbol;
            InputPos = inputPos;
            Length = length;
        }


        public bool NewProcess(List<Token> input, int level,
            Stack<Goal> goalStack,
            Stack<RuleRange> ruleRangeStack,
            Stack<RuleInputMap> ruleInputMapStack,
            TFTreeNode<ParseTreeNodeData> parentParseTreeNode,
            TFTreeNode<Rule> ruleNode,
            StringBuilder debugOut,
            RuleCollection ruleCollection)
        {
            if (visitCount++ == 10000)
            {
                visitCount++;
            }

            try
            {
                string levelString = new string(Enumerable.Repeat(' ', level * 8).ToArray());
                var goalEnterTime = Stopwatch.GetTimestamp();

                goalStack.Push(this);
                int origGoalStackSize = goalStack.Count;

                var rules = ruleCollection.FindByLHS(Symbol);
                foreach (Rule rule in rules)
                {
                    var ruleRange = new RuleRange(rule, InputPos, Length);
                    var newRuleNode = ruleNode.AddNewChild(rule);
                    ruleRangeStack.Push(ruleRange);
                    int origRuleRangeStackSize = ruleRangeStack.Count;
                    int origRuleInputMapStackSize = ruleInputMapStack.Count;

#if DEBUG_OUT
                    textBox1.AppendText(Environment.NewLine);
                    textBox1.AppendText(levelString + "-----------------------------------------------------");
                    textBox1.AppendText(Environment.NewLine);
                    textBox1.AppendText(levelString + rule);
                    textBox1.AppendText(Environment.NewLine);
                    textBox1.AppendText(levelString + "-----------------------------------------------------");
                    textBox1.AppendText(Environment.NewLine);
#endif

                    var ruleInputMapper = new RuleInputMapper(rule, InputPos, Length);
                    foreach (List<int> ruleInputMap in ruleInputMapper.GetInputMap())
                    {
                        ruleInputMapStack.Push(new RuleInputMap(ruleInputMap, InputPos));

                        // do a little preprocessing to see if we can quit this ruleInputMap earlier
                        int symbolCount = 0;
                        int currentPos = InputPos;
                        foreach (int u in ruleInputMap)
                        {
                            if (rule.RHS[symbolCount].IsTerminal)
                            {
#if USE_TOKENTYPE_IN_SYMBOL
                                if (u != 1 || rule.RHS[symbolCount].TheTokenType != input[currentPos].TheTokenType)
#else
                                if (u != 1 || rule.RHS[symbolCount].TheSymbol != input[currentPos].)
#endif
                                {
                                    goto NextRuleInputMap;                                                                    
                                }
                            }

                            if (!rule.RHS[symbolCount].IsTerminal && u < 1)
                            {
                                goto NextRuleInputMap;
                            }

/*
                            if (!rule.RHS[symbolCount].IsTerminal || u != 1)
                            {
                                goto NextRuleInputMap;
                            }

                            if (rule.RHS[symbolCount].TheTokenType == TokenType.None)   // this is a specific string in the rule
                            {
                                if (input[currentPos].TheTokenType != TokenType.String || input[currentPos].Lexeme != rule.RHS[symbolCount].TheSymbol)
                                {
                                    goto NextRuleInputMap;
                                }
                            }
                            else if (rule.RHS[symbolCount].TheTokenType != input[currentPos].TheTokenType)
                            {
                                goto NextRuleInputMap;
                            }
*/
                            currentPos += u;
                            symbolCount++;
                        }

                        symbolCount = 0;
                        currentPos = InputPos;
                        foreach (int u in ruleInputMap)
                        {
                            var newGoal = new Goal(rule.RHS[symbolCount], currentPos, u);

                            if (rule.RHS[symbolCount].IsTerminal)
                            {
                                string subInput = null;
                                if (u != 0)
                                {
                                    subInput = input[currentPos].ToString();
                                }

#if DEBUG_OUT
                                textBox1.AppendText(levelString
                                    + String.Format("{0}{1}-->{2} (input/symbol mapping)",
                                    (symbolCount == 0 ? "" : ", "), rule.RHS[symbolCount].TheSymbol ?? "\u0190", subInput));
#endif

                                if (rule.RHS[symbolCount].TheTokenType == TokenType.None &&
                                    input[currentPos].TheTokenType == TokenType.String && input[currentPos].Lexeme == rule.RHS[symbolCount].TheSymbol)
                                {
#if DEBUG_OUT
                                    textBox1.AppendText(levelString + "TERMINAL MATCH ON TEXT!!" + Environment.NewLine);                                    
#endif                                    

                                    var newNodeData = new ParseTreeNodeData(newGoal, ruleRange);
                                    newNodeData.Lexemes.Add(input[currentPos].Lexeme);
                                    newNodeData.IsAMatch = true;
                                    var newNode = new TFTreeNode<ParseTreeNodeData>(newNodeData);
                                    parentParseTreeNode.AddChild(newNode);
                                }
                                else if (rule.RHS[symbolCount].TheTokenType != TokenType.None &&
                                    rule.RHS[symbolCount].TheTokenType == input[currentPos].TheTokenType)
                                {
#if DEBUG_OUT
                                    textBox1.AppendText(levelString + "TERMINAL MATCH ON TOKEN!!" + Environment.NewLine);
#endif
                                    parentParseTreeNode.Data.Lexemes.Add(input[currentPos].Lexeme);

                                    var newNodeData = new ParseTreeNodeData(newGoal, ruleRange);
                                    newNodeData.Lexemes.Add(input[currentPos].Lexeme);
                                    newNodeData.IsAMatch = true;
                                    var newNode = new TFTreeNode<ParseTreeNodeData>(newNodeData);
                                    parentParseTreeNode.AddChild(newNode);
                                }
                                else
                                {
#if DEBUG_OUT
                                    textBox1.AppendText(levelString + "TERMINAL NO MATCH!!" + Environment.NewLine);
#endif
#if INCLUDE_UNSUCCESSFUL_NODES_IN_PARSE_TREE
                                    var newNodeData = new ParseTreeNodeData(newGoal, ruleRange);
                                    newNodeData.Lexemes.Add(input[currentPos].Lexeme);
                                    var newNode = new TFTreeNode<ParseTreeNodeData>(newNodeData);
                                    parentParseTreeNode.AddChild(newNode);
#endif
                                    goto NextRuleInputMap;
                                }
                            }
                            else
                            {
                                try
                                {
                                    var newNodeData = new ParseTreeNodeData(newGoal, ruleRange);
                                    var newNode = new TFTreeNode<ParseTreeNodeData>(newNodeData);
                                    parentParseTreeNode.AddChild(newNode);
                                    if (!newGoal.NewProcess(input, level + 1, goalStack, ruleRangeStack, ruleInputMapStack, newNode, newRuleNode, debugOut, ruleCollection))
                                    {
#if INCLUDE_UNSUCCESSFUL_NODES_IN_PARSE_TREE
#else
                                        parentParseTreeNode.RemoveChild(newNode);
#endif
                                        // if we fail at any part of a rule input map we have failed the whole thing))
                                        goto NextRuleInputMap;
                                    }
                                    foreach (string lexeme in newNode.Data.Lexemes)
                                    {
                                        parentParseTreeNode.Data.Lexemes.Add(lexeme);
                                    }
                                }

                                catch (Exception)
                                {
                                    ;
                                }
                            }

                            currentPos += u;
                            symbolCount++;
                        }

                        // if we've arrived here we have a recognized ruleInputMap for this range of input for our grammar
                        parentParseTreeNode.Data.TimeInNode = (long)Math.Truncate(((decimal)Stopwatch.GetTimestamp() - (decimal)goalEnterTime) * 1000000.0m / Stopwatch.Frequency);
                        parentParseTreeNode.Data.IsAMatch = true;
                        return true;

NextRuleInputMap:
                        ruleInputMapStack.SetNewSize(origRuleInputMapStackSize);
                        ruleRangeStack.SetNewSize(origRuleRangeStackSize);
                        // Since we could have generated goals on the goal stack that were successful but
                        // not all of the goals for that rule input map were successful we need to pop off those
                        // successful goals.
                        goalStack.SetNewSize(origGoalStackSize);
                    }
                    ruleRangeStack.Pop();
                }
#if DEBUG_OUT
                textBox1.AppendText(levelString + "-----------------------------------------------------");
                textBox1.AppendText(Environment.NewLine);
#endif
                goalStack.Pop();
                //GC.Collect();
                return false;


            }
            catch (Exception ex)
            {
                //var numNodes = parentNode.Root.GetDepthFirstEnumerable(TreeTraversalDirection.TopDown).Count();
                throw;
            }

        }

#if NON_TOKEN_BASED_PARSER
        public bool NewProcess(string input, TextBox textBox1, int level,  
            Stack<Goal> goalStack, 
            Stack<RuleRange> ruleRangeStack, 
            Stack<RuleInputMap> ruleInputMapStack,
            SimpleTreeNode<ParseTreeNodeData> parentNode)
        {
            if (visitCount++ == 10000)
            {
                visitCount++;
            }
                
            try
            {
            string levelString = new string(Enumerable.Repeat(' ', level * 8).ToArray());
            var goalEnterTime = Stopwatch.GetTimestamp();

            goalStack.Push(this);
            int origGoalStackSize = goalStack.Count;

            var rules = RuleCollection.FindByLHS(Symbol);
            foreach (Rule rule in rules)
            {
                var ruleRange = new RuleRange(rule, this.InputPos, this.Length);
                ruleRangeStack.Push(ruleRange);
                int origRuleRangeStackSize = ruleRangeStack.Count;
                int origRuleInputMapStackSize = ruleInputMapStack.Count;

#if DEBUG_OUT
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText(levelString + "-----------------------------------------------------");
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText(levelString + rule);
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText(levelString + "-----------------------------------------------------");
                textBox1.AppendText(Environment.NewLine);
#endif

                var ruleInputMapper = new RuleInputMapper(rule, input, InputPos, Length);
                foreach (List<int> ruleInputMap in ruleInputMapper.GetInputMap())
                {
                    ruleInputMapStack.Push(new RuleInputMap(ruleInputMap, InputPos));

                    int symbolCount = 0;
                    int currentPos = InputPos;
                    foreach (int u in ruleInputMap)
                    {
                        if (!rule.RHS[symbolCount].IsTerminal || u == 0)
                        {
                            continue;
                        }

                        if (rule.RHS[symbolCount].TheSymbol != input.Substring(currentPos, u))
                        {
                            goto NextRuleInputMap;
                        }

                        currentPos += u;
                        symbolCount++;
                    }

                    symbolCount = 0;
                    currentPos = InputPos;
                    foreach (int u in ruleInputMap)
                    {
                        if (rule.RHS[symbolCount].IsTerminal)
                        {
                            string subInput = null;
                            if (u != 0)
                            {
                                subInput = input.Substring(currentPos, u);
                            }

#if DEBUG_OUT
                            textBox1.AppendText(levelString
                                + String.Format("{0}{1}-->{2} (input/symbol mapping)",
                                (symbolCount == 0 ? "" : ", "), rule.RHS[symbolCount].TheSymbol ?? "\u0190", subInput));
#endif

                            if (rule.RHS[symbolCount].TheSymbol == subInput)
                            {
#if DEBUG_OUT
                                textBox1.AppendText(levelString + "TERMINAL MATCH!!" + Environment.NewLine);
#endif
                            }
                            else
                            {
#if DEBUG_OUT
                                textBox1.AppendText(levelString + "TERMINAL NO MATCH!!" + Environment.NewLine);
#endif
                                goto NextRuleInputMap;
                            }                            
                        }
                        else
                        {
                            var newGoal = new Goal(rule.RHS[symbolCount], currentPos, u);
                            try
                            {
                                var newNodeData = new ParseTreeNodeData(newGoal, ruleRange);
                                var newNode = new SimpleTreeNode<ParseTreeNodeData>(newNodeData);
                                parentNode.Children.Add(newNode);
                                if (!newGoal.NewProcess(input, textBox1, level + 1, goalStack, ruleRangeStack, ruleInputMapStack, newNode))
                                {
                                    // if we fail at any part of a rule input map we have failed the whole thing
                                    goto NextRuleInputMap;
                                }                            
                            }
#pragma warning disable 168
                            catch (Exception ex)
#pragma warning restore 168
                            {
                                ;
                            }
                        }

                        currentPos += u;
                        symbolCount++;
                    }

                    // if we've arrived here we have a recognized ruleInputMap for this range of input for our grammar
                    parentNode.Value.TimeInNode = (long) Math.Truncate(((decimal)Stopwatch.GetTimestamp() - (decimal)goalEnterTime)*1000000.0m/Stopwatch.Frequency);
                    return true;

                NextRuleInputMap:
                    ruleInputMapStack.SetNewSize(origRuleInputMapStackSize);
                    ruleRangeStack.SetNewSize(origRuleRangeStackSize);
                    // Since we could have generated goals on the goal stack that were successful but
                    // not all of the goals for that rule input map were successful we need to pop off those
                    // successful goals.
                    goalStack.SetNewSize(origGoalStackSize);
                }
                ruleRangeStack.Pop();
            }
#if DEBUG_OUT
            textBox1.AppendText(levelString + "-----------------------------------------------------");
            textBox1.AppendText(Environment.NewLine);
#endif
            goalStack.Pop();
            //GC.Collect();
            return false;


            }
            catch (Exception ex)
            {
                var numNodes = parentNode.Root.GetDepthFirstEnumerable(TreeTraversalDirection.TopDown).Count();
                throw;
            }

        }
#endif

/*
        public bool Process(string input, TextBox textBox1, int level, Stack<RuleRange> ruleStack)
        {
            string levelString = new string(Enumerable.Repeat(' ', level * 8).ToArray());

            textBox1.AppendText(Environment.NewLine + levelString + this.ToString(input) + Environment.NewLine);

            var rules = RuleCollection.FindByLHS(Symbol);
            foreach (Rule rule in rules)
            {
                var symbolInputMapping = new RuleInputMapper(rule, input, InputPos, Length);
                int origRuleStackSize = ruleStack.Count;
                ruleStack.Push(new RuleRange(rule, InputPos, Length));
                int beforeInputMapStackSize = ruleStack.Count;

                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText(levelString + "-----------------------------------------------------");
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText(levelString + rule.ToString());
                textBox1.AppendText(Environment.NewLine);
                textBox1.AppendText(levelString + "-----------------------------------------------------");
                textBox1.AppendText(Environment.NewLine);

                foreach (List<int> list in symbolInputMapping.GetInputMap())
                {
                    ruleStack.SetNewSize(beforeInputMapStackSize);

                    int symbolCount = 0;
                    int currentPos = InputPos;
                    foreach (int u in list)
                    {
                        string subInput;
                        if (u == 0)
                        {
                            subInput = "\u0190";
                        }
                        else
                        {
                            subInput = input.Substring(currentPos, u);
                        }
                        textBox1.AppendText(levelString 
                            + String.Format("{0}{1}-->{2} (input/symbol mapping)", 
                            (symbolCount == 0 ? "" : ", "), rule.RHS[symbolCount].TheSymbol, subInput));

                        if (rule.RHS[symbolCount].IsTerminal)
                        {
                            if (rule.RHS[symbolCount].TheSymbol == subInput)
                            {
                                textBox1.AppendText(levelString + "TERMINAL MATCH!!" + Environment.NewLine);
                            }
                            else
                            {
                                textBox1.AppendText(levelString + "TERMINAL NO MATCH!!" + Environment.NewLine);
                                goto NextInputMap;
                            }
                        }
                        else
                        {
                            var newGoal = new Goal(rule.RHS[symbolCount], currentPos, u);
                            if (!newGoal.Process(input, textBox1, level + 1, ruleStack))
                            {
                                goto NextInputMap;
                            }
                        }

                        symbolCount++;
                        currentPos += u;
                    }
                    textBox1.AppendText(Environment.NewLine);
                    return true;
                NextInputMap:
                    ;
                }
                ruleStack.SetNewSize(origRuleStackSize);
                textBox1.AppendText(levelString + "-----------------------------------------------------");
                textBox1.AppendText(Environment.NewLine);
            }

            return false;
        }
        */
        public string ToString(string input)
        {
            return Symbol.TheSymbol + " from " + input.Substring(InputPos, Length) + " (Goal)";
        }

        public override string ToString()
        {
            return Symbol.TheSymbol + ": " + InputPos + "-" + (InputPos + Length - 1);
        }
    }
}
