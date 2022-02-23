using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Shop : MonoBehaviour
{
    public GameObject[] Sellbuttons;
    public GameObject[] Buybuttons;

    public Text header;


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

    System.Guid myGUID = System.Guid.NewGuid();

    // Start is called before the first frame update
    void Start()
    {
        // 89a89392-6aee-4104-b36b-88867c7b54cb
        myGUID = System.Guid.Parse("89a89392-6aee-4104-b36b-88867c7b54cb");

        //Gallery();
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
        using (UnityWebRequest www = UnityWebRequest.Get("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/myNFTs?secret=foobar&guid=" + myGUID))
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

                Debug.Log("Done deserializing list of entries");
                Debug.Log(entries.entries);
                for (int i = 0; i < 3 && i < entries.entries.Length; i++)
                {
                    Debug.Log(i);
                    SaleEntry entry = entries.entries[i];
                    SerializeTexture importObj = JsonUtility.FromJson<SerializeTexture>(entry.imageData);
                    Texture2D tex = new Texture2D(importObj.x, importObj.y);
                    tex.filterMode = FilterMode.Point;
                    ImageConversion.LoadImage(tex, importObj.bytes);
                    Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);
                    Sellbuttons[i].GetComponentInChildren<Image>().sprite = mySprite;

                    Sellbuttons[i].GetComponentInChildren<Text>().text = entry.name + "\n$" + entry.price;

                    Sellbuttons[i].GetComponentInChildren<NFTData>().name = entry.name;
                    Sellbuttons[i].GetComponentInChildren<NFTData>().guid = System.Guid.Parse(entry.artGuid);
                    Sellbuttons[i].GetComponentInChildren<NFTData>().price = entry.price;
                    Sellbuttons[i].GetComponentInChildren<NFTData>().sold = entry.sold;
                    Sellbuttons[i].GetComponentInChildren<NFTData>().forSale = entry.forSale;
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

        using (UnityWebRequest www = UnityWebRequest.Get("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/browse?secret=foobar&guid=" + myGUID))
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

                Debug.Log("Done deserializing list of entries");
                Debug.Log(entries.entries);
                for (int i=0; i < 3 && i < entries.entries.Length; i++)
                {
                    Debug.Log(i);
                    SaleEntry entry = entries.entries[i];
                    SerializeTexture importObj = JsonUtility.FromJson<SerializeTexture>(entry.imageData);
                    Texture2D tex = new Texture2D(importObj.x, importObj.y);
                    tex.filterMode = FilterMode.Point;
                    ImageConversion.LoadImage(tex, importObj.bytes);
                    Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), Vector2.one);
                    Buybuttons[i].GetComponentInChildren<Image>().sprite = mySprite;
                    Buybuttons[i].GetComponentInChildren<Text>().text = entry.name + "\n$" + entry.price;

                    Buybuttons[i].GetComponentInChildren<NFTData>().name = entry.name;
                    Buybuttons[i].GetComponentInChildren<NFTData>().guid = System.Guid.Parse(entry.artGuid);
                    Buybuttons[i].GetComponentInChildren<NFTData>().price = entry.price;
                    Buybuttons[i].GetComponentInChildren<NFTData>().sold = entry.sold;
                    Buybuttons[i].GetComponentInChildren<NFTData>().forSale = entry.forSale;
                }
            }
        }
    }

    public void Sell(GameObject option)
    {
        StartCoroutine(MakeSale(option));
    }

    IEnumerator MakeSale(GameObject option)
    {
        NFTData data = option.GetComponentInChildren<NFTData>();
        if (data.guid == System.Guid.Empty)
        {
            yield break;
        }

        Texture2D tex = option.GetComponentInChildren<Image>().sprite.texture;

        SerializeTexture exportObj = new SerializeTexture();
        exportObj.x = tex.width;
        exportObj.y = tex.height;
        exportObj.bytes = ImageConversion.EncodeToPNG(tex);
        string imageText = JsonUtility.ToJson(exportObj);

        WWWForm form = new WWWForm();

        SaleEntry entry = new SaleEntry();
        entry.userGuid = myGUID.ToString();
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
                Debug.Log(www.ToString());
            }
        }
    }
}
