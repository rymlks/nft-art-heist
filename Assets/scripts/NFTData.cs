using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NFTData : MonoBehaviour
{
    public string name;
    public System.Guid guid = System.Guid.NewGuid();
    public double price;
    public bool sold;
    public bool forSale;
}
