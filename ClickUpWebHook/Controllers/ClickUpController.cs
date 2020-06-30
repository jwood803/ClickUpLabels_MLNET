﻿using ClickUpWebHook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClickUpWebHook.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClickUpController : ControllerBase
    {
        private readonly ILogger<ClickUpController> _logger;
        private readonly PredictionEnginePool<TaskInput, TaskOutput> _predictionEnginePool;
        private readonly HttpClient _client;

        public ClickUpController(ILogger<ClickUpController> logger, PredictionEnginePool<TaskInput, TaskOutput> predictionEnginePool)
        {
            _logger = logger;
            _predictionEnginePool = predictionEnginePool;
            _client = new HttpClient();

            var apiKey = Environment.GetEnvironmentVariable("CLICKUP_API_KEY");

            _client.BaseAddress = new Uri("https://api.clickup.com/api/v2/task/");

            _client.DefaultRequestHeaders.Add("Authorization", apiKey);
        }

        [HttpPost]
        public async Task Post([FromBody] WebHookResponse body)
        {
            if (String.IsNullOrWhiteSpace(body.TaskId))
            {
                return;
            }

            var taskCall = await _client.GetAsync($"{body.TaskId}");

            var taskContent = await taskCall.Content.ReadAsStringAsync();

            var task = JsonSerializer.Deserialize<ClickUpTask>(taskContent);

            var prediction = _predictionEnginePool.Predict(new TaskInput { TaskName = task.Name });

            var response = await _client.PostAsync($"{body.TaskId}/tag/{prediction.PredictedLabel}", new StringContent(""));

            var responseContent = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Update on task ID {body.TaskId} with tag {prediction.PredictedLabel}. {responseContent}");
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("Working!");

            return "Working";
        }
    }
}
