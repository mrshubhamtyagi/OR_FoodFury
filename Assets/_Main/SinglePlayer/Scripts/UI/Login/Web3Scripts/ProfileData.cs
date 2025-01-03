using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileData : MonoBehaviour
{
    public bool error { get; set; }
    public Result result { get; set; }
}
public class Result
{
    public Dictionary<string, int> ingredients { get; set; }
    public Dictionary<string, int> dishes { get; set; }
    public int points { get; set; }
    public long lastLogin { get; set; }
    public Dictionary<string, int> forRent { get; set; }
    public Dictionary<string, int> rented { get; set; }
    public bool freezed { get; set; }
    public int dishCount { get; set; }
    public int ingredientCount { get; set; }
    public int score { get; set; }
    public int level7Dishes { get; set; }
    public int dishBought { get; set; }
    public int dishSold { get; set; }
    public int ingredientBought { get; set; }
    public int ingredientSold { get; set; }
    public double orareSpent { get; set; }
    public double marketProfit { get; set; }
    public double upgradesSpent { get; set; }
    public int totalUpgrades { get; set; }
    public double rewards { get; set; }
    public bool otpClaim { get; set; }
    public string userType { get; set; }
    public int[] questArray { get; set; }
    public int questStatus { get; set; }
    public string promoCode { get; set; }
    public bool verified { get; set; }
    public int stage { get; set; }
    public bool migrated { get; set; }
    public bool farmMigrated { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string telegram { get; set; }
    public string phone { get; set; }
    public int[] notificationFlag { get; set; }
    public string profilePic { get; set; }
    public string userId { get; set; }
    public long freeFTWGames { get; set; }
    public DateTimeOffset createdDate { get; set; }
    public long showFTWTutorial { get; set; }
    public object[] saladDaysAllocated { get; set; }
    public object[] saladDaysRemaining { get; set; }
    public object[] tooYummAllocated { get; set; }
    public object[] tooYummRemaining { get; set; }
    public object[] blueTribeAllocated { get; set; }
    public object[] blueTribeRemaining { get; set; }
    public object[] bagrrysAllocated { get; set; }
    public object[] bagrrysRemaining { get; set; }
    public string _id { get; set; }
    public string address { get; set; }
    public SignUpVia signUpVia { get; set; }
    public DateTimeOffset lastUpdated { get; set; }
    public long __v { get; set; }
}
public class SignUpVia
{
    [JsonProperty("via")]
    public string Via { get; set; }

    [JsonProperty("handle")]
    public string Handle { get; set; }
}