using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using API.GGP.GeneralGameNS;
using API.GGP.GGPInterfacesNS;
using API.SWIProlog.Engine;
using API.UtilitiesAndExtensions;

namespace API.GGP.GeneralGamePlayerNS
{
    public class FirstDarkChessPlayer : GeneralGamePlayerBase
    {
#if (WIN64)
        private const string ExtraLevelIfNeeded = "\\..";
#else
        private const string ExtraLevelIfNeeded = "";
#endif
        public FirstDarkChessPlayer(string role, string wcfSvcHostExePath, string tempFilePath) 
            : base(role, wcfSvcHostExePath, tempFilePath)
        {
            var darkChessPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(
                System.IO.Path.GetDirectoryName(new Uri(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath)))) + ExtraLevelIfNeeded + "\\PrologEngine\\Prolog Files\\DarkChess.pl";

            darkChessPath = darkChessPath.Replace("\\", "/");
            darkChessPath = darkChessPath.Replace("file:/", "");

            PlayerSpecificPrologFiles.Add(darkChessPath);

            var alphaBetaPath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(
                System.IO.Path.GetDirectoryName(new Uri(
                System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath)))) + ExtraLevelIfNeeded + "\\PrologEngine\\Prolog Files\\AlphaBeta.pl";

            alphaBetaPath = alphaBetaPath.Replace("\\", "/");
            alphaBetaPath = alphaBetaPath.Replace("file:/", "");

            PlayerSpecificPrologFiles.Add(alphaBetaPath);

        }

        public override string PlayAsync(string id, string moves)
        {
            DebugAndTraceHelper.WriteTraceLine("\n\n\n*************************** moves ", Role);
            DebugAndTraceHelper.WriteTraceLine(moves, Role);

            var nextState = TheGeneralGame.ApplyMoves(moves, false, role: Role);
            DebugAndTraceHelper.WriteTraceLine(String.Format("({0}) Next state is {1}", Role, nextState), Role);

            //DebugAndTraceHelper.WriteTraceLine("*************************** after moves applied ", Role);
            //string clauseDump = this.GetPrologEngine().ListAllFacts();
            //DebugAndTraceHelper.WriteTraceLine(clauseDump, Role);

            Move nextMove = null;
            var legalMovesWithEval = TheGeneralGame.FindLegalsWithEval(Role);
            int? maxEval = legalMovesWithEval.Max(n => n.Tag as int?);
            int? minEval = legalMovesWithEval.Min(n => n.Tag as int?);

            if (legalMovesWithEval.Any())
            {
                if (maxEval != minEval)
                {
                    nextMove = legalMovesWithEval.Where(n => n.Tag as int? == maxEval.Value).First();
                }
                else
                {
                    nextMove = TheGeneralGame.FindRandomLegal(Role);
                }                
            }

            if (nextMove != null)
            {
                DebugAndTraceHelper.WriteTraceLine(String.Format("({0}) Next move is {1} with eval {2}", Role, nextMove, maxEval.Value), Role);
                return nextMove.TheMove;
            }
            else
            {
                return String.Empty;
            }
        }

        protected IEnumerable<Move> sortLegalMoves(IEnumerable<Move> legalMoves)
        {
            return legalMoves.OrderBy(n => n.TheMove).ToList();
        }
    }
}
