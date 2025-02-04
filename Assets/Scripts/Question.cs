using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Quiz/Question")]
public class Question : ScriptableObject
{
    [TextArea]
    public string questionText;

    public string optionA;
    public string optionB;

    [Tooltip("Set to true if Option A is correct; otherwise, Option B is correct.")]
    public bool isOptionACorrect; // True if Option A is correct, False for Option B
}
