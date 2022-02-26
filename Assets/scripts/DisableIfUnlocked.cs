using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableIfUnlocked : MonoBehaviour
{
    public GameManager gameManager;
    public string item;

    bool check = true;

    // Update is called once per frame
    void Update()
    {
        if (!check) return;

        foreach (string upgrade in gameManager.userData.upgrades)
        {
            if (item.Equals(upgrade))
            {
                GetComponent<Button>().interactable = false;
                check = false;
            }
        }
    }
}
