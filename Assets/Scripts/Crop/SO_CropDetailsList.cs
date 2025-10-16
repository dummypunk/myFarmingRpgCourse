using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Scriptable Objects/Crop/Crop Details List")]
public class SO_CropDetailsList : ScriptableObject
{
    [SerializeField] public List<CropDetail> cropDetails;

    public CropDetail GetCropDetails(int seedItemCode)
    {
        return cropDetails.Find(x => x.seedItemCode == seedItemCode);
    }
}