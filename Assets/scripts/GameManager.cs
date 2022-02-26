using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject menuView;
    public GameObject galleryView;
    public GameObject drawView;
    public GameObject upgradeView;

    public Shop saleManager;
    public DrawManager drawManager;
    public AudioManager audioManager;

    public Text moneyText;

    public Toggle muteButton;

    public System.Guid guid = System.Guid.NewGuid();

    public JSONTypes.UserData userData;
    public List<NFTData> NFTs;

    // Start is called before the first frame update
    void Start()
    {
        // 89a89392-6aee-4104-b36b-88867c7b54cb
        //guid = System.Guid.Parse("89a89392-6aee-4104-b36b-88867c7b54cb");
        menuView.SetActive(true);
        galleryView.SetActive(true);
        drawView.SetActive(false);
        upgradeView.SetActive(false);

        NFTs = new List<NFTData>();

        string _guid = PlayerPrefs.GetString("guid", null);
        Debug.Log(_guid);

        if (_guid != null && !_guid.Equals(""))
        {
            guid = System.Guid.Parse(_guid);
        }

        PlayerPrefs.SetString("guid", guid.ToString());

        StartCoroutine(RequestGetUserStart());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
            UpdateUserData();
        }
    }

    public void UpdateUserData()
    {
        StartCoroutine(RequestGetUser());
    }

    IEnumerator RequestGetUser()
    {

        using (UnityWebRequest www = UnityWebRequest.Get("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/getUser?secret=foobar&guid=" + guid))
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
                userData = JsonUtility.FromJson<JSONTypes.UserData>(www.downloadHandler.text);
            }
        }
        UpdateMoneyText();
    }

    IEnumerator RequestGetUserStart()
    {

        using (UnityWebRequest www = UnityWebRequest.Get("https://us-east-1.aws.data.mongodb-api.com/app/test-nfts-kfnqu/endpoint/getUser?secret=foobar&guid=" + guid))
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
                userData = JsonUtility.FromJson<JSONTypes.UserData>(www.downloadHandler.text);
            }
        }
        UpdateMoneyText();
        saleManager.ShowHeists();
    }

    public void UpdateMoneyText()
    {
        moneyText.text = "Money: " + userData.money.ToString("$0.00");
    }

    public void ToggleMenu()
    {
        menuView.SetActive(!menuView.activeSelf);
    }

    public void StartDrawing(GameObject option)
    {
        audioManager.StartRandomMusic();
        drawView.SetActive(true);
        drawManager.StartDrawing(option);
    }

    public void EndDrawing(Texture2D drawing)
    {
        audioManager.EndMusic();
        drawView.SetActive(false);
        saleManager.Gallery();
    }

    public void OpenUpgradeView()
    {
        upgradeView.SetActive(true);
    }

    public void CloseUpgradeView()
    {
        upgradeView.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
