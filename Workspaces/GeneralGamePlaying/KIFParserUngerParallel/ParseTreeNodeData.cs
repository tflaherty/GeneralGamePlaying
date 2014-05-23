using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using API.GGP.PredicateLogic;
using API.Utilities.TFTree;

namespace API.Parsing.KIFParserUngerParallel
{
    public class ParseTreeNodeData : IXmlSerializable
    {
        public Goal TheGoal { get; set; }
        public RuleRange TheRuleRange { get; set; }
        public long TimeInNode { get; set; }
        public List<string> Lexemes { get; set; }
        public bool IsAMatch { get; set; }

        public ParseTreeNodeData(Goal goal, RuleRange ruleRange)
        {
            TheGoal = goal;
            TheRuleRange = ruleRange;
            IsAMatch = false;
            Lexemes = new List<string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Goal(" + TheGoal + ")");
            sb.Append(" : ");
            sb.Append(TheRuleRange == null ? String.Empty : "Rule(" + TheRuleRange + " )");
            sb.Append(" : ");
            sb.Append("Time(" + TimeInNode + " )");
            return sb.ToString();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartAttribute("TimeInNode");
            writer.WriteValue(TimeInNode.ToString("N") + " usec");
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("IsAMatch");
            writer.WriteValue(IsAMatch);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("Lexemes");
            if (Lexemes.Any())
            {
                writer.WriteValue(Lexemes.Aggregate((workingOutput, next) => workingOutput + " " + next));
            }
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("Goal");
            writer.WriteValue(TheGoal.Symbol.TheSymbol);
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("GoalRange");
            writer.WriteValue(TheGoal.InputPos + "-" + (TheGoal.InputPos + TheGoal.Length - 1));
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("Rule");
            writer.WriteValue(TheRuleRange == null ? String.Empty : TheRuleRange.TheRule.ToString());
            writer.WriteEndAttribute();

            writer.WriteStartAttribute("RuleRange");
            writer.WriteValue(TheRuleRange == null ? String.Empty : String.Format(" {0}-{1}", TheRuleRange.InputPos, TheRuleRange.InputPos + TheRuleRange.Length - 1));
            writer.WriteEndAttribute();
        }

        static public Predicate CreatePredicate(TFTreeNode<ParseTreeNodeData> node, out string error)
        {
            Predicate predicate = null;

            if (node.Children.Count < 1 || node.Children[0].Data.TheGoal.Symbol.TheSymbol == "Word")
            {
                Atom functor;
                if (node.Children.Any())
                {
                    if (node.Children[0].Data.Lexemes[0] == "true")
                    {
                        ;
                    }
                    
                    functor = new Atom(node.Children[0].Data.Lexemes[0]);                        
                }
                else
                {
                    functor = new Atom(node.Data.Lexemes[0]);
                }

                if (node.Children.Count > 1)
                {
                    predicate = new Predicate(functor, new Term[node.Children.Count - 1]);
                }
                else
                {
                    predicate = new Predicate(functor);
                }


                int count = 0;
                foreach (TFTreeNode<ParseTreeNodeData> childNode in node.Children.Skip(1))
                {
                    if (childNode.Data.TheGoal.Symbol.TheSymbol == "RelSent")
                    {
                        predicate.Arguments[count] = CreatePredicate(childNode, out error);
                        if (predicate.Arguments[count] == null)
                        {
                            return null;
                        }
                    }
                    else if (childNode.Data.TheGoal.Symbol.TheSymbol == "Variable")
                    {
                        predicate.Arguments[count] = new Variable(childNode.Data.Lexemes[0].Substring(1, 1).ToUpper() + childNode.Data.Lexemes[0].Substring(2));
                    }
                    else if (childNode.Data.TheGoal.Symbol.TheSymbol == "Word")
                    {
                        predicate.Arguments[count] = new Atom(childNode.Data.Lexemes[0]);
                    }
                    else if (childNode.Data.TheGoal.Symbol.TheSymbol == "Number")
                    {
                        predicate.Arguments[count] = new Number(childNode.Data.Lexemes[0]);
                    }
                    // ToDo: What do I do with quoted strings?

                    count++;
                }
            }
            else
            {
                error = "First child of a Predicate is not a Word";
                return null;
            }

            error = String.Empty;
            return predicate;
        }

        static public HornClause CreateHornClause(TFTreeNode<ParseTreeNodeData> node, out string error)
        {
            HornClause hornClause = null;
            Predicate head = null;
            Predicate[] body = null;

            if (node.Data.TheGoal.Symbol.TheSymbol == "LogSent")
            {
                // a LogSent will become a rule
                if (node.Children.Count < 2)
                {
                    error = "LogSent has fewer than two children";
                    return null;
                }

                if (node.Children[0].Data.TheGoal.Symbol.TheSymbol != "ImplicationToLeft")
                {
                    error = "First child of LogSent is not ImplicationToLeft";
                    return null;
                }

                if (node.Children[1].Data.TheGoal.Symbol.TheSymbol != "RelSent" && node.Children[1].Data.TheGoal.Symbol.TheSymbol != "Word")
                {
                    error = "Second child of LogSent is not RelSent or Word";
                    return null;
                }
                else
                {
                    head = CreatePredicate(node.Children[1], out error);
                    if (head == null)
                    {
                        return null;
                    }
                    if (head.Functor.ToString() == "next")
                    {
                        ;
                    }
                       
                    body = new Predicate[node.Children.Count - 2];
                    hornClause = new HornClause(head, body);
                }

                int count = 0;
                foreach (TFTreeNode<ParseTreeNodeData> childNode in node.Children.Skip(2))
                {
                    if (childNode.Data.TheGoal.Symbol.TheSymbol != "RelSent" && childNode.Data.TheGoal.Symbol.TheSymbol != "Word")
                    {
                        error = "A child of LogSent is not RelSent or Word";
                        return null;
                    }
                    else
                    {
                        hornClause.Body[count] = CreatePredicate(node.Children[count + 2], out error);
                        if (hornClause.Body[count] == null)
                        {
                            return null;
                        }
                    }

                    count++;
                }

            }
            else if (node.Data.TheGoal.Symbol.TheSymbol == "RelSent")
            {
                // a RelSent will become a fact
                if (node.Children.Count < 1)
                {
                    error = "RelSent doesn't have any children";
                    return null;
                }

                head = CreatePredicate(node, out error);
                if (head == null)
                {
                    return null;
                }

                hornClause = new HornClause(head);
            }
            else
            {
                error = "HornClause does not have either a LogSent or RelSent";
                return null;
            }

            hornClause = hornClause.RemoveFunctor(new Atom("true"));

            error = string.Empty;
            return hornClause;
        }

    }
}
