using GeminiApp.Business;
using GeminiApp.Data;
using GeminiApp.Data.Models;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GeminiApp.Presentation
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // 🔧 Load Configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            string apiUrl = config["Gemini:ApiUrl"];
            string apiKey = config["Gemini:ApiKey"];

            var chatService = new ChatService(apiUrl, apiKey);
            var promptTemplate = new PromptTemplate(config);
            var retriever = new Retriever();
            Console.WriteLine(Directory.GetCurrentDirectory());
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "GeminiDocs");

            if (Directory.Exists(folderPath))
            {
                Console.WriteLine($"Loading text files from: {folderPath}");

                string[] textFiles = Directory.GetFiles(folderPath, "*.txt");

                foreach (var file in textFiles)
                {
                    string fileContent = await File.ReadAllTextAsync(file);
                    retriever.AddDocument(fileContent);
                    Console.WriteLine($"Added file: {Path.GetFileName(file)}");
                }

                if (textFiles.Length == 0)
                {
                    Console.WriteLine("No text files found in the folder.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Folder not found. Please check the path and try again.");
                return;
            }

            // 📝 User Input
            Console.WriteLine($"\r\nThe articles about France in directory {folderPath} are written in English." +
                "\r\nPlease provide me with the title of the article that interests you, " +
                "and I will translate it into German using Hugging Face.");

            string userInput = Console.ReadLine();
            // Sanitize the user input to make it safe for use in file names
            string sanitizedInput = SanitizeFileName(userInput);

            // Ensure the file name ends with ".txt"
            string fileName = sanitizedInput + ".txt";

            string filePath = Path.Combine(folderPath, fileName);

            // Check if the file exists
            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);
                
                string retrievedDocument = retriever.RetrieveRelevantDocument(fileContent);

                var question = new Question<string>(retrievedDocument);

                try
                {
                    var answer = await chatService.AskQuestionAsync(question);
                    string finalPrompt = promptTemplate.BuildPrompt(question.Content, answer.Content);
                    Console.WriteLine($"{finalPrompt}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"The file '{userInput}' does not exist in the folder '{folderPath}'.");
            }
        }
        static string SanitizeFileName(string input)
        {
            // List of characters not allowed in file names
            char[] invalidChars = Path.GetInvalidFileNameChars();

            foreach (char c in invalidChars)
            {
                input = input.Replace(c.ToString(), "_"); // Replace invalid characters with underscores
            }

            return input;
        }
    }
}
