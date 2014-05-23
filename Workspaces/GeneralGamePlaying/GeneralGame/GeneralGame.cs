#define REMOVE_BASES_AS_A_WAY_OF_APPLYING_MOVES

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using API.GGP.GGPInterfacesNS;
using API.GGP.PredicateLogic;
using API.Parsing.KIFParserUngerParallel;
using API.SWIProlog.Engine;
using API.UtilitiesAndExtensions;
using API.SWIProlog.SWIPrologServiceLibrary;
using API.UtilitiesAndExtensions;

namespace API.GGP.GeneralGameNS
{
    public class GeneralGame : IGeneralGame
    {
        readonly char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public enum GameDescriptionHornClauseClassifications
        {
            Role,
            Input,
            Base,
            Init,
            Legal,
            Next,
            Goal,
            Terminal,
            _AllOther    
        }

        public string[] GameDescriptionHornClauseClassificationsNames =
            Enum.GetNames(typeof (GameDescriptionHornClauseClassifications)).Select(n => n.ToLower()).ToArray();

        public ConcurrentDictionary<GameDescriptionHornClauseClassifications, List<HornClause>> ClassifiedHornClauses;        
        
        protected State CurrentState { get; private set; }

        public int CurrentTurn { get; set; }

        // I make this static so that changing it for one GeneralGame instance changes it for all 
        static public bool UseOldWayOfGeneratingAndApplyingMoves { get; set; }

        protected List<string> Roles = new List<string>();
        // These are the functors that change from state to state.
        // They are declared dynamic.  They are the only clauses contained within the state
        protected List<Tuple<string, int>> BaseAndNextFunctors = new List<Tuple<string, int>>();

        // These are the functors of the inputs
        protected List<Tuple<string, int>> InputFunctors = new List<Tuple<string, int>>();

        public string KIFFilePath { get; private set; }
        public string XMLStyleSheetFilePath { get; private set; }
        private int _MinNumPlayers, _MaxNumPlayers;

        private bool preventAnyChangesToPrologClauses = false;   

        public PrologEngine PrologEngine;

        private Random Rand;

        private KIFParserUngerParallel KIFParser = new KIFParserUngerParallel();

        private string _TempFilePath = Path.GetTempPath();
        public string TempFilePath
        {
            get { return _TempFilePath; }
            set { _TempFilePath = value ?? Path.GetTempPath(); }
        }

        public GeneralGame(string wcfSvcHostExePath = null)
        {
            Rand = Utilities.RandomHelper.Instance;
            PrologEngine = new PrologEngine(wcfSvcHostExePath);
            KIFFilePath = string.Empty;
        }

        public int MinNumPlayers
        {
            get { return _MinNumPlayers; }
        }

