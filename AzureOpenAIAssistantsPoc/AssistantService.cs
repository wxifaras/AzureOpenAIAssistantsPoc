using Azure;
using Azure.AI.OpenAI.Assistants;
using Microsoft.Extensions.Configuration;

namespace AssistantsSdkPoc
{

    public class AssistantService
    {
        private string _endPoint;
        private string _key;
        private string _deployedModel;
        private string _instructions;
        private string _assistantName;
        private string _prompt;
        private AssistantsClient _assistantsClient;

        public AssistantService(IConfiguration config)
        {
            _endPoint = config["AZURE_OPENAI_ENDPOINT"] ?? throw new ArgumentNullException("AZURE_OPENAI_ENDPOINT");
            _key = config["AZURE_OPENAI_API_KEY"] ?? throw new ArgumentNullException("AZURE_OPENAI_API_KEY");
            _deployedModel = config["AZURE_OPENAI_DEPLOYED_MODEL"] ?? throw new ArgumentNullException("AZURE_OPENAI_DEPLOYED_MODEL");
            _instructions = config["AZURE_OPENAI_INSTRUCTIONS"] ?? throw new ArgumentNullException("AZURE_OPENAI_INSTRUCTIONS");
            _assistantName = config["AZURE_OPENAI_ASSISTANT_NAME"] ?? throw new ArgumentNullException("AZURE_OPENAI_ASSISTANT_NAME");
            _prompt = config["AZURE_OPENAI_PROMPT"] ?? throw new ArgumentNullException("AZURE_OPENAI_PROMPT");

            _assistantsClient = new AssistantsClient(new Uri(_endPoint), new AzureKeyCredential(_key));
        }

        public async Task<(string assistantId, byte[] fileBytes)> RunAssistantAsync()
        {
            Assistant assistant = await _assistantsClient.CreateAssistantAsync(

            new AssistantCreationOptions(_deployedModel)
            {
                Name = _assistantName,
                Instructions = _instructions,
                Tools = { new CodeInterpreterToolDefinition() },
            });

            AssistantThread thread = await _assistantsClient.CreateThreadAsync();

            while (true)
            {
                Console.WriteLine($"User > {_prompt}");

                // Add a user question to the thread
                ThreadMessage message = await _assistantsClient.CreateMessageAsync(
                    thread.Id,
                    MessageRole.User,
                    _prompt);

                // Run the thread
                ThreadRun run = await _assistantsClient.CreateRunAsync(
                    thread.Id,
                    new CreateRunOptions(assistant.Id)
                );

                // Wait for the assistant to respond
                do
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                    run = await _assistantsClient.GetRunAsync(thread.Id, run.Id);
                }

                while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress);

                if (run.Status == RunStatus.Completed)
                {
                    // Get the messages
                    PageableList<ThreadMessage> messagesPage = await _assistantsClient.GetMessagesAsync(thread.Id);
                    IReadOnlyList<ThreadMessage> messages = messagesPage.Data;

                    var ts = messagesPage.Data.FirstOrDefault(x => x.FileIds.Count > 0);
                    var fileId = ts.FileIds.FirstOrDefault();
                    Response<BinaryData> content = await _assistantsClient.GetFileContentAsync(fileId);

                    // Convert the binary data to a byte array
                    byte[] data = content.Value.ToArray();
                    return (assistant.Id, data);
                }
            }
        }

        public async Task DeleteAssistantAsync(string assistantId)
        {
            await _assistantsClient.DeleteAssistantAsync(assistantId);
        }
    }
}