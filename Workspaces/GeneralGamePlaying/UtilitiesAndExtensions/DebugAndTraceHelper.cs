using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Xsl;

namespace API.UtilitiesAndExtensions
{
    // ToDo:  Put in an option to put out a time stamp with each output
    // ToDo:  Create a log file viewer than can view more than one file at
    //          a time and reorder entries by time stamp!
    // ToDo:  Find out if there's a better way to do synchronization on the lists
    //          of listeners for each channel rather than the lock on the readers and writers.
    //          System.Threading.ReaderWriterLockSlim is a good candidate because
    //          it allows multiple readers (which DebugAndTraceHelper needs because
    //          several threads can be writing at the same time but only one should be
    //          writing at any one time) .
    public class DebugAndTraceHelper
    {
        public static readonly string DefaultChannelId = "__defaultChannelIdx";
        public static readonly string ManagerChannelId = "manager";
        public static readonly string StatusStripChannelId = "status";
        public static readonly string StateChannelId = "state";
        public static readonly string HistoryChannelId = "history";

        // this is based on the Visual Basic LogFileLocation class
        public enum LogFileLocation
        {
            // The path for the application data that is shared among all users, with the format:
            //      BasePath\ CompanyName\ ProductName\ ProductVersion
            //      A typical value for BasePath is: 
            //          C:\Documents and Settings\All Users\Application Data
            //      The values of CompanyName, ProductName, and ProductVersion come from the assembly. 
            CommonApplicationDirectory,

            //The path for the executable file that started the application.
            ExecutableDirectory,

            // The path for the application data of a user, with the format:
            //      BasePath\ CompanyName\ ProductName\ ProductVersion
            //      A typical value for BasePath is: 
            //          C:\Documents and Settings\ username\Application Data 
            //      The values of CompanyName, ProductName, and ProductVersion come form the assembly. 
            LocalUserApplicationDirectory,

            // The path of the current system's temporary folder.
            TempDirectory,

            // If the string specified by CustomLocation is not empty, then use it as the path; 
            // otherwise use the path for the application data of a user. 
            Custom
        }

        public static LogFileLocation TheLogFileLocation { get; set; }

        //  The name of the log-file directory. The default setting for this property is the user's directory for application data. 
        public static string CustomLocation { get; set; }

        public static string BaseFileName { get; set; }

        public static bool FlushAfterEachOutput { get; set; }

        // string is the channel id (it's String.Empty for the default channel)
        private static readonly ConcurrentDictionary<string, List<TraceListener>> listeners = new ConcurrentDictionary<string, List<TraceListener>>();

        static DebugAndTraceHelper()
        {
            // I do this so everytime we write to the default channel we
            // don't have to check to see if a listener list exists for it.
            listeners.TryAdd(DefaultChannelId, new List<TraceListener>());

            // ToDo:  Load this from the configuration file
            TheLogFileLocation = LogFileLocation.Custom;
            // ToDo:  Load this from the configuration file
            CustomLocation = System.IO.Path.GetTempPath() + Application.ProductName + @"\";
            BaseFileName = Application.ProductName;

            // I don't need to do this for Debug too
            // because it shares its list of listeners
            // with Trace.
            Trace.Listeners.Clear();

            FlushAfterEachOutput = true;
        }

        // This adds a listener for all channels without a specific listener
        public static void AddListener(TraceListener listener)
        {
            AddListener(listener, DefaultChannelId);
        }

        public static List<TraceListener>GetListeners(string channelId)
        {
            var requestedListeners = new List<TraceListener>(listeners[channelId]);

            return requestedListeners;
        }

        public static void RemoveListener(TraceListener listener)
        {
            RemoveListener(listener, DefaultChannelId);
        }

