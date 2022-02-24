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

    [System.Serializable]
    public class SerializeTexture
    {
        [SerializeField]
        public int x;
        [SerializeField]
        public int y;
        [SerializeField]
        public byte[] bytes;
    }

    [System.Serializable]
    public class SaleEntry
    {
        [SerializeField]
        public string userGuid;
        [SerializeField]
        public string artGuid;
        [SerializeField]
        public string name;
        [SerializeField]
        public double price;
        [SerializeField]
        public string imageData;
        [SerializeField]
        public bool sold;
        [SerializeField]
        public bool forSale;
    }

    [System.Serializable]
    public class SaleEntryList
    {
        [SerializeField]
        public SaleEntry[] entries;
    }


    // Start is called before the first frame update
    void Start()
    {

        Gallery();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Gallery()
    {
        header.text = "Click an NFT to\nmake a sale offer";

        foreach (GameObject button in Sellbuttons)
        {
            button.GetComponentInChildren<Image>().sprite = null;
            button.GetComponentInChildren<Text>().text = "";
            button.SetActive(true);
            button.GetComponentInChildren<NFTData>().name = "";
            button.GetComponentInChildren<NFTData>().guid = System.Guid.Empty;
            button.GetComponentInChildren<NFTData>().price = 0;
            button.GetComponentInChildren<NFTData>().sold = false;
            button.GetComponentInChildren<NFTData>().forSale = false;
        }

        foreach (GameObject button in Buybuttons)
        {
            button.GetComponentInChildren<Image>().sprite = null;
            button.GetComponentInChildren<Text>().text = "";
            button.SetActive(false);
            button.GetComponentInChildren<NFTData>().name = "";
            button.GetComponentInChildren<NFTData>().guid = System.Guid.Empty;
            button.GetComponentInChildren<NFTData>().price = 0;
            button.GetComponentInChildren<NFTData>().sold = false;
            button.GetComponentInChildren<NFTData>().forSale = false;
        }

        StartCoroutine(RequestGallery());
    }

    IEnumerator RequestGallery()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/myNFTs?secret=foobar&guid=" + gameManager.guid))
        {

            yield return www.SendWebRequest();

            if (www.responseCode != 200)
            {
                Debug.Log("fucked");
                Debug.Log(www.error);
                Debug.Log(www.ToString());
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                SaleEntryList entries = JsonUtility.FromJson<SaleEntryList>(www.downloadHandler.text);

                for (int i = 0; i < Sellbuttons.Length; i++)
                {
                    // Show your own art
                    if (i < entries.entries.Length )
                    {
                        SaleEntry entry = entries.entries[i];
                        SerializeTexture importObj = JsonUtility.FromJson<SerializeTexture>(entry.imageData);
                        Texture2D tex = new Texture2D(importObj.x, importObj.y);
                        tex.filterMode = FilterMode.Point;
                        ImageConversion.LoadImage(tex, importObj.bytes);
                        Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);
                        Sellbuttons[i].GetComponentInChildren<Image>().sprite = mySprite;

                        Sellbuttons[i].GetComponentInChildren<Text>().text = entry.name + "\n" + entry.price.ToString("$0.00");

                        Sellbuttons[i].GetComponentInChildren<NFTData>().name = entry.name;
                        Sellbuttons[i].GetComponentInChildren<NFTData>().guid = System.Guid.Parse(entry.artGuid);
                        Sellbuttons[i].GetComponentInChildren<NFTData>().price = entry.price;
                        Sellbuttons[i].GetComponentInChildren<NFTData>().sold = entry.sold;
                        Sellbuttons[i].GetComponentInChildren<NFTData>().forSale = entry.forSale;

                    } else // Show heist option
                    {
                        double price = Mathf.Round(Random.value * 10000) / 100.0d;

                        Sellbuttons[i].GetComponentInChildren<Text>().text = "Heist Contract\n" + price.ToString("$0.00");
                        Sellbuttons[i].GetComponentInChildren<Image>().sprite = heistSprite;
                        Sellbuttons[i].GetComponentInChildren<NFTData>().price = price;


                    }
                }
            }
        }
    }

    public void BrowseSales()
    {
        header.text = "Click an NFT to\nmake a buy offer";
        foreach (GameObject button in Sellbuttons)
        {
            button.GetComponentInChildren<Image>().sprite = null;
            button.GetComponentInChildren<Text>().text = "";
            button.SetActive(false);

            button.GetComponentInChildren<NFTData>().name = "";
            button.GetComponentInChildren<NFTData>().guid = System.Guid.Empty;
            button.GetComponentInChildren<NFTData>().price = 0;
            button.GetComponentInChildren<NFTData>().sold = false;
            button.GetComponentInChildren<NFTData>().forSale = false;
        }

        foreach (GameObject button in Buybuttons)
        {
            button.GetComponentInChildren<Image>().sprite = null;
            button.GetComponentInChildren<Text>().text = "";
            button.SetActive(true);
            button.GetComponentInChildren<NFTData>().name = "";
            button.GetComponentInChildren<NFTData>().guid = System.Guid.Empty;
            button.GetComponentInChildren<NFTData>().price = 0;
            button.GetComponentInChildren<NFTData>().sold = false;
            button.GetComponentInChildren<NFTData>().forSale = false;
        }

        StartCoroutine(RequestBrowseSales());
    }

    IEnumerator RequestBrowseSales()
    {

        using (UnityWebRequest www = UnityWebRequest.Get("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/browse?secret=foobar&guid=" + gameManager.guid))
        {

            yield return www.SendWebRequest();

            if (www.responseCode != 200)
            {
                Debug.Log("fucked");
                Debug.Log(www.error);
                Debug.Log(www.ToString());
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                SaleEntryList entries = JsonUtility.FromJson<SaleEntryList>(www.downloadHandler.text);

                for (int i=0; i < 3 && i < entries.entries.Length; i++)
                {
                    SaleEntry entry = entries.entries[i];
                    SerializeTexture importObj = JsonUtility.FromJson<SerializeTexture>(entry.imageData);
                    Texture2D tex = new Texture2D(importObj.x, importObj.y);
                    tex.filterMode = FilterMode.Point;
                    ImageConversion.LoadImage(tex, importObj.bytes);
                    Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);
                    Buybuttons[i].GetComponentInChildren<Image>().sprite = mySprite;
                    Buybuttons[i].GetComponentInChildren<Text>().text = entry.name + "\n" + entry.price.ToString("$0.00");

                    Buybuttons[i].GetComponentInChildren<NFTData>().name = entry.name;
                    Buybuttons[i].GetComponentInChildren<NFTData>().guid = System.Guid.Parse(entry.artGuid);
                    Buybuttons[i].GetComponentInChildren<NFTData>().ownderGuid = System.Guid.Parse(entry.userGuid);
                    Buybuttons[i].GetComponentInChildren<NFTData>().price = entry.price;
                    Buybuttons[i].GetComponentInChildren<NFTData>().sold = entry.sold;
                    Buybuttons[i].GetComponentInChildren<NFTData>().forSale = entry.forSale;
                }
            }
        }
    }

    public void Sell(GameObject option)
    {
        NFTData data = option.GetComponentInChildren<NFTData>();
        // Start Heist
        if (data.guid == System.Guid.Empty)
        {
            gameManager.StartDrawing(option);
        // Sell NFT
        } else
        {
            StartCoroutine(MakeSale(option));
        }
    }

    IEnumerator MakeSale(GameObject option)
    {
        NFTData data = option.GetComponentInChildren<NFTData>();

        Texture2D tex = option.GetComponentInChildren<Image>().sprite.texture;

        SerializeTexture exportObj = new SerializeTexture();
        exportObj.x = tex.width;
        exportObj.y = tex.height;
        exportObj.bytes = ImageConversion.EncodeToPNG(tex);
        string imageText = JsonUtility.ToJson(exportObj);

        WWWForm form = new WWWForm();

        SaleEntry entry = new SaleEntry();
        entry.userGuid = gameManager.guid.ToString();
        entry.artGuid = data.guid.ToString();
        entry.name = data.name;
        entry.price = data.price;
        entry.imageData = imageText;
        entry.sold = false;
        entry.forSale = true;

        form.AddField("entry", JsonUtility.ToJson(entry));

        using (UnityWebRequest www = UnityWebRequest.Post("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/sell?secret=foobar", form))
        {

            yield return www.SendWebRequest();

            if (www.responseCode != 200)
            {
                Debug.Log("fucked");
                Debug.Log(www.error);
                Debug.Log(www.ToString());
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    public void Buy(GameObject option)
    {
        NFTData data = option.GetComponentInChildren<NFTData>();
        if (data.guid == System.Guid.Empty)
        {
            Debug.Log("Empty");
        }
        else
        {
            StartCoroutine(MakePurchase(option));
        }
    }

    IEnumerator MakePurchase(GameObject option)
    {
        NFTData data = option.GetComponentInChildren<NFTData>();
        Texture2D tex = option.GetComponentInChildren<Image>().sprite.texture;

        SerializeTexture exportObj = new SerializeTexture();
        exportObj.x = tex.width;
        exportObj.y = tex.height;
        exportObj.bytes = ImageConversion.EncodeToPNG(tex);
        string imageText = JsonUtility.ToJson(exportObj);

        WWWForm form = new WWWForm();

        SaleEntry entry = new SaleEntry();
        entry.userGuid = data.ownderGuid.ToString();
        entry.artGuid = data.guid.ToString();
        entry.name = data.name;
        entry.price = data.price;
        entry.imageData = imageText;
        entry.sold = true;
        entry.forSale = true;

        form.AddField("entry", JsonUtility.ToJson(entry));

        using (UnityWebRequest www = UnityWebRequest.Post("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/buy?secret=foobar&guid=" + gameManager.guid, form))
        {

            yield return www.SendWebRequest();

            if (www.error != null)
            {
                Debug.Log("fucked with code: " + www.responseCode);
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                BrowseSales();
            }
        }
    }
}
