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

    public List<Texture2D> monkeyOutline;
    public List<Texture2D> monkeyBrows;
    public List<Texture2D> monkeyEyes;
    public List<Texture2D> monkeyFur;
    public List<Texture2D> monkeyMouth;
    public List<Texture2D> monkeySkin;

    public double value;

    public void GenerateNFT()
    {
        if (UnityEngine.Random.value < 0.25f)
        {
            GenerateGoon();
        } else
        {
            GenerateMonkey();
        }
    }

    public void GenerateMonkey()
    {

        //create texture

        int rOutline = (int)Mathf.Abs(RandomGaussian(-monkeyOutline.Count + 0.5f, monkeyOutline.Count - 0.5f));
        int rBrow = (int)Mathf.Abs(RandomGaussian(-monkeyBrows.Count + 0.5f, monkeyBrows.Count - 0.5f));
        int rEyes = (int)Mathf.Abs(RandomGaussian(-monkeyEyes.Count + 0.5f, monkeyEyes.Count - 0.5f));
        int rFur = (int)Mathf.Abs(RandomGaussian(-monkeyFur.Count + 0.5f, monkeyFur.Count - 0.5f));
        int rMouth = (int)Mathf.Abs(RandomGaussian(-monkeyMouth.Count + 0.5f, monkeyMouth.Count - 0.5f));
        int rSkin = (int)Mathf.Abs(RandomGaussian(-monkeySkin.Count + 0.5f, monkeySkin.Count - 0.5f));
        int randombg = (int)Mathf.Abs(RandomGaussian(-palette.Count + 0.5f, palette.Count - 0.5f));

        float exp = 1.75f;
        value = exp;
        value *= Mathf.Pow(exp, rOutline);
        value *= Mathf.Pow(exp, rBrow);
        value *= Mathf.Pow(exp, rEyes);
        value *= Mathf.Pow(exp, rFur);
        value *= Mathf.Pow(exp, rMouth);
        value *= Mathf.Pow(exp, rSkin);
        value *= Mathf.Pow(exp, randombg);

        //Debug.Log("randomBottomPiece value is equal to " + randomTopPiece);
        //Debug.Log("randomMiddlePiece value is equal to " + randomMiddlePiece);
        //Debug.Log("randomBottomPiece value is equal to " + randomBottomPiece);

        Texture2D NFT = new Texture2D(32, 32);
        NFT.filterMode = FilterMode.Point;

        Color bg = palette[randombg];

        for (int x = 0; x < NFT.width; x++)
        {
            for (int y = 0; y < NFT.height; y++)
            {
                NFT.SetPixel(x, y, bg);

            }
        }

        NFT.Apply();

        for (int x = 0; x < NFT.width; x++)
        {
            for (int y = 0; y < NFT.height; y++)
            {

                if (monkeyOutline[rOutline].GetPixel(x, y).a == 1)
                {

                    NFT.SetPixel(x, y, monkeyOutline[rOutline].GetPixel(x, y));

                }
                if (monkeyFur[rFur].GetPixel(x, y).a == 1)
                {

                    NFT.SetPixel(x, y, monkeyFur[rFur].GetPixel(x, y));

                }
                if (monkeySkin[rSkin].GetPixel(x, y).a == 1)
                {

                    NFT.SetPixel(x, y, monkeySkin[rSkin].GetPixel(x, y));

                }
                if (monkeyBrows[rBrow].GetPixel(x, y).a == 1)
                {

                    NFT.SetPixel(x, y, monkeyBrows[rBrow].GetPixel(x, y));

                }
                if (monkeyEyes[rEyes].GetPixel(x, y).a == 1)
                {

                    NFT.SetPixel(x, y, monkeyEyes[rEyes].GetPixel(x, y));

                }
                if (monkeyMouth[rMouth].GetPixel(x, y).a == 1)
                {

                    NFT.SetPixel(x, y, monkeyMouth[rMouth].GetPixel(x, y));

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
        Image myImage = GetComponent<Image>();
        myImage.sprite = mySprite;
        myImage.color = Color.white;
    }

    public void GenerateGoon() {

        //create texture

        float topNFTListCount = top_nft_list.Count;
        float middleNFTnftListCount = middle_nft_list.Count;
        float bottomNFTnftListCount = bottom_nft_list.Count;

        int randomTopPiece = (int) Mathf.Abs(RandomGaussian(-topNFTListCount + 0.5f, topNFTListCount - 0.5f ));
        int randomMiddlePiece = (int) Mathf.Abs(RandomGaussian(-middleNFTnftListCount + 0.5f, middleNFTnftListCount - 0.5f));
        int randomBottomPiece = (int)Mathf.Abs(RandomGaussian(-bottomNFTnftListCount + 0.5f, bottomNFTnftListCount - 0.5f));
        int randombg = (int)Mathf.Abs(RandomGaussian(-palette.Count + 0.5f, palette.Count - 0.5f));

        float exp = 2.5f;
        value = 2.5;
        value *= Mathf.Pow(exp, randomTopPiece);
        value *= Mathf.Pow(exp, randomMiddlePiece);
        value *= Mathf.Pow(exp, randomBottomPiece);
        value *= Mathf.Pow(exp, randombg);

        //Debug.Log("randomBottomPiece value is equal to " + randomTopPiece);
        //Debug.Log("randomMiddlePiece value is equal to " + randomMiddlePiece);
        //Debug.Log("randomBottomPiece value is equal to " + randomBottomPiece);

        Texture2D NFT = new Texture2D(32, 32);
        NFT.filterMode = FilterMode.Point;

        Color bg = palette[randombg];

        for (int x = 0; x < NFT.width; x++)
        {
            for (int y = 0; y < NFT.height; y++)
            {
                NFT.SetPixel(x, y, bg);
  
            }
        }

        NFT.Apply();

        for (int x = 0; x < NFT.width; x++)
        {
            for (int y = 0; y < NFT.height; y++)
            {

                if (top_nft_list[randomTopPiece].GetPixel(x,y).a == 1) {

                    NFT.SetPixel(x,y, top_nft_list[randomTopPiece].GetPixel(x,y));

                }
                
                if (middle_nft_list[randomMiddlePiece].GetPixel(x,y).a == 1) {

                    NFT.SetPixel(x,y, middle_nft_list[randomMiddlePiece].GetPixel(x,y));

                }
                
                if (bottom_nft_list[randomBottomPiece].GetPixel(x,y).a == 1) {

                    NFT.SetPixel(x,y, bottom_nft_list[randomBottomPiece].GetPixel(x,y));

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
        Image myImage = GetComponent<Image>();
        myImage.sprite = mySprite;
        myImage.color = Color.white;

    }

    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

}
