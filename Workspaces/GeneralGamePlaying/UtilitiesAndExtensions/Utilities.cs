using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace API.UtilitiesAndExtensions
{
    public class Utilities
    {
        public void ShowNonModalMessageBox(IWin32Window owner, string message)
        {
            new Thread(new ThreadStart(delegate
            {
                MessageBox.Show
                (
                  owner,
                  message
                );
            })).Start();            
        }

        // This is a nice little thread safe class that can be used
        // to generate Rands from different threads but to guarantee 
        // that they're seeded with different numbers.  If you just seed
        // Rand with DateTime.Now.Millisecond in each thread, if they 
        // are created within the same millisecond then they will generate
        // the same list of random numbers.  This class nicely takes care of that.
        // It was found here:
        //        http://stackoverflow.com/questions/1785744/how-do-i-seed-a-random-class-to-avoid-getting-duplicate-random-values
        public static class RandomHelper
        {
            private static int seedCounter = new Random(DateTime.Now.Millisecond).Next();

            [ThreadStatic]
            private static Random rng;

            public static Random Instance
            {
                get
                {
                    if (rng == null)
                    {
                        int seed = Interlocked.Increment(ref seedCounter);
                        rng = new Random(seed);
                    }
                    return rng;
                }
            }
        }
    }
}