        public static TraceListener AddLogFileListener(string channelId)
        {
            string fileName;

            switch(TheLogFileLocation)
            {
                case LogFileLocation.CommonApplicationDirectory:
                    throw new NotImplementedException();
                    break;

                case LogFileLocation.ExecutableDirectory:
                    throw new NotImplementedException();
                    break;

                case LogFileLocation.LocalUserApplicationDirectory:
                    throw new NotImplementedException();
                    break;

                case LogFileLocation.TempDirectory:
                    throw new NotImplementedException();
                    break;

                case LogFileLocation.Custom:
                    fileName = CustomLocation + BaseFileName + "_" + channelId + "_" +
                               (DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToLongTimeString()).Replace(" ", "_").Replace(@"/", "-").Replace(":", "-") + ".log";
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }

            var newListener = new TextWriterTraceListener(fileName);
            AddListener(newListener, channelId);
            return newListener;
        }

        public static TraceListener AddHistoryFileListener(string channelId)
        {
            string fileName;

            switch (TheLogFileLocation)
            {
                case LogFileLocation.CommonApplicationDirectory:
                    throw new NotImplementedException();
                    break;

                case LogFileLocation.ExecutableDirectory:
                    throw new NotImplementedException();
                    break;

                case LogFileLocation.LocalUserApplicationDirectory:
                    throw new NotImplementedException();
                    break;

                case LogFileLocation.TempDirectory:
                    throw new NotImplementedException();
                    break;

                case LogFileLocation.Custom:
                    fileName = CustomLocation + BaseFileName + "_" + channelId + "_" +
                               (DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToLongTimeString()).Replace(" ", "_").Replace(@"/", "-").Replace(":", "-") + ".his";
                    break;

                default:
                    throw new NotImplementedException();
                    break;
            }

            var newListener = new TextWriterTraceListener(fileName);
            AddListener(newListener, channelId);
            return newListener;
        }

        public static void AddListener(TraceListener listener, string channelId)
        {
            listeners.TryAdd(channelId, new List<TraceListener>());
            lock(listeners[channelId])
            {
                listeners[channelId].Add(listener);                
            }
        }

        public static void RemoveListener(TraceListener listener, string channelId)
        {
            List<TraceListener> listenersForThisChannel;
            if (listeners.TryGetValue(channelId, out listenersForThisChannel))
            {
                lock(listenersForThisChannel)
                {
                    listenersForThisChannel.Remove(listener);                    
                }
            }
        }

        private static DecoupledConsole _DecoupledConsole;
        public static DecoupledConsole DecoupledConsole
        {
            get { return _DecoupledConsole; }

            set
            {
                if (value != _DecoupledConsole)
                {
                    if (decoupledConsoleTraceListener != null)
                    {
                        Trace.Listeners.Remove(decoupledConsoleTraceListener);
                    }

                    decoupledConsoleTraceListener = new DecoupledConsoleTraceListener(value);

                    if (OutputToDecoupledConsole)
                    {
                        Trace.Listeners.Add(decoupledConsoleTraceListener);
                    }
                }
            }
        }

        private static DecoupledConsoleTraceListener decoupledConsoleTraceListener;

        private static bool _OutputToDecoupledConsole;
        public static bool OutputToDecoupledConsole
        {
            get { return _OutputToDecoupledConsole; }

            set
            {
                if (value)
                {
                    Trace.Listeners.Add(decoupledConsoleTraceListener);
                }
                else
                {
                    Trace.Listeners.Remove(decoupledConsoleTraceListener);
                }
            }
        }

        [Conditional("TRACE")]
        public static void WriteTraceLineToAllChannels(object value)
        {
            lock (listeners)
            {
                foreach (KeyValuePair<string, List<TraceListener>> keyValuePair in listeners)
                {
                    foreach (TraceListener traceListener in keyValuePair.Value)
                    {
                        traceListener.WriteLine(value);
                        if (FlushAfterEachOutput)
                        {
                            traceListener.Flush();
                        }
                    }
                }
            }
        }

        [Conditional("TRACE")]
        public static void WriteTraceLine(object value)
        {
            lock (listeners[DefaultChannelId])
            {
                foreach (TraceListener traceListener in listeners[DefaultChannelId])
                {
                    traceListener.WriteLine(value);
                    if (FlushAfterEachOutput)
                    {
                        traceListener.Flush();
                    }
                }                
            }
        }

