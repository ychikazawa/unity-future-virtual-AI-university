using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField] private SpeechToText speechToText;
    [SerializeField] private UnityEvent onButtonStartClicked;
    [SerializeField] private UnityEvent onButtonStopClicked;

    private UIDocument uiDocument;
    private string currentRecognizingText;
    private List<string> textBuffer = new List<string>();

    

    // Start is called before the first frame update
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();

        var buttonStartElement = uiDocument.rootVisualElement.Q<Button>("ButtonStart");
        buttonStartElement.clicked += () => onButtonStartClicked.Invoke();   

        var buttonStopElement = uiDocument.rootVisualElement.Q<Button>("ButtonStop");
        buttonStopElement.clicked += () => onButtonStopClicked.Invoke();

        speechToText.OnRecognizing += AddRecognizingText;
        speechToText.OnRecognized += AddRecognizedText;
    }


    void Update()
    {
        UpdateText(string.Join("\n", textBuffer) + "\n" + currentRecognizingText);
    }


    void AddRecognizingText(string text)
    {
        currentRecognizingText = text;
    }

    void AddRecognizedText(string text)
    {
        textBuffer.Add(text);
        currentRecognizingText = "";
    }


    void UpdateText(string text)
    {
        var textElement = uiDocument.rootVisualElement.Q<Label>("OutputField");
        textElement.text = text;
    }
}
