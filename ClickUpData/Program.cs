using ClickUpData.Models;
using System;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace ClickUpData
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var csvPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tasks.csv");
            var apiKey = Environment.GetEnvironmentVariable("CLICKUP_API_KEY", EnvironmentVariableTarget.User);

            var client = new HttpClient();

            client.BaseAddress = new Uri("https://api.clickup.com/api/v2/");

            client.DefaultRequestHeaders.Add("Authorization", apiKey);

            var teamsCall = await client.GetAsync("team");

            var teamsContent = await teamsCall.Content.ReadAsStringAsync();

            var teams = JsonSerializer.Deserialize<Teams>(teamsContent);

            var team = teams.TeamsList.First();

            var spacesCall = await client.GetAsync($"team/{team.Id}/space");

            var spacesContent = await spacesCall.Content.ReadAsStringAsync();

            var spaces = JsonSerializer.Deserialize<Spaces>(spacesContent);

            var space = spaces.SpacesList.Where(s => s.Name.Contains("Jon")).First();

            var foldersCall = await client.GetAsync($"space/{space.Id}/folder");

            var foldersContent = await foldersCall.Content.ReadAsStringAsync();

            var folders = JsonSerializer.Deserialize<Folders>(foldersContent);

            var folder = folders.FoldersList.Where(f => f.Name.Contains("Personal")).First();

            var list = folder.Lists.Where(l => l.Name == "YouTube").First();

            var tasksCall = await client.GetAsync($"list/{list.Id}/task?archived=false&include_closed=true");

            var taskContent = await tasksCall.Content.ReadAsStringAsync();

            var tasks = JsonSerializer.Deserialize<Tasks>(taskContent);

            var csvData = tasks.TasksList.Select(t => new CsvData
            {
                TaskName = t.Name,
                Tags = String.Join(",", t.Tags.Select(t => t.Name).FirstOrDefault())
            });

            using (var writer = new StreamWriter(csvPath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(csvData);
            }
        }
    }
}
