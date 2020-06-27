using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.CompilerServices;
using System;

public class DataImport : MonoBehaviour
{
    public MoveScript ms;
    // Start is called before the first frame update
    void Start()
    {
        StreamReader sr = new StreamReader("params.txt");
        string stringLine;
        while((stringLine = sr.ReadLine()) != null)
        {
            string[] divLines = stringLine.Split(':');
            if(divLines.Length > 1)
            {
                if(divLines[0] == "Speed")
                {
                    ms.speedVal = Convert.ToInt32(divLines[1]);
                } else
                {
                    ms.distVal = Convert.ToInt32(divLines[1]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
