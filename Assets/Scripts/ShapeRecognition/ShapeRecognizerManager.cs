using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapeRecognizerManager : MonoBehaviour
{
    [SerializeField] 
    private Drawable drawableComponent;
    [SerializeField] 
    private TextMeshProUGUI outputLabel;
    [SerializeField] 
    private Button addTemplateButton;
    [SerializeField] 
    private Button recognizeButton;
    [SerializeField] 
    private Button reviewTemplatesButton;
    [SerializeField] 
    private TMP_InputField templateNameInput;
    [SerializeField] 
    private TemplateReviewPanel reviewPanel;
    [SerializeField] 
    private RecognitionPanel recognitionDisplay;

    public RecognizedShape LastShape = RecognizedShape.Unrecognized;
    
    private GestureTemplates Templates => GestureTemplates.Get();
    private static readonly BaseRecognizeAlgorithm defaultRecognizer = new BaseRecognizeAlgorithm();
    private IRecognizer currentRecognizer = defaultRecognizer;
    private RecognizerState currentState = RecognizerState.RECOGNITION;

    public enum RecognizerState
    {
        TEMPLATE,
        RECOGNITION,
        TEMPLATE_REVIEW
    }

    [Serializable]
    public struct GestureTemplate
    {
        public string Name;
        public DollarPoint[] Points;

        public GestureTemplate(string name, DollarPoint[] points)
        {
            Name = name;
            Points = points;
        }
    }

    private string TemplateName => templateNameInput.text;

    private void Start()
    {
        drawableComponent.OnDrawFinished += ProcessCompletedDrawing;
        addTemplateButton.onClick.AddListener(() => SetState(RecognizerState.TEMPLATE));
        recognizeButton.onClick.AddListener(() => SetState(RecognizerState.RECOGNITION));
        reviewTemplatesButton.onClick.AddListener(() => SetState(RecognizerState.TEMPLATE_REVIEW));
        recognitionDisplay.Initialize(ChangeRecognitionAlgorithm);

        SetState(currentState);
    }

    private void ChangeRecognitionAlgorithm(int algorithm)
    {
        if (algorithm == 0)
        {
            currentRecognizer = defaultRecognizer;
        }
    }

    private void SetState(RecognizerState state)
    {
        currentState = state;
        addTemplateButton.image.color = currentState == RecognizerState.TEMPLATE ? Color.green : Color.white;
        recognizeButton.image.color = currentState == RecognizerState.RECOGNITION ? Color.green : Color.white;
        reviewTemplatesButton.image.color = currentState == RecognizerState.TEMPLATE_REVIEW ? Color.green : Color.white;
        templateNameInput.gameObject.SetActive(currentState == RecognizerState.TEMPLATE);
        outputLabel.gameObject.SetActive(currentState == RecognizerState.RECOGNITION);

        reviewPanel.SetVisibility(currentState == RecognizerState.TEMPLATE_REVIEW);
        //recognitionDisplay.SetVisibility(currentState == RecognizerState.RECOGNITION);
    }

    private Vector2 ComputeCenter(DollarPoint[] points)
    {
        float xSum = 0f, ySum = 0f;
        foreach (var point in points)
        {
            xSum += point.Point.x;
            ySum += point.Point.y;
        }
        return new Vector2(xSum / points.Length, ySum / points.Length);
    }

    private void ProcessCompletedDrawing(DollarPoint[] points)
    {
        if (currentState == RecognizerState.TEMPLATE)
        {
            GestureTemplate normalizedTemplate = new GestureTemplate(TemplateName, currentRecognizer.Normalize(points, 64));
            Templates.RawTemplates.Add(new GestureTemplate(TemplateName, points));
            Templates.ProceedTemplates.Add(normalizedTemplate);
        }
        else
        {
            (string name, float score) recognitionResult = currentRecognizer.DoRecognition(
                points, 
                64,
                Templates.RawTemplates
            );
            
            string displayText = $"Результат распознавания: {recognitionResult.name} (оценка: {recognitionResult.score})";
            outputLabel.text = displayText;
            Debug.Log(displayText);

            LastShape = Enum.TryParse(recognitionResult.name, out RecognizedShape shape)
                ? shape : RecognizedShape.Unrecognized;

            if (recognitionResult.score >= 0.7f)
            {
                Vector2 centerPixels = ComputeCenter(points);
                Vector2 worldCenter = drawableComponent.PixelToWorldCoordinates(centerPixels);
                ZoneManager.Instance.CheckZones(worldCenter, recognitionResult.name);
            }
        }
    }

    /*private void OnApplicationQuit()
    {
        Templates.Save();
    }*/
}