        public int MaxNumPlayers
        {
            get { return _MaxNumPlayers; }
        }

        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(KIFFilePath); }
        }

        public string LongName
        {
            get { throw new NotImplementedException(); }
        }

        public string FileName
        {
            get { return KIFFilePath; }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            private set { _Description = value; }
        }

        protected bool ParseFromKIFFile(string kifFilePath, out string error)
        {
            try
            {
                KIFParser.ParseFromFilePath(kifFilePath);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            error = String.Empty;
            return true;
        }

        protected bool ParseFromGameDescriptionString(string gameDescription, out string error)
        {
            try
            {
                KIFParser.ParseFromString(gameDescription);
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            error = String.Empty;
            return true;
        }

        public bool LoadDescriptionFromGameDescriptionString(string gameDescription)
        {
            string error;

            ParseFromGameDescriptionString(gameDescription, out error);
            ConcurrentDictionary<int, HornClause> hornClauses = KIFParser.GetHornClauses();
            var modifiedHornClauses = new ConcurrentDictionary<int, HornClause>();
            foreach (KeyValuePair<int, HornClause> keyValuePair in hornClauses)
            {
                modifiedHornClauses.GetOrAdd(keyValuePair.Key, keyValuePair.Value.ReplaceAllFunctorsUsedByProlog());
            }
            hornClauses = modifiedHornClauses;

            if (!ClassifyHornClauses(hornClauses, out error))
            {
                return false;
            }

            Description = gameDescription;
 
            return true;            
        }

        public bool LoadDescriptionFromKIFFile(string filePath)
        {
            string error;

            KIFFilePath = filePath;
            XMLStyleSheetFilePath = Path.GetDirectoryName(KIFFilePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(KIFFilePath) + ".xsl";
            if (!File.Exists(XMLStyleSheetFilePath))
            {
                XMLStyleSheetFilePath = null;
            }

            ParseFromKIFFile(filePath, out error);

            ConcurrentDictionary<int, HornClause> hornClauses = KIFParser.GetHornClauses();

            var modifiedHornClauses = new ConcurrentDictionary<int, HornClause>();
            foreach (KeyValuePair<int, HornClause> keyValuePair in hornClauses)
            {
                modifiedHornClauses.GetOrAdd(keyValuePair.Key, keyValuePair.Value.ReplaceAllFunctorsUsedByProlog());
            }
            hornClauses = modifiedHornClauses;

            if (!ClassifyHornClauses(hornClauses, out error))
            {
                return false;
            }

            using (var sw = new StreamWriter(TempFilePath + "allclauses.txt", false))
            {
                foreach (KeyValuePair<int, HornClause> keyValuePair in hornClauses)
                {
                    sw.WriteLine(keyValuePair.Value.ToPrologClause());
                }                
            }

            StreamReader streamReader = new StreamReader(filePath);
            Description = streamReader.ReadToEnd();
            streamReader.Close();

            return true;
        }

        protected bool ClassifyHornClauses(ConcurrentDictionary<int, HornClause> hornClauses, out string error)
        {
            ClassifiedHornClauses = new ConcurrentDictionary<GameDescriptionHornClauseClassifications, List<HornClause>>();
            foreach (string gameDescriptionHornClauseClassificationsName in GameDescriptionHornClauseClassificationsNames)
            {
                ClassifiedHornClauses[(GameDescriptionHornClauseClassifications)Enum.Parse(typeof(GameDescriptionHornClauseClassifications), gameDescriptionHornClauseClassificationsName, true)] = new List<HornClause>();
            }

            int terminalClauseCount = 0;
            foreach (KeyValuePair<int, HornClause> keyValuePair in hornClauses)
            {
                var hornClause = keyValuePair.Value;
                if (GameDescriptionHornClauseClassificationsNames.Contains(hornClause.Head.Functor.ToString()))
                {
                    var classificationType =
                        (GameDescriptionHornClauseClassifications)
                        Enum.Parse(typeof (GameDescriptionHornClauseClassifications), hornClause.Head.Functor.ToString(), true);

                    switch (classificationType)
                    {
                        case GameDescriptionHornClauseClassifications.Init:
                            // all the init HornClauses are actually facts within an init() compound term
                            ClassifiedHornClauses[classificationType].Add(hornClause.RemoveFirstFunctor());
                            break;

                        case GameDescriptionHornClauseClassifications.Terminal:
                            var newTerminalHornClause =
                                hornClause.InsertArgumentIntoHead(new Number(terminalClauseCount.ToString()), 0);

                            ClassifiedHornClauses[classificationType].Add(newTerminalHornClause);
                            terminalClauseCount++;
                            break;

                        default:
                            ClassifiedHornClauses[classificationType].Add(hornClause);
                            break;
                    }
                }
                else
                {
                    ClassifiedHornClauses[GameDescriptionHornClauseClassifications._AllOther].Add(hornClause);
                }
            }

            error = String.Empty;
            return true;
        }

        public bool InitializeGame(out string error)
        {
            PrologEngine.Reset();

            CurrentTurn = 0;

            // add in my support clauses here
            // I don't use this anymore because the C# Prolog parser can't handle doing
            // true(...) this way.  So I remove all true relations from the Horn Clauses instead
            //string query1 = "assert(true(X) :- X).";
            //PrologEngine.Query = query1;
            //PrologEngine.GetFirstSolution(query1);

            string clause = @"distinct(X, Y):- \+ X == Y";
            PrologEngine.Assert(clause);

            // A difference list version of append
            clause = @"append_dl(A-B, B-C, A-C)";
            PrologEngine.Assert(clause);

            // might want to use these
            // next_state(State, Player, Move, NewState) :- current_state(State), setof(NewStateVariable, Move^State^next(State, [does(Player, Move)], NewStateVariable), NewState).
            // next_state_1(State, Moves, NewState) :- current_state(State), setof(NewStateVariable, Moves^State^next(State, Moves, NewStateVariable), NewState).

            clause = @"next_state(S, M, NSL) :- findall(NS, next(S, M, NS), NSL)";
            PrologEngine.Assert(clause);

            clause = @"legal_as_does(NM) :- findall(does(P, Mv), legal(P, Mv), NM)";
            PrologEngine.Assert(clause);

            clause = @"legal_as_does(P, NM) :- findall(does(P, Mv), legal(P, Mv), NM)";
            PrologEngine.Assert(clause);

            clause =
                @"legal_as_does(S,M,NM) :- findall(does(P,Mv),legal(S,M,P,Mv),NM)";
            PrologEngine.Assert(clause);

            clause =
                @"legal_as_does(S,M,P,NM) :- findall(does(P,Mv),legal(S,M,P,Mv),NM)";
            PrologEngine.Assert(clause);

            clause =
                @"(all_legals(Moves) :- role(Player), findall((Player, Move), legal(Player, Move), Moves))";
            PrologEngine.Assert(clause);

            clause =
                @"(all_legals(Player, Moves) :- role(Player), findall((Player, Move), legal(Player, Move), Moves))";
            PrologEngine.Assert(clause);

            clause =
                @"(all_legal_as_does_move_combinations(Moves) :- findall(Role, role(Role), Roles), all_legal_as_does_move_combinations(Roles, Moves))";
            PrologEngine.Assert(clause);

            clause =
                @"(all_legal_as_does_move_combinations([], []))";
            PrologEngine.Assert(clause);

            clause =
                @"(all_legal_as_does_move_combinations([Role | Roles], [does(Role, Move) | Moves]) :- legal(Role, Move), all_legal_as_does_move_combinations(Roles, Moves))";
            PrologEngine.Assert(clause);

            // now do all the rewrite clauses
            // top level
            clause = @"(rewrite_with_state_and_moves(_G103,_G104) :- 
                            _G103=..[:-,_G109,_G112],atomic(_G109),
                            !,
                            _G119 =.. [_G109, _G121, _G122],
                            rewrite_with_state_and_moves(_G112,_G125,b,_G121,_G122),
                            _G104=..[:-,_G119,_G125])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT) :- 
	                        T =.. [:-, Head, Body],
	                        !,
	                        rewrite_with_state_and_moves(Head, NewHead, h, S, M),
	                        rewrite_with_state_and_moves(Body, NewBody, b, S, M), 
	                        NT =.. [:-, NewHead, NewBody])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT) :- 
	                        T =.. [:-, Head, true],
	                        !,
	                        rewrite_with_state_and_moves(Head, NewHead, h, S, M), 
	                        NT =.. [:-, NewHead])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT) :- 
	                        !, 
	                        rewrite_with_state_and_moves(T, NT, h, S, M))";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, T) :- 
                            !)";
            PrologEngine.Assert(clause);

            // head
            clause = @"(rewrite_with_state_and_moves(T, T, _, _, _) :- 
                            var(T),!)";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, T, _, _, _) :- 
                            atomic(T),!)";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves([], [], _, _, _) :- 
                            !)";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves([Arg | RestOfArgs], [NewArg | NewRestOfArgs], h, S, M) :-
	                        !,
	                        rewrite_with_state_and_moves(Arg, NewArg, h, S, M),
	                        rewrite_with_state_and_moves(RestOfArgs, NewRestOfArgs, h, S, M))";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, h, S, M) :-
	                        T =.. ['legal' | Args],
	                        !,
	                        NT =.. ['legal', S, M | Args])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, h, S, M) :-
	                        T =.. [PredName | Args],
	                        member(PredName, ['not', 'distinct', ';']),
	                        !,
	                        rewrite_with_state_and_moves(Args, NewArgs, h, S, M),
	                        NT =.. [PredName | NewArgs])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, h, S, M) :-
	                        T =.. [PredName | Args],
	                        baseAndNextPredicates(_G118),member(PredName, _G118),
	                        !,
	                        rewrite_with_state_and_moves(Args, NewArgs, h, S, M),
	                        NT =.. [PredName | NewArgs])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, h, S, M) :-
	                        T =.. [PredName | Args],
	                        !,
	                        rewrite_with_state_and_moves(Args, NewArgs, h, S, M),
	                        NT =.. [PredName, S, M | NewArgs])";
            PrologEngine.Assert(clause);

            // body
            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :- 
	                        T =.. [,, _G115, _G118],
	                        !,
	                        rewrite_with_state_and_moves(_G115, _G125, b, S, M),
	                        rewrite_with_state_and_moves(_G118, _G131, b, S, M),
	                        NT =.. [,, _G125, _G131])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves([Arg | RestOfArgs], [NewArg | NewRestOfArgs], b, S, M) :-
	                        !,
	                        rewrite_with_state_and_moves(Arg, NewArg, b, S, M),
	                        rewrite_with_state_and_moves(RestOfArgs, NewRestOfArgs, b, S, M))";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :-
	                        T =.. ['does' | Args],
	                        !,
	                        NT =.. [member, T, M])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :-
	                        T =.. [PredName | Args],
	                        baseAndNextPredicates(_G118),member(PredName,_G118),
	                        !,
	                        NT =.. [member, T, S])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :-
	                        T =.. [PredName | Args],
	                        member(PredName, ['not', 'distinct', ';']),
	                        !,
	                        rewrite_with_state_and_moves(Args, NewArgs, b, S, M),
	                        NT =.. [PredName | NewArgs])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :-
	                        T =.. [PredName | Args],
	                        !,
	                        rewrite_with_state_and_moves(Args, NewArgs, b, S, M),
	                        NT =.. [PredName, S, M | NewArgs])";
            PrologEngine.Assert(clause);


