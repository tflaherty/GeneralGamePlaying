using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using API.GGP.GeneralGameNS;
using API.GGP.GeneralGamePlayerNS;
using API.GGP.GGPInterfacesNS;
using API.GGP.PredicateLogic;
using API.UtilitiesAndExtensions;
using API.UtilitiesAndExtensions;


namespace API.GGP.GeneralGameManagerNS
{
    [Serializable]
    public class GameState
    {
        public string GameStateAsHTML;
        public string GameStateAsXML;
        public string GameStateFromProlog;
    }

    [Serializable]
    public class TurnRecord
    {
        public int Turn;    // starts at turn 1
        public bool IsInitialState;
        public bool IsTerminalState;
        public List<Move> Moves = new List<Move>();
        public GameState GameState = new GameState(); // the game state that results from the moves being applied to the game state before this turn
        public List<int> Scores = new List<int>();
    }

    public class GeneralGameManager : IDisposable
    {
        public List<IGeneralGamePlayer> Players { get; private set; }
        public GeneralGame TheGeneralGame { get; private set; }
        public int StartClock { get; set; }
        public int PlayClock { get; set; }
        public string MatchId { get; set; }
        public List<Move> AllMoves { get; private set; }
        public AutoResetEvent StartNextMoveEvent { get; set; }
        public AutoResetEvent PrevMoveEvent { get; set; }
        public int MilliSecondsBetweenFreeRunningMoves { get; set; }
        public bool AllowFreeRunningOfTurnsWithNoPlayerMoves { get; set; }
        public List<TurnRecord> TurnRecords = new List<TurnRecord>();

        public GeneralGameManager(string gameDescriptionFileName, int startClock, int playClock, string wcfSvcHostExePath = null, string tempFilePath = null)
        {
            string error;

            AllMoves = new List<Move>();

            MilliSecondsBetweenFreeRunningMoves = 500;
            AllowFreeRunningOfTurnsWithNoPlayerMoves = true;

            TheGeneralGame = new GeneralGame(wcfSvcHostExePath) { TempFilePath = tempFilePath };

            DebugAndTraceHelper.WriteTraceLine("Loading and parsing game description...", DebugAndTraceHelper.StatusStripChannelId);
            DebugAndTraceHelper.WriteTraceLine(String.Format("Started loading and parsing game description at {0}", DateTime.Now.ToLongTimeString()), DebugAndTraceHelper.ManagerChannelId);
            TheGeneralGame.LoadDescriptionFromKIFFile(gameDescriptionFileName);
            DebugAndTraceHelper.WriteTraceLine(String.Format("Finished loading and parsing game description at {0}", DateTime.Now.ToLongTimeString()), DebugAndTraceHelper.ManagerChannelId);

            DebugAndTraceHelper.WriteTraceLine("Initializing game...", DebugAndTraceHelper.StatusStripChannelId);
            DebugAndTraceHelper.WriteTraceLine(String.Format("Started initializing game at {0}", DateTime.Now.ToLongTimeString()), DebugAndTraceHelper.ManagerChannelId);
            TheGeneralGame.InitializeGame(out error);
            DebugAndTraceHelper.WriteTraceLine(String.Format("Finished initializing game at {0}", DateTime.Now.ToLongTimeString()), DebugAndTraceHelper.ManagerChannelId);

            StartClock = startClock;
            PlayClock = playClock;

            MatchId = DateTime.Now.ToString() + TheGeneralGame.Name;

            DebugAndTraceHelper.AddLogFileListener(DebugAndTraceHelper.ManagerChannelId);

            DebugAndTraceHelper.WriteTraceLine("Creating players...", DebugAndTraceHelper.StatusStripChannelId);
            Players = new List<IGeneralGamePlayer>();
            var roles = TheGeneralGame.FindRoles().ToArray();

            if (TheGeneralGame.Name == "DarkChess")
            {
                for (int i = 0; i < roles.Count(); i++)
                {
                    if (roles[i] != "random")
                    {
                        var newPlayer = new FirstDarkChessPlayer(roles[i], wcfSvcHostExePath, tempFilePath);
                        DebugAndTraceHelper.AddLogFileListener(roles[i]);
                        Players.Add(newPlayer);                        
                    }
                    else
                    {
                        var newPlayer = new FirstGeneralGamePlayer(roles[i], wcfSvcHostExePath, tempFilePath);
                        DebugAndTraceHelper.AddLogFileListener(roles[i]);
                        Players.Add(newPlayer);                                                
                    }
                }
            }
            else
            {
                for (int i = 0; i < roles.Count(); i++)
                {
                    var newPlayer = new FirstGeneralGamePlayer(roles[i], wcfSvcHostExePath, tempFilePath);
                    DebugAndTraceHelper.AddLogFileListener(roles[i]);
                    Players.Add(newPlayer);
                }                
            }

            foreach (TraceListener traceListener in DebugAndTraceHelper.GetListeners(DebugAndTraceHelper.StateChannelId))
            {
                var ieWebBrowserListener = traceListener as IEWebBrowserListener;
                if (ieWebBrowserListener != null)
                {
                    ieWebBrowserListener.xslFilePath = TheGeneralGame.XMLStyleSheetFilePath;
                }
            }
        }

