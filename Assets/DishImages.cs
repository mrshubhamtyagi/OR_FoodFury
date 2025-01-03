using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FoodFury
{
    public class DishImages : MonoBehaviour
    {
        private static Dictionary<int, Texture2D> dishimages;
        private static Texture2D dummyImage;
        //private void OnEnable()
        //{
        //    LoadAllDishImages();
        //}

        //void LoadAllDishImages()
        //{
        //    dishimages = new Dictionary<int, Texture2D>();

        //    Texture2D[] textures = Resources.LoadAll<Texture2D>("Dishes");

        //    foreach (Texture2D texture in textures)
        //    {
        //        string fileName = texture.name;
        //        if (int.TryParse(fileName, out int textureId))
        //        {
        //            dishimages[textureId] = texture;
        //        }
        //        else
        //        {
        //            Debug.LogWarning($"Filename '{fileName}' could not be parsed as an integer.");
        //        }
        //    }

        //    Debug.Log($"Loaded {dishimages.Count} textures into the dictionary.");
        //}

        public static Texture2D LoadDishImage(int id)
        {
            string path = $"Dishes/{id}";
            Texture2D texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                return texture;
            }
            else
            {
                path = $"Dishes/dummy";
                texture = Resources.Load<Texture2D>(path);
                Debug.LogWarning($"Dish Nahi Mili:{id}");
                return texture;
            }

        }
        public static Texture2D LoadFlagImage(string country)
        {
            string path = $"Flags/{country}";
            Texture2D texture = Resources.Load<Texture2D>(path);
            if (texture != null)
            {
                return texture;
            }
            else
            {
                path = $"Dishes/dummy";
                texture = Resources.Load<Texture2D>(path);
                Debug.LogWarning($"Flag Nahi Mila:{country}");
                return texture;
            }

        }
        //    public static Texture2D LoadFromStreamingAsset(int id)
        //    {
        //        string path = $"{Application.streamingAssetsPath}/Dishes/{id}.png";
        //        byte[] fileData = File.ReadAllBytes(path);
        //        Texture2D texture = new Texture2D(2, 2);
        //        if (File.Exists(path))
        //        {

        //            if (texture.LoadImage(fileData))
        //            {

        //                return texture;
        //            }
        //            else
        //            {
        //                Debug.LogError("Dish nahi mili:" + id);
        //                path = $"{Application.streamingAssetsPath}/Dishes/dummy.png";
        //                fileData = File.ReadAllBytes(path);
        //                texture = new Texture2D(2, 2);


        //                if (texture.LoadImage(fileData))
        //                {

        //                    return texture;
        //                }
        //                else
        //                {
        //                    Debug.LogError("Dummy Image Not Available");
        //                    return null;
        //                }

        //            }
        //        }
        //        else
        //        {
        //            Debug.LogError("File Does not Exist");
        //            path = $"{Application.streamingAssetsPath}/Dishes/dummy";
        //            fileData = File.ReadAllBytes(path);
        //            texture = new Texture2D(2, 2);


        //            if (texture.LoadImage(fileData))
        //            {

        //                return texture;
        //            }
        //            else
        //            {
        //                Debug.LogError("Dummy Image Not Available");
        //                return null;
        //            }
        //        }

        //    }
        //}
    }
}
