using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace API.GGP.PredicateLogic
{
    // a predicate is a statement that returns true or false
    //      when a predicate has an arity of one it can be called a property
    //      when a predicate has an arity greater than one it can be called a relation
    // a term is a constant, a variable or a compound term
    // a constant is an atom or number
    // a compound term is a predicate p and terms ti as in p(t1, t2, ..., ti)
    // a literal can be a positive literal or negative literal (e.g. A, ¬B)
    // a positive literal is (atomic formula)
    // a negative literal is not (atomic formula)
    // for predicate logic an (atomic formula) is a compound term
    // a horn clause is a clause with at most one positive literal in the
    //      form (¬A1 ∨· · ·∨¬Am ∨ B) or (A1 ∧· · ·∧ Am) ⇒ B 
    //      the positive literal is called the head
    //      a horn clause exactly one single positive literal is a fact

    [Serializable]
    abstract public class Term
    {
        public abstract Term DeepCopy();
        public abstract string ToPrologString();
    }

    [Serializable]
    abstract public class Constant : Term
    {        
    }

    [Serializable]
    abstract public class CompoundTerm : Term
    {
    }

    [Serializable]
    public class Variable : Term
    {
        public string TheString { get; private set; }

        public Variable(string theString)
        {
            TheString = theString;
        }

        public override Term DeepCopy()
        {
            return new Variable(String.Copy(TheString));
        }

        public override string ToPrologString()
        {
            return TheString;
        }

        public override string ToString()
        {
            return TheString;
        }
    }

    [Serializable]
    public class Atom : Constant
    {
        public static readonly Atom[,] PrologPredicatesAndTheirReplacements = new Atom[,]
        {
            {new Atom("succ"), new Atom("succ_x")},
            {new Atom("true"), new Atom("")}
        };

        public string TheString { get; private set; }
        public Atom(string theString)
        {
            TheString = theString;
        }

        // this does a value comparison
        public static bool operator ==(Atom lhs, Atom rhs)
        {
            return (lhs.TheString == rhs.TheString);
        }

        // this does a value comparison
        public static bool operator !=(Atom lhs, Atom rhs)
        {
            return !(lhs == rhs);
        }

        public override Term DeepCopy()
        {
            return new Atom(String.Copy(TheString));
        }

        public override string ToPrologString()
        {
            return TheString;
        }

        public override string ToString()
        {
            return TheString;
        }
    }

    [Serializable]
    public class Number : Constant
    {
        public string TheString { get; private set; }
        public Number(string theString)
        {
            TheString = theString;
        }

        public override Term DeepCopy()
        {
            return new Number(String.Copy(TheString));
        }

        public override string ToPrologString()
        {
            return TheString;
        }

        public override string ToString()
        {
            return TheString;
        }
    }

    [Serializable]
    public class Predicate : CompoundTerm
    {
        public Atom Functor { get; private set; }
        public int Arity
        {
            get { return Arguments.Count(); }
        }
        public Term[] Arguments { get; private set; }

        public Predicate(Atom functor, IEnumerable<Term> arguments = null)
        {
            Functor = functor;
            Arguments = arguments == null ? new Term[0] : arguments.ToArray();
        }

        public Predicate(Atom functor, Term argument)
        {
            Functor = functor;
            Arguments = argument == null ? new Term[0] : new Term[] { argument };
        }

        protected Predicate()
        {            
        }

        public override Term DeepCopy()
        {
            var newArguments = new Term[Arguments.Count()];
            for (int x = 0; x < Arguments.Count(); x++)
            {
                newArguments[x] = Arguments[x].DeepCopy();
            }

            return new Predicate(Functor.DeepCopy() as Atom, newArguments);
        }

        public override string ToPrologString()
        {
            try
            {
                if (Arguments.Any())
                {
                    if (Functor.TheString != "or")
                    {
                        return Functor + "(" + Arguments.Select(n => n.ToPrologString()).Aggregate((workingSentence, next) => workingSentence + ", " + next) + ")";
                    }
                    else
                    {
                        return "(" + Arguments.Select(n => n.ToPrologString()).Aggregate((workingSentence, next) => workingSentence + "; " + next) + ")";
                    }
                }
                else
                {
                    return Functor.ToString();  // prolog doesn't like predicates with zero arity with parentheses
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override string ToString()
        {
            try
            {
                if (Arguments.Any())
                {
                    return Functor + "(" + Arguments.Select(n => n.ToString()).Aggregate((workingSentence, next) => workingSentence + ", " + next) + ")";                        
                }
                else
                {
                    return Functor.ToString();  // prolog doesn't like predicates with zero arity with parentheses
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public Predicate ReplaceFunctor(Atom functorToBeReplaced, Atom functorToReplaceWith)
        {
            Atom newFunctor;

            if (Functor == functorToBeReplaced)
            {
                newFunctor = new Atom(functorToReplaceWith.TheString);
            }
            else
            {
                newFunctor = Functor.DeepCopy() as Atom;
            }

            var newArguments = new Term[Arguments.Count()];
            for (int i = 0; i < Arguments.Count(); i++)
            {
                var pred = Arguments[i] as Predicate;
                if (pred != null)
                {
                    newArguments[i] = pred.ReplaceFunctor(functorToBeReplaced,
                                                                                    functorToReplaceWith);
                }
                else
                {
                    newArguments[i] = Arguments[i].DeepCopy();
                }
            }
            
            return new Predicate(newFunctor, newArguments);
        }

        public Predicate RemoveFunctor(Atom functorToBeRemoved)
        {
            Atom newFunctor;
            Term[] newArguments;

            if (Functor == functorToBeRemoved)
            {
                var firstArgPred = Arguments[0] as Predicate;
                if (firstArgPred != null)
                {
                    newFunctor = new Atom(firstArgPred.Functor.TheString);
                    newArguments = new Term[firstArgPred.Arguments.Count()];
                    for (int i = 0; i < firstArgPred.Arguments.Count(); i++)
                    {
                        var argPred = firstArgPred.Arguments[i] as Predicate;
                        if (argPred != null)
                        {
                            newArguments[i] = argPred.RemoveFunctor(functorToBeRemoved);
                        }
                        else
                        {
                            newArguments[i] = firstArgPred.Arguments[i].DeepCopy();
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException();    
                }
            }
            else
            {
                newFunctor = Functor.DeepCopy() as Atom;
                newArguments = new Term[Arguments.Count()];
                for (int i = 0; i < Arguments.Count(); i++)
                {
                    var pred = Arguments[i] as Predicate;
                    if (pred != null)
                    {
                        newArguments[i] = pred.RemoveFunctor(functorToBeRemoved);
                    }
                    else
                    {
                        newArguments[i] = Arguments[i].DeepCopy();
                    }
                }
            }

            return new Predicate(newFunctor, newArguments);
        }
    }

    // ToDo:  right now the ToString() returns a Prolog formatted version of the HornClause.  I might want it to return other versions someday
    [Serializable]
    public class HornClause
    {
        public Predicate Head { get; private set; }
        public Predicate[] Body { get; private set; }

        public HornClause(Predicate head, IEnumerable<Predicate> body = null)
        {
            Head = head;
            Body = body == null ? new Predicate[0] : body.ToArray();
        }

        public HornClause DeepCopy()
        {
            var newBody = new Predicate[Body.Count()];
            for (int x = 0; x < Body.Count(); x++)
            {
                newBody[x] = Body[x].DeepCopy() as Predicate;
            }

            return new HornClause(Head.DeepCopy() as Predicate, newBody);
        }

        // The general game description language defines a relation true(X)
        // I want to remove this before putting the horn clause into Prolog
        // ToDo: make this general purpose and make it go all the way down to the nodes
        // of the HornClauses tree!
        public HornClause RemoveTrueRelation()
        {
            var newHornClause = this.DeepCopy();

            for (int i = 0; i < newHornClause.Body.Count(); i++)
            {
                if (newHornClause.Body[i].Functor.ToString() == "true")
                {
                    if (newHornClause.Body[i].Arguments.Count() == 1 && newHornClause.Body[i].Arguments[0] is Predicate)
                    {
                        newHornClause.Body[i] = newHornClause.Body[i].Arguments[0] as Predicate;
                    }
                    else
                    {
                        // ToDo: Throw an error here!
                        ;
                    }
                }
            }

            return newHornClause;
        }

        // this is usually used to remove relation constants (like init)
        // from a horn clause
        public HornClause RemoveFirstFunctor()
        {
            var newHornClause = this.DeepCopy();

            //if (newHornClause.IsAFact())
            {
                var headPredicate = newHornClause.Head.Arguments[0] as Predicate;
                if (headPredicate != null)
                {
                    var newHead = new Predicate(new Atom(headPredicate.Functor.ToString()), headPredicate.Arguments);
                    newHornClause.Head = newHead;
                }
                else
                {
                    var newHead = new Predicate(new Atom(Head.Arguments[0].ToString()));
                    newHornClause.Head = newHead;
                }
            }

            return newHornClause;

        }

        public HornClause GetHeadsFirstArgument()
        {
            var newHornClause = this.DeepCopy();

            var headPredicate = newHornClause.Head.Arguments[0] as Predicate;
            if (headPredicate != null)
            {
                var newHead = new Predicate(new Atom(headPredicate.Functor.ToString()), headPredicate.Arguments);
                newHornClause.Head = newHead;
            }
            else
            {
                var newHead = new Predicate(new Atom(Head.Arguments[0].ToString()));
                newHornClause.Head = newHead;
            }

            return newHornClause;
        }

        public HornClause InsertArgumentIntoHead(Term term, int argumentIndex)
        {
            var newHornClause = this.DeepCopy();

            var newArguments = new Term[1 + newHornClause.Head.Arity];
            int origArgumentIndex = 0;
            for (int x = 0; x < 1 + newHornClause.Head.Arity; x++)
            {
                if (x != argumentIndex)
                {
                    newArguments[x] = newHornClause.Head.Arguments[origArgumentIndex++];
                }
                else
                {
                    newArguments[x] = term;
                }
            }

            newHornClause.Head = new Predicate(newHornClause.Head.Functor, newArguments);

            return newHornClause;
        }

        // this works recursively
        public HornClause ReplaceFunctor(Atom functorToBeReplaced, Atom functorToReplaceWith)
        {
            var newHead = Head.ReplaceFunctor(functorToBeReplaced, functorToReplaceWith);
            var newBody = new Predicate[Body.Count()];

            for (int i = 0; i < Body.Count(); i++)
            {
                newBody[i] = Body[i].ReplaceFunctor(functorToBeReplaced, functorToReplaceWith);
            }

            return new HornClause(newHead, newBody);
        }

        // this works recursively
        public HornClause RemoveFunctor(Atom functorToBeRemoved)
        {
            var newHead = Head.RemoveFunctor(functorToBeRemoved);
            var newBody = new Predicate[Body.Count()];

            for (int i = 0; i < Body.Count(); i++)
            {
                newBody[i] = Body[i].RemoveFunctor(functorToBeRemoved);
            }

            return new HornClause(newHead, newBody);
        }

        public HornClause ReplaceAllFunctorsUsedByProlog()
        {
            var newHornClause = this;
            for (int i = 0; i < Atom.PrologPredicatesAndTheirReplacements.GetLength(0); i++)
            {
                newHornClause = newHornClause.ReplaceFunctor(Atom.PrologPredicatesAndTheirReplacements[i, 0], 
                                                             Atom.PrologPredicatesAndTheirReplacements[i, 1]);
            }

            return newHornClause;
        }

        public bool IsAFact()
        {
            return (Body.Count() == 0 && Head.Arguments.Count() == 1);
        }

        static public bool SerializeToFile(ConcurrentDictionary<int, HornClause> clauses, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            using (FileStream s = File.Create (fileName))
            {
                formatter.Serialize(s, clauses);
            }

            return true;
        }

        static public ConcurrentDictionary<int, HornClause> SerializeFromFile(string fileName)
        {
            ConcurrentDictionary<int, HornClause> clauses;
            IFormatter formatter = new BinaryFormatter();
            using (FileStream s = File.OpenRead (fileName))
            {
                clauses = (ConcurrentDictionary<int, HornClause>) formatter.Deserialize (s);
            }

            return clauses;
        }

        public string ToPrologClause(bool includeTrailingPeriod = true)
        {
            if (includeTrailingPeriod)
            {
                return ToPrologString() + ".";                
            }
            else
            {
                return ToPrologString();                
            }
        }

        public string ToPrologString()
        {
            try
            {
                if (Body.Any())
                {
                    return Head + " :- (" +
                           Body.Select(n => n.ToPrologString()).Aggregate((workingSentence, next) => workingSentence + ", " + next) + ")";    // it's a rule
                }
                else
                {
                    return Head.ToString();      // it's a fact
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public override string ToString()
        {
            try
            {
                if (Body.Any())
                {
                    return Head + " :- (" +
                           Body.Select(n => n.ToString()).Aggregate((workingSentence, next) => workingSentence + ", " + next) + ")";    // it's a rule
                }
                else
                {
                    return Head.ToString();      // it's a fact
                }
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}
