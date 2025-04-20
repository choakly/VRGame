using UnityEngine;
using TMPro;
using UnityEngine.Events;
public class ScoreManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI inputScore;
    [SerializeField]
    private TMP_InputField inputName;
    [SerializeField]
    private TextMeshProUGUI lastScore;

    public UnityEvent<string, int> submitScoreEvent;

    public void SubmitScore() {
        submitScoreEvent.Invoke(inputName.text, int.Parse(lastScore.text));//int.Parse(inputScore.text));
    }
}
