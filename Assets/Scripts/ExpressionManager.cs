using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionManager : MonoBehaviour
{
    public uint result;
    public uint numOperator;
    public uint expectedNumDigit;
    public OperatorType[] allowOperatorTypeArray;

    // Start is called before the first frame update
    void Start()
    {
        MathExpression e = MathExpressionGenerator.GenerateMathExpressionByValue( result, numOperator, expectedNumDigit, allowOperatorTypeArray );
        Debug.Log(e.ToString());
        /*
        System.Random random = new System.Random();
        uint numOperator = 2;
        uint firstNumOperator = (uint)random.Next(0, (int)numOperator);
        uint secondNumOperator = numOperator - firstNumOperator - 1;
        Debug.Log(firstNumOperator);
        Debug.Log(secondNumOperator);
        */
    }
}
