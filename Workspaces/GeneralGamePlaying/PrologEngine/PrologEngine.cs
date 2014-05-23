using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.ServiceModel;
using System.Threading;
using API.SWIProlog.SWIPrologServiceLibrary;
using API.UtilitiesAndExtensions;

namespace API.SWIProlog.Engine
{
    public class PrologEngine : IDisposable
    {
        public static string AlphabeticVariableList = "VA,VB,VC,VD,VE,VF,VG,VH,VI,VJ,VK,VL,VM,VN,VO,VP,VQ,VR,VS,VT,VU,VV,VW,VX,VY,VZ";

        static private int prologEngineCount;
        private static object myLock = new object();

        private ISWIPrologService swiPrologServiceClient ;
        private int engineIndex;

        private readonly List<Process> processesStarted = new List<Process>();

        private string WcfSvcHostExePath = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\WcfSvcHost.exe";

        public static void KillAllPrologEngineServicesRunning()
        {
            var wcfSvcHosts = Process.GetProcessesByName("WcfSvcHost");
            //var allProcs = Process.GetProcesses();
            foreach (Process process in wcfSvcHosts)
            {
                process.Kill();
            }
        }

        public PrologEngine(string wcfSvcHostExePath = null)
        {
            lock (myLock)
            {
                WcfSvcHostExePath = wcfSvcHostExePath ?? WcfSvcHostExePath;

                engineIndex = prologEngineCount;

                if (engineIndex == 0)
                {
                    KillAllPrologEngineServicesRunning();
                }

                var myBinding = new NetTcpBinding(SecurityMode.None);
                myBinding.MaxReceivedMessageSize = 262144 * 64;
                myBinding.MaxBufferSize = 262144 * 64;
                EndpointAddress myEndpoint = null;

                bool needToStartService = true;
                switch (prologEngineCount)
                {
                    case 0:
                        myEndpoint = new EndpointAddress("net.tcp://localhost/Design_Time_Addresses/SWIPrologService/SWIPrologService1");
#if LOOK_FOR_WCF_SERVICE
                        {
                            var myChannelFactoryTmp = new ChannelFactory<ISWIPrologService>(myBinding, myEndpoint);
                            var swiPrologServiceClientTmp = myChannelFactoryTmp.CreateChannel();

                            try
                            {
                                swiPrologServiceClientTmp.ExecuteClause("member(X, [One]).");
                                needToStartService = false;
                            }
                            catch (Exception)
                            {                                
                                //throw;
                            }
                        }
#else
                        needToStartService = true;
#endif

                        if (needToStartService)
                        {
                            StartWCFService1();                            
                        }
                        //myEndpoint = new EndpointAddress("net.tcp://localhost/Design_Time_Addresses/SWIPrologService/SWIPrologService1");
                        break;

                    case 1:
                        myEndpoint = new EndpointAddress("net.tcp://localhost:810/Design_Time_Addresses/SWIPrologService2/SWIPrologService1");
#if LOOK_FOR_WCF_SERVICE
                        {
                            var myChannelFactoryTmp = new ChannelFactory<ISWIPrologService>(myBinding, myEndpoint);
                            var swiPrologServiceClientTmp = myChannelFactoryTmp.CreateChannel();

                            try
                            {
                                swiPrologServiceClientTmp.ExecuteClause("member(X, [One]).");
                                needToStartService = false;
                            }
                            catch (Exception)
                            {
                                //throw;
                            }
                        }
#else
                        needToStartService = true;
#endif
                        if (needToStartService)
                        {
                            StartWCFService2();                            
                        }
                        //myEndpoint = new EndpointAddress("net.tcp://localhost:810/Design_Time_Addresses/SWIPrologService2/SWIPrologService1");
                        break;

                    case 2:
                        myEndpoint = new EndpointAddress("net.tcp://localhost:812/Design_Time_Addresses/SWIPrologService3/SWIPrologService1");
#if LOOK_FOR_WCF_SERVICE
                        {
                            var myChannelFactoryTmp = new ChannelFactory<ISWIPrologService>(myBinding, myEndpoint);
                            var swiPrologServiceClientTmp = myChannelFactoryTmp.CreateChannel();

                            try
                            {
                                swiPrologServiceClientTmp.ExecuteClause("member(X, [One]).");
                                needToStartService = false;
                            }
                            catch (Exception)
                            {
                                //throw;
                            }
                        }
#else
                        needToStartService = true;
#endif
                        if (needToStartService)
                        {
                            StartWCFService3();
                        }
                        //myEndpoint = new EndpointAddress("net.tcp://localhost:812/Design_Time_Addresses/SWIPrologService3/SWIPrologService1");
                        break;

                    case 3:
                        myEndpoint = new EndpointAddress("net.tcp://localhost:8732/Design_Time_Addresses/SWIPrologService4/SWIPrologService1");
#if LOOK_FOR_WCF_SERVICE
                        {
                            var myChannelFactoryTmp = new ChannelFactory<ISWIPrologService>(myBinding, myEndpoint);
                            var swiPrologServiceClientTmp = myChannelFactoryTmp.CreateChannel();

                            try
                            {
                                swiPrologServiceClientTmp.ExecuteClause("member(X, [One]).");
                                needToStartService = false;
                            }
                            catch (Exception)
                            {
                                //throw;
                            }
                        }
#else
                        needToStartService = true;
#endif
                        if (needToStartService)
                        {
                            StartWCFService4();
                        }
                        //myEndpoint = new EndpointAddress("net.tcp://localhost:8732/Design_Time_Addresses/SWIPrologService3/SWIPrologService1");
                        break;

                    default:
                        throw new NotImplementedException("Currently there is no support for more than 4 Prolog Engines");
                        break;
                }

                var myChannelFactory = new ChannelFactory<ISWIPrologService>(myBinding, myEndpoint);
                swiPrologServiceClient = myChannelFactory.CreateChannel();

/*  works
                var codeBasePath = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
                codeBasePath = System.IO.Path.GetDirectoryName(new Uri(codeBasePath).LocalPath);
                codeBasePath = Path.GetDirectoryName(new Uri(codeBasePath).LocalPath);
                codeBasePath = Path.GetDirectoryName(new Uri(codeBasePath).LocalPath);
                var localCodeBasePath = new Uri(codeBasePath).LocalPath;
                var xmlprotestPath = localCodeBasePath + "\\..\\PrologEngine\\Prolog Files\\xmlprotest.pl";
*/
                var codeBasePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
                codeBasePath = Path.GetDirectoryName(codeBasePath);
                codeBasePath = Path.GetDirectoryName(codeBasePath);
                codeBasePath = Path.GetDirectoryName(codeBasePath);
                codeBasePath = Path.GetDirectoryName(codeBasePath);
                var xmlprotestPath = codeBasePath + "\\PrologEngine\\Prolog Files\\xmlprotest.pl";
                
                xmlprotestPath = xmlprotestPath.Replace("\\", "/");
                xmlprotestPath = xmlprotestPath.Replace("file:/", "");

                Consult(xmlprotestPath);


                /*
                var darkChessPath = Path.GetDirectoryName(Path.GetDirectoryName(
                    System.IO.Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase))) + "/../PrologEngine/Prolog Files/DarkChess.pl";

                darkChessPath = darkChessPath.Replace("\\", "/");
                darkChessPath = darkChessPath.Replace("file:/", "");

                Consult(darkChessPath);
                 */ 

                prologEngineCount++;                
            }
        }

