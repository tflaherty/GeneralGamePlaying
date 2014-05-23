using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using API.GGP.GeneralGameNS;
using API.GGP.GGPInterfacesNS;
using API.UtilitiesAndExtensions;
using API.SWIProlog.Engine;
using API.UtilitiesAndExtensions;

namespace API.GGP.GeneralGamePlayerNS
{
    public class FirstGeneralGamePlayer : GeneralGamePlayerBase
    {
        public FirstGeneralGamePlayer(string role, string wcfSvcHostExePath = null, string tempFilePath = null)
            : base(role, wcfSvcHostExePath, tempFilePath)
        {
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

            var nextMove = TheGeneralGame.FindRandomLegal(Role);
            if (nextMove != null)
            {
                DebugAndTraceHelper.WriteTraceLine(String.Format("({0}) Next move is {1}", Role, nextMove), Role);
                return nextMove.TheMove;                
            }
            else
            {
                return String.Empty;
            }
        }

        public override void Dispose()
        {
            try
            {
                if (TheGeneralGame != null)
                {
                    TheGeneralGame.Dispose();
                }
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                base.Dispose();
            }
        }
    }
}
