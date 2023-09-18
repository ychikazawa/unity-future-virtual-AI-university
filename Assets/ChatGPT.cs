using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.IO;


public class ChatGPT : MonoBehaviour
{
    static string azureApiKey = EnvManager.Get("AZURE_API_KEY");
    static string instanceName = EnvManager.Get("AZURE_INSTANCE_NAME");
    static string deploymentName = EnvManager.Get("AZURE_DEPLOYMENT_NAME");
    static string apiVersion = EnvManager.Get("AZURE_API_VERSION");
    static string versionGPTOpenAI = EnvManager.Get("GPT_VERSION");
    string url;
    string jsonBody;


    // parameters for GPT
    [SerializeField] public int maxTokens = 80;
    [SerializeField] public int memorablePairs = 0;
    List<Dictionary<string, string>> previousMessage;


    // Start is called before the first frame update
    async void Start()
    {
        url = $"https://{instanceName}.openai.azure.com/openai/deployments/{deploymentName}/chat/completions?api-version={apiVersion}";

        previousMessage = new List<Dictionary<string, string>>();
        try
        {
            string[] SettingTextForPreprocessing;
            string SettingText;
            // get the setting text from the .txt file
            using (StreamReader reader = new StreamReader("./AgentSettings.txt", Encoding.UTF8))
            {
                SettingTextForPreprocessing = reader.ReadToEnd().Split('\n');
            }
            SettingText = string.Join("", SettingTextForPreprocessing);

            previousMessage.Add(new Dictionary<string, string>  // add the agent's role to jsonBody
            {
                {"role", "system"},
                {"content", SettingText}
            });
        }
        catch
        {
            Debug.Log("Failed to read the setting text...");
        }

        await GPTCompletion("Hello, my name is John. ");
    }


        // connect API and get the response -----------------------------------------------------------------
    public async Task<JObject> ConnectToOpenAPI(string url, string jsonBody)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("api-key", $"{azureApiKey}");
        
        JObject response = await HTTPRequest.Link(url, headers, jsonBody);

        return response;
    }

    // prepare the input json body for GPT -------------------------------------------------------------
    public async Task<string> GPTCompletion(string text) 
    {
        previousMessage.Add(new Dictionary<string, string>   // add the user's text to jsonBody
        {
            {"role", "user"},
            {"content", text}
        });

        var stop = new List<string> {"]","。"};    // for Japanese(optional)

        var request = new       // add parameters to jsonBody
        {
            model = versionGPTOpenAI,
            max_tokens = maxTokens,
            stop = stop,
            messages = previousMessage
        };

        jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(request);

        var result = await ConnectToOpenAPI(url, jsonBody);                     // get response from GPT
        var resText = result["choices"][0]["message"]["content"].ToString();   // get the text from the response
        Debug.Log(resText);
        // if(memorablePairs != 0)     // if memorize the past(history) messages
        // {
        //     Dictionary<string, string> ResponseMessage = new Dictionary<string, string>
        //     {
        //         {"role", "assistant"},
        //         {"content", textGPT.text}
        //     };

        //     previousMessage.Add(ResponseMessage);

        //     if (previousMessage.Count >= 1 + memorablePairs * 2)     // if the number of previous messages is over the limit
        //     {
        //         previousMessage.RemoveAt(1);    //remove the oldest message (user text)
        //         previousMessage.RemoveAt(1);    //remove the oldest message (agent text)
        //     }
        // }else{
        //     previousMessage.RemoveAt(1);        //remove the message (user text)
        // }

        return resText;
    }
}