        // This code was taken from:
        //    http://www.global-webnet.com/blog/post/2012/05/20/Windows-8-Programmatically-starting-WcfSvcHost-for-unit-test.aspx
        // Gwn.Data WCF Service is added as reference to Tests project and
        // the Gwn.Data project's App.Config "Copy to Output Directory" property
        // is set to "Copy always" to ensure both files are available for WcfSvcHost
        protected void StartWCFService1()
        {
            Process newProcess = Process.Start(WcfSvcHostExePath, @"/service:../../../SWIPrologServiceLibrary/bin/debug/SWIPrologServiceLibrary.dll /config:../../../SWIPrologServiceLibrary/bin/debug/App.Config");
            if (newProcess != null)
            {
                processesStarted.Add(newProcess);
            }
        }

        protected void StartWCFService2()
        {
            Process newProcess = Process.Start(WcfSvcHostExePath, @"/service:../../../SWIPrologService2Library/bin/debug/SWIPrologService2Library.dll /config:../../../SWIPrologService2Library/bin/debug/App.Config");
            if (newProcess != null)
            {
                processesStarted.Add(newProcess);
            }
        }

        protected void StartWCFService3()
        {
            Process newProcess = Process.Start(WcfSvcHostExePath, @"/service:../../../SWIPrologService3Library/bin/debug/SWIPrologService3Library.dll /config:../../../SWIPrologService3Library/bin/debug/App.Config");
            if (newProcess != null)
            {
                processesStarted.Add(newProcess);
            }
        }

        protected void StartWCFService4()
        {
            Process newProcess = Process.Start(WcfSvcHostExePath, @"/service:../../../SWIPrologService4Library/bin/debug/SWIPrologService4Library.dll /config:../../../SWIPrologService4Library/bin/debug/App.Config");
            if (newProcess != null)
            {
                processesStarted.Add(newProcess);
            }
        }

        public bool Assert(string clause)
        {
            return swiPrologServiceClient.ExecuteClause("assert(" + clause + ").");
        }

        public bool DeclareDynamic(string predicate, int arity)
        {
            return
                swiPrologServiceClient.ExecuteClause("dynamic " + predicate + "/" +
                                                     arity + "");
        }

        public bool Retract(string clause, bool retractAll = true)
        {
            return swiPrologServiceClient.ExecuteClause((retractAll ? "retractall(" : "retract(") + clause + ").");
        }

        public bool ExecuteClause(string clause)
        {
            return swiPrologServiceClient.ExecuteClause(clause);
        }

