# unity-future-virtual-AI-university

Microsoft Hackathon 2023.

## Development

First, you need to create .env file.
Run following command.

```powershell
cp .env.example .env
```

Then, update each variable for your Azure resource.

| Variables | Description |
| --- | --- |
| `SPEECH_KEY` | Key at Overview in Azure Speech service |
| `SPEECH_REGION` | Region at Overview in Azure Speech service |
| `AZURE_API_KEY` | Key at Develop tab for Azure OpenAI resource in Azure |
| `AZURE_INSTANCE_NAME` | Azure OpenAI resource name in Azure |
| `AZURE_DEPLOYMENT_NAME` | Deployment name in Azure OpenAI Studio |
| `AZURE_API_VERSION`| Azure OpenAI API version. Normally, `2023-05-15` |
| `GPT_VERSION` | Model name of deployment in Azure OpenAI Studio. Normally, `gpt-35-turbo` or `gpt-35-turbo-16k` |