        [Conditional("TRACE")]
        public static void WriteTraceLine(object value, string channelId)
        {
            if (listeners.ContainsKey(channelId))
            {
                lock(listeners[channelId])
                {
                    foreach (TraceListener traceListener in listeners[channelId])
                    {
                        traceListener.WriteLine(value);
                        if (FlushAfterEachOutput)
                        {
                            traceListener.Flush();
                        }
                    }                                    
                }
            }
        }

        [Conditional("TRACE")]
        public static void WriteTraceLineUnchecked(object value, string channelId)
        {
            lock (listeners[channelId])
            {
                foreach (TraceListener traceListener in listeners[channelId])
                {
                    traceListener.WriteLine(value);
                    if (FlushAfterEachOutput)
                    {
                        traceListener.Flush();
                    }
                }
            }
        }

        [Conditional("DEBUG")]
        public static void WriteDebugLine(object value)
        {
            lock (listeners[DefaultChannelId])
            {
                foreach (TraceListener traceListener in listeners[DefaultChannelId])
                {
                    traceListener.WriteLine(value);
                    if (FlushAfterEachOutput)
                    {
                        traceListener.Flush();
                    }
                }                
            }
        }

        [Conditional("DEBUG")]
        public static void WriteDebugLine(object value, string channelId)
        {
            if (listeners.ContainsKey(channelId))
            {
                lock (listeners[DefaultChannelId])
                {
                    foreach (TraceListener traceListener in listeners[channelId])
                    {
                        traceListener.WriteLine(value);
                        if (FlushAfterEachOutput)
                        {
                            traceListener.Flush();
                        }
                    }
                }
            }
        }

        [Conditional("DEBUG")]
        public static void WriteDebugLineUnchecked(object value, string channelId)
        {
            lock (listeners[DefaultChannelId])
            {
                foreach (TraceListener traceListener in listeners[channelId])
                {
                    traceListener.WriteLine(value);
                    if (FlushAfterEachOutput)
                    {
                        traceListener.Flush();
                    }
                }
            }
        }
    }

    public class DecoupledConsole
    {
        // queue-based blocking collection
        private BlockingCollection<Action> blockingQueue;
        // task that processes messages to the console
        private Task messageWorker;