        public List<List<SolutionVariable>> GetSolutionVariables(string query)
        {
            return swiPrologServiceClient.GetSolutionVariables(query);
        }

        public void Reset()
        {
            //DebugAndTraceHelper.WriteTraceLineToAllChannels("Must implement PrologEngine.Reset()!!");    
        }

        public void Dispose()
        {
            foreach (Process process in processesStarted)
            {
                //process.Dispose();
                //process.CloseMainWindow();
                //process.Close();
                //while (!process.HasExited)
                //{
                //    Thread.Yield();
                //}

                // Why don't any of the above work????
                if (!process.HasExited)
                    process.Kill();
            }
        }

        public bool Consult(string fileName)
        {
            string clause = "consult('" + fileName + "').";
            ExecuteClause(clause);

            return true;
        }

        public string ListAll(bool sortAlphabetically = true)
        {
            /*
                current_predicate(Name/Arity),
                functor(Pred, Name, Arity),
                nth_clause(Pred, Index, Ref),
                clause(Head, Body, Ref),
                findall(Q, predicate_property(Head, Q), L), predicate_property(Head, dynamic), \+predicate_property(Head, built_in).            
             */

            const string query = @"current_predicate(Name/Arity), 
                        functor(Pred, Name, Arity),
                        nth_clause(Pred, Index, Ref),
                        clause(Head, Body, Ref),
                        predicate_property(Head, dynamic), \+predicate_property(Head, built_in).";

            var strings = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (List<SolutionVariable> solutionVariables in swiPrologServiceClient.GetSolutionVariables(query))
            {
                var head = solutionVariables.Where(n => n.Variable == "Head").FirstOrDefault();
                var body = solutionVariables.Where(n => n.Variable == "Body").FirstOrDefault();

                if (head != null && body != null)
                {
                    strings.Add(head.Value + " :- " + body.Value + ".");
                }
            }

            if (sortAlphabetically)
            {
                foreach (string s in strings.OrderBy(n => n))
                {
                    sb.AppendLine(s);
                }                
            }
            else
            {
                foreach (string s in strings)
                {
                    sb.AppendLine(s);
                }                
            }

            var tmp = sb.ToString();
            return tmp;
        }

        public string ListAllFactsAsCompleteXML(IEnumerable<Tuple<string, int>> predicatesAndArities, string styleSheetName = null)
        {
            var query = new StringBuilder();
            query.Append("dumpXML([ ");
            foreach (Tuple<string, int> predicateAndArity in predicatesAndArities)
            {
                query.Append("[");
                query.Append(predicateAndArity.Item1).ToString();
                query.Append(", ");
                query.Append(predicateAndArity.Item2.ToString());
                query.Append("],");
            }
            // remove the last comma
            query.Remove(query.Length - 1, 1);

            query.Append(" ], XML).");

            var output = new StringBuilder();
            output.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");          
            if (styleSheetName != null)
            {
                output.AppendFormat("<?xml-stylesheet type=\"text/xsl\" href=\"{0}\"?>", styleSheetName);
            }

            output.AppendLine("<state>");
            foreach (List<SolutionVariable> solutionVariables in swiPrologServiceClient.GetSolutionVariables(query.ToString()))
            {
                var xml = solutionVariables.Where(n => n.Variable == "XML").FirstOrDefault();
                if (xml != null)
                {
                    output.AppendLine(xml.Value);
                }
            }
            output.AppendLine("</state>");

            return output.ToString();
        }

        public string ListAllFacts(string[] predicates = null, bool sortAlphabetically = true)
        {
            /*
                current_predicate(Name/Arity),
                functor(Pred, Name, Arity),
                nth_clause(Pred, Index, Ref),
                clause(Head, Body, Ref),
                findall(Q, predicate_property(Head, Q), L), predicate_property(Head, dynamic), \+predicate_property(Head, built_in).            
             */

            const string query = @"current_predicate(Name/Arity), 
                        functor(Pred, Name, Arity),
                        nth_clause(Pred, Index, Ref),
                        clause(Head, Body, Ref),
                        predicate_property(Head, dynamic), \+predicate_property(Head, built_in).";

            var strings = new List<string>();
            StringBuilder sb = new StringBuilder();
            foreach (List<SolutionVariable> solutionVariables in swiPrologServiceClient.GetSolutionVariables(query))
            {
                var head = solutionVariables.Where(n => n.Variable == "Head").FirstOrDefault();
                var body = solutionVariables.Where(n => n.Variable == "Body").FirstOrDefault();

                if (head != null && (body == null || body.Value == "true"))
                {
                    strings.Add(head.Value + ".");
                }
            }

            if (sortAlphabetically)
            {
                foreach (string s in strings.OrderBy(n => n))
                {
                    sb.AppendLine(s);
                }
            }
            else
            {
                foreach (string s in strings)
                {
                    sb.AppendLine(s);
                }
            }

            var tmp = sb.ToString();
            return tmp;
        }

    }

}
