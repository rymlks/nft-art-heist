using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTData : MonoBehaviour
{
    public string name;
    public System.Guid guid = System.Guid.NewGuid();
    public System.Guid ownerGuid = System.Guid.Empty;
    public double price;
    public bool sold;
    public bool forSale;

    public bool isHeist;

    public NFTData(NFTData copy)
    {
        Copy(copy);
    }

    public void Copy(NFTData copy)
    {
        name = copy.name;
        price = copy.price;
        sold = copy.sold;
        forSale = copy.forSale;
        guid = System.Guid.NewGuid();
        ownerGuid = copy.ownerGuid;
        isHeist = copy.isHeist;
    }
}
