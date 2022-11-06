using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WikiExtractor.Process
{
    public class WikiMasterTablePageExtractor
    {
        //public string WikiPageRouteResponse(string route)
        //{
        //    var cache = GetFromCache(route);
        //    if (cache.IsEmpty())
        //    {
        //        var resp = new TestApiHttp()
        //           .SetEnvironment("https://en.wikipedia.org")
        //           .PrepareRequest(route)
        //           .GetWithRetry(
        //               assertOk: true,
        //               timeToSleepBetweenRetryInMilliseconds: 1000,
        //               retryOption: 10,
        //               throwExceptionOnAssertFail: true,
        //               retryOnRequestTimeout: true,
        //               httpStatusCodes: new[] { HttpStatusCode.ProxyAuthenticationRequired }
        //           );

        //        resp.AssertResponseStatusForSuccess();
        //        ToCache(route, resp.ResponseBody.ContentString);
        //        cache = resp.ResponseBody.ContentString;
        //    }
        //    return cache;
        //}
    }
}
