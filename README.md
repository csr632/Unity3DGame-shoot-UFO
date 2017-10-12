# Unity3DGame-shoot-UFO
A simple Unity3D game
> 看完了这篇文章以后，可以在[另一个分支](https://github.com/csr632/Unity3DGame-shoot-UFO/tree/Improvement1_PhysicalUFO)查看它的增强版。

# 任务概述
这次我们重新制作一个打飞碟小游戏。游戏每一轮生成10个飞碟，每个飞碟随机飞行，玩家要在这一轮结束之前尽快地射击飞碟，击中了就加分，分数达到一定的程度就提升难度。这个游戏很基本，也很简单，我们通过它来学习玩家输入、使用射线、使用工厂来获取和回收对象，并且体会代码复用的技巧。

# 游戏截图
![](http://upload-images.jianshu.io/upload_images/4888929-1ec7b022805bfd79.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
![](http://upload-images.jianshu.io/upload_images/4888929-6a763c4a3755f0ed.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

# 下载我的项目在本地查看！
从[我的github](https://github.com/csr632/Unity3DGame-shoot-UFO)下载项目资源，将Assets文件夹覆盖你的项目中的Assets文件夹，然后在U3D中双击“hw5”，就可以运行了！

# 学会使用他人的资源
这个游戏有一些资源是从外部导入的，比如说RigidBodyFPSController（第一人称控制器，可以像CS一样控制主角）来自标准资源库的Characters包（在[这篇文章](http://www.jianshu.com/p/5a572a61f809)中我教大家导入了标准资源的Environments包）。

枪支的预制和爆炸的预制，是从Asset Store中免费下载的资源，下载好之后会弹出选择框，让你从下载的资源包中选择自己需要的资源。适当地使用他人的资源能够让你专注于自己的游戏内容。

# 玩家输入、使用射线
在Update中使用[Input.GetButton(string buttonName)](https://docs.unity3d.com/ScriptReference/Input.GetButton.html)，在某一帧如果这个按键出于按下状态，就返回true，否则返回false。通过这个方式来监测用户的输入并做出反应。
> 使用GetButton可以得到“扫射”的效果，也就是说如果你按着这个键不放，那么就一直返回true。[Input.GetButtonDown](https://docs.unity3d.com/ScriptReference/Input.GetButtonDown.html)则不一样，只有你“按下”的那一帧会返回true，只能得到“点射”的效果。
Input还可以监测键盘按键、鼠标移动等，其他的使用方式可以查找[官方文档](https://docs.unity3d.com/ScriptReference/Input.html)或搜索其他博客，这里我们专注于这个小游戏。

射线：
``` C#
Ray ray = cam.ScreenPointToRay(Input.mousePosition);
RaycastHit hit;
if (Physics.Raycast(ray, out hit))
{
        // do something
}
```
通过`cam.ScreenPointToRay(Input.mousePosition)`我们得到了一条射线，从摄像机摄像鼠标点击的方向。`Physics.Raycast(ray, out hit)`将这条射线发射出去，如果射线击中了物体则返回true，并将射线击中的信息保存在参数`hit`中，你可以从中获得击中的物体、击中的位置等信息。

out是一个关键字，类似于传递引用、只不过函数会将out传进去的参数清空，再放入数据。也就是说如果使用ref关键字，信息有进有出；使用out，信息只出不进。
****
#### Shooter
在我们的游戏中，Shooter就是用来监测鼠标点击并发射射线的，挂载在枪支对象上，射线击中UFO或地面就通知sceneController。
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;

public class Shooter : MonoBehaviour
{
    public Camera cam;
    private FirstController sceneController;
    LayerMask layerMask;    // 指定一些layer层，下面我们让射线只能击中这些layer中的物体

    public GameObject muzzleFlash;  // 枪口火焰的预制，我已经将预制拖动到了Inspector中
    bool muzzleFlashEnable = false; // 是否显示枪口火焰
    float muzzleFlashTimer = 0; // 记录枪口火焰已经显示了多久
    const float muzzleFlashMaxTime = 0.1F;  // 枪口火焰每次显示0.1秒

    void Awake()
    {
        muzzleFlash.SetActive(false);
        layerMask = LayerMask.GetMask("Shootable", "RayFinish");
        // 指定这两个层，Shootable中是飞碟，RayFinish中的是地面Terrain
    }

    void Start()
    {
        cam = Camera.main;
        sceneController = Director.getInstance().currentSceneController as FirstController;
    }

    void Update()
    {
        if (Input.GetButton("Fire1"))   // Fire1按键是鼠标左键或左Ctrl键
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // layerMask参数使这个射线只能打中指定layer的物体
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.gameObject.layer == 8)
                {  // 通过hit获取到了击中物体所在的层
                    UFOController UFOCtrl = hit.transform.GetComponent<UFOScript>().ctrl;
                    sceneController.UFOIsShot(UFOCtrl); // 通知sceneController
                }
                else if (hit.transform.gameObject.layer == 9)
                {
                    sceneController.GroundIsShot(hit.point); // 通知sceneController
                }
            }
        }

        if (muzzleFlashEnable == false) // 显示枪口火焰
        {
            muzzleFlashEnable = true;
            muzzleFlash.SetActive(true);
        }
        if (muzzleFlashEnable)      // 计时，枪口火焰显示0.1秒后消失
        {
            muzzleFlashTimer += Time.deltaTime;
            if (muzzleFlashTimer >= muzzleFlashMaxTime)
            {
                muzzleFlashEnable = false;
                muzzleFlash.SetActive(false);
                muzzleFlashTimer = 0;
            }
        }
    }
}
```
关键的代码我都已经注释说明。物体被击中以后的事情交给sceneController来安排，Shooter只专注于“射击”的功能。
****
# 使用工厂来获取、回收对象
GameObject.Instantiate是一个非常消耗系统资源的函数。如果每一次我们需要飞碟的时候，我们都使用GameObject.Instantiate，游戏的性能会很差。所以我们现在使用一个工厂来回收利用使用完毕的UFO。**UFO销毁的时候，我们不调用系统的destroy函数，而是仅仅setactive(false)，下次需要UFO的时候让它出现在应该的位置就可以了**。这样做减少了Instantiate和Destroy的调用。

```
public class UFOFactory : MonoBehaviour
{
    Queue<UFOController> freeQueue; // 储存空闲状态的UFO
    List<UFOController> usingList;  // 储存正在使用的UFO

    GameObject originalUFO; // UFO的原型，以后创建UFO就克隆这个对象

    int count = 0;
    void Awake()
    {
        freeQueue = new Queue<UFOController>();
        usingList = new List<UFOController>();

        originalUFO = Instantiate(Resources.Load("ufo", typeof(GameObject))) as GameObject;
        originalUFO.SetActive(false);
    }

    public UFOController produceUFO(UFOAttributes attr)
    {
        UFOController newUFO;
        if (freeQueue.Count == 0)       // 如果没有UFO空闲，则克隆一个对象
        {
            GameObject newObj = GameObject.Instantiate(originalUFO);
            newUFO = new UFOController(newObj);
            newObj.transform.position += Vector3.forward * Random.value * 5;
            count++;
        }
        else                            // 如果有UFO空闲，则取出这个UFO
        {
            newUFO = freeQueue.Dequeue();
        }
        newUFO.setAttr(attr);           // 将UFO的颜色速度大小设置成参数指定的样子
        usingList.Add(newUFO);          // 将UFO加入使用中的队列
        newUFO.appear();
        return newUFO;
    }

    public UFOController[] produceUFOs(UFOAttributes attr, int n)
    {
        // 一次性产生n个UFO

        UFOController[] arr = new UFOController[n];
        for (int i = 0; i < n; i++)
        {
            arr[i] = produceUFO(attr);
        }
        return arr;
    }

    public void recycle(UFOController UFOCtrl)
    {
        // 回收一个UFO，将其加入空闲队列
        UFOCtrl.disappear();
        usingList.Remove(UFOCtrl);
        freeQueue.Enqueue(UFOCtrl);
    }

    public void recycleAll()
    {
        while(usingList.Count != 0)
        {
            recycle(usingList[0]);
        }
    }

    public List<UFOController> getUsingList()
    {
        return usingList;
    }
}
```
除了UFOFactory以外，还有一个ExplosionFactory，作用一样，用来获取和回收“爆炸对象”，因为爆炸也像飞碟一样，频繁产生、消失的。ExplosionFactory的实现很相似，代码我就不放在这里了，要查看的话下载我的项目就可以了。
****
# 使用场景控制器协调各个场景组件
FirstController是场景中最高级别的控制器，所有的部件相互之间不会直接通信，只能与FirstController直接通信，这样可以大大降低各个组件之间的耦合，当我们更改某个部件时，最多只需要修改一下FirstController中的代码就可以了。
![场景控制器所控制的部件](http://upload-images.jianshu.io/upload_images/4888929-723d0857d2b97af4.gif?imageMogr2/auto-orient/strip)

```
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
        // 挂载各种控制组件

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
        // 初始化场景中的物体
        new FirstCharacterController();
        Instantiate(Resources.Load("Terrain"));
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
            print("All UFO is shot down! Next round in 3 sec");
            roundHasStarted = false;
            Invoke("roundStart", 3);
            difficultyManager.setDifficultyByScore(scorer.getScore());
        }
        else if (roundHasStarted && checkTimeOut()) // 检查这一轮是否已经超时
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
        // 开始新的一轮
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
        // 响应UFO被击中的事件
        scorer.record(difficultyManager.getDifficulty());
        actionManager.removeActionOf(UFOCtrl.getObj());
        UFOfactory.recycle(UFOCtrl);
        explosionFactory.explodeAt(UFOCtrl.getObj().transform.position);
    }

    public void GroundIsShot(Vector3 pos) {
        // 响应地面被击中的事件（直接产生一个爆炸）
        explosionFactory.explodeAt(pos);
    }
}
```
****
# 其他的类
其他的类实现非常简单，都是三、四十行代码，也没有涉及新的知识，我就不在这里一一讲解了，大家可以下载我的代码自己查看。
****
# 可以做的改进
* 设计失败的规则，比如规定时间内没拿到多少分就失败。
* 设计一套UI，让用户可以控制游戏的难度。
* “子弹”发射的速度太快了，如果按住鼠标的话，会每一帧发出一条射线。让射速慢下来吧。
* 增加换弹机制。
* 让飞碟在主角身边生成，或者会自动飞到主角附近。

****
# 感悟
我们在上一个游戏的时候，我们定义了几个关于动作的类（**ObjAction、MoveToAction、SequenceAction、ActionManager**）。在这个游戏中，我可以几乎一字未改地复制到了这个游戏中（后来调整了一些参数的顺序），为什么能够复用如此之多的代码？

**这是因为关于动作的基本类与上一个游戏的业务逻辑没有任何关系，这些代码是很容易复用的**。我们上一个游戏的业务逻辑封装在了一个`FirstSceneActionManager`类中，通过调用这些基本类的API来控制动作。

在这个游戏中，我们也是只需要重新写`FirstSceneActionManager`类就可以了，底层的代码不用改变。

这就告诉我们**在实现底层代码的时候不要实现具体的业务逻辑，我们只实现抽象的、通用的、基础的一些功能，当我们针对游戏需要实现业务逻辑的时候，通过调用这些底层的基本功能来完成具体的功能，这样可以让代码的复用最大化**。

在实现底层的类的时候必须要从长远来考虑，我们将来可能需要底层代码来做什么？底层代码的API是否能满足我所有可能的需求？怎么设计API来让它们使用起来更方便？

**如果你以后实现各种业务逻辑的时候，发现一点也不用修改底层的代码，就说明底层这套API实现足够的健壮、通用了。**

**职责分离**也有利于代码的模块化、减少耦合。比如说不要在工厂中直接给产生的飞碟添加动作（因为管理动作不是工厂的职责），而要将飞碟传递给FirstController以后，让FirstController去调用动作管理器来添加。这样就将工厂和动作管理器之间的耦合降低了。将来你想要给飞碟添加更多种运动方式的时候，只需要更改动作管理器类就可以了，完全不用管工厂类。否则，你会发现飞碟一旦生成就会按照旧方式来运动，这样你就要修改更多的代码（既要改动工厂类、又要改动动作管理器类）。

****
感谢阅读！该项目还有后续增强，在[另一个分支](https://github.com/csr632/Unity3DGame-shoot-UFO/tree/Improvement1_PhysicalUFO)。
