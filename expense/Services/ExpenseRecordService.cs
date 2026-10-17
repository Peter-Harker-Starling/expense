using Google.GenAI;
using Google.GenAI.Types;
using expense.Models;

namespace expense.Services
{
    public class ExpenseRecordService
    {
        private readonly Client _client;

        public ExpenseRecordService(IConfiguration configuration)
        {
            var apiKey = configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("找不到給咪咪的 APIKey");
            this._client = new Client(apiKey: apiKey);
        }

        public async Task<ExpenseRecord> AnalyzeExpenseRecord(string input)
        {
            var prompt = $@"
            今天是 {DateTime.Now:yyyy-MM-dd}。
            請解析這句話：""{input}""
            並只輸出這個 JSON 格式：{{ ""Item"": ""品名"", ""Date"": ""YYYY-MM-DD"", ""Amount"": 數字, ""Category"": ""類別"" }}
            
            Category 的類別有限制，請從以下選項中選擇：餐飲、交通、娛樂、生活。
            禁止輸出 Markdown 格式。
            ";

            var response = await _client.Models.GenerateContentAsync(
                model: "gemini-3.1-flash-lite",
                contents: prompt
            );

            var data = System.Text.Json.JsonSerializer.Deserialize<ExpenseRecord>(response.Text!);

            return data!;
        }
    }
}
