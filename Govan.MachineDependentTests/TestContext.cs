namespace Govan.MachineDependentTests
{
    using System.Net;

    static public class TestContext
    {
        public static NetworkCredential LocalAdminCredential
        {
            get { return new NetworkCredential("TestUser", "!+Pa55word"); }
        }
    }
}