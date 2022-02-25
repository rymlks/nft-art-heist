using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class DrawManager : MonoBehaviour
{
    public GameManager gameManager;

    public Image myImage;
    public Text myImageText;
    public Text countdownText;

    public Image previewImage;
    public Text previewText;

    Texture2D drawing;
    Texture2D reference;
    bool drawingActive = false;
    int secondsLeft = 0;

    NFTData currentData;
    NFTData referenceData;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void StartDrawing(GameObject option)
    {
        secondsLeft = 30;
        referenceData = option.GetComponentInChildren<NFTData>();
        currentData = option.AddComponent<NFTData>();
        currentData.Copy(referenceData);
        reference = option.GetComponentInChildren<Image>().sprite.texture;

        previewImage.sprite = Sprite.Create(reference, new Rect(0.0f, 0.0f, reference.width, reference.height), Vector2.one);

        previewText.text = referenceData.name + "\n" + referenceData.price.ToString("$0.00");

        drawing = new Texture2D(32, 32);
        drawing.filterMode = FilterMode.Point;

        for (int x = 0; x < drawing.width; x++)
        {
            for (int y = 0; y < drawing.height; y++)
            {
                drawing.SetPixel(x, y, new Color(1, 1, 1, 0.5f));
            }
        }
        drawing.Apply();
        Sprite mySprite = Sprite.Create(drawing, new Rect(0.0f, 0.0f, drawing.width, drawing.height), Vector2.one);
        myImage.sprite = mySprite;
        myImage.color = Color.white;

        option.GetComponentInChildren<Image>().sprite = mySprite;

        drawingActive = true;
        StartCoroutine(CountDown());
    }

    public void StopDrawing()
    {
        drawingActive = false;
        Destroy(referenceData);
        StartCoroutine(SaveDrawing());
    }

    IEnumerator SaveDrawing()
    {

        JSONTypes.SerializeTexture exportObj = new JSONTypes.SerializeTexture();
        exportObj.x = drawing.width;
        exportObj.y = drawing.height;
        exportObj.bytes = ImageConversion.EncodeToPNG(drawing);
        string imageText = JsonUtility.ToJson(exportObj);

        WWWForm form = new WWWForm();

        JSONTypes.SaleEntry entry = new JSONTypes.SaleEntry();
        entry.userGuid = gameManager.guid.ToString();
        entry.artGuid = currentData.guid.ToString();
        entry.name = currentData.name;
        entry.price = currentData.price;
        entry.imageData = imageText;
        entry.sold = false;
        entry.forSale = false;

        form.AddField("entry", JsonUtility.ToJson(entry));

        using (UnityWebRequest www = UnityWebRequest.Post("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/save?secret=foobar&guid="+gameManager.guid, form))
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

        gameManager.EndDrawing(drawing);
    }

    public void updateCurrentPrice()
    {
        double diffValue = 0;
        for (int x = 0; x < drawing.width; x++)
        {
            for (int y = 0; y < drawing.height; y++)
            {
                Color myColor = drawing.GetPixel(x, y);
                Color theirColor = reference.GetPixel(reference.width * x / drawing.width, reference.height * y / drawing.height);

                Color diff = myColor - theirColor;
                diffValue += (Mathf.Abs(diff.r) + Mathf.Abs(diff.g) + Mathf.Abs(diff.b) + Mathf.Abs(diff.a)) / 4.0;
            }
        }
        diffValue /= drawing.width * drawing.height;

        //Debug.Log(diffValue);

        currentData.price = referenceData.price * 1.1 * (1 - diffValue);

        myImageText.text = currentData.name + "\n" + currentData.price.ToString("$0.00");
    }

    IEnumerator CountDown()
    {
        while (secondsLeft > 0)
        {
            int minutes = secondsLeft / 60;
            int seconds = secondsLeft % 60;
            countdownText.text = string.Format("{0}:{1}", minutes, seconds.ToString("00"));
            updateCurrentPrice();

            secondsLeft--;
            yield return new WaitForSeconds(1);
        }
        countdownText.text = "0:00";

        StopDrawing();
    }

    // Update is called once per frame
    void Update()
    {
        if (!drawingActive) return;

        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        myImage.rectTransform.GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2]-corners[0]);

        Vector3 translatedMousePos = mousePos - corners[0];
        
        int width = drawing.width;

        int height = drawing.height;

        translatedMousePos.x = Mathf.Floor(width * translatedMousePos.x/newRect.width);
        translatedMousePos.y =  Mathf.Floor(height * translatedMousePos.y/newRect.height);

        bool widthValid = translatedMousePos.x >=0 && translatedMousePos.x < drawing.width;
        bool heightValid = translatedMousePos.y >=0 && translatedMousePos.y < drawing.height;
        bool isMouseDown = Input.GetMouseButton(0);

        if (widthValid && heightValid && isMouseDown) {

            drawing.SetPixel((int) translatedMousePos.x, (int) translatedMousePos.y, Color.black);
            drawing.Apply();

        }

    }

    private void OnMouseDrag()
    {
    }

}
