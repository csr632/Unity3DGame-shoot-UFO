using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;


public class FirstController : MonoBehaviour, SceneController
{
    Director director;
    UFOFactory UFOfactory;

    ExplosionFactory explosionFactory;
    FirstSceneActionManager actionManager;

    Scorer scorer;

    DifficultyManager difficultyManager;

    float timeAfterRoundStart = 10;
    bool roundHasStarted = false;

    void Awake()
    {
        director = Director.getInstance();
        director.currentSceneController = this;

        actionManager = gameObject.AddComponent<FirstSceneActionManager>();

        UFOfactory = gameObject.AddComponent<UFOFactory>();

        explosionFactory = gameObject.AddComponent<ExplosionFactory>();

        scorer = Scorer.getInstance();
        difficultyManager = DifficultyManager.getInstance();


        loadResources();
    }
    public void loadResources()
    {
        new FirstCharacterController();
        Instantiate(Resources.Load("Terrain"));
    }

    public void Start()
    {
        roundStart();
    }

    void Update()
    {
        timeAfterRoundStart += Time.deltaTime;
        if (roundHasStarted && checkAllUFOIsShot())
        {
            print("All UFO is shot down! Next round in 3 sec");
            roundHasStarted = false;
            Invoke("roundStart", 3);
            difficultyManager.setDifficultyByScore(scorer.getScore());
        }
        else if (roundHasStarted && checkTimeOut())
        {
            print("Time out! Next round in 3 sec");
            roundHasStarted = false;
            foreach (UFOController ufo in UFOfactory.getUsingList())
            {
                actionManager.removeActionOf(ufo.getObj());
            }
            UFOfactory.recycleAll();
            Invoke("roundStart", 3);
            difficultyManager.setDifficultyByScore(scorer.getScore());
        }
    }

    void roundStart()
    {
        roundHasStarted = true;
        timeAfterRoundStart = 0;
        UFOController[] ufoCtrlArr = UFOfactory.produceUFOs(difficultyManager.getUFOAttributes(), difficultyManager.UFONumber);
        for (int i = 0; i < ufoCtrlArr.Length; i++)
        {
            ufoCtrlArr[i].appear();
        }
        actionManager.addRandomActionForArr(ufoCtrlArr, ufoCtrlArr[0].attr.speed);
    }

    bool checkTimeOut()
    {
        if (timeAfterRoundStart > difficultyManager.currentSendInterval)
        {
            return true;
        }
        return false;
    }

    bool checkAllUFOIsShot()
    {
        return UFOfactory.getUsingList().Count == 0;
    }

    public void UFOIsShot(UFOController UFOCtrl)
    {
        scorer.record(difficultyManager.getDifficulty());

        actionManager.removeActionOf(UFOCtrl.getObj());
        UFOfactory.recycle(UFOCtrl);
        explosionFactory.explodeAt(UFOCtrl.getObj().transform.position);
    }

    public void GroundIsShot(Vector3 pos) {
        explosionFactory.explodeAt(pos);
    }
}