        public void RecordMoves(IEnumerable<Move> moves)
        {
            foreach (Move move in moves)
            {
                AllMoves.Add(move);
            }
        }

        public bool PlayFromHistoryFile(string filePath)
        {
            int currentTurnIndex = 0;

            IFormatter formatter = new BinaryFormatter();
            using (FileStream s = File.OpenRead(filePath))
            {
                TurnRecords = (List<TurnRecord>)formatter.Deserialize(s);
            }

            bool? goForward = null;
            while (true)
            {
                if (goForward != null)
                {
                    if (goForward.Value)
                    {
                        foreach (Move move in TurnRecords[currentTurnIndex].Moves)
                        {
                            // output the moves that got us to this new state
                            DebugAndTraceHelper.WriteTraceLine(move.Role + " makes move " + move.TheMove, DebugAndTraceHelper.ManagerChannelId);
                        }                        
                    }
                    else
                    {
                        foreach (Move move in TurnRecords[currentTurnIndex+1].Moves)
                        {
                            // output the moves that we are undoing to get to this state
                            DebugAndTraceHelper.WriteTraceLine(move.Role + " makes move " + move.TheMove, DebugAndTraceHelper.ManagerChannelId);
                        }                                                
                    }
                }

                DebugAndTraceHelper.WriteTraceLine(TurnRecords[currentTurnIndex].GameState.GameStateAsXML, DebugAndTraceHelper.StateChannelId);

                if (StartNextMoveEvent != null)
                {
                    DebugAndTraceHelper.WriteTraceLine("Please click prev/next turn button to go to prev/next turn...", DebugAndTraceHelper.StatusStripChannelId);

                    goForward = null;
                    while(goForward == null)
                    {
                        if (StartNextMoveEvent.WaitOne(5))
                        {
                            goForward = true;
                        }
                        else if (PrevMoveEvent.WaitOne(5))
                        {
                            goForward = false;
                        }                        
                    }
                }
                else
                {
                    if (MilliSecondsBetweenFreeRunningMoves >= 0)
                    {
                        Thread.Sleep(MilliSecondsBetweenFreeRunningMoves);
                    }

                    goForward = true;
                }

                if (goForward.Value)
                {
                    currentTurnIndex = Math.Min(currentTurnIndex + 1, TurnRecords.Max(n => n.Turn));
                }
                else
                {
                    currentTurnIndex = Math.Max(currentTurnIndex - 1, 0);
                }
            }

            return true;
        }

