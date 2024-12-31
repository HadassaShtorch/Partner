using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GeminiApp.Data.Models;

namespace GeminiApp.Business
{
    public class ChatService
    {
        private readonly string _apiUrl;
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public ChatService(string apiUrl, string apiKey)
        {
            _apiUrl = apiUrl;
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }

        public async Task<Answer<string>> AskQuestionAsync(Question<string> question)
        {
            // ✅ Build the payload for the chat API
            var payload = new
            {
                inputs = question.Content
            };

            // ✅ Serialize the payload to JSON
            var jsonPayload = JsonConvert.SerializeObject(payload);

            // ✅ Send the POST request to the Gemini API
            var response = await _httpClient.PostAsync(
                _apiUrl,
                new StringContent(jsonPayload, Encoding.UTF8, "application/json")
            );

            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Failed to parse the response from Hugging Face API. Status: {response.StatusCode}");
            
            return new Answer<string>(responseString);
        }
    }
}
