using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NFTGenerator : MonoBehaviour
{

    public List<Texture2D> top_nft_list;
    public List<Texture2D> middle_nft_list;
    public List<Texture2D> bottom_nft_list;
    public List<Color> palette;

    private Image myImage;



    // Start is called before the first frame update
    void Start()
    {

        myImage = GetComponent<Image>();
        GenerateNFT(top_nft_list, middle_nft_list, bottom_nft_list);
        
    }

    void GenerateNFT(List<Texture2D> top_NFT_element, List<Texture2D> middle_NFT_element, List<Texture2D> bottom_NFT_element) {

        //create texture

        float topNFTListCount = top_NFT_element.Count;
        float middleNFTnftListCount = middle_NFT_element.Count;
        float bottomNFTnftListCount = bottom_NFT_element.Count;

        int randomTopPiece = (int) Random.Range(0f, topNFTListCount);
        int randomMiddlePiece = (int) Random.Range(0f, middleNFTnftListCount);
        int randomBottomPiece = (int) Random.Range(0f, bottomNFTnftListCount);

        Debug.Log("randomBottomPiece value is equal to " + randomTopPiece);
        Debug.Log("randomMiddlePiece value is equal to " + randomMiddlePiece);
        Debug.Log("randomBottomPiece value is equal to " + randomBottomPiece);

        Texture2D NFT = new Texture2D(32, 32);
        NFT.filterMode = FilterMode.Point;

        for (int x = 0; x < NFT.width; x++)
        {
            for (int y = 0; y < NFT.height; y++)
            {
                NFT.SetPixel(x, y, new Color(1, 1, 1, 1f));
  
            }
        }

        NFT.Apply();

        for (int x = 0; x < NFT.width; x++)
        {
            for (int y = 0; y < NFT.height; y++)
            {

                if (top_NFT_element[randomTopPiece].GetPixel(x,y).a == 1) {

                    NFT.SetPixel(x,y, top_NFT_element[randomTopPiece].GetPixel(x,y));

                }
                
                if (middle_NFT_element[randomMiddlePiece].GetPixel(x,y).a == 1) {

                    NFT.SetPixel(x,y, middle_NFT_element[randomMiddlePiece].GetPixel(x,y));

                }
                
                if (bottom_NFT_element[randomBottomPiece].GetPixel(x,y).a == 1) {

                    NFT.SetPixel(x,y, bottom_NFT_element[randomBottomPiece].GetPixel(x,y));

                }

            }
        }

        //TO-DO List

        //write a function that grabs a random NFT Layer
        //  - function needs to grab one eyebrow, one eye, and one mouth
        //  - change public List<Texture2D> Pieces and add two more lists for eyebrows, eyes, and mouth
        //  - (if there's time) add a fourth list with color palette values that generate can pull from
        //  - 

        NFT.Apply();
        //create sprite to hold texture
        Sprite mySprite = Sprite.Create(NFT, new Rect(0.0f, 0.0f, NFT.width, NFT.height), Vector2.one);

        //assign sprite to image
        myImage.sprite = mySprite;
        myImage.color = Color.white;

    }

   
}
