using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Roar.DependencyInjection.Abstractions;

namespace GrpcServiceIntegrationTest.Services;

public class WeatherService : WeatherContract.WeatherContractBase, IGrpcService
{
    public override async Task<GetWeatherReply> GetWeathersAsync(GetWeatherRequest request, ServerCallContext context)
    {
        var summaries = new List<string>() { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        var weatherForecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = Timestamp.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = summaries[Random.Shared.Next(summaries.Count)]
        });

        return new GetWeatherReply()
        {
            WeatherForecasts = { weatherForecasts }
        };
    }
}
