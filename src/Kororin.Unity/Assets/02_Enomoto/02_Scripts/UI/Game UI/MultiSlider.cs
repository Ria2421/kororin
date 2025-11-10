using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiSlider : MonoBehaviour
{
    [SerializeField]
    List<Slider> sliders = new List<Slider>();

    [SerializeField]
    List<Sprite> characterIcons = new List<Sprite>();

    void Start()
    {
        StartCoroutine(UpdateSliders());
    }

    IEnumerator UpdateSliders()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            //for (int i = 0; i < sliders.Count; i++)
            //{

            //}
        }
    }
}
