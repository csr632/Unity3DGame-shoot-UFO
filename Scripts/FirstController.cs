using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;
using UnityEngine.UI;

public class FirstController : MonoBehaviour, SceneController
{
    Director director;
    UFOFactory UFOfactory;

    ExplosionFactory explosionFactory;
    ActionManagerTarget actionManagerTarget;
	
    bool switchAMInNextRound = false;
    Scorer scorer;

    DifficultyManager difficultyManager;

    float timeAfterRoundStart = 10;
    bool roundHasStarted = false;

    FirstCharacterController firstCharacterController;

    Text hint;

    void Awake()
    {
        // 挂载各种控制组件

        director = Director.getInstance();
        director.currentSceneController = this;

        // actionManager = gameObject.AddComponent<FirstSceneActionManager>();
        actionManagerTarget = new ActionManagerAdapter(gameObject);

        UFOfactory = gameObject.AddComponent<UFOFactory>();

        explosionFactory = gameObject.AddComponent<ExplosionFactory>();

        scorer = Scorer.getInstance();
        difficultyManager = DifficultyManager.getInstance();


        loadResources();
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Shootable"), LayerMask.NameToLayer("Shootable"), true);
    }
    public void loadResources()
    {
        // 初始化场景中的物体
        firstCharacterController = new FirstCharacterController();
        Instantiate(Resources.Load("Terrain"));
        hint = (Instantiate(Resources.Load("ShowResult")) as GameObject).GetComponentInChildren<Text>();
        hint.text = "";
    }

    public void Start()
    {
        roundStart();
    }

    void Update()
    {
        if (roundHasStarted) {
            timeAfterRoundStart += Time.deltaTime;
        }

        if (roundHasStarted && checkAllUFOIsShot()) // 检查是否所有UFO都已经被击落
        {
            hint.text = "All UFO has crashed in this round! Next round in 3 sec";
            roundHasStarted = false;
            Invoke("roundStart", 3);
            difficultyManager.setDifficultyByScore(scorer.getScore());
        }
        else if (roundHasStarted && checkTimeOut()) // 检查这一轮是否已经超时
        {
            hint.text = "Time out! Next round in 3 sec";
            roundHasStarted = false;
            foreach (UFOController ufo in UFOfactory.getUsingList())
            {
                actionManagerTarget.removeActionOf(ufo.getObj(), new Dictionary<string, object>());
            }
            UFOfactory.recycleAll();
            Invoke("roundStart", 3);
            difficultyManager.setDifficultyByScore(scorer.getScore());
        }
        if (Input.GetButtonDown("Fire2")) {
            hint.text = "Action of UFOs will change in the next round!";
            switchAMInNextRound = true;
        }
    }

    void roundStart()
    {   
        // 开始新的一轮
        if (switchAMInNextRound) {
            switchAMInNextRound = false;
            actionManagerTarget.switchActionMode();
        }

        roundHasStarted = true;
        timeAfterRoundStart = 0;
        UFOController[] ufoCtrlArr = UFOfactory.produceUFOs(difficultyManager.getUFOAttributes(), difficultyManager.UFONumber);
        for (int i = 0; i < ufoCtrlArr.Length; i++)
        {
            ufoCtrlArr[i].appear();
            ufoCtrlArr[i].setPosition(getRandomUFOPosition());
        }

        actionManagerTarget.addActionForArr(ufoCtrlArr, new Dictionary<string, object>() {
            {"speed", ufoCtrlArr[0].attr.speed},
            {"force", difficultyManager.getGravity()}
        });
        hint.text = "";
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
        // 响应UFO被击中的事件
        scorer.record(difficultyManager.getDifficulty());
        actionManagerTarget.removeActionOf(UFOCtrl.getObj(), new Dictionary<string, object>());
        UFOfactory.recycle(UFOCtrl);
        explosionFactory.explodeAt(UFOCtrl.getObj().transform.position);
    }

    public void GroundIsShot(Vector3 pos) {
        // 响应地面被击中的事件（直接产生一个爆炸）
        explosionFactory.explodeAt(pos);
    }

    public void UFOCrash(UFOController UFOCtrl) {
        actionManagerTarget.removeActionOf(UFOCtrl.getObj(), new Dictionary<string, object>());
        UFOfactory.recycle(UFOCtrl);
        explosionFactory.explodeAt(UFOCtrl.getObj().transform.position);
    }

    public Vector3 getRandomUFOPosition() {
        Vector3 relativeToCharacter = new Vector3(Random.Range(-10, 10), Random.Range(10, 15), Random.Range(-10, 10));
        return firstCharacterController.getPosition()+relativeToCharacter;
    }
}
