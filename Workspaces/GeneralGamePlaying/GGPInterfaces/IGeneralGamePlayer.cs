using System;
using API.SWIProlog.Engine;

namespace API.GGP.GGPInterfacesNS
{
    public interface IGeneralGamePlayer : IDisposable
    {
        string Role { get; }
        string MatchId { get; }

        string InfoAsync();
        string StartAsync(string id, string role, string gameDescription, int startClock, int playClock);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moves">
        ///  The order of the moves in the record is the same as the order of roles in the game description. 
        /// On the first request, where there is no preceding move, the move field is nil.
        /// </param>
        /// <returns></returns>
        string PlayAsync(string id, string moves);

        string StopAsync(string id, string moves);
        string AbortAsync(string id);

        PrologEngine GetPrologEngine();
    }
}
