using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lx.Service.WcfService;

namespace Lx.Service.UnitTests
{
    [TestClass]
    public class UserServiceTests
    {


        UserService userService = new UserService();
        [TestMethod]
        public void GetUserData()
        {

            var result = userService.GetUserData(1);
            Assert.IsTrue(result.Result);
            Assert.IsNotNull(result.ResultData);
        }

    }
}
