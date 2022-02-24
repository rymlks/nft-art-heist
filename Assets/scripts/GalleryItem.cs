using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GalleryItem : MonoBehaviour
{
    public Image indicator;

    public void Start()
    {
        indicator.gameObject.SetActive(false);
    }
}
