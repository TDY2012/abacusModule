using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionManager : MonoBehaviour
{
    public uint result;
    public OperatorType operatorType;
    public uint expectedDigitNum;
    public uint biasDigitNum;

    // Start is called before the first frame update
    void Start()
    {
        uint[] r = MathExpressionGenerator.GetLowerExpectedRange(result, operatorType, expectedDigitNum, biasDigitNum);
        string log = "";
        foreach(uint x in r){
            log += x + ",";
        }
        Debug.Log(log);
    }
}
