using Microsoft.AspNetCore.Mvc;
using ServiceIntegrationTest.Services;

namespace ServiceIntegrationTest.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly TransientTestServive _transientService;

    public WeatherForecastController(TransientTestServive transientService)
    {
        _transientService = transientService;
    }

    [HttpGet("GetMessage")]
    public string Get() => _transientService.GetMessage();
}