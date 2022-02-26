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
    public Text FlavorText;

    public Image previewImage;
    public Text previewText;

    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;
    public Slider alphaSlider;
    public Slider sizeSlider;


    public GameObject tutorialPane;

    Texture2D drawing;
    Texture2D reference;
    bool drawingActive = false;
    int secondsLeft = 0;

    NFTData currentData;
    NFTData referenceData;

    bool stroking = false;

    // Start is called before the first frame update
    void Start()
    {
        tutorialPane.SetActive(true);
    }

    public void ResetTimer()
    {
        secondsLeft = 30;
        int minutes = secondsLeft / 60;
        int seconds = secondsLeft % 60;
        countdownText.text = string.Format("{0}:{1}", minutes, seconds.ToString("00"));

    }

    public void StartDrawing(GameObject option)
    {
        redSlider.interactable = true;
        greenSlider.interactable = true;
        blueSlider.interactable = true;
        sizeSlider.interactable = true;
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
                drawing.SetPixel(x, y, new Color(0,0,0,0));
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
        redSlider.interactable = false;
        greenSlider.interactable = false;
        blueSlider.interactable = false;
        sizeSlider.interactable = false;

        FlavorText.text = "Finalizing.";
        yield return new WaitForSeconds(2);
        FlavorText.text = "Finalizing..";
        yield return new WaitForSeconds(2);
        FlavorText.text = "Finalizing...";
        yield return new WaitForSeconds(2);

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
                Debug.Log("save" + ": " + www.downloadHandler.text);
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

                if (myColor == Color.white)
                {
                    diff = Color.white;
                }
                diffValue += (Mathf.Abs(diff.r) + Mathf.Abs(diff.g) + Mathf.Abs(diff.b) + Mathf.Abs(diff.a)) / 4.0;
            }
        }
        diffValue /= drawing.width * drawing.height;

        //Debug.Log(diffValue);

        currentData.price = referenceData.price * (1 - diffValue);

        myImageText.text = currentData.name + "\n" + currentData.price.ToString("$0.00");
    }

    IEnumerator CountDown()
    {
        while (secondsLeft > 0)
        {
            if (tutorialPane.activeSelf)
            {
                yield return new WaitForSeconds(1);
                continue;
            }
            int minutes = secondsLeft / 60;
            int seconds = secondsLeft % 60;
            countdownText.text = string.Format("{0}:{1}", minutes, seconds.ToString("00"));
            updateCurrentPrice();

            if (secondsLeft > 20)
            {
                FlavorText.text = "Forging";
            }
            else if (secondsLeft > 10)
            {
                FlavorText.text = "Hacking Blockchain";
            }
            else
            {
                FlavorText.text = "Downloading Pixels";
            }

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

        bool widthValid = translatedMousePos.x >= 0 && translatedMousePos.x < drawing.width;
        bool heightValid = translatedMousePos.y >= 0 && translatedMousePos.y < drawing.height;
        if (Input.GetMouseButtonDown(0) && widthValid && heightValid)
        {
            stroking = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            stroking = false;
        }

        if (stroking) {

            int penSize = (int)sizeSlider.value;
            for (int x = -penSize; x <= penSize; x++)
            {
                for (int y = -penSize; y <= penSize; y++)
                {
                    int drawX = (int)translatedMousePos.x + x;
                    int drawY = (int)translatedMousePos.y + y;

                    bool widthValid_ = drawX >= 0 && drawX < drawing.width;
                    bool heightValid_ = drawY >= 0 && drawY < drawing.height;

                    if (widthValid_ && heightValid_)
                    {
                        drawing.SetPixel(drawX, drawY, new Color(redSlider.value, greenSlider.value, blueSlider.value, 1));
                    }
                }
            }

            drawing.Apply();

        }

    }

}
