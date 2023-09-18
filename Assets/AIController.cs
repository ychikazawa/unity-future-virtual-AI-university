using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class AIController : MonoBehaviour
{
    protected SpeechToText speechToText;
    protected TextToSpeech textToSpeech;
    protected ChatGPT chatGPT;

    void Awake()
    {
        speechToText = GetComponent<SpeechToText>();
        textToSpeech = GetComponent<TextToSpeech>();
        chatGPT = GetComponent<ChatGPT>();

        // Subscribe to the speech recognizer events
        speechToText.OnRecognized += (recognizedText => OnSpeak(recognizedText));
    }

    // Start is called before the first frame update
    async void Start()
    {
        await textToSpeech.Speech("こんにちは。");
        Listen();
    }

    void Listen()
    {
        Debug.Log("Listening...");
        speechToText.OnStart();
    }

    void OnSpeak(string recognizedText)
    {
        Debug.Log("Speaking...");
        Task.Run(() => Speak(recognizedText));
    }

    public async Task Speak(string recognizedText)
    {
        speechToText.OnStop();
        Debug.Log("Create response...");
        string response = await chatGPT.GPTCompletion(recognizedText);
        Debug.Log("Speaking...");
        await textToSpeech.Speech(response);
        Debug.Log("Done speaking.");
        Listen();
    }
}
