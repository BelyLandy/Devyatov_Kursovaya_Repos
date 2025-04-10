using System.Collections.Generic;
using UnityEngine;

public interface IRecognizer
{
    public DollarPoint[] Normalize(DollarPoint[] points, int n,
        BaseRecognizeAlgorithm.Step step = BaseRecognizeAlgorithm.Step.TRANSLATED);

    public (string, float) DoRecognition(DollarPoint[] points, int n,
        List<ShapeRecognizerManager.GestureTemplate> gestureTemplates);
}