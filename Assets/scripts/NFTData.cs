using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTData : MonoBehaviour
{
    public string name;
    public System.Guid guid = System.Guid.NewGuid();
    public System.Guid ownerGuid = System.Guid.Empty;
    public double price;
    public double appraisedValue;
    public bool sold;
    public bool forSale;

    public Texture2D texture;

    public bool isHeist;

    public NFTData(NFTData copy)
    {
        Copy(copy);
    }

    public void Copy(NFTData copy)
    {
        name = copy.name;
        price = copy.price;
        appraisedValue = copy.appraisedValue;
        sold = copy.sold;
        forSale = copy.forSale;
        guid = System.Guid.NewGuid();
        ownerGuid = copy.ownerGuid;
        isHeist = copy.isHeist;
        texture = copy.texture;
    }
}
