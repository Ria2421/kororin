using Pixeye.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    static public GameUIManager Instance {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}