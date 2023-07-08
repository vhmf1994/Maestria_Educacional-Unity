using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;
using ZXing;

public class CSVReader : MonoBehaviour
{
    public static void ConvertCSVIntoDictionary<T>(string csvFilePath, Func<string, T> converterFunction, out Dictionary<string, T> dictionaryValues)
    {
        TextAsset file = Resources.Load<TextAsset>(csvFilePath);

        dictionaryValues = new Dictionary<string, T>();

        string[] lines = file.text.Split("\n");

        bool isFirstLine = true;

        foreach (var column in lines)
        {
            string[] values = column.Split(',');

            if (isFirstLine)
            {
                isFirstLine = false;
                continue;
            }

            // Faça algo com os valores lidos do arquivo CSV
            for (int i = 0; i < values.Length - 1; i += 2)
            {
                //Debug.Log(values[i] + ": " + values[i + 1]);

                string key = values[i];
                T value = converterFunction(values[i + 1]);

                dictionaryValues.Add(key, value);
            }
        }
    }
}
