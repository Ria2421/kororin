//*********************************************************
// ���炩�ɃV�[���J�ڂ�����X�N���v�g
// Author:Rui Enomoto
//*********************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] GameObject camera;
    [SerializeField] Animator canvasAnimator;
    [SerializeField] float fadeCompDurarion = 1.0f;

    string sceneName;
    public bool isLoading { get; private set; }

    public static LoadingManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Destroy(camera);
            isLoading = false;
            Instance = this;
            canvasAnimator.gameObject.SetActive(false);
            canvasAnimator.enabled = false;

            // �V�[���J�ڂ��Ă��j�����Ȃ��悤�ɂ���
            DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// �񓯊��̃V�[���ǂݍ���
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        yield return new WaitForSeconds(fadeCompDurarion);

        // �V�[����񓯊��Ń��[�h����
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);

        // ���[�h����������܂őҋ@����
        while (true)
        {
            yield return null;

            if (async.isDone)
            {
                canvasAnimator.Play("FadeOut", 0, 0);
                break;
            }
        }

        yield return new WaitForSeconds(fadeCompDurarion);
        canvasAnimator.gameObject.SetActive(false);
        canvasAnimator.enabled = false;
        isLoading = false;
    }

    /// <summary>
    /// �V�[���ǂݍ��݊J�n
    /// </summary>
    /// <param name="sceneName"></param>
    public void StartSceneLoad(string sceneName)
    {
        if (isLoading) return;
        isLoading = true;

        canvasAnimator.gameObject.SetActive(true);
        canvasAnimator.enabled = true;
        canvasAnimator.Play("FadeIn", 0, 0);

        this.sceneName = sceneName;
        StartCoroutine(Load());
    }
}