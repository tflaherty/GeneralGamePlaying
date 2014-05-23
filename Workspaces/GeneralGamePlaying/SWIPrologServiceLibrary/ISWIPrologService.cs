using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SbsSW.SwiPlCs;

namespace API.SWIProlog.SWIPrologServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ISWIPrologService
    {
        [OperationContract]
        // this prepends "assert" to the clause
        bool Assert(string clause);

        [OperationContract]
        // this does not prepend "assert" to the clause
        bool ExecuteClause(string clause);

        [OperationContract]
        List<List<SWIPrologServiceLibrary.SolutionVariable>> GetSolutionVariables(string query);

        //[OperationContract]
        //List<PlQueryVariables> GetSolutionVariables(string query);

        //[OperationContract]
        //List<PlTermV> GetSolutions(string query);
    }

    [DataContract]
    public class SolutionVariable
    {
        [DataMember]
        public string Variable { get; set; }

        [DataMember]
        public string Value { get; set; }
    }

/*
    // Use a data contract as illustrated in the sample below to add composite types to service operations
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
 */
}
