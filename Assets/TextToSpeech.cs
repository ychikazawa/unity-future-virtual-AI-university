using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;



public enum VoiceList
{
    en_US_JennyNeural,
    en_US_GuyNeural,
    en_US_AriaNeural,
    en_US_DavisNeural,
    en_GB_SoniaNeural,
    en_GB_RyanNeural,
    ja_JP_NanamiNeural,
    ja_JP_KeitaNeural,
    ja_JP_AoiNeural,
    ja_JP_DaichiNeural
}

public static class VoiceListExtensions
{
    public static string ToVoiceTypeName(VoiceList voiceType)
    {
        // Enum cannot have hyphens, so initialize it with underscores. Then replace them with hyphens.
        return voiceType.ToString().Replace("_", "-");
    }
}



public class TextToSpeech : MonoBehaviour
{
    static string speechKey = EnvManager.Get("SPEECH_KEY");
    static string speechRegion = EnvManager.Get("SPEECH_REGION");

    // The language of the voice that speaks.
    [SerializeField] private VoiceList voiceType = VoiceList.ja_JP_NanamiNeural;
    
    protected string voiceTypeName;
    protected SpeechConfig speechConfig;

    // Events
    // Other scripts can subscribe to this event.
    // Note: this script should not have any dependencies on other scripts. 
    public event Action OnSpeechStart = delegate { };
    public event Action OnSpeechEnd = delegate { };


    void Start()
    {
        // Initialize voice type name
        voiceTypeName = VoiceListExtensions.ToVoiceTypeName(voiceType);
        // Initialize speech config
        speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);  
        speechConfig.SpeechSynthesisVoiceName = voiceTypeName; 
    }


    public void OnStart()
    {
        Debug.Log("TextToSpeech.OnStart()");
        Task.Run(() => Speech("Hello, world!") );
        // Invoke the event
        OnSpeechStart.Invoke();
    }

    async Task Speech(string speechText)
    {
        using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
        {
            var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(speechText);
            OutputSpeechSynthesisResult(speechSynthesisResult, speechText);
        }
        // Invoke the event
        OnSpeechEnd.Invoke();
    }


    static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                Debug.Log($"Speech synthesized for text: [{text}]");
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }
    }
}
