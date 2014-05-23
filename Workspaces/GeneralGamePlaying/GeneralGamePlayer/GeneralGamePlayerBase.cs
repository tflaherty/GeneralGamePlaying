using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using API.GGP.GeneralGameNS;
using API.GGP.GGPInterfacesNS;
using API.SWIProlog.Engine;

namespace API.GGP.GeneralGamePlayerNS
{
    public abstract class GeneralGamePlayerBase : IGeneralGamePlayer
    {
        // ToDo:  I could have a dictionary that stores data for multiple games
        // being played by this player if so desired.
        protected GeneralGame TheGeneralGame { get; set; }

        public string Role { get; protected set; }
        public string MatchId { get; protected set; }

        protected string TempFilePath { get; set; }
        protected string WcfSvcHostExePath { get; set; }

        protected List<string> PlayerSpecificPrologFiles = new List<string>();

        protected GeneralGamePlayerBase(string role, string wcfSvcHostExePath = null, string tempFilePath = null)
        {
            WcfSvcHostExePath = wcfSvcHostExePath;
            TempFilePath = tempFilePath;
            Role = role;            
        }

        public virtual void Dispose()
        {
            if (TheGeneralGame != null)
            {
                TheGeneralGame.Dispose();
            }
        }

        public virtual string InfoAsync()
        {
            return PlayerManagerProtocolConstants.ReadyReplyString;
        }

        public virtual string StartAsync(string id, string role, string gameDescription, int startClock, int playClock)
        {
            string error;

            MatchId = id;
            Role = role;

            TheGeneralGame = new GeneralGame(WcfSvcHostExePath) { TempFilePath = TempFilePath };

            TheGeneralGame.LoadDescriptionFromGameDescriptionString(gameDescription);
            TheGeneralGame.InitializeGame(out error);
            TheGeneralGame.ConsultPrologFiles(PlayerSpecificPrologFiles);

            return PlayerManagerProtocolConstants.ReadyReplyString;
        }

        public abstract string PlayAsync(string id, string moves);

        public virtual string StopAsync(string id, string moves)
        {
            TheGeneralGame.ApplyMoves(moves, false, role: Role);
            return PlayerManagerProtocolConstants.DoneReplyString;
        }

        public virtual string AbortAsync(string id)
        {
            // ToDo: Clean up stuff
            return PlayerManagerProtocolConstants.DoneReplyString;
        }

        public virtual PrologEngine GetPrologEngine()
        {
            return TheGeneralGame.PrologEngine;
        }
    }
}
