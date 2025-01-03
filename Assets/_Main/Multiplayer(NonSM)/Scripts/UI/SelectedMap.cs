using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class SelectedMap : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mapNameTMP;
        [SerializeField] private RawImage thumbnail;
        [field: SerializeField] public ModelClass.MapDetail Data { get; private set; }

        public void SetData(ModelClass.MapDetail _data, Texture thumbnailTexture)
        {
            Data = _data;
            mapNameTMP.text = _data.mapName;
            thumbnail.texture = thumbnailTexture;
            gameObject.name = _data.mapName;
        }



    }
}

