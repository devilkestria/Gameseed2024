using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class Testing : MonoBehaviour
{
    [Serializable]
    public class DataProgress
    {
        public string valuename;
        public List<bool> value;
    }
    public List<DataProgress> listdata;
    public Dictionary<string, DataProgress> dicData = new Dictionary<string, DataProgress>();
    public Dictionary<string, List<Dictionary<string, DataProgress>>> dicdicdata = new Dictionary<string, List<Dictionary<string, DataProgress>>>();
    public Dictionary<string, Dictionary<string, Dictionary<string, DataProgress>>> bigdicdata = new Dictionary<string, Dictionary<string, Dictionary<string, DataProgress>>>();

    [Button]
    public void ButtonAddDicData()
    {
        foreach (var data in listdata)
        {
            dicData.Add(data.valuename, data);
        }
    }
    [Button]
    public void ButtonAddDicDicData()
    {
        string[] key = dicData.Keys.ToArray();
        for (int i = 0; i < key.Length; i++)
        {
            string[] chapter = key[i].Split('-');
            string keydicdic = chapter[0];
            Dictionary<string, DataProgress> dicti = new Dictionary<string, DataProgress>();
            dicti.Add(chapter[1], dicData[key[i]]);
            if (dicdicdata.ContainsKey(keydicdic))
                dicdicdata[keydicdic].Add(dicti);
            else
            {
                List<Dictionary<string, DataProgress>> listdat = new List<Dictionary<string, DataProgress>>();
                listdat.Add(dicti);
                dicdicdata.Add(keydicdic, listdat);
            }
        }
    }

    [Button]
    public void ButtonShowDataDicData(string key, int index)
    {
        DataProgress data = dicData[key];
        Debug.Log(data.value[index]);
    }

    [Button]
    public void ButtonShowDataDicDicData(string key1, string key2, int index)
    {
        List<Dictionary<string, DataProgress>> listdicti = dicdicdata[key1];
        int indexter = listdicti.FindIndex(x=> x.ContainsKey(key2));
        DataProgress data = listdicti[indexter][key2];
        Debug.Log(data.value[index]);
    }
}