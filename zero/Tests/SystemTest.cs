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
        public void SistemResultEqualValue()
        {
            // Arrange
            //HomeController controller = new HomeController();
            TXmlConnector xmlConnector = new TXmlConnector();

            //RdBaza rdBaza = new RdBaza(xmlConnector);
            // Act
            //ViewResult result = controller.Index() as ViewResult;
            xmlConnector.RdBazaNew.TransaqString = new Cash();
            xmlConnector.RdBazaNew.TransaqString.Status = "вход";
            xmlConnector.RdBazaNew.TransaqString.Buysell = "B";
            xmlConnector.RdBazaNew.TransaqString.Вход = 123.0;
            xmlConnector.RdBazaNew.TransaqString.Quantity = 1;            

            xmlConnector.QuotationsNew.Bid = 125.0;
            xmlConnector.QuotationsNew.Offer = 121.0;
            xmlConnector.QuotationsNew.Low = 121.0;
            xmlConnector.QuotationsNew.High = 125.0;
            xmlConnector.QuotationsNew.Last = 123.0;

            RdBaza.Mode = "тест";

            xmlConnector.RdBazaNew.Orders(DateTime.Now.TimeOfDay.ToString());            
            /*rdBaza.ServOrder(DateTime.Now.TimeOfDay.ToString(), rdBaza.TransaqString.Вход,
                rdBaza.TransaqString.Status, 1);*/

            // Assert
            Assert.Equal("active", xmlConnector.RdBazaNew.TransaqString.Status);
        }
    }
}
