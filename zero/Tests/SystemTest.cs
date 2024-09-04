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
        // Arrange           
        private TXmlConnector tXmlConnector = new TXmlConnector();

        [Fact]
        public void SistemResultEqualValue()
        {                        
            
            // Act            
            tXmlConnector.RdBazaNew.TransaqString = new Cash();
            tXmlConnector.RdBazaNew.TransaqString.Status = "вход";
            tXmlConnector.RdBazaNew.TransaqString.Buysell = "B";
            tXmlConnector.RdBazaNew.TransaqString.Вход = 123.0;
            tXmlConnector.RdBazaNew.TransaqString.Quantity = 1;            

            tXmlConnector.QuotationsNew.Bid = 125.0;
            tXmlConnector.QuotationsNew.Offer = 121.0;
            tXmlConnector.QuotationsNew.Low = 121.0;
            tXmlConnector.QuotationsNew.High = 125.0;
            tXmlConnector.QuotationsNew.Last = 123.0;

            RdBaza.Mode = "тест";

            tXmlConnector.RdBazaNew.Orders(DateTime.Now.TimeOfDay.ToString());            
            /*rdBaza.ServOrder(DateTime.Now.TimeOfDay.ToString(), rdBaza.TransaqString.Вход,
                rdBaza.TransaqString.Status, 1);*/

            // Assert
            Assert.Equal("active", tXmlConnector.RdBazaNew.TransaqString.Status);
        }

        [Fact]
        public void CalculateResultEqualValue()
        {
            double delta = tXmlConnector.RdBazaNew.Calculate(DateTime.Now.TimeOfDay.ToString(), "ROSN", "B", 123.0, 
                tXmlConnector.RdBazaNew.TransaqString.Orderno, 1, "д");

            Assert.Equal(1, delta);
        }
    }
}
