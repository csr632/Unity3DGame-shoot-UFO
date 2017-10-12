# Unity3DGame-shoot-UFO
A simple Unity3D game

# 改进描述
在[我们之前完成的飞碟游戏](http://www.jianshu.com/p/58af7f81c2e8)中，UFO是在两点之间来回飞行，我们是通过修改position来使得飞碟运动起来的。

现在，为了练习对Unity物理引擎的使用和适配器模式的使用，我们想要加入另一种飞碟运动模式：**物理运动模式**，飞碟受到向下的力，向地面撞去。玩家要在飞碟撞到地面之前击中飞碟才能得分，飞碟撞上地面则不得分。

并且，我们不仅要实现物理运动模式，还要保留着原本的普通运动模式，通过**鼠标右键**，用户可以在两种模式之间切换。

# 游戏截图
![正常运动模式下飞碟来回飞行](http://upload-images.jianshu.io/upload_images/4888929-bba6a5cad8420a2f.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![鼠标右键可以切换运动模式](http://upload-images.jianshu.io/upload_images/4888929-3f1ac0ccbb075bad.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)


![物理模式下飞碟会缓缓掉落地面](http://upload-images.jianshu.io/upload_images/4888929-8040ff29becd5d19.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

# 在自己的电脑上运行这个游戏！
从[我的github](https://github.com/csr632/Unity3DGame-shoot-UFO/tree/Improvement1_PhysicalUFO)下载项目资源，将所有文件放进你的项目的Assets文件夹（如果有重复则覆盖），然后在U3D中双击“hw5”，就可以运行了！

# 实现物理模式的动作管理器
这次的改进有一点特别。**正常运动模式不能删除，而是与新的物理运动模式共存，我们要在游戏运行的时候来决定使用哪种运动模式**。也就是说，原本的动作管理器类不能删除，它们是管理正常运动模式的。我们还要再实现一个动作管理器，用来管理物理运动模式。最后想一种办法将两个动作管理器结合起来。
首先我们实现物理模式动作管理器:
```
public class PhysicsActionManager : MonoBehaviour {

	public void addForce(GameObject gameObj, Vector3 force) {
		ConstantForce originalForce = gameObj.GetComponent<ConstantForce>();
		if (originalForce) {
			originalForce.enabled = true;
			originalForce.force = force;
		} else {
			gameObj.AddComponent<Rigidbody>().useGravity = false;
			gameObj.AddComponent<ConstantForce>().force = force;
		}
	}

	public void removeForce(GameObject gameObj) {
		gameObj.GetComponent<ConstantForce>().enabled = false;
	}
}
```
这个管理器的实现非常简单，只需要负责增加\移除ConstantForce组件就可以了。
> 要使物体受到力的影响，必须先让他具有Rigidbody（刚体）组件。对物理引擎的使用，网上有很多教程。你可以[查看官方文档](https://docs.unity3d.com/Manual/PhysicsSection.html)或[学习其他作者的博客](http://www.cnblogs.com/edisonchou/p/3546724.html)。

****
# 适配器模式
如何将两种动作管理器有机地结合起来呢？让FirstController（场景控制器）同时拥有两个变量，分别指向这两个动作管理器吗？这样不好，如果我们以后又要增加新的动作管理器呢？如果我们要增加新的飞碟工厂类呢？这样的话FirstController就需要管理太多**功能相同的部件**了，FirstController会越来越臃肿，可扩展性很差。

我们希望FirstController只需要**为同一个用途的所有组件保存1个变量**。

这就是为什么我们需要适配器模式。
让我通过一个生活中的例子来解释适配器模式：现在大部分的的平板电脑只有一个USB接口，现在我想在我的平板电脑上同时使用键盘和鼠标，怎么办？很简单，买一个这样的USB扩展器：
![USB扩展器就是一种适配器](http://upload-images.jianshu.io/upload_images/4888929-5fcb64b56209a3a8.jpg?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

将USB扩展器插在平板的USB接口上，然后将键盘、鼠标插在USB扩展器的USB接口上，你就可以同时使用键盘和鼠标了！

在这个例子中，我们的平板就像是FirstController，两个输入设备就像是两个动作管理器。要将两个动作管理器同时接入FirstController，我们要实现一个适配器，让FirstController连接适配器，然后让适配器连接两个动作管理器。

我们先将FirstController中原本保存ActionManager的变量删掉，然后添加这一行：
```
ActionManagerTarget actionManagerTarget;
```
ActionManagerTarget是一个接口，它就相当于平板电脑上的USB接口：
```
public interface ActionManagerTarget {
	void switchActionMode();
	
	void addAction(GameObject gameObj, Dictionary<string, object> option);

	void addActionForArr(GameObject[] Arr, Dictionary<string, object> option);

	void addActionForArr(UFOController[] Arr, Dictionary<string, object> option);

	void removeActionOf(GameObject obj, Dictionary<string, object> option);
}
```
然后实现一个适配器类ActionManagerAdapter，这个类要实现这个接口：
```
public class ActionManagerAdapter: ActionManagerTarget {
	FirstSceneActionManager normalAM;
	PhysicsActionManager PhysicsAM;

	int whichActionManager = 0; // 0->normal, 1->physics

	public ActionManagerAdapter(GameObject main) {
		normalAM = main.AddComponent<FirstSceneActionManager>();
		PhysicsAM = main.AddComponent<PhysicsActionManager>();
		whichActionManager = 0;
	}

	public void switchActionMode() {
		whichActionManager = 1-whichActionManager;
	}

	public void addAction(GameObject gameObj, Dictionary<string, object> option) {
		if (whichActionManager == 0)
		//	use normalAM
		{
			Debug.Log("use normalAM");
			normalAM.addRandomAction(gameObj, (float)option["speed"]);
		}

		else
		//	use PhysicsAM
		{
			Debug.Log("use PhysicsAM");
			PhysicsAM.addForce(gameObj, (Vector3)option["force"]);
		}
	}

	public void addActionForArr(GameObject[] Arr, Dictionary<string, object> option) {
		if (whichActionManager == 0)
		//	use normalAM
		{
			Debug.Log("use normalAM");
			float speed = (float)option["speed"];
			foreach (GameObject gameObj in Arr) {
				normalAM.addRandomAction(gameObj, speed);
			}
		}

		else
		//	use PhysicsAM
		{
			Debug.Log("use PhysicsAM");
			Vector3 force = (Vector3)option["force"];
			foreach (GameObject gameObj in Arr) {
				PhysicsAM.addForce(gameObj, force);
			}
		}
	}

	public void addActionForArr(UFOController[] Arr, Dictionary<string, object> option) {
		if (whichActionManager == 0)
		//	use normalAM
		{
			Debug.Log("use normalAM");
			float speed = (float)option["speed"];
			foreach (UFOController ctrl in Arr) {
				normalAM.addRandomAction(ctrl.getObj(), speed);
			}
		}

		else
		//	use PhysicsAM
		{
			Debug.Log("use PhysicsAM");
			Vector3 force = (Vector3)option["force"];
			foreach (UFOController ctrl in Arr) {
				PhysicsAM.addForce(ctrl.getObj(), force);
			}
		}
	}

	public void removeActionOf(GameObject gameObj, Dictionary<string, object> option){
		if (whichActionManager == 0)
		//	use normalAM
		{
			Debug.Log("use normalAM");
			normalAM.removeActionOf(gameObj);
		}

		else
		//	use PhysicsAM
		{
			Debug.Log("use PhysicsAM");
			PhysicsAM.removeForce(gameObj);
		}
	}
}
```
可以看出，我们在实现适配器的时候，将两个动作管理器“焊死”在适配器上了，你还可以自己尝试，实现一个可以“自由插拔”的适配器:)。

然后我们在FirstController的构造函数中实例化一个适配器（相当于将USB扩展器插在平板电脑上）：
```
actionManagerTarget = new ActionManagerAdapter(gameObject);
```
最后不要忘了在Update中监测用户鼠标的右键输入，切换动作管理模式。最终的FirstController是这样的：
```
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
```
注意我没有在监测到鼠标右键输入以后马上切换动作管理模式，而是通过一点小技巧，延迟到下一轮开始的时候再切换。这是因为如果立刻切换，这一轮的“动作取消”会出很大的问题。你仔细想想，这一轮开始的时候，我们使用正常动作管理器给每个飞碟添加了一个普通的动作，而取消动作的时候却使用物理动作管理器！这样，飞碟上的普通动作就无法被回收，下一轮开始的时候飞碟依然在来回移动。


> ActionManagerAdapter使用了一个非常灵活的方式来接收参数：`Dictionary<string, object> option` 其中的object可以传递任何类型的值，甚至是int、float原始类型。因为FirstController不知道当前的运动模式是什么，不知道应该给ActionManagerAdapter传递speed参数还是force参数，于是干脆两个都传进去，让ActionManagerAdapter自己选择：
```
actionManagerTarget.addActionForArr(ufoCtrlArr, new Dictionary<string, object>() {
            {"speed", ufoCtrlArr[0].attr.speed},
            {"force", difficultyManager.getGravity()}
        });
```

****
# 适配器模式补充说明
#### 适配器模式定义
适配器模式(Adapter Pattern) ：将一个接口转换成客户希望的另一个接口，适配器模式使接口不兼容的那些类可以一起工作，其别名为包装器(Wrapper)。
> 需要接入2个类，而客户类只提供1个接口，这也是一种“接口不兼容”。

#### 适配器模式的组成
* Target：目标抽象类（USB接口）
* Adapter：适配器类（USB扩展器）
* Adaptee：适配者类（鼠标、键盘、U盘）
* Client：客户类（平板电脑）

适配器的作用，除了我们刚才所说的，将多个类接入同一个接口以外，还有转接“不兼容”接口的作用。比如说，如果我们想将U盘插入USB-typeC接口中，我们要买另一种适配器：

![USB转接器也是一种适配器](http://upload-images.jianshu.io/upload_images/4888929-e771a38219aad49c.jpg?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

这个适配器也解决了“接口不兼容”的问题。当“客户类提供的接口”与“适配者类”不兼容的时候，可以实现一个适配器，让适配器实现“客户类提供的接口”，并在这个适配器中调用“适配者类”的方法。

如果还想深入学习有关适配器模式的内容，可以看看[这个网站](http://design-patterns.readthedocs.io/zh_CN/latest/structural_patterns/adapter.html)。

****
谢谢阅读！
