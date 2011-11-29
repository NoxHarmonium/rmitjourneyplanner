using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
namespace UnitTests
{
    [TestClass]
    public static class InitTestEnv
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            RmitJourneyPlanner.CoreLibraries.DataAccess.ConnectionInfo.Proxy =
                new System.Net.WebProxy("http://aproxy.rmit.edu.au:8080", false, null, new NetworkCredential("s3229159", "MuchosRowlies1"));
        }
    }
}