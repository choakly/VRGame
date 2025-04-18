using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    //[SerializeField] CreatePoints pointCounter;
    [SerializeField] HighScoreHandler highscorehandler;
    //[SerializeField] PointHUD pointHUD;
    public ScoreManager scoremanager;
    [SerializeField] string playerName;

    private void Start()
    {
        Debug.Log("Editor path: " + Application.dataPath);
        //Debug.Log("Persistent path: " + Application.persistentDataPath);
    }

    public void StartGame()
    {
        //pointCounter.StartGame();
    }

    public void StopGame()
    {
        //highscorehandler.AddHighScore(new HighScore(playerName, int.Parse(scoremanager.inputScore.text)));
        //pointCounter.StopGame();
    }
}