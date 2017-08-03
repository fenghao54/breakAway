using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//Loading界面处理
public class Loading : MonoBehaviour
{

    private AsyncOperation m_AsyncOperation;//异步操作变量
    public GameObject slider;//进度条slider
    public GameObject loadingText;//UI进度文本


    void Start()
    {
        //初始化进度条与进度值
        slider.GetComponent<Slider>().value = 0;
        loadingText.GetComponent<Text>().text = "Loading:11111";


        StartCoroutine(LoadScene(0));//加载ID0场景

    }

    //
    IEnumerator LoadScene(int sceneID)
    {
 
        int displayProgress = 0;
        int toProgress = 0;
        m_AsyncOperation = SceneManager.LoadSceneAsync(sceneID);//异步加载指定ID场景
        m_AsyncOperation.allowSceneActivation = false;
        while (m_AsyncOperation.progress < 0.9f)
        {
           
            toProgress = (int)m_AsyncOperation.progress * 100;
            Debug.Log("1 "+toProgress);
            toProgress = 100;//
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                Debug.Log("2 " + toProgress);
                loadingText.GetComponent<Text>().text = "loading:"+displayProgress.ToString();
                slider.GetComponent<Slider>().value = displayProgress;
                yield return new WaitForEndOfFrame();
            }


        }
        toProgress = 100;
        while (displayProgress < toProgress)
        {
            ++displayProgress;
            slider.GetComponent<Slider>().value = displayProgress;
            yield return new WaitForEndOfFrame();
        }
        m_AsyncOperation.allowSceneActivation = true;

    }

    void Update()
    {
    }
}
