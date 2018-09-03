using Lx.Common.Models.Var;
using Lx.Service.WcfHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lx.Service.UnitTests
{
    [TestClass]
    public class OrderServiceTests
    {
        [TestMethod]
        public void TestPayOrderMq()
        {

            PayOrderMqHelper.SendPayOrderMessageToMq(new OrderMqMessage
            {
                FAmount = 100,
                FCardNo = "123456",
                FHolders = "mi",
                FMerchantCode = "100003",
                FMerchantOrderNumber = DateTime.Now.ToString("yyyyHHddhhmmss")
            });

        }

        [TestMethod]
        public void GetPayTautoTransferInfo()
        {
            var result = OrderServiceHelper.GetTautoTransferInfoByMerchantCode("1002", new Common.Models.Para.PageInfoParam { PageIndex = 0, PageSize = 10 });
            Assert.IsTrue(result.Result);
            Assert.IsNotNull(result.ResultData);
        }

    }
}
