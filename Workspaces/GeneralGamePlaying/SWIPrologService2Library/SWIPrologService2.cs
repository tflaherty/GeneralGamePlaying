using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SbsSW.SwiPlCs;
using API.SWIProlog.SWIPrologServiceLibrary;


namespace API.SWIProlog.SWIPrologService2Library
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(UseSynchronizationContext = false)]
    public class SWIPrologService2 : ISWIPrologService
    {
        public SWIPrologService2()
        {
            if (!PlEngine.IsInitialized)
            {
                String[] param = { "-q" }; // suppressing informational and banner messages
                PlEngine.Initialize(param);
            }
        }

        public bool Assert(string clause)
        {
            PlEngine.PlThreadAttachEngine();
            return PlQuery.PlCall("assert(" + clause + ")");
        }

        public bool ExecuteClause(string clause)
        {
            PlEngine.PlThreadAttachEngine();
            return PlQuery.PlCall(clause);
        }

        public List<List<SWIPrologServiceLibrary.SolutionVariable>> GetSolutionVariables(string query)
        {
            PlEngine.PlThreadAttachEngine();

            var outerList = new List<List<SolutionVariable>>();

            using (PlQuery q = new PlQuery(query))
            {
                foreach (PlQueryVariables plQueryVariable in q.SolutionVariables)
                {
                    var innerList = new List<SolutionVariable>();

                    foreach (string variableName in q.VariableNames)
                    {
                        innerList.Add(new SolutionVariable() { Variable = variableName, Value = plQueryVariable[variableName].ToString() });
                    }

                    outerList.Add(innerList);
                }
            }

            return outerList;
        }

        public List<PlTermV> GetSolutions(string query)
        {
            var list = new List<PlTermV>();

            using (PlQuery q = new PlQuery(query))
            {
                foreach (PlTermV solution in q.Solutions)
                {
                    list.Add(solution);
                }
            }

            return list;
        }

    }
}
