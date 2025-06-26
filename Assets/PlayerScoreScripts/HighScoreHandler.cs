using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class HighScore
{
    public string playerName;
    public int playerScore;
    public int playerMaxWave;

    public HighScore(string name, int score, int maxWave)
    {
        playerName = name;
        playerScore = score;
        playerMaxWave = maxWave;
    }
}

public class HighScoreHandler : MonoBehaviour
{
    public static HighScoreHandler inst;

    List<HighScore> scoreList = new List<HighScore>();
    public HighScore highscore;                             //The score to be written/saved to json
    public HighScore tempScore;// = new HighScore("Name", 0);  //Temporary score
    [SerializeField] string fileName;
    [SerializeField] int maxCount = 2;

    public UnityEvent<HighScore> savePlayerScore;

    public void SaveScoreEvent()
    {
        Debug.Log("SaveScoreEvent has run");
        //AddHighScore(tempScore);
        SaveHighScore();
    }

    private void OnApplicationQuit()
    {
        SaveHighScore();
    }

    private void Awake()
    {
        //Keep only one highscorehandler object
        if(inst == null) inst = this;
        else if(inst != this)
        {
            Debug.LogWarning("HighScoreHandler already exists, destroying object!");
            Destroy(this);
        }
        /*GameObject[] objs = GameObject.FindGameObjectsWithTag("HighscoreObj");

        if(objs.Length > 1)
        {
            Destroy(objs[1]);
        }

        DontDestroyOnLoad(this.gameObject);*/
    }

    private void Start()
    {
        scoreList.Capacity = maxCount;

        Debug.Log("Editor path: " + Application.dataPath);

        LoadHighScore();
    }

    //For debugging
    /*public void Update()
    {
        Debug.Log("Savefile date: " + highscore.playerName + highscore.playerScore.ToString());
    }*/

    private void LoadHighScore()
    {
        Debug.Log("Loadhighscore");
        scoreList = JSONFileHandler.ReadListFromJSON<HighScore>(fileName);

        if((scoreList.Count == 0))// || (scoreList.Equals("[]")))
        {
            Debug.Log("score list is empty");
            scoreList.Insert(0, new HighScore("abcd", 0, 1));
            scoreList.Insert(1, new HighScore("abcd", 0, 1));
        }

        highscore = scoreList[0];
        tempScore = scoreList[1];
        //highscore = JSONFileHandler.ReadFromJSON<HighScore>(fileName);
    }

    private void SaveHighScore()
    {
        Debug.Log("SaveHighScore has run");

        //scoreList.Clear();
        for(int i = 0; i < scoreList.Count; i++)
        {
            if(i == 0) scoreList[i] = highscore;
            if(i == 1) scoreList[i] = tempScore;
        }

        if(scoreList != null) JSONFileHandler.SaveToJSON<HighScore>(scoreList, fileName);
        else Debug.LogWarning("No data to save!");
        //JSONFileHandler.SaveToJSON<HighScore>(highscore, fileName);
    }

    public void AddPoints(int points)
    {
        tempScore.playerScore = points;
        Debug.Log("AddPoints has run");
        AddHighScore(tempScore);
    }

    public void AddHighScore(HighScore element)
    {
        Debug.Log("Addhighscore");
        if(element.playerScore > highscore.playerScore)
        {
            highscore = element;
            //SaveHighScore();
        }
    }

    public void AddMaxWave(int wave)
    {
        Debug.Log("AddMaxWave");
        if(wave > highscore.playerMaxWave)
        {
            highscore.playerMaxWave = wave;
        }
    }

    public void RemoveTempScore()
    {
        tempScore = new HighScore("temp", 0, 1);
    }

    public void RemoveHighScore(HighScore element, int index)
    {
        scoreList.RemoveAt(index);

        SaveHighScore();

        /*for(int i = 0; i < maxCount; i++)
        {
            if((i >= scoreList.Count) || (element.playerScore > scoreList[i].playerScore))
            {
                //Add new high score
                scoreList.Insert(i, element);

                //If file has more than maxCount then remove all extras
                while(scoreList.Count > maxCount)
                {
                    scoreList.RemoveAt(maxCount);
                }

                SaveHighScore();

                break;
            }
        }*/
    }
}