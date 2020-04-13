using UnityEngine;
using UnityEditor;

/// <summary>
/// This class describes the information to be shown on Tutorial Panel
/// </summary>
[System.Serializable]
public class TutorialDescriptor 
{
    [SerializeField] [TextArea(3,30)]public string ExplanationText;
    //[SerializeField] public ExplanationText;
}