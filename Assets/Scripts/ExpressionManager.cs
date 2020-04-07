using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpressionManager : MonoBehaviour
{
    public uint result;
    public uint expectedDigitNum;
    public uint biasDigitNum;

    // Start is called before the first frame update
    void Start()
    {
        IntRange r = MathExpressionGenerator.GetLowerExpectedRange(result, OperatorType.DIVIDE, expectedDigitNum, biasDigitNum);
        Debug.Log(string.Format("LOWER : Min = {0}, Max = {1}", r.Min, r.Max));
        Debug.Log(string.Format("HIGHER : Min = {0}, Max = {1}", r.Min*result, r.Max*result));

        List<uint> primeFactorList = MathHelper.Factorize(result);
        string log = "";
        foreach (uint primeFactor in primeFactorList)
            log += primeFactor + ",";
        Debug.Log(log);

        List<uint> allFactorList = MathHelper.GenerateAllFactorList(primeFactorList);
        log = "";
        foreach (uint factor in allFactorList)
            log += factor + ",";
        Debug.Log(log);
    }
}
