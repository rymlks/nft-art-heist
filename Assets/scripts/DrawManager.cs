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

        drawing = new Texture2D(16, 16);    
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


    }

    private void OnMouseDrag()
    {
        
    }

}
