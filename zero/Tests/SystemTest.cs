using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using zero.Data;
using zero.Structure;

namespace zero.Tests
{
    public class SystemTest
    {
        [Fact]
        public void SistemResultNotNull()
        {
            // Arrange
            //HomeController controller = new HomeController();
            TXmlConnector xmlConnector = new TXmlConnector();
            RdBaza rdBaza = new RdBaza(xmlConnector);
            // Act
            //ViewResult result = controller.Index() as ViewResult;
            rdBaza.TransaqString = new Cash();
            rdBaza.TransaqString.Status = "вход";
            rdBaza.TransaqString.Buysell = "B";
            rdBaza.TransaqString.Вход = 123.0;

            //rdBaza.Orders(DateTime.Now.TimeOfDay.ToString());            
            string status = rdBaza.ServOrder(DateTime.Now.TimeOfDay.ToString(), rdBaza.TransaqString.Вход,
                rdBaza.TransaqString.Status, 1);

            // Assert
            Assert.Equal("вход", status);
        }
    }
}
