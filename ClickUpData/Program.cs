using ClickUpData.Models;
using System;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClickUpData
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var apiKey = Environment.GetEnvironmentVariable("CLICKUP_API_KEY", EnvironmentVariableTarget.User);

            var client = new HttpClient();

            client.BaseAddress = new Uri("https://api.clickup.com/api/v2/");

            client.DefaultRequestHeaders.Add("Authorization", apiKey);

            var teamsCall = await client.GetAsync("team");

            var teamsContent = await teamsCall.Content.ReadAsStringAsync();

            var teams = JsonSerializer.Deserialize<Teams>(teamsContent);

            var team = teams.TeamsList.First();

            var spacesCall = await client.GetAsync($"team/{team.Id}/space");

            var spacesContent = spacesCall.Content.ReadAsStringAsync();

            var spaces = JsonSerializer
        }
    }
}
