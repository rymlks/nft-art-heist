using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Shop : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject[] Sellbuttons;
    public GameObject[] Buybuttons;

    public Text header;

    public Sprite heistSprite;
    public Sprite soldSprite;
    public Sprite forSaleSprite;
    public Sprite buySprite;
    public Sprite emptySprite;

    public Sprite heistPreviewSprite;

    public GameObject HeistTutorialPane;
    public GameObject SellTutorialPane;
    public GameObject BuyTutorialPane;

    public Button buyButton;

    private bool success = true;

    public bool sellTutorial = true;
    public bool buyTutorial = true;

    public void Start()
    {
        if (gameManager.first)
        {
            HeistTutorialPane.SetActive(true);
        }
        SellTutorialPane.SetActive(false);
        BuyTutorialPane.SetActive(false);
    }

    public void Gallery()
    {
        header.text = "Click an NFT to\nmake a sale offer\n";

        if (sellTutorial && gameManager.first)
        {
            SellTutorialPane.SetActive(true);
            sellTutorial = false;
        }

        foreach (GameObject button in Sellbuttons)
        {
            button.SetActive(true);
            ResetButton(button);
        }

        foreach (GameObject button in Buybuttons)
        {
            button.SetActive(false);
            ResetButton(button);
        }

        StartCoroutine(RequestGallery());
    }

    public void BrowseSales()
    {

        if (buyTutorial)
        {
            BuyTutorialPane.SetActive(true);
            buyTutorial = false;
        }

        header.text = "Click an NFT to\nmake a buy offer";
        foreach (GameObject button in Sellbuttons)
        {
            button.SetActive(false);
            ResetButton(button);
        }

        foreach (GameObject button in Buybuttons)
        {
            button.SetActive(true);
            ResetButton(button);
        }

        StartCoroutine(RequestBrowseSales());
    }

    public void ShowHeists()
    {
        header.text = "Click an NFT to\nstart a heist";

        foreach (GameObject button in Sellbuttons)
        {
            button.SetActive(true);
            ResetButton(button);
        }

        foreach (GameObject button in Buybuttons)
        {
            button.SetActive(false);
            ResetButton(button);
        }

        SetHeists();
    }

    public void DisableBuying()
    {
        buyButton.interactable = false;
        ShowHeists();
    }

    private void ResetButton(GameObject button)
    {
        button.GetComponentInChildren<Image>().sprite = emptySprite;
        button.GetComponentInChildren<Text>().text = "";
        button.GetComponentInChildren<InputField>().text = "";
        button.GetComponentInChildren<InputField>().interactable = false;

        NFTData data = button.GetComponentInChildren<NFTData>();
        data.name = "";
        data.guid = System.Guid.Empty;
        data.price = 0;
        data.sold = false;
        data.forSale = false;
        data.isHeist = false;
        data.texture = null;
        button.GetComponent<GalleryItem>().indicator.gameObject.SetActive(false);
    }

    public void Sell(GameObject option)
    {
        NFTData data = option.GetComponentInChildren<NFTData>();
        // Start Heist
        if (data.isHeist)
        {
            gameManager.StartDrawing(option);
        // Sell NFT
        } else if (data.guid != System.Guid.Empty)
        {
            if (data.sold)
            {
                StartCoroutine(RequestCollection(option));
            } else if (!data.forSale)
            {
                StartCoroutine(MakeSale(option));

                if (data.price < data.appraisedValue * 1.1 && data.price <= 100)
                {
                    gameManager.userData.money += data.price;
                    gameManager.UpdateMoneyText();
                }
            } else
            {
                StartCoroutine(CancelSale(option));
            }
        }
    }

    public void Buy(GameObject option)
    {
        NFTData data = option.GetComponentInChildren<NFTData>();
        if (data.guid == System.Guid.Empty)
        {
            Debug.Log("Cannot buy empty NFT");
        } else if (data.price > gameManager.userData.money)
        {
            Debug.Log("Not enough money");
        }
        else
        {
            StartCoroutine(MakePurchase(option));
        }
    }

    public void UpdatePrice(GameObject option)
    {
        NFTData data = option.GetComponent<NFTData>();
        try
        {
            data.price = double.Parse(option.GetComponentInChildren<InputField>().text);
        } catch
        {
            option.GetComponentInChildren<InputField>().text = data.price.ToString("n2");
        }
        StartCoroutine(RequestUpdatePrice(option));
    }

    IEnumerator RequestGallery()
    {
        return RequestAndFill(null, "myNFTs", SetGallery);
    }

    IEnumerator RequestBrowseSales()
    {
        return RequestAndFill(null, "browse", SetBrowse);
    }

    IEnumerator MakeSale(GameObject option)
    {
        return RequestAndFill(option, "sell", SetGallery);
    }

    IEnumerator CancelSale(GameObject option)
    {
        return RequestAndFill(option, "cancelSale", SetGallery);
    }

    IEnumerator MakePurchase(GameObject option)
    {
        double cost = double.Parse(option.GetComponentInChildren<InputField>().text);
        yield return RequestAndFill(option, "buy", SetBrowse);
        if (success && cost <= gameManager.userData.money)
        {
            gameManager.userData.money -= cost;
            gameManager.UpdateMoneyText();
        }
    }

    IEnumerator RequestCollection(GameObject option)
    {
        double profit = double.Parse(option.GetComponentInChildren<InputField>().text);
        yield return RequestAndFill(option, "collectSale", SetGallery);
        if (success)
        {
            gameManager.userData.money += profit;
            gameManager.UpdateMoneyText();
        }
    }

    IEnumerator RequestUpdatePrice(GameObject option)
    {
        return RequestAndFill(option, "updatePrice", SetGallery);
    }

    private void PopulateForm(WWWForm form, GameObject option)
    {
        NFTData data = option.GetComponent<NFTData>();
        Texture2D tex = option.GetComponent<Image>().sprite.texture;

        JSONTypes.SerializeTexture exportObj = new JSONTypes.SerializeTexture();
        exportObj.x = tex.width;
        exportObj.y = tex.height;
        exportObj.bytes = ImageConversion.EncodeToPNG(tex);
        string imageText = JsonUtility.ToJson(exportObj);


        JSONTypes.SaleEntry entry = new JSONTypes.SaleEntry();
        entry.userGuid = data.ownerGuid.ToString();
        entry.artGuid = data.guid.ToString();
        entry.name = data.name;
        entry.price = data.price;
        entry.imageData = imageText;
        entry.sold = data.sold;
        entry.forSale = data.forSale;
        form.AddField("entry", JsonUtility.ToJson(entry));
    }

    private void SetGallery(string serverResponse)
    {
        JSONTypes.SaleEntryList entries = JsonUtility.FromJson<JSONTypes.SaleEntryList>(serverResponse);

        gameManager.NFTs = new List<NFTData>();
        for (int i = 0; i < Sellbuttons.Length; i++)
        {
            GameObject button = Sellbuttons[i];
            GalleryItem galleryItem = button.GetComponent<GalleryItem>();
            // Show your own art
            if (i < entries.entries.Length)
            {
                JSONTypes.SaleEntry entry = entries.entries[i];

                if (entry.sold)
                {
                    galleryItem.indicator.gameObject.SetActive(true);
                    galleryItem.indicator.sprite = soldSprite;
                }
                else if (entry.forSale)
                {
                    galleryItem.indicator.gameObject.SetActive(true);
                    galleryItem.indicator.sprite = forSaleSprite;
                }
                else
                {
                    galleryItem.indicator.gameObject.SetActive(false);
                }

                JSONTypes.SerializeTexture importObj = JsonUtility.FromJson<JSONTypes.SerializeTexture>(entry.imageData);
                Texture2D tex = new Texture2D(importObj.x, importObj.y);
                tex.filterMode = FilterMode.Point;


                ImageConversion.LoadImage(tex, importObj.bytes);
                Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);
                button.GetComponentInChildren<Image>().sprite = mySprite;

                button.GetComponentInChildren<Text>().text = "Appraised: $" + entry.appraisedValue.ToString("N2");

                button.GetComponentInChildren<NFTData>().name = entry.name;
                button.GetComponentInChildren<NFTData>().ownerGuid = System.Guid.Parse(entry.userGuid);
                button.GetComponentInChildren<NFTData>().guid = System.Guid.Parse(entry.artGuid);
                button.GetComponentInChildren<NFTData>().price = entry.price;
                button.GetComponentInChildren<NFTData>().appraisedValue = entry.appraisedValue;
                button.GetComponentInChildren<NFTData>().sold = entry.sold;
                button.GetComponentInChildren<NFTData>().forSale = entry.forSale;
                button.GetComponentInChildren<NFTData>().texture = tex;


                gameManager.NFTs.Add(new NFTData(button.GetComponentInChildren<NFTData>()));

                button.GetComponentInChildren<InputField>().text = entry.price.ToString("N2");

                if (!entry.sold && !entry.forSale)
                {
                    button.GetComponentInChildren<InputField>().interactable = true;
                }
                else
                {
                    button.GetComponentInChildren<InputField>().interactable = false;
                }

            }
            else 
            {
                ResetButton(button);
            }
        }
    }

    private void SetHeists()
    {
        int idx = 0;
        for (int i = 0; i < Sellbuttons.Length; i++)
        {
            GameObject button = Sellbuttons[i];
            GalleryItem galleryItem = button.GetComponent<GalleryItem>();
            galleryItem.indicator.gameObject.SetActive(true);
            galleryItem.indicator.sprite = heistSprite;

            float rnd = UnityEngine.Random.value;
            double price;


            if (rnd < 0.5 && idx < gameManager.NFTs.Count)
            {
                Texture2D tex = gameManager.NFTs[idx].texture;
                button.GetComponentInChildren<Image>().sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);
                price = gameManager.NFTs[idx].appraisedValue;
                idx++;
            } else
            {
                button.GetComponentInChildren<NFTGenerator>().GenerateNFT();
                price = button.GetComponentInChildren<NFTGenerator>().value;
            }





            button.GetComponentInChildren<Text>().text = "Heist Contract";
            button.GetComponentInChildren<NFTData>().price = price;
            button.GetComponentInChildren<NFTData>().appraisedValue = price;
            button.GetComponentInChildren<NFTData>().ownerGuid = gameManager.guid;
            button.GetComponentInChildren<NFTData>().isHeist = true;
            button.GetComponentInChildren<NFTData>().texture = heistPreviewSprite.texture;

            button.GetComponentInChildren<InputField>().text = price.ToString("n2");
            button.GetComponentInChildren<InputField>().interactable = false;
        }
    }

    private void SetBrowse(string serverResponse)
    {

        JSONTypes.SaleEntryList entries = JsonUtility.FromJson<JSONTypes.SaleEntryList>(serverResponse);

        for (int i = 0; i < Buybuttons.Length; i++)
        {
            GameObject button = Buybuttons[i];
            GalleryItem galleryItem = button.GetComponent<GalleryItem>();
            if (i < entries.entries.Length)
            {
                JSONTypes.SaleEntry entry = entries.entries[i];
                JSONTypes.SerializeTexture importObj = JsonUtility.FromJson<JSONTypes.SerializeTexture>(entry.imageData);
                Texture2D tex = new Texture2D(importObj.x, importObj.y);
                tex.filterMode = FilterMode.Point;
                ImageConversion.LoadImage(tex, importObj.bytes);

                galleryItem.indicator.sprite = buySprite;
                galleryItem.indicator.gameObject.SetActive(true);

                Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);
                button.GetComponentInChildren<Image>().sprite = mySprite;
                button.GetComponentInChildren<Text>().text = "Appraised: " + entry.appraisedValue.ToString("$0.00");

                button.GetComponentInChildren<NFTData>().name = entry.name;
                button.GetComponentInChildren<NFTData>().guid = System.Guid.Parse(entry.artGuid);
                button.GetComponentInChildren<NFTData>().ownerGuid = System.Guid.Parse(entry.userGuid);
                button.GetComponentInChildren<NFTData>().price = entry.price;
                button.GetComponentInChildren<NFTData>().appraisedValue = entry.appraisedValue;
                button.GetComponentInChildren<NFTData>().sold = entry.sold;
                button.GetComponentInChildren<NFTData>().forSale = entry.forSale;
                button.GetComponentInChildren<NFTData>().texture = tex;

                button.GetComponentInChildren<InputField>().text = entry.price.ToString("N2");
                button.GetComponentInChildren<InputField>().interactable = false;
            } else
            {
                ResetButton(button);
            }
        }
    }

    private IEnumerator RequestAndFill(GameObject option, string endpoint, Action<string> response)
    {
        WWWForm form = new WWWForm();

        string URL = "https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/" + endpoint + "?secret=foobar&guid=" + gameManager.guid;

        if (option != null)
        {
            PopulateForm(form, option);
        }
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {

            yield return www.SendWebRequest();

            if (www.error != null)
            {
                success = false;
                Debug.Log("fucked");
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                success = true;
                Debug.Log(endpoint + ": " + www.downloadHandler.text);
                response(www.downloadHandler.text);
            }
        }
    }
}