        public bool Play()
        {
            IEnumerable<HornClause> satisfiedTerminalClauses;
            try
            {
                if (!CheckIfAllPlayersAreUpAndRunning())
                {
                    return false;
                }

                var traceListener = DebugAndTraceHelper.GetListeners(DebugAndTraceHelper.StateChannelId).FirstOrDefault();
                var ieWebBrowserListener = traceListener as IEWebBrowserListener;

                // show this in the game manager's web browser
                var initialStateAsXML = TheGeneralGame.GetStateAsCompleteXML();
                DebugAndTraceHelper.WriteTraceLine(initialStateAsXML, DebugAndTraceHelper.StateChannelId);

                TurnRecord initialTurnRecord = new TurnRecord();
                initialTurnRecord.IsInitialState = true;
                initialTurnRecord.GameState.GameStateAsXML = initialStateAsXML;
                //initialTurnRecord.GameState.GameStateAsHTML = ieWebBrowserListener == null ? "" : ieWebBrowserListener.ieWebBrowser.DocumentText;
                TurnRecords.Add(initialTurnRecord);

                IFormatter formatter = new BinaryFormatter();
                var historyFileName = DebugAndTraceHelper.CustomLocation + DebugAndTraceHelper.BaseFileName + "_" +
                           (DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToLongTimeString()).Replace(" ", "_").Replace(@"/", "-").Replace(":", "-") + ".his";

                DebugAndTraceHelper.WriteTraceLine("Starting Players...", DebugAndTraceHelper.StatusStripChannelId);
                StartAllPlayers();

                var prevMoves = new List<Move>();
                DebugAndTraceHelper.WriteTraceLine("Checking for game termination...", DebugAndTraceHelper.StatusStripChannelId);
                bool gameHasTerminated = TheGeneralGame.FindTerminal(out satisfiedTerminalClauses);
                Move terminalMove = null;
                while (!gameHasTerminated)
                {
                    DebugAndTraceHelper.WriteTraceLine("Waiting for next moves...", DebugAndTraceHelper.StatusStripChannelId);
                    var nextMoves = GetNextMoves(prevMoves);
                    RecordMoves(nextMoves);

                    DebugAndTraceHelper.WriteTraceLine("**** moves ****", DebugAndTraceHelper.ManagerChannelId);
                    foreach (Move nextMove in nextMoves)
                    {
                        DebugAndTraceHelper.WriteTraceLine(nextMove.Role + " makes move " + nextMove.TheMove, DebugAndTraceHelper.ManagerChannelId);
                    }

                    // ToDo:  
                    DebugAndTraceHelper.WriteTraceLine("Checking for game termination...", DebugAndTraceHelper.StatusStripChannelId);

                    TheGeneralGame.ApplyMoves(nextMoves);
                    if (TheGeneralGame.FindTerminal(out satisfiedTerminalClauses))
                    {
                        gameHasTerminated = true;
                        DebugAndTraceHelper.WriteTraceLine(
                            String.Format("******** Game Ended for reason(s) {0} ********",
                                          String.Join(", ", satisfiedTerminalClauses)), DebugAndTraceHelper.ManagerChannelId);
                        terminalMove = nextMoves.Where(n => !n.IsNoop()).FirstOrDefault();
                    }

                    var stateAsXML = TheGeneralGame.GetStateAsCompleteXML();
                    DebugAndTraceHelper.WriteTraceLine(stateAsXML, DebugAndTraceHelper.StateChannelId);

                    prevMoves = nextMoves;

                    // serialize the history information
                    TurnRecord thisTurnRecord = new TurnRecord();
                    thisTurnRecord.Turn = TheGeneralGame.CurrentTurn;
                    thisTurnRecord.GameState.GameStateAsXML = stateAsXML;
                    thisTurnRecord.Moves = nextMoves;
                    //thisTurnRecord.GameState.GameStateAsHTML = ieWebBrowserListener == null ? "" : ieWebBrowserListener.ieWebBrowser.DocumentText;
                    thisTurnRecord.IsTerminalState = gameHasTerminated;
                    TurnRecords.Add(thisTurnRecord);
                    using (FileStream s = File.Create(historyFileName))
                    {
                        formatter.Serialize(s, TurnRecords);
                    }

                    if (gameHasTerminated)
                    {
                        DebugAndTraceHelper.WriteTraceLine("Game has ended", DebugAndTraceHelper.StatusStripChannelId);                        
                    }
                    else
                    {
                        if (StartNextMoveEvent != null && (!AllowFreeRunningOfTurnsWithNoPlayerMoves || nextMoves.Where(n => !n.IsTheRandomRole() && !n.IsNoop()).Any()))
                        {
                            DebugAndTraceHelper.WriteTraceLine("Please click next turn button to go to next turn...", DebugAndTraceHelper.StatusStripChannelId);
                            StartNextMoveEvent.WaitOne();
                        }
                        else
                        {
                            if (MilliSecondsBetweenFreeRunningMoves >= 0)
                            {
                                if (AllowFreeRunningOfTurnsWithNoPlayerMoves && !nextMoves.Where(n => !n.IsTheRandomRole() && !n.IsNoop()).Any())
                                {
                                    Thread.Sleep(MilliSecondsBetweenFreeRunningMoves/4);
                                }
                                else
                                {
                                    Thread.Sleep(MilliSecondsBetweenFreeRunningMoves);                                                                    
                                }
                            }
                        }
                    }
                }

                var scores = TheGeneralGame.FindScores();
                DebugAndTraceHelper.WriteTraceLine("**** scores ****", DebugAndTraceHelper.ManagerChannelId);
                foreach (Score score in scores)
                {
                    DebugAndTraceHelper.WriteTraceLine(score, DebugAndTraceHelper.ManagerChannelId);
                }

                StopAllPlayers(terminalMove);

                return true;
            }
            catch (Exception ex)
            {                
                throw;
            }
        }

