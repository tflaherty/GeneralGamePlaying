using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using API.GGP.GeneralGameManagerNS;
using API.Utilities.TFTree;

namespace GeneralGameManagerRestService
{
    public class GeneralGameManagerSingleton
    {
        private static GeneralGameManagerSingleton instance;

        private GeneralGameManager generalGameManager;

        private GeneralGameManagerSingleton()
        {
            //generalGameManager = new GeneralGameManager();
        }

        public static GeneralGameManagerSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GeneralGameManagerSingleton();
                }
                return instance;
            }
        }
    }


    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GeneralGameManagerRestServiceImpl" in code, svc and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GeneralGameManagerRestServiceImpl : IGeneralGameManagerRestServiceImpl
    {
        public GeneralGameManagerRestServiceImpl()
        {
            return;
        }

        public string JSONData(string id)
        {
            return "You requested product " + id;
        }

        public string Init(string kifContents, int startClock, int playClock)
        {
            TFTree<String> foo = new TFTree<String>();
            return kifContents + " (" + startClock + ", " + playClock + ")";
        }
    }
}
