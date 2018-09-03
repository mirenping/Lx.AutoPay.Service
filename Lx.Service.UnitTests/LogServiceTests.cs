using Lx.Common.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Service.UnitTests
{

    [TestClass]
    public class LogServiceTests
    {
        [TestMethod]
        public void LogServiceMessage()
        {
            SysLogHelper.LogServiceMessage("test01", "sdfsdf01");

           
        }

        [TestMethod]
        public void LogMessage()
        {
            SysLogHelper.LogMessage("LogHost.Main", "hello mirenping");
        }
    }
}
