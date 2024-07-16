# Overview
Simple Console app demonstrating how to use the Code Interpreter tool to create files using the Azure OpenAI Assistants client library for .NET.

# Settings

You can adjust the  file, the file type, and prompt.

```json
{
   "AZURE_OPENAI_ENDPOINT": "",
   "AZURE_OPENAI_API_KEY": "",
   "AZURE_OPENAI_DEPLOYED_MODEL": "gpt-4o",
   "FilePath": "C:\\temp\\Test.PNG",
   "AZURE_OPENAI_INSTRUCTIONS": "You are an AI assistant. You will create files of my choosing.",
   "AZURE_OPENAI_ASSISTANT_NAME": "File Creator",
   "AZURE_OPENAI_PROMPT": "Create a PIE chart with the following data: 1,2,3,4,5 to a PNG file. Use blue, green, and purple as colors."
}
```
## Reference Documenation

[Azure OpenAI Assistants](https://learn.microsoft.com/en-us/azure/ai-services/openai/assistants-quickstart?tabs=command-line%2Ctypescript&pivots=programming-language-studio)
