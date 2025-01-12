using balanceSimple.Models;
using balanceSimple.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace balanceSimple.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private ICalculatorService _calculatorService;
        private readonly ILogger<BalanceController> _logger;

        public BalanceController(ICalculatorService calcServ, ILogger<BalanceController> logger)
        {
            _calculatorService = calcServ;
            _logger = logger;
        }

        [EnableCors]
        [HttpPost]
        public ActionResult balanceCalculate(BalanceInput inputFlows)
        {
            _logger.LogInformation("Начат расчёт материального баланса для входных данных: {@InputFlows}", inputFlows);
            try
            {
                var resultData = _calculatorService.Calculate(inputFlows);
                _logger.LogInformation("balance controller works success");
                _logger.LogInformation("Расчёт материального баланса успешно завершён");
                return Ok(resultData);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при расчёте материального баланса. Входные данные: {@InputFlows}", inputFlows);
                return BadRequest("Произошла ошибка при расчёте материального баланса. Проверьте входные данные.");
            }
        }
    }
}