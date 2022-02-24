using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ImageSetter : MonoBehaviour
{
    public Button ButtonToUse;
    public Sprite PNGToUse;
    public string TextYouWantOnTheButton;
    void Awake()
    {
        Transform btuTranform = ButtonToUse.transform;
        btuTranform.GetComponent<Image>().sprite = PNGToUse;
        btuTranform.GetChild(0).GetComponent<TMP_Text>().text = TextYouWantOnTheButton;
    }
}
