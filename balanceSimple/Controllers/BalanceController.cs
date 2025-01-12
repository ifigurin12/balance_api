using balanceSimple.Models;
using balanceSimple.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;

namespace balanceSimple.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private ICalculatorService _calculatorService;

        public BalanceController(ICalculatorService calcServ)
        {
            _calculatorService = calcServ;

        }

        [EnableCors]
        [HttpPost]
        public ActionResult balanceCalculate(BalanceInput inputFlows)
        {
            try
            {
                var resultData = _calculatorService.Calculate(inputFlows);
                return Ok(resultData);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}