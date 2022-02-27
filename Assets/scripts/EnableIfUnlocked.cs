using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableIfUnlocked : MonoBehaviour
{
    public GameManager gameManager;
    public string item;

    bool check = true;

    // Update is called once per frame
    void Update()
    {
        if (!check) return;
        GetComponent<Button>().interactable = false;

        foreach (string upgrade in gameManager.userData.upgrades)
        {
            if (item.Equals(upgrade))
            {
                GetComponent<Button>().interactable = true;
                check = false;
            }
        }
    }
}