#if OLD_REWRITE_CLAUSES
            // now do all the rewrite clauses
            clause =
                @"(rewrite_with_state_and_moves(T, NT) :- T =.. [:-, Head, Body], !, rewrite_with_state_and_moves(Head, NewHead, h, S, M), rewrite_with_state_and_moves(Body, NewBody, b, S, M), NT =.. [:-, NewHead, NewBody])";
            PrologEngine.Assert(clause);

            clause =
                "(rewrite_with_state_and_moves(T, NT) :- T =.. [:-, Head, true], !, rewrite_with_state_and_moves(Head, NewHead, h, S, M), NT =.. [:-, NewHead])";
            PrologEngine.Assert(clause);

            clause = "(rewrite_with_state_and_moves(T, NewHead) :- !, rewrite_with_state_and_moves(T, NewHead, h, S, M))";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, T) :- !)";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, T, _, _, _) :- var(T),!)";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, T, _, _, _) :- atomic(T),!)";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves([], [], _, _, _) :- !)";
            PrologEngine.Assert(clause);

            // transformation rules for the body of rules
            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :- T =.. [',', Arg, RestOfArgs], !, rewrite_with_state_and_moves(Arg, NewArg, b, S, M), rewrite_with_state_and_moves(RestOfArgs, NewRestOfArgs, b, S, M), NT =.. [',', NewArg, NewRestOfArgs])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, _) :- T =.. [PredName | Body], baseAndNextPredicates(StatePreds), member(PredName, StatePreds), !, NT2 =.. [PredName | Body], NT =.. ['member', NT2, S])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, b, _, M) :- T =.. ['does' | Body], !, NT2 =.. ['does' | Body], NT =.. ['member', NT2, M])";
            PrologEngine.Assert(clause);

            //ToDo: Find a better way to deal with built in predicates
            //clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :- T =.. ['distinct' , Args], !, rewrite_with_state_and_moves(Args,ArgsNT,b,S,M), NT =.. ['distinct' , ArgsNT])";
            //PrologEngine.Assert(clause);

            //clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :- T =.. ['not' , Args], !, rewrite_with_state_and_moves(Args,ArgsNT,b,S,M), NT =.. ['not' , ArgsNT])";
            //PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :- T =.. [PredName | Args], \+predicate_property(PredName, built_in), !, NT =.. [PredName, S, M | Args])";
            PrologEngine.Assert(clause);

            clause = @"(rewrite_with_state_and_moves(T, NT, b, S, M) :- T =.. [PredName | Args], predicate_property(PredName, built_in), !, NT =.. [PredName | Args])";
            PrologEngine.Assert(clause);

            // transformation rules for the head of rules	
            clause = @"(rewrite_with_state_and_moves(T, NT, h, S, M) :- !, T =.. [PredName | Args], NT =.. [PredName, S, M | Args])";
            PrologEngine.Assert(clause);
