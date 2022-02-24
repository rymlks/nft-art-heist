using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONTypes : MonoBehaviour
{

    [System.Serializable]
    public class UserData
    {
        public string guid;
        public double money;
        public string[] NFTs;
        public string[] upgrades;
    }

    [System.Serializable]
    public class SerializeTexture
    {
        [SerializeField]
        public int x;
        [SerializeField]
        public int y;
        [SerializeField]
        public byte[] bytes;
    }

    [System.Serializable]
    public class SaleEntry
    {
        [SerializeField]
        public string userGuid;
        [SerializeField]
        public string artGuid;
        [SerializeField]
        public string name;
        [SerializeField]
        public double price;
        [SerializeField]
        public double appraisedValue;
        [SerializeField]
        public string imageData;
        [SerializeField]
        public bool sold;
        [SerializeField]
        public bool forSale;
    }

    [System.Serializable]
    public class SaleEntryList
    {
        [SerializeField]
        public SaleEntry[] entries;
    }
}
