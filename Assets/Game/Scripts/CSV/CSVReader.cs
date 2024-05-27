using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    [SerializeField] private TextAsset testData;

    public struct TestStruct
    {
        public string name;
        public string clan;
        public string status;
    }

    void Start()
    {
        ReadData();
    }

    public void ReadData()
    {
        string[] data = testData.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
        List<TestStruct> list = new List<TestStruct>();

        for(int i = 3; i < data.Length - 3; i += 3) 
        {
            TestStruct newTest = new TestStruct();
            newTest.name = data[i];
            newTest.clan = data[i + 1];
            newTest.status = data[i + 2];
            list.Add(newTest);
        }

        //Uncomment to print out the data for testing purposes
        /*Debug.Log("---------------------");
        foreach (TestStruct tester in list)
        {
            Debug.Log("NAME: " + tester.name + " CLAN: " + tester.clan + " STATUS: " + tester.status);
        }
        Debug.Log("---------------------");*/
    }
}
