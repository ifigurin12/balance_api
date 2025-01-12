using balanceSimple.Controllers;
using balanceSimple.Models;
using balanceSimple.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Web.Http.Results;

namespace balanceSimple.Tests
{
    public class ServiceTests
    {
        [Fact]
        public void negative_service_throw_exc_when_ub_less_then_lb()
        {

            var mock = new Mock<ILogger<BalanceController>>();
            ILogger<BalanceController> loggerBC = mock.Object;
            var servicemock = new Mock<ILogger<CalculatorService>>();
            ILogger<CalculatorService> loggerCS = servicemock.Object;

            BalanceInput balanceInput = new BalanceInput();

            var flows = new List<Flow>()
            {
               new Flow {Id = 0, Name = "x1", Value = 10.005, Tols = 0.200, IsUsed = true, LowerBound = 100, UpperBound = 10, SourceNode = -1, DestNode = 1 }
            };

            balanceInput.Id = 0;
            balanceInput.flows = flows;

            ICalculatorService calculatorService = new CalculatorService(loggerCS);
            var conroller = new BalanceController(calculatorService, loggerBC);

            var response = conroller.balanceCalculate(balanceInput);
            Assert.Equal(400, (response as ObjectResult)?.StatusCode);
        }

        [Fact]
        public void negative_service_throw_exc_when_flow_is_missing()
        {
            var mock = new Mock<ILogger<BalanceController>>();
            ILogger<BalanceController> loggerBC = mock.Object;
            var servicemock = new Mock<ILogger<CalculatorService>>();
            ILogger<CalculatorService> loggerCS = servicemock.Object;

            BalanceInput balanceInput = new BalanceInput();

            var flows = new List<Flow>()
            {
               new Flow {Id = 0, Name = "x1", Value = 10.005, Tols = 0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = -1, DestNode = 1 },
               new Flow {Id = 1, Name = "x2", Value = 10.005, Tols = 0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = 1, DestNode = 2 },
               new Flow {Id = 3, Name = "x3", Value = 10.005, Tols = 0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = 2, DestNode = -1 }
            };

            balanceInput.Id = 0;
            balanceInput.flows = flows;

            ICalculatorService calculatorService = new CalculatorService(loggerCS);
            var conroller = new BalanceController(calculatorService, loggerBC);

            var response = conroller.balanceCalculate(balanceInput);
            Assert.Equal(400, (response as ObjectResult)?.StatusCode);
        }

        [Fact]
        public void negative_service_throw_exc_with_incorrect_value()
        {
            var mock = new Mock<ILogger<BalanceController>>();
            ILogger<BalanceController> loggerBC = mock.Object;
            var servicemock = new Mock<ILogger<CalculatorService>>();
            ILogger<CalculatorService> loggerCS = servicemock.Object;

            BalanceInput balanceInput = new BalanceInput();

            var flows = new List<Flow>()
            {
               new Flow {Id = 0, Name = "x1", Value = 10.005, Tols = 0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = -1, DestNode = 1 },
               new Flow {Id = 1, Name = "x2", Value = -1.004, Tols = 0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = 1, DestNode = 2 },
               new Flow {Id = 2, Name = "x3", Value = 10.005, Tols = 0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = 2, DestNode = -1 }
            };

            balanceInput.Id = 0;
            balanceInput.flows = flows;

            ICalculatorService calculatorService = new CalculatorService(loggerCS);
            var conroller = new BalanceController(calculatorService, loggerBC);

            var response = conroller.balanceCalculate(balanceInput);
            Assert.Equal(400, (response as ObjectResult)?.StatusCode);
        }

        [Fact]
        public void negative_service_throw_exc_with_incorrect_tols()
        {
            var mock = new Mock<ILogger<BalanceController>>();
            ILogger<BalanceController> loggerBC = mock.Object;
            var servicemock = new Mock<ILogger<CalculatorService>>();
            ILogger<CalculatorService> loggerCS = servicemock.Object;

            BalanceInput balanceInput = new BalanceInput();

            var flows = new List<Flow>()
            {
               new Flow {Id = 0, Name = "x1", Value = 10.005, Tols = 0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = -1, DestNode = 1 },
               new Flow {Id = 1, Name = "x2", Value = 10.005, Tols = -0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = 1, DestNode = 2 },
               new Flow {Id = 2, Name = "x3", Value = 10.005, Tols = 0.200, IsUsed = true, LowerBound = 0, UpperBound = 1000, SourceNode = 2, DestNode = -1 }
            };

            balanceInput.Id = 0;
            balanceInput.flows = flows;

            ICalculatorService calculatorService = new CalculatorService(loggerCS);
            var conroller = new BalanceController(calculatorService, loggerBC);

            var response = conroller.balanceCalculate(balanceInput);
            Assert.Equal(400, (response as ObjectResult)?.StatusCode);
        }
    }
}