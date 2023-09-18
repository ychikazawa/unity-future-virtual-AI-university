using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading;

public class SpeechToText : MonoBehaviour
{
    static string speechKey = EnvManager.Get("SPEECH_KEY");
    static string speechRegion = EnvManager.Get("SPEECH_REGION");

    protected SpeechConfig speechConfig;
    protected AudioConfig audioConfig;
    protected SpeechRecognizer speechRecognizer;
    protected TaskCompletionSource<int> stopRecognition;
    
    // Properties
    // Note: RecognizedText is never used yet.
    public string RecognizedText { get; private set; }

    // Events
    // Other scripts can subscribe to this event.
    // Note: this script should not have any dependencies on other scripts. 
    public event Action<string> OnRecognizing = delegate { };
    public event Action<string> OnRecognized = delegate { };


    void Awake()
    {   
        // initialize speech recognizer
        speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
        speechConfig.SpeechRecognitionLanguage = "ja-JP";
        audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);
        stopRecognition = new TaskCompletionSource<int>();

        // subscribe recognizer events
        speechRecognizer.Recognizing += (s, e) =>
        {
            Debug.Log($"RECOGNIZING: Text={e.Result.Text}");
            // Invoke the event
            OnRecognizing.Invoke(e.Result.Text);
        };

        speechRecognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                Debug.Log($"RECOGNIZED: Text={e.Result.Text}");
                // Save the recognized text
                RecognizedText = e.Result.Text;
                // Invoke the event
                OnRecognized.Invoke(e.Result.Text);
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                Debug.Log($"NOMATCH: Speech could not be recognized.");
            }
        };

        speechRecognizer.Canceled += (s, e) =>
        {
            Debug.Log($"CANCELED: Reason={e.Reason}");

            if (e.Reason == CancellationReason.Error)
            {
                Debug.Log($"CANCELED: ErrorCode={e.ErrorCode}");
                Debug.Log($"CANCELED: ErrorDetails={e.ErrorDetails}");
                Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
            }

            stopRecognition.TrySetResult(0);
        };

        speechRecognizer.SessionStopped += (s, e) =>
        {
            Debug.Log("\n    Session stopped event.");
            stopRecognition.TrySetResult(0);
        };
    }


    public void OnStart()
    {
        Debug.Log("OnStart");
        Task.Run(() => Recognize());
    }


    public void OnStop()
    {
        Debug.Log("OnStop");
        Task.Run(() => StopRecognition());
    }

    void OnDestroy() {
        speechRecognizer.Dispose();
    }


    async Task Recognize()
    {
        await speechRecognizer.StartContinuousRecognitionAsync();
        Debug.Log("Say something...");
        Task.WaitAny(new[] { stopRecognition.Task });
    }

    async Task StopRecognition()
    {
        await speechRecognizer.StopContinuousRecognitionAsync();
        Debug.Log("Stop recognition.");
    }
}