        public void StopAllPlayers(Move terminalMove)
        {
            var tasks = new List<Task<string>>();

            foreach (var _player in Players)
            {
                var player = _player;
                var newTask = new Task<string>(() => player.StopAsync(MatchId, terminalMove == null ? String.Empty : terminalMove.ToString()));
                tasks.Add(newTask);
                newTask.Start();
            }

            Task.WaitAll(tasks.ToArray());            
        }

        public List<Move> GetNextMoves(IEnumerable<Move> previousMoves)
        {
            var tasks = new List<Task<string>>();
            var nextMoves = new List<Move>();

            try
            {
                var roleTaskIds = new Dictionary<string, int?>();

                foreach (var _player in Players)
                {
                    var player = _player;
                    Task<string> newTask;

                    if (previousMoves.Any())
                    {
                        var previousMovesAsString = String.Empty.JoinAdvanced("(", ") ", previousMoves.Select(n => n.TheMove).ToArray());
                        newTask = new Task<string>(() => player.PlayAsync(MatchId, "(" + previousMovesAsString + ")"));
                    }
                    else
                    {
                        newTask = new Task<string>(() => player.PlayAsync(MatchId, PlayerManagerProtocolConstants.NilReplyString));
                    }

                    roleTaskIds.Add(player.Role, newTask.Id);
                    tasks.Add(newTask);
                    newTask.Start();
                }

                Task.WaitAll(tasks.ToArray());

                foreach (string _role in TheGeneralGame.FindRoles())
                {
                    var role = _role;
                    var task = tasks.Where(n => n.Id == roleTaskIds[role]).First();
                    nextMoves.Add(new Move(role, task.Result));
                }

                return nextMoves;
            }
            catch (AggregateException ex)
            {
                ex.Handle(_ => false);
            }
            catch (Exception)
            {
                throw;
            }

            return nextMoves;
        }

        public void StartAllPlayers()
        {
            var tasks = new List<Task<string>>();

            try
            {
                var roles = TheGeneralGame.FindRoles().ToArray();

                int roleCount = 0;
                foreach (var _player in Players)
                {
                    var player = _player;
                    string tmpRoleString = roles[roleCount++].ToString();

#if DO_SERIAL
                    var foo = player.StartAsync(MatchId, tmpRoleString, TheGeneralGame.Description, StartClock,
                                                PlayClock);
#else
                    var newTask = new Task<string>(() => player.StartAsync(MatchId, tmpRoleString, TheGeneralGame.Description, StartClock, PlayClock));
                    tasks.Add(newTask);
                    newTask.Start();
#endif
                }

                //Task.WaitAll(tasks.ToArray(), StartClock*1000);
                Task.WaitAll(tasks.ToArray());
            }
            catch (AggregateException ex)
            {
                ex.Handle(_ => false);
            }
            catch (Exception)
            {
                throw;
            }            
        }

        public bool CheckIfAllPlayersAreUpAndRunning()
        {
            var tasks = new List<Task<string>>();

            try
            {
                foreach (var player in Players)
                {
                    var newTask = new Task<string>(player.InfoAsync);
                    tasks.Add(newTask);
                    newTask.Start();
                }

                Task.WaitAll(tasks.ToArray());

                return !tasks.Where(n => n.Result != PlayerManagerProtocolConstants.ReadyReplyString).Any();
            }
            catch (AggregateException ex)
            {
                ex.Handle(_ => false);
            }
            catch (Exception)
            {                
                throw;
            }

            return false;
        }

        public void Dispose()
        {
            foreach (IGeneralGamePlayer generalGamePlayer in Players)
            {
                generalGamePlayer.Dispose();
            }

            if (TheGeneralGame != null)
            {
                TheGeneralGame.Dispose();
            }
        }
    }
}
