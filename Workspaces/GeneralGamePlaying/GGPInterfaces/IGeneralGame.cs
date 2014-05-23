using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.GGP.PredicateLogic;
using Proposition = System.String;
using Action = System.String;
using Init = System.String;
using Goal = System.String;

namespace API.GGP.GGPInterfacesNS
{
    [Serializable]
    public class Move
    {
        public string Role { get; set; }
        public string TheMove { get; set; }
        public Object Tag { get; set; }

        public Move(string role, string move)
        {
            Role = role;
            TheMove = move;
        }

        public override Goal ToString()
        {
            return Role + "(" + TheMove + ")";
        }

        public bool IsNoop()
        {
            return TheMove == "noop";
        }

        public bool IsTheRandomRole()
        {
            return Role.Equals("Random", StringComparison.CurrentCultureIgnoreCase);
        }

        public string ToPrologDoesClause()
        {
            return "does(" + Role + ", " + TheMove + ")";
        }
    }

    public class State
    {
        public string TheState { get; set; }

        public State(string state)
        {
            TheState = state;
        }

        public override Goal ToString()
        {
            return TheState;
        }
    }

    public class Score
    {
        public string Role { get; set; }
        public decimal TheScore { get; set; } 

        public Score(string role, decimal score)
        {
            Role = role;
            TheScore = score;
        }

        public Score(string role, int score)
        {
            Role = role;
            TheScore = score;
        }

        public override Goal ToString()
        {
            return (Role + ":  " + TheScore);
        }
    }


    public interface IGeneralGame : IDisposable
    {
        int MinNumPlayers { get; }
        int MaxNumPlayers { get; }

        string Name { get; }
        string LongName { get; }

        string FileName { get; }

        string Description { get; }
        bool LoadDescriptionFromKIFFile(string fileName);
        bool LoadDescriptionFromFileAsync(string fileName);

        IEnumerable<string> FindRoles();
        IEnumerable<Proposition> FindPropositions();
        //IEnumerable<Action> FindActions();
        IEnumerable<Init> FindInits();
        //IEnumerable<Move> FindLegalx(Role role, State state);
        IEnumerable<Move> FindLegals(string role, State state = null);
        IEnumerable<Proposition> FindNext(Move move, State state = null);
        IEnumerable<Goal> FindReward(string role, State state = null);
        bool FindTerminal(State state, out IEnumerable<HornClause> satisfiedTerminalClauses);
        IEnumerable<Score> FindScores(State state);

        State ApplyMoves(string moves, bool movesContainRoles, State state = null, string role = null);
        Move GetTerminalMove();
    }

}
