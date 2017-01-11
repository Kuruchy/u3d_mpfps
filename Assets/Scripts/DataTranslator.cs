using UnityEngine;
using System;

public class DataTranslator : MonoBehaviour {

    private static string KILLS_TAG = "[KILLS]";
    private static string DEATHS_TAG = "[DEATHS]";

    public static int DataToKills(string data)
    {
        return int.Parse(DataToValue(data, KILLS_TAG));
    }

    public static int DataToDeaths(string data)
    {
        return int.Parse(DataToValue(data, DEATHS_TAG));
    }

    private static string DataToValue(string data, string tag)
    {
        string[] dataArray = data.Split('/');

        foreach (string piece in dataArray)
        {
            if (piece.StartsWith(tag))
            {
                return piece.Substring(tag.Length);
            }
        }
        return "";
    }

}
