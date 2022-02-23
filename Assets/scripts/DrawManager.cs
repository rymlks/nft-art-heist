using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawManager : MonoBehaviour
{
    
    Texture2D drawing;
    Image myImage;
    
    // Start is called before the first frame update
    void Start()
    {
        myImage = GetComponent<Image>();

        drawing = new Texture2D(32, 32);  
        drawing.filterMode = FilterMode.Point;
        Sprite mySprite = Sprite.Create(drawing, new Rect(0.0f, 0.0f, drawing.width, drawing.height), Vector2.one);
        myImage.sprite = mySprite;

        myImage.color = Color.white;

        Debug.Log("It did the thing!");
        
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 mousePos = Input.mousePosition;

        Vector3[] corners = new Vector3[4];
        GetComponent<Image>().rectTransform.GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2]-corners[0]);
        //Debug.Log(newRect.Contains(Input.mousePosition));

        Vector3 translatedMousePos = mousePos - corners[0];
        
        int width = drawing.width;

        int height = drawing.height;

        translatedMousePos.x = Mathf.Floor(width * translatedMousePos.x/newRect.width);
        translatedMousePos.y =  Mathf.Floor(height * translatedMousePos.y/newRect.height);

        Debug.Log(translatedMousePos);

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
