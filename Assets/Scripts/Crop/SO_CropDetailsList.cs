using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Scriptable Objects/Crop Details List")]
public class CropDetailsList : ScriptableObject
{
    [SerializeField] public List<CropDetail> cropDetails;

    public CropDetail GetCropDetail(int seedItemCode)
    {
        return cropDetails.Find(x => x.seedItemCode == seedItemCode);
    }
}