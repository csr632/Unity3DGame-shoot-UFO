using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Scorer
{
    private int score = 0;

    Text scoreText;

    private static Scorer instance;
    public static Scorer getInstance()
    {
        if (instance == null)
        {
            instance = new Scorer();
        }
        return instance;
    }
    private Scorer()
    {
        scoreText = (GameObject.Instantiate(Resources.Load("ShowScore")) as GameObject).transform.Find("Score").GetComponent<Text>();
        scoreText.text = "" + score;
    }

    public void record(int difficulty)
    {
        switch(difficulty) {
            case 0:
            score += 1;
            break;
            case 1:
            score += 2;
            break;
            case 2:
            score += 3;
            break;
            default:
            throw new System.Exception("difficulty is out of range!");
        }
        scoreText.text = "" + score;
    }

    public int getScore()
    {
        return score;
    }
}
