using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FoodFury
{
    public class DishCard : MonoBehaviour
    {
        [SerializeField] private Image levelImage;
        [SerializeField] private RawImage dishImage, countryIcon;
        [SerializeField] private TextMeshProUGUI pointsTMP, countryTMP, continentTMP, regionTMP;
        [SerializeField] private Image continentIcon, regionIcon;
        [SerializeField] private TextMeshProUGUI dishNameTMP, brandNameTMP;
        [SerializeField] private Image dietIcon, methodIcon;
        [SerializeField] private TextMeshProUGUI ingredientsTMP, spiceMeterTMP, skillMeterTMP, timeMeterTMP;
        //  [SerializeField] private Texture2D defaultDishImage;
        [Header("-----Lists")]
        [SerializeField] Color[] levelColors;
        [SerializeField] Sprite[] levels;
        [SerializeField] Sprite[] diets;
        [SerializeField] Sprite[] methods;
        private int tokenId = -1;
        public async Task FillData(ModelClass.Dish _dish)
        {

            tokenId = _dish.tokenId;
            levelImage.sprite = levels[_dish.level - 1];
            // Top

            dishImage.texture = DishImages.LoadDishImage(_dish.tokenId);// await APIManager.GetTextureAsync(_dish.tokenId.ToString(), APIManager.AWSUrl.Dish);
            countryIcon.texture = DishImages.LoadFlagImage(RemoveSpacesAndLowerCaseFlagName(_dish.country));// await APIManager.GetTextureAsync(RemoveSpacesAndLowerCaseFlagName(_dish.country), APIManager.AWSUrl.Flag);
            //if (ProfileScreen.Instance.HasDishImage(tokenId))
            //{
            //    dishImage.texture = ProfileScreen.Instance.dishDictionary[tokenId];
            //}
            //else
            //{
            //    Texture image = await APIManager.GetTextureAsync(_dish.tokenId.ToString(), APIManager.AWSUrl.Dish);
            //    if (image != null)
            //    {
            //        dishImage.texture = image;
            //        ProfileScreen.Instance.AddDishImage(_dish.tokenId, dishImage.texture);
            //    }

            //}
            pointsTMP.text = _dish.points.ToString();
            countryTMP.text = _dish.country;
            continentTMP.text = _dish.continent;
            regionTMP.text = _dish.region;
            //int countryHashCode = _dish.country.ToLower().GetHashCode();
            //if (ProfileScreen.Instance.HasCountryImage(countryHashCode))
            //{
            //    countryIcon.texture = ProfileScreen.Instance.CountryFlagDictionary[countryHashCode];
            //}
            //else
            //{
            //    countryIcon.texture = await APIManager.GetTextureAsync(_dish.country.ToLower(), APIManager.AWSUrl.Flag);
            //    if (countryIcon.texture != null)
            //    {
            //        ProfileScreen.Instance.AddCountryImage(countryHashCode, countryIcon.texture);
            //    }
            //}
            continentIcon.color = regionIcon.color = levelColors[_dish.level - 1];


            // Middle
            dishNameTMP.text = _dish.name;
            brandNameTMP.text = _dish.brand;
            dietIcon.sprite = GetDietSprite(_dish.diet);
            methodIcon.sprite = GetMethodSprite(_dish.method);


            // Bottom
            ingredientsTMP.text = _dish.number_of_ingredients.ToString();
            spiceMeterTMP.text = _dish.spice_meter.ToString();
            skillMeterTMP.text = _dish.skill_meter.ToString();
            timeMeterTMP.text = _dish.time_meter.ToString();
        }


        private Sprite GetDietSprite(string _diet)
        {
            //  NON_VEGETARIAN, VEGAN, VEGETARIAN
            return _diet switch
            {
                "VEGAN" => diets[0],
                "VEGETARIAN" => diets[1],
                "NON VEGETARIAN" => diets[2],
                _ => diets[0]
            };
        }


        private Sprite GetMethodSprite(Enums.DishMethod _method)
        {
            // Raw, Boil, Cook, Grill, Fry, Bake, Roast, Toast, Steam, Fried, Baked 
            return _method switch
            {
                Enums.DishMethod.Raw => methods[0],
                Enums.DishMethod.Boil => methods[1],
                Enums.DishMethod.Cook => methods[2],
                Enums.DishMethod.Grill => methods[3],
                Enums.DishMethod.Fry => methods[4],
                Enums.DishMethod.Bake => methods[5],
                Enums.DishMethod.Roast => methods[6],
                Enums.DishMethod.Toast => methods[7],
                Enums.DishMethod.Steam => methods[8],
                Enums.DishMethod.Fried => methods[9],
                _ => methods[0]
            };
        }
        private string RemoveSpacesAndLowerCaseFlagName(string country)
        {
            country = country.Replace(" ", string.Empty).ToLower();
            return country;
        }

    }

}
