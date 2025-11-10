using Pixeye.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    #region 非アクティブにするオブジェクト
    [SerializeField] GameObject camera;
    [SerializeField] GameObject light;
    #endregion

    [SerializeField] GameTimerText gameTimerText;

    static public GameUIManager Instance {  get; private set; }

    private void Awake()
    {
        camera.SetActive(false);
        light.SetActive(false);

        Instance = this;
        StartGameTimerText();
    }

    public void StartGameTimerText()
    {
        gameTimerText.StartCountUpTimer();
    }
}