using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

[Serializable]
public class HighScore
{
    public string playerName;
    public int playerScore;

    public HighScore(string name, int score)
    {
        playerName = name;
        playerScore = score;
    }
}

public class HighScoreHandler : MonoBehaviour
{
    List<HighScore> scoreList = new List<HighScore>();
    public HighScore highscore;                             //The score to be written/saved to json
    public HighScore tempScore = new HighScore("temp", 0);  //Temporary score
    //[SerializeField] int maxCount = 1;
    [SerializeField] string fileName;

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
        GameObject[] objs = GameObject.FindGameObjectsWithTag("HighscoreObj");

        if(objs.Length > 1)
        {
            Destroy(objs[1]);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
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
        //scoreList = JSONFileHandler.ReadListFromJSON<HighScore>(fileName);
        highscore = JSONFileHandler.ReadFromJSON<HighScore>(fileName);
    }

    private void SaveHighScore()
    {
        Debug.Log("SaveHighScore has run");
        //JSONFileHandler.SaveToJSON<HighScore>(scoreList, fileName);
        JSONFileHandler.SaveToJSON<HighScore>(highscore, fileName);
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