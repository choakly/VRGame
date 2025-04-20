using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Dan.Main;
using System;

public class Leaderboard : MonoBehaviour
{
    [SerializeField]
    private List<TextMeshProUGUI> names;
    [SerializeField]
    private List<TextMeshProUGUI> scores;
    private string publicLeaderboardKey = "51f81ad0547b8a0f1a32a0726f00b0437135ac843c41bc96f352ce4c9fae588e";
    public TextMeshProUGUI playerHighscoreUI;
    public HighScoreHandler highScoreHandler;
    [SerializeField] TextMeshProUGUI lastScore;

    private void Start()
    {
        GetLeaderboard();
        Invoke("DisplaySaveData", 0.5f);
    }

    private void DisplaySaveData()
    {
        if(highScoreHandler == null)
        {
            GameObject hs = GameObject.FindGameObjectWithTag("HighscoreObj");
            highScoreHandler = hs.GetComponent<HighScoreHandler>();
        }

        playerHighscoreUI.text = "Your Highscore: " + highScoreHandler.highscore.playerScore.ToString();
        lastScore.text = highScoreHandler.tempScore.playerScore.ToString();
    }

    public void GetLeaderboard() {
        LeaderboardCreator.GetLeaderboard(publicLeaderboardKey, ((msg) => {
            int loopLength = (msg.Length < names.Count) ? msg.Length : names.Count;
            for (int i = 0; i < loopLength; ++i) {
                names[i].text = msg[i].Username;
                scores[i].text = msg[i].Score.ToString();
            }
        }));
    }

    public void SetLeaderboardEntry(string username, int score) {
        LeaderboardCreator.UploadNewEntry(publicLeaderboardKey, username, score, ((msg) => {
            username.Substring(0, 4);
            GetLeaderboard();
        }));
    }
}
