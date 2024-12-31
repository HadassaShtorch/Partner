using Microsoft.Extensions.Configuration;

namespace GeminiApp.Business
{
    public class PromptTemplate
    {
        private readonly string _template;

        public PromptTemplate(IConfiguration config)
        {
            _template = config["PromptTemplate"] ?? "Context: {0}\nUser Question: {1}\nInstructions: Provide a concise and accurate response.";
        }

        public string BuildPrompt(string userQuestion, string userAnswer)
        {
            return $"User Article: {userQuestion}\nHugging Face Translation Article: {userAnswer}";
            //return string.IsNullOrEmpty(userQuestion)
            //    ? $"User Article: {userQuestion}"
            //    : $"\nHugging Face Translation Article:: {userAnswer}";
        }


    }
}