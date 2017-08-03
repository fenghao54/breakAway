using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class GameStatic : MonoBehaviour {

    public Slider bg_VolumeSlider;//背景音乐大小slider
    public Toggle bg_MuteToggle;//背景音乐静音toggle
    public Text bg_VolumeText;//背景音量Text

    public static float bg_volume=0.1f;//默认背景音乐音量
    public static bool bgSoundIsMute = false;//背景音乐是否静音
    void Start () {
        DontDestroyOnLoad(this.gameObject);//不释放GameStatic物体，作为设置参数来源

        bg_MuteToggle.onValueChanged.AddListener(bg_ToggleClick);//监听toggle.isOn的布尔值作为参数传入监听方法
        bg_VolumeSlider.onValueChanged.AddListener(bg_VolumeChange);//监听Slider的value值作为参数传入监听方法

        //初始化背景音乐音量
        bg_VolumeText.text = (int)(bg_VolumeSlider.value * 100) + "%";

    }
    //背景音量Slider监听处理
    void bg_VolumeChange(float volume)
    {
        bg_volume = volume;//更新音量
        bg_VolumeText.text = (int)(bg_VolumeSlider.value * 100) + "%";//更新音量显示
        if (volume == 0)
        {
            bg_MuteToggle.isOn = false;
        }
        else
        {
            bg_MuteToggle.isOn = true;
        }
    }

    //背景音乐静音toggle监听处理
    void bg_ToggleClick(bool istrue)
    {
        if (istrue)//取消静音恢复默认
        {
            Debug.Log("true");
            bgSoundIsMute = true;
            bg_VolumeSlider.value = 0.1f;

        }
        else//静音
        {
            Debug.Log("false");
            bgSoundIsMute = false;
            bg_VolumeSlider.value = 0;
        }

    }

}
