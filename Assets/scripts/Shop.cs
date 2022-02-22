using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Shop : MonoBehaviour
{
    public GameObject[] buttons;


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
        public string guid;
        [SerializeField]
        public string name;
        [SerializeField]
        public double price;
        [SerializeField]
        public string imageData;
        [SerializeField]
        public bool sold;
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Sell(GameObject option)
    {
        StartCoroutine(MakeSale(option));
    }

    public void Browse()
    {
        foreach (GameObject button in buttons)
        {
            button.GetComponentInChildren<Image>().sprite = null;
            button.GetComponentInChildren<Text>().text = "";
        }

        StartCoroutine(RequestBrowse());
    }

    IEnumerator RequestBrowse()
    {

        using (UnityWebRequest www = UnityWebRequest.Get("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/browse?secret=foobar"))
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
                    buttons[i].GetComponentInChildren<Image>().sprite = mySprite;

                    buttons[i].GetComponentInChildren<Text>().text = entry.name + "\n$" + entry.price;
                }
            }
        }
    }

    IEnumerator MakeSale(GameObject option)
    {
        Texture2D tex = option.GetComponentInChildren<Image>().sprite.texture;

        string labelText = option.GetComponentInChildren<Text>().text;

        string name = labelText.Split('\n')[0];
        double price = double.Parse(labelText.Split('\n')[1].Substring(1));

        SerializeTexture exportObj = new SerializeTexture();
        exportObj.x = tex.width;
        exportObj.y = tex.height;
        exportObj.bytes = ImageConversion.EncodeToPNG(tex);
        string imageText = JsonUtility.ToJson(exportObj);

        WWWForm form = new WWWForm();

        SaleEntry entry = new SaleEntry();
        entry.guid = myGUID.ToString();
        entry.name = name;
        entry.price = price;
        entry.imageData = imageText;
        entry.sold = false;

        //form.AddField("dataSource", "nft-art-heist-0");
        //form.AddField("database", "testingArt");
        //form.AddField("collection", "artworks");
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
