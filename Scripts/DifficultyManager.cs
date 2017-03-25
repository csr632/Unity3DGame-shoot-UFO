using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyManager
{
    private int currentDifficulty = 0;  // 0~2
    public float currentSendInterval;

    // float[] sendUFOInterval = {4, 3, 2.5F};
    public readonly int UFONumber = 10;
    float[] sendUFOInterval = { 10, 9, 8 };
    float[] UFOScale = { 0.9F, 0.9F, 0.9F };
    float[] UFOSpeed = { 5, 8, 10 };
    Color[] UFOColor = { Color.red, Color.blue, Color.gray };

    public Text difficultyText;


    // singleton pattern
    private static DifficultyManager instance;
    public static DifficultyManager getInstance()
    {
        if (instance == null)
        {
            instance = new DifficultyManager();
        }
        return instance;
    }

    private DifficultyManager()
    {
        currentDifficulty = 0;
        currentSendInterval = sendUFOInterval[0];
        difficultyText = (GameObject.Instantiate(Resources.Load("ShowDifficulty")) as GameObject).transform.Find("Difficulty").GetComponent<Text>();
    }

    public UFOAttributes getUFOAttributes()
    {
        // 根据当前难度获取UFO的参数
        return new UFOAttributes(
            UFOColor[currentDifficulty],
            UFOScale[currentDifficulty],
            UFOSpeed[currentDifficulty]
        );
    }

    public void increaseDifficulty()
    {
        if (currentDifficulty < 2)
        {
            currentDifficulty++;
            currentSendInterval = sendUFOInterval[currentDifficulty];
            difficultyText.text = "" + currentDifficulty;
        }
    }

    public int getDifficulty()
    {
        return currentDifficulty;
    }

    public void setDifficulty(int dif)
    {
        if (currentDifficulty != dif)
        {
            if (dif > 2)
            {
                throw new System.Exception("difficulty is out of range!");
            }

            currentDifficulty = dif;
            currentSendInterval = sendUFOInterval[currentDifficulty];
            difficultyText.text = "" + currentDifficulty;
        }
    }

    public void setDifficultyByScore(int currentScore)
    {
        if (currentScore > 100)
        {
            setDifficulty(2);
        }
        else if (currentScore > 20)
        {
            setDifficulty(1);
        }
        else
        {
            setDifficulty(0);
        }
    }

    public void resetDifficulty()
    {
        currentDifficulty = 0;
        currentSendInterval = sendUFOInterval[currentDifficulty];
    }
}
