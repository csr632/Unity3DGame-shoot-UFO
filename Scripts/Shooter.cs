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
                    sceneController.GroundIsShot(hit.point);
                }
            }
            if (muzzleFlashEnable == false) // 显示枪口火焰
            {
                muzzleFlashEnable = true;
                muzzleFlash.SetActive(true);
            }
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
