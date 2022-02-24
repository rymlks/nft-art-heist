using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject menuView;
    public GameObject galleryView;
    public GameObject drawView;

    public Shop saleManager;
    public DrawManager drawManager;

    public System.Guid guid = System.Guid.NewGuid();

    // Start is called before the first frame update
    void Start()
    {
        // 89a89392-6aee-4104-b36b-88867c7b54cb
        guid = System.Guid.Parse("89a89392-6aee-4104-b36b-88867c7b54cb");
        menuView.SetActive(true);
        galleryView.SetActive(true);
        drawView.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        menuView.SetActive(!menuView.activeSelf);
    }

    public void StartDrawing(GameObject option)
    {
        drawView.SetActive(true);
        drawManager.StartDrawing(option);
    }

    public void EndDrawing(Texture2D drawing)
    {
        drawView.SetActive(false);
        saleManager.Gallery();

    }
}
