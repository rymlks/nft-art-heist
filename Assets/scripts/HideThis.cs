using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideThis : MonoBehaviour
{
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
