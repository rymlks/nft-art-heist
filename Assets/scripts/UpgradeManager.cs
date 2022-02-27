using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject tutorialPane;

    bool tutorial = true;

    public void Show()
    {
        if (gameManager.first && tutorial)
        {
            tutorial = false;
            tutorialPane.SetActive(true);
        }
    }

    public void BuyRed(Button button)
    {
        double price = 100;

        if (gameManager.userData.money < price)
        {
            return;
        }
        button.interactable = false;
        Unlock("red", 100);
    }
    public void BuyGreen(Button button)
    {
        double price = 1000;

        if (gameManager.userData.money < price)
        {
            return;
        }
        button.interactable = false;
        Unlock("green", 1000);
    }
    public void BuyBlue(Button button)
    {
        double price = 10000;

        if (gameManager.userData.money < price)
        {
            return;
        }
        button.interactable = false;
        Unlock("blue", 10000);
    }

    public void BuyTime(Button button)
    {
        Unlock("time", 1000);
    }

    public void BuyBuy(Button button)
    {
        double price = 200;

        if (gameManager.userData.money < price)
        {
            return;
        }
        button.interactable = false;
        Unlock("buy", 200);
    }

    public void Unlock(string item, double price)
    {
        if (gameManager.userData.money >= price)
        {
            StartCoroutine(RequestAndUpdate(item, price));
        }
    }

    public IEnumerator RequestAndUpdate(string item, double price)
    {
        WWWForm form = new WWWForm();

        string URL = "https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/buyUpgrade?secret=foobar&guid=" + gameManager.guid + "&item=" + item + "&price=" + price;

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {

            yield return www.SendWebRequest();

            if (www.error != null)
            {
                Debug.Log("fucked");
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log( www.downloadHandler.text);
                gameManager.userData = JsonUtility.FromJson<JSONTypes.UserData>(www.downloadHandler.text);
            }
        }
        gameManager.UpdateMoneyText();
    }
}