#endif

#if LATER_IF_NEEDED
            // create a list of all the input predicates
            foreach (var hornClauseList in ClassifiedHornClauses
                .Where(n => n.Key == GameDescriptionHornClauseClassifications.Input))
            {
                foreach (var hornClause in hornClauseList.Value)
                {
                    var tmp = hornClause.RemoveFirstFunctor();
                    if (!InputFunctors.Where(n => n.Item1 == tmp.Head.Functor.TheString && n.Item2 == tmp.Head.Arity).Any())
                    {
                        //PrologEngine.DeclareDynamic(tmp.Head.Functor.TheString, tmp.Head.Arity);
                        InputFunctors.Add(new Tuple<string, int>(tmp.Head.Functor.TheString, tmp.Head.Arity));
                    }
                }
            }

            // now write out a Prolog list of all input predicate names 
            var sb = new StringBuilder();
            sb.Clear();
            sb.Append("inputPredicates([");
            // ToDo: do I need to use the arity's here?
            int functorCounter = 1;
            int numInputFunctors = InputFunctors.Count;
            foreach (Tuple<string, int> inputFunctor in InputFunctors)
            {
                sb.Append(inputFunctor.Item1);
                if (functorCounter++ != numInputFunctors)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("])");
            var tmpgoo = sb.ToString();
            PrologEngine.Assert(sb.ToString());
#endif

            // Set all the predicates declared as base to dynamic in the Prolog engine
            // so it will fail (instead of error) if that predicate is not currently in the 
            // list of user clauses            )))
            foreach (var hornClauseList in ClassifiedHornClauses
                .Where(n => n.Key == GameDescriptionHornClauseClassifications.Base
                || n.Key == GameDescriptionHornClauseClassifications.Next))
            {
                foreach (var hornClause in hornClauseList.Value)
                {
                    var tmp = hornClause.RemoveFirstFunctor();
                    if (!BaseAndNextFunctors.Where(n => n.Item1 == tmp.Head.Functor.TheString && n.Item2 == tmp.Head.Arity).Any())
                    {
                        PrologEngine.DeclareDynamic(tmp.Head.Functor.TheString, tmp.Head.Arity);
                        BaseAndNextFunctors.Add(new Tuple<string, int>(tmp.Head.Functor.TheString, tmp.Head.Arity));
                    }
                }
            }

            // now write out a Prolog list of all predicate names in a state
            var sb = new StringBuilder();
            sb.Clear();
            sb.Append("baseAndNextPredicates([");
            // ToDo: do I need to use the arity's here?
            int functorCounter = 1;
            int numBaseAndNextFunctors = BaseAndNextFunctors.Count;
            foreach (Tuple<string, int> baseAndNextFunctor in BaseAndNextFunctors)
            {
                sb.Append(baseAndNextFunctor.Item1);
                if (functorCounter++ != numBaseAndNextFunctors)
                {
                    sb.Append(", ");
                }
            }
            sb.Append("])");
            var tmpgoo = sb.ToString();
            PrologEngine.Assert(sb.ToString());

            // Now create a "current_state(S)" rule from all the Base and Next functors
            functorCounter = 1;
            sb.Clear();
            sb.AppendLine(String.Format("(current_state(S{0}) :-", numBaseAndNextFunctors));
            foreach (Tuple<string, int> baseAndNextFunctor in BaseAndNextFunctors)
            {
                sb.AppendLine(String.Format("\tfindall({0}({1}), {0}({1}), {2}List),", baseAndNextFunctor.Item1, global::API.SWIProlog.Engine.PrologEngine.AlphabeticVariableList.Substring(0, (baseAndNextFunctor.Item2*3)-1), baseAndNextFunctor.Item1.ToUpper()));
                if (functorCounter == 1)
                {
                    sb.AppendLine(String.Format("\tappend({0}List, [], S{1}),", baseAndNextFunctor.Item1.ToUpper(), functorCounter));
                }
                else if (functorCounter == numBaseAndNextFunctors)
                {
                    sb.Append(String.Format("\tappend({0}List, S{2}, S{1}))", baseAndNextFunctor.Item1.ToUpper(), functorCounter, functorCounter - 1));
                }
                else
                {
                    sb.AppendLine(String.Format("\tappend({0}List, S{2}, S{1}),", baseAndNextFunctor.Item1.ToUpper(), functorCounter, functorCounter - 1));
                }

                functorCounter++;
            }
            var tmpfoo = sb.ToString();
            PrologEngine.Assert(tmpfoo);

            // ToDo:  Find out why I only seem to need to do this for firesheep when it really should be a problem for many other games
            // set does/2 to be dynamic
            PrologEngine.DeclareDynamic("does", 2);
            
            foreach (var hornClauseList in ClassifiedHornClauses
                .Where(n => n.Key == GameDescriptionHornClauseClassifications.Init
                || n.Key == GameDescriptionHornClauseClassifications.Goal
                || n.Key == GameDescriptionHornClauseClassifications.Terminal
                || n.Key == GameDescriptionHornClauseClassifications.Role
                || n.Key == GameDescriptionHornClauseClassifications.Legal
                || n.Key == GameDescriptionHornClauseClassifications.Next
                || n.Key == GameDescriptionHornClauseClassifications._AllOther))
            {
                int counter = 0;
                var type = hornClauseList.Key;
                foreach (HornClause hornClause in hornClauseList.Value)
                {
                    PrologEngine.Assert(hornClause.ToPrologClause(false));

                    if (type != GameDescriptionHornClauseClassifications.Init)
                    {
                        var hornClauseAsString = hornClause.ToPrologClause(false);
                        var query = "rewrite_with_state_and_moves((" + hornClauseAsString + "), Q)";
                        if (hornClauseAsString.Contains("doing_force"))
                        {
                            counter++;
                        }
                        foreach (List<SolutionVariable> solutionVariables in PrologEngine.GetSolutionVariables(query))
                        {
                            foreach (SolutionVariable solutionVariable in solutionVariables)
                            {
                                if (solutionVariable.Variable == "Q" && !String.IsNullOrEmpty(solutionVariable.Value) && solutionVariable.Value != hornClauseAsString)
                                {
                                    if (!solutionVariable.Value.StartsWith("@"))
                                    {
                                        PrologEngine.Assert("(" + solutionVariable.Value + ")");                                        
                                    }
                                    else
                                    {
                                        // this is sometimes how a solution comes across:
                                        // @(_G100,[_G100= (next(_G112,_G113,forced(_G100)):-member(does(_G121,force_noop(_G100)),_G113))]) :- true.
                                        // instead of (next(_G112,_G113,forced(_G100)):-member(does(_G121,force_noop(_G100)),_G113))
                                        var secondLParenIndex = solutionVariable.Value.IndexOfNth("(", 0, 2);
                                        var indexOfRightBracket = solutionVariable.Value.IndexOf("]");
                                        var tmpString = solutionVariable.Value.Substring(secondLParenIndex,
                                                                                         indexOfRightBracket - secondLParenIndex);
                                        PrologEngine.Assert(tmpString);

                                    }
                                }                                
                            }
                        }                         
                    }
                }                    
            }

            foreach (List<HornClause> roleFactList in ClassifiedHornClauses
                .Where(n => n.Key == GameDescriptionHornClauseClassifications.Role).Select(n => n.Value))
            {
                Roles.Clear();
                foreach (HornClause roleHornClause in roleFactList)
                {
                    Roles.Add(roleHornClause.Head.Arguments[0].ToString());
                }
            }

            _MinNumPlayers = _MaxNumPlayers = ClassifiedHornClauses[GameDescriptionHornClauseClassifications.Role].Count();

            /*
            engine.Query = query;
            if (query.Contains("goal"))
            {
                ;
            }
            var goo = engine.GetFirstSolution(query);
            if (!goo.Solved)
            {
                ;
            }

            engine.Query = "init(X).";
            foreach (Engine.ISolution solution in engine.SolutionIterator)
            {
                ;
            }

            //engine.Ps.CrossRefTableToSpreadsheet(@"D:\temp\foo.csv");            
            */

            error = String.Empty;
            return true;
        }


        public bool LoadDescriptionFromFileAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> FindRoles()
        {
            return Roles;
        }

        public IEnumerable<string> FindPropositions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> FindInits()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Move> FindLegals(string role, State state = null)
        {
            List<Move> legalMoves = new List<Move>();

            var legalMoveQuery = "legal("+ role + ", Y).";

            try
            {
                foreach (List<SolutionVariable> solutionVariables in PrologEngine.GetSolutionVariables(legalMoveQuery))
                {
                    // ToDo:  Do this a better way
                    var newMove = new Move(role, solutionVariables[0].Value);
                    legalMoves.Add(newMove);
                }
            }
            catch (Exception)
            {
                
                throw;
            }

            return legalMoves;
        }

        public IEnumerable<Move> FindLegalsWithEval(string role, State state = null)
        {
            List<Move> legalMovesWithEval = new List<Move>();

            var legalMoveWithEvalQuery = "legal(" + role + ", M), eval_move(" + role + ", M, V)";

            try
            {
                foreach (List<SolutionVariable> solutionVariables in PrologEngine.GetSolutionVariables(legalMoveWithEvalQuery))
                {
                    // ToDo:  Do this a better way
                    var newMove = new Move(role, solutionVariables.Where(n => n.Variable == "M").First().Value);
                    int? tag = Convert.ToInt32(solutionVariables.Where(n => n.Variable == "V").First().Value);
                    newMove.Tag = tag;
                    legalMovesWithEval.Add(newMove);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return legalMovesWithEval;
        }

        public IEnumerable<string> FindNext(Move move, State state = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> FindReward(string role, State state = null)
        {
            throw new NotImplementedException();
        }

        public bool FindTerminal(out IEnumerable<HornClause> satisfiedTerminalRules)
        {
            return FindTerminal(null, out satisfiedTerminalRules);
        }

        public bool FindTerminal(State state, out IEnumerable<HornClause> satisfiedTerminalRules)
        {
            var satisfiedTerminalRulesList = new List<HornClause>();

            foreach (var hornClauseList in ClassifiedHornClauses
                .Where(n => n.Key == GameDescriptionHornClauseClassifications.Terminal).Select(n => n.Value))
            {
                foreach (HornClause hornClause in hornClauseList)
                {
                    var terminalFunctor = hornClause.Head.Functor.ToString();
                    var query = new Predicate(new Atom(terminalFunctor), new Variable("X")).ToString();
                    var solutions = PrologEngine.GetSolutionVariables(query);
                    if (solutions.Any())
                    {
                        List<HornClause> terminalHornClauses =
                            ClassifiedHornClauses.Where(n => n.Key == GameDescriptionHornClauseClassifications.Terminal).Select(n => n.Value).First();
                        foreach (List<SolutionVariable> solutionVariables in solutions)
                        {
                            var terminalClause = terminalHornClauses.Where(
                                n => n.Head.Arguments[0].ToString() == solutionVariables[0].Value.ToString()).
                                FirstOrDefault();
                            if (terminalClause != null)
                            {
                                satisfiedTerminalRulesList.Add(terminalClause);
                            }
                        }

                        satisfiedTerminalRules = satisfiedTerminalRulesList; 
                        return true;
                    }
                }
            }

            satisfiedTerminalRules = satisfiedTerminalRulesList;
            return false;
        }

        public IEnumerable<Score> FindScores(State state = null)
        {
            List<Score> Scores = new List<Score>();

            var query = "goal(P, S).";
            var solutions = PrologEngine.GetSolutionVariables(query);
            foreach (List<SolutionVariable> solutionVariables in solutions)
            {
                var role = solutionVariables.Where(n => n.Variable == "P").Select(n => n.Value).First().ToString();
                var score = Decimal.Parse(solutionVariables.Where(n => n.Variable == "S").Select(n => n.Value).First().ToString());
                Scores.Add(new Score(role, score));
            }

            return Scores;
        }

        public State ApplyMoves(string moves, bool movesContainRoles, State state = null, string role = null)
        {
            if (state != null)
            {
                throw new NotImplementedException();                
            }

            List<Move> moveList;

            if (movesContainRoles)
            {
                throw new NotImplementedException();
            }
            else
            {
                moveList = ParseMovesWithoutRoles(moves);
                var allRoles = FindRoles();
                if (allRoles.Count() != moveList.Count)
                {
                    return CurrentState;
                }

                for (int roleCount = 0; roleCount < allRoles.Count(); roleCount++)
                {
                    moveList[roleCount].Role = allRoles.ElementAt(roleCount);
                }                    
            }

            CurrentState = ApplyMoves(moveList, state, role);

            return CurrentState;
        }

        // Here's how we should apply moves:
        //  1 - assert each move into the existing list of clauses (using does(role, move))
        //  2 - run a query of "next(X)" and save the results
        //  3 - create a new set of clauses (which should be cached for future use as a singleton) 
        //      from the following clause classifications:
        //        Goal
        //        Terminal
        //        Role
        //        Legal
        //        Next
        //        _AllOther
        //  OR remove all the base(...) clauses (Hmmmm, connectfour.kif doesn't seem
        //                                       to mark control(X) as a base clause. Ugh.
        //  4 - assert the results of the "next(X)" query
        //  5 - this is our new current game state
        public State ApplyMoves(IEnumerable<Move> moves, State state = null, string role = null)
        {
            if (preventAnyChangesToPrologClauses)
            {
                return new State("");
            }

            if (state != null)
            {
                throw new NotImplementedException();
            }

            try
            {
                var prologMoveListBuilder = new StringBuilder();
                prologMoveListBuilder.Append("[");

                // add the moves in as does(...) claues in the existing Prolog clause list)
                foreach (Move move in moves)
                {
                    var doesClause = move.ToPrologDoesClause();

                    if (UseOldWayOfGeneratingAndApplyingMoves)
                    {
                        if (!move.IsNoop())
                        {
                            PrologEngine.Assert(doesClause);                        
                        }                        
                    }

                    prologMoveListBuilder.Append(doesClause);
                    prologMoveListBuilder.Append(", ");
                }

                prologMoveListBuilder.Remove(prologMoveListBuilder.Length - 2, 2);  // remove the ", "
                prologMoveListBuilder.Append("]");
                string prologMoveList = prologMoveListBuilder.ToString();

                string currentState = string.Empty;
                string query = "current_state(S).";
                foreach (List<SolutionVariable> solutionVariables in PrologEngine.GetSolutionVariables(query))
                {
                    var currentStateSolutions = solutionVariables.Where(n => n.Variable == "S");
                    if (currentStateSolutions.Any())
                    {
                        currentState = currentStateSolutions.First().Value;
                    }
                }

/*
                string clauseDump = PrologEngine.ListAll();
                if (role != null)
                {
                    DebugAndTraceHelper.WriteTraceLine("*************************** before moves applied ", role);
                    DebugAndTraceHelper.WriteTraceLine(clauseDump, role);
                }
                else
                {
                    DebugAndTraceHelper.WriteTraceLine("*************************** before moves applied ");
                    DebugAndTraceHelper.WriteTraceLine(clauseDump);                    
                }
*/

                List<string> nextSolutions = new List<string>();
                query = String.Format("next({0}, {1}, X).", currentState, prologMoveList);                    
                foreach (List<SolutionVariable> solutionVariables in PrologEngine.GetSolutionVariables(query))
                {
                    // ToDo:  See if I can add this functionality later
                    //if (solution.ToString().Contains("error"))
                    //{
                    //    continue;
                    //}

                    // ToDo:  Find a better way of doing this
                    nextSolutions.Add(solutionVariables[0].Value);

#if SHOW_NEXT_CLAUSES
                    if (role != null)
                    {
                        DebugAndTraceHelper.WriteDebugLine(solutionVariables[0].Value, role);
                    }
                    else
                    {
                        DebugAndTraceHelper.WriteDebugLine(solutionVariables[0].Value);
                    }
#endif 
                }
#warning Remove this after debugging new state-less code is debugged!
                var nextSolutionsDistinct = nextSolutions.Distinct().ToList();

                List<string> nextOldSolutions = new List<string>();
                query = String.Format("next(X).");
                foreach (List<SolutionVariable> solutionVariables in PrologEngine.GetSolutionVariables(query))
                {
                    // ToDo:  See if I can add this functionality later
                    //if (solution.ToString().Contains("error"))
                    //{
                    //    continue;
                    //}

                    // ToDo:  Find a better way of doing this
                    nextOldSolutions.Add(solutionVariables[0].Value);

#if SHOW_NEXT_CLAUSES
                    if (role != null)
                    {
                        DebugAndTraceHelper.WriteDebugLine(solutionVariables[0].Value, role);
                    }
                    else
                    {
                        DebugAndTraceHelper.WriteDebugLine(solutionVariables[0].Value);
                    }
#endif
                }
#warning Remove this after debugging new state-less code is debugged!
                var nextOldSolutionsDistinct = nextOldSolutions.Distinct().ToList();

                var oldway = string.Join(", ", nextOldSolutionsDistinct);
                var newway = string.Join(", ", nextSolutionsDistinct);

                if (oldway != newway)
                {
                    ;
                    oldway = newway;
                }

                // This protects me from poorly written game description files that generate
                // the same move more than once from a state
                IEnumerable<string> dns;
                if (UseOldWayOfGeneratingAndApplyingMoves)
                {
                    dns = nextOldSolutionsDistinct;
                }
                else
                {
                    dns = nextSolutionsDistinct;
                }

#if REMOVE_BASES_AS_A_WAY_OF_APPLYING_MOVES
                if (UseOldWayOfGeneratingAndApplyingMoves)
                {
                    // now remove the moves in as does(...) claues in the existing Prolog clause list
                    // ToDo:  make a better way of detecting/marking that a move is a noop!
                    foreach (Move move in moves.Where(n => !n.IsNoop()))
                    {
                        PrologEngine.Retract(move.ToPrologDoesClause().ToString());
                    }                    
                }

                // Retract all the predicates referenced by the next and base
                // type of clauses.  These are the state variables of the game
                // and must be removed before the new state variables are inserted
                // (the clauses that came from the "next" query above)
                StringBuilder sb = new StringBuilder();
                foreach (var hornClauseList in ClassifiedHornClauses
                    .Where(n =>
                    n.Key == GameDescriptionHornClauseClassifications.Base
                    || n.Key == GameDescriptionHornClauseClassifications.Next
                    ).Select(n => n.Value))
                {
                    foreach (HornClause hornClause in hornClauseList)
                    {
                        var firstArgument = hornClause.GetHeadsFirstArgument();
                        var functorToRemove = hornClause.GetHeadsFirstArgument().Head.Functor;

                        sb.Clear();
                        sb.Append(functorToRemove + "(");
                        
                        for (int x = 0; x < firstArgument.Head.Arity; x++)
                        {
                            if (x < firstArgument.Head.Arity - 1)
                            {
                                sb.Append(alpha[x] + ",");
                            }
                            else
                            {
                                sb.Append(alpha[x]);                                
                            }
                        }

                        sb.Append(")");

                        PrologEngine.Retract(sb.ToString(), true);
                    }
                }

                foreach (string ds in dns)
                {
                    PrologEngine.Assert(ds);
                }
#else
                PrologEngine.Reset();

                string query1 = @"assert(distinct(X, Y):- \+ X == Y).";
                PrologEngine.ExecuteClause(query1);

                foreach (var hornClauseList in ClassifiedHornClauses
                    .Where(n => 
                    n.Key == GameDescriptionHornClauseClassifications.Goal
                    || n.Key == GameDescriptionHornClauseClassifications.Terminal
                    || n.Key == GameDescriptionHornClauseClassifications.Role
                    || n.Key == GameDescriptionHornClauseClassifications.Legal
                    || n.Key == GameDescriptionHornClauseClassifications.Next
                    || n.Key == GameDescriptionHornClauseClassifications._AllOther).Select(n => n.Value))
                {
                    foreach (HornClause hornClause in hornClauseList)
                    {
                        // ToDo: use engine.Ps.Assert(); instead of creating an assert query  ??      
                        // all the init HornClauses are actually facts within an init() compound term
                        string queryx = hornClause.ToPrologAssertQuery();
                        PrologEngine.ExecuteClause(queryx);
                    }
                }

                if (role != null)
                {
                    DebugAndTraceHelper.WriteDebugLine("num distinct next solutions " + nextSolutions.Distinct().Count(), role);
                }
                else
                {
                    DebugAndTraceHelper.WriteDebugLine("num distinct next solutions " + nextSolutions.Distinct().Count());
                }

                foreach (string nextSolution in nextSolutions.Distinct())
                {
                    string queryn = "assert(" + nextSolution + ").";

                    if (role != null)
                    {
                        DebugAndTraceHelper.WriteDebugLine(queryn, role);                        
                    }
                    else
                    {
                        DebugAndTraceHelper.WriteDebugLine(queryn);
                    }
                    PrologEngine.ExecuteClause(queryn);
                }
#endif

            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                CurrentTurn++;
            }

            string outputString;
            //PrologEngine.Ps.ListAll(null, -1, false, true, out outputString);
            return new State("");
        }

        // moves look like:
        //      ((mark 2 2) noop)
        //      (noop (mark 2 2) noop)
        //      ((action1 1 2) (action2 2 4))
        //      nil
        //      ((action1 1 2) noop (action2 2 4))
        //      (() (noop))
        public List<Move> ParseMovesWithoutRoles(string movesString)
        {
            var cleanedUpMovesString = movesString.Trim();
            var moveList = new List<Move>();

            if ((cleanedUpMovesString != PlayerManagerProtocolConstants.NilReplyString)
                && (cleanedUpMovesString.StartsWith("(") && cleanedUpMovesString.EndsWith(")")) 
                && cleanedUpMovesString.HasParenParity())
            {
                var tmpString = cleanedUpMovesString.Substring(1, cleanedUpMovesString.Length-2);

                int parenNestLevel = 0;
                int moveStartPos = 0;
                for (int pos = 0; pos < tmpString.Length; pos++)
                {
                    char c = tmpString[pos];

                    if (c == '(')
                    {
                        parenNestLevel++;

                        if (parenNestLevel == 1)
                        {
                            if (pos > moveStartPos)
                            {
                                var newMoveString = tmpString.Substring(moveStartPos, pos - moveStartPos).Trim();
                                if (newMoveString.Any())
                                {
                                    var newMove = new Move(null, newMoveString);
                                    moveList.Add(newMove);
                                }
                            }

                            moveStartPos = pos;
                        }
                    }
                    else if (c == ')')
                    {
                        parenNestLevel--;

                        if (parenNestLevel == 0)
                        {
                            if (pos - moveStartPos > 1)
                            {
                                var newMoveString = tmpString.Substring(moveStartPos + 1, pos - moveStartPos - 1).Trim();
                                if (newMoveString.Any())
                                {
                                    var newMove = new Move(null, newMoveString);
                                    moveList.Add(newMove);
                                    moveStartPos = pos + 1;                                    
                                }
                            }
                            else if (pos - moveStartPos == 1)   // this is just "()"
                            {
                                moveStartPos = pos + 1;
                            }
                        }
                    }
                    else if (c == ' ')
                    {
                        if (parenNestLevel == 0)
                        {
                            if (pos > moveStartPos)
                            {
                                var newMoveString = tmpString.Substring(moveStartPos, pos - moveStartPos).Trim();
                                if (newMoveString.Any())
                                {
                                    var newMove = new Move(null, newMoveString);
                                    moveList.Add(newMove);
                                    moveStartPos = pos;                                    
                                }
                            }                            
                        }
                    }
                    else
                    {
                        ;
                    }
                }
            }

            return moveList;
        }

        public Move GetTerminalMove()
        {
            throw new NotImplementedException();
        }

        public Move FindRandomLegal(string role, State state = null)
        {
            if (state != null)
            {
                throw new NotImplementedException();
            }

            var allLegals = FindLegals(role, state);
            if (!allLegals.Any())
            {
                // ToDo: Do I return a nil move here?
                return null;
            }

            var randVal = Rand.Next(allLegals.Count());

/*
            if (role == "white")
            {
                return new Move(role, "mark(1,1)");
            }
            else
            {
                return new Move(role, "noop");
            }
 */ 
            return allLegals.ElementAt(randVal);
        }

        public void SaveParseTrees(string filePath, string baseName, FileMode mode)
        {
            KIFParser.SaveParseTrees(filePath, baseName, mode);
        }

        public void SaveCollapsedParseTrees(string filePath, string baseName, FileMode mode)
        {
            KIFParser.SaveCollapsedParseTrees(filePath, baseName, mode);
        }

        public void SaveHornClauses(string filePath, string baseName, FileMode mode)
        {
            KIFParser.SaveHornClauses(filePath, baseName, mode);
        }

        public string GetStateAsCompleteXML()
        {
            var basePredicatesAndArities = new List<Tuple<string, int>>();

            foreach (var hornClauseList in ClassifiedHornClauses
                .Where(n => n.Key == GameDescriptionHornClauseClassifications.Base
                || n.Key == GameDescriptionHornClauseClassifications.Next))
            {
                foreach (var hornClause in hornClauseList.Value)
                {
                    var tmp = hornClause.RemoveFirstFunctor();
                    var newPredicateAndArity = new Tuple<string, int>(tmp.Head.Functor.ToString(), tmp.Head.Arity);
                    if (!basePredicatesAndArities.Contains(newPredicateAndArity))
                    {
                        basePredicatesAndArities.Add(newPredicateAndArity);                        
                    }
                }
            }

            return PrologEngine.ListAllFactsAsCompleteXML(basePredicatesAndArities, XMLStyleSheetFilePath);
        }

        public void Dispose()
        {
            if (PrologEngine != null)
            {
                PrologEngine.Dispose();
            }
        }

        public void ConsultPrologFiles(IEnumerable<string> fileNames)
        {
            foreach (string fileName in fileNames)
            {
                PrologEngine.Consult(fileName);
            }
        }
    }
}
