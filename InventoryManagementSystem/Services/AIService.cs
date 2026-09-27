using Google.GenAI;

namespace InventoryManagementSystem.Services
{
    public class AIService
    {
        private readonly Client _client;

        public AIService(IConfiguration configuration)
        {
            var apiKey = configuration["Gemini:ApiKey"];

            _client = new Client(apiKey: apiKey);
        }

        public async Task<string> AskAsync(
            string question,
            string inventoryData)
        {
            string prompt = $"""
                You are an AI Inventory Assistant for an Inventory Management System.

                Answer the user's question using ONLY the inventory data provided below.

                Rules:
                - Do not invent products or quantities.
                - If the requested information is not available, say so.
                - Keep the answer clear and concise.
                - You can analyze products, quantities, prices and categories.

                INVENTORY DATA:
                {inventoryData}

                USER QUESTION:
                {question}
                """;

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-flash-latest",
                contents: prompt
            );

            return response.Candidates[0]
                .Content
                .Parts[0]
                .Text;
        }
    }
}