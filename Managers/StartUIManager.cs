using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StartUIManager : MonoBehaviour
{
    //按钮
    public Button start_Btn;
    public Button setting_Btn;
    public Button settingClose_Btn;
    public Slider bg_VolumeSlider;//背景音乐大小slider
    public Toggle bg_MuteToggle;//背景音乐静音toggle
    public Text bg_VolumeText;//背景音量Text
    //设置页面
    public RectTransform setting_Transform;

    public float bg_volume;//背景音乐音量
    public bool bgSoundIsMute = false;//背景音乐是否静音

    void Start()
    {


        start_Btn.onClick.AddListener(LoadingScene);//开始按钮监听
        setting_Btn.onClick.AddListener(OnSettingClick);//设置按钮监听
        settingClose_Btn.onClick.AddListener(OnSettingCloseClick);//关闭设置按钮监听

        Tweener tweener = setting_Transform.DOLocalMove(new Vector3(0, 0, 0), 0.5f);
        tweener.SetAutoKill(false);
        tweener.Pause();
    }



    //开始按钮点击监听方法
    void LoadingScene()
    {
        SceneManager.LoadScene("Loading");//进入加载界面
    }
    //设置按钮点击监听方法
    void OnSettingClick()
    {
        setting_Transform.DOPlayForward();
    }
    //关闭设置按钮点击监听方法
    void OnSettingCloseClick()
    {
        setting_Transform.DOPlayBackwards();
    }

}