        public DecoupledConsole()
        {
            // create the blocking collection
            blockingQueue = new BlockingCollection<Action>();
            // create and start the worker task
            messageWorker = Task.Factory.StartNew(() =>
            {
                foreach (Action action in blockingQueue.GetConsumingEnumerable())
                {
                    // invoke the action
                    action.Invoke();
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Write(object value)
        {
            blockingQueue.Add(new Action(() =>
                Console.Write(value)));
        }

        public void WriteLine(object value)
        {
            blockingQueue.Add(new Action(() =>
                Console.WriteLine(value)));
        }

        public void WriteLine(string format, params object[] values)
        {
            blockingQueue.Add(new Action(() =>
                Console.WriteLine(format, values)));
        }
    }

    public class DecoupledConsoleTraceListener : TraceListener
    {
        private DecoupledConsole decoupledConsole;
        public DecoupledConsoleTraceListener(DecoupledConsole decoupledConsole)
        {
            this.decoupledConsole = decoupledConsole;
        }

        public override void Write(string message)
        {
            if (decoupledConsole != null)
            {
                decoupledConsole.Write(message);
            }
        }

        public override void WriteLine(string message)
        {
            if (decoupledConsole != null)
            {
                decoupledConsole.WriteLine(message);
            }
        }
    }

    public class TextboxTraceListener : TraceListener
    {
        private readonly TextBox textBox;

        public TextboxTraceListener(TextBox textBox)
        {
            this.textBox = textBox;
            TraceListenerCollection traceListeners = Trace.Listeners;
            if (!traceListeners.Contains(this))
            {
                traceListeners.Add(this);                
            }
        }

        protected delegate void Delegate();

        protected virtual void RunOnUiThread(Delegate method)
        {
            textBox.Invoke(method);
        }

        public override void Write(string message)
        {
            RunOnUiThread(() => WriteInUiThread(message));
        }

        private void WriteInUiThread(string message)
        {
            // accessing statusStrip.Text is not fast but works for small trace logs
            textBox.Text = textBox.Text + message;
            textBox.Select(textBox.Text.Length, 0);
            textBox.ScrollToCaret();
        }

        public override void WriteLine(string message)
        {
            Write(message + Environment.NewLine);
        }
    }

    public class ToolStripStatusLabelListener : TraceListener
    {
        private readonly ToolStripStatusLabel toolStripStatusLabel;

        public ToolStripStatusLabelListener(ToolStripStatusLabel toolStripStatusLabel)
        {
            this.toolStripStatusLabel = toolStripStatusLabel;
            TraceListenerCollection traceListeners = Trace.Listeners;
            if (!traceListeners.Contains(this))
            {
                traceListeners.Add(this);
            }
        }

        protected delegate void Delegate();

        protected virtual void RunOnUiThread(Delegate method)
        {
            if (toolStripStatusLabel.GetCurrentParent().InvokeRequired)
            {
                toolStripStatusLabel.GetCurrentParent().Invoke(method);                
            }
            else
            {
                method();
            }
        }

        public override void Write(string message)
        {
            RunOnUiThread(() => toolStripStatusLabel.Text = message);
            // This is done so that I can see the status update as soon as possible,
            // otherwise it can be held off for a long time.
            Application.DoEvents();
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }
    }

    public class IEWebBrowserListener : TraceListener
    {
        private XslCompiledTransform xslDocument = null;
        private string _xslFilePath;
        public string xslFilePath
        {
            get { return _xslFilePath; }
            set 
            { 
                _xslFilePath = value;
                if (_xslFilePath == null)
                {
                    xslDocument = null;
                }
                else
                {
                    xslDocument = new XslCompiledTransform();
                    xslDocument.Load(_xslFilePath);
                }
            }
        }

        public readonly WebBrowser ieWebBrowser;

        public IEWebBrowserListener(WebBrowser ieWebBrowser)
        {
            this.ieWebBrowser = ieWebBrowser;
            TraceListenerCollection traceListeners = Trace.Listeners;
            if (!traceListeners.Contains(this))
            {
                traceListeners.Add(this);
            }
        }

        protected delegate void Delegate(string message);

        protected virtual void RunOnUiThread(Delegate method, string message)
        {
            if (ieWebBrowser.InvokeRequired)
            {
                ieWebBrowser.Invoke(method, message);
            }
            else
            {
                method(message);
            }
        }

        public void DoTheWrite(string message)
        {
            if (xslDocument == null)
            {
                ieWebBrowser.DocumentText = message;
            }
            else
            {
                // original example code
                //string xmlFile = @"c:\Alerts.xml";
                //string xslFile = @"c:\Alerts.xsl";
                //XslCompiledTransform xslDocument = new XslCompiledTransform();
                //xslDocument.Load(xslFile);
                //StringWriter stringWriter = new StringWriter();
                //XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
                //xslDocument.Transform(xmlFile, xmlWriter);
                //uxWebBrowser.DocumentText = stringWriter.ToString(); 
                
                StringWriter stringWriter = new StringWriter();
                XmlWriter xmlWriter = new XmlTextWriter(stringWriter);
                XmlReader xmlReader = new XmlTextReader(new StringReader(message));
                xslDocument.Transform(xmlReader, xmlWriter);
                ieWebBrowser.DocumentText = stringWriter.ToString();                
            }
        }

        public override void Write(string message)
        {
            //RunOnUiThread(() => ieWebBrowser.DocumentText = message);
            RunOnUiThread(DoTheWrite, message);
            // This is done so that I can see the status update as soon as possible,
            // otherwise it can be held off for a long time.
            Application.DoEvents();
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }
    }

}
