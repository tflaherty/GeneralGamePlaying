using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Services;

namespace GeneralGameManagerRestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IGeneralGameManagerRestServiceImpl" in both code and config file together.
    [ServiceContract]
    public interface IGeneralGameManagerRestServiceImpl
    {
        [OperationContract]

        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "json/{id}")]
        string JSONData(string id);

        // how to call this:  http://localhost:52552/GeneralGameManagerRestServiceImpl.svc/json?kifContents=foobar&startClock=5&playClock=1
        [WebInvoke(Method = "POST",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "json?kifContents={kifContents}&startClock={startClock}&playClock={playClock}")]
        string Init(string kifContents, int startClock, int playClock);

    }
}
