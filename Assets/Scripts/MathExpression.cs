using System.Collections.Generic;

public enum OperatorType
{
    PLUS, MINUS, MULTIPLY, DIVIDE
}

public static class MathHelper
{
    public static List<uint> Factorize(uint number)
    {
        List<uint> primeFactorList = new List<uint>();

        uint div = 2;
        while(number > 1 || div <= number/2 )
        {
            if(number % div == 0)
            {
                primeFactorList.Add(div);
                number /= div;
                div = 2;
            }
            else
                div++;
        }

        return primeFactorList;
    }

    public static List<uint> GenerateAllFactorList( List<uint> primeFactorList )
    {
        Dictionary<uint, uint> primeFactorToNumFactorDict = new Dictionary<uint, uint>();
        foreach (uint primeFactor in primeFactorList)
        {
            if (!primeFactorToNumFactorDict.ContainsKey(primeFactor))
                primeFactorToNumFactorDict[primeFactor] = 0;
            primeFactorToNumFactorDict[primeFactor]++;
        }

        //  Initialize all factor list with 1
        List<uint> allFactorList = new List<uint>();
        allFactorList.Add(1);

        foreach(KeyValuePair<uint,uint> primeFactorToNumFactorTuple in primeFactorToNumFactorDict)
        {
            List<uint> multipliedList = new List<uint>();
            for(uint i = 1; i <= primeFactorToNumFactorTuple.Value; i++)
            {
                uint multiplier = (uint)System.Math.Pow(primeFactorToNumFactorTuple.Key, i);
                foreach (uint factor in allFactorList)
                    multipliedList.Add(multiplier * factor);
            }
            allFactorList.AddRange(multipliedList);
        }

        return allFactorList;
    }
}

public class IntRange
{
    private uint min;
    public uint Min {
        set {
            if (value > max)
                max = value;
            else
                min = value;
        }
        get {
            return min;
        }
    }
    
    private uint max;
    public uint Max {
        set {
            if (value < min)
                min = value;
            else
                max = value;
        }
        get {
            return max;
        }
    }

    public IntRange( uint min=uint.MinValue, uint max=uint.MaxValue )
    {
        this.min = min;
        this.max = max;
    }
}

public abstract class MathExpression
{
    public abstract uint GetValue();
}

public class ValueExpression: MathExpression
{
    private uint value;

    public ValueExpression( uint value )
    {
        this.value = value;
    }

    public override uint GetValue()
    {
        return value;
    }
}

public class OperationExpression: MathExpression
{
    private OperatorType operatorType;
    private MathExpression firstOperand;
    private MathExpression secondOperand;

    public OperationExpression( OperatorType operatorType, MathExpression firstOperand, MathExpression secondOperand )
    {
        this.operatorType = operatorType;
        this.firstOperand = firstOperand;
        this.secondOperand = secondOperand;
    }

    public override uint GetValue()
    {
        uint firstOperandValue = firstOperand.GetValue();
        uint secondOperandValue = secondOperand.GetValue();

        switch( operatorType )
        {
            case OperatorType.PLUS:
                return firstOperandValue + secondOperandValue;
            case OperatorType.MINUS:
                if (secondOperandValue > firstOperandValue)
                    throw new System.ArithmeticException();
                return firstOperandValue - secondOperandValue;
            case OperatorType.MULTIPLY:
                return firstOperandValue * secondOperandValue;
            case OperatorType.DIVIDE:
                if (secondOperandValue == 0)
                    throw new System.DivideByZeroException();
                else if (firstOperandValue % secondOperandValue != 0)
                    throw new System.ArithmeticException();
                else
                    return firstOperandValue / secondOperandValue;
            default:
                throw new System.InvalidOperationException();
        }
    }
}

public static class MathExpressionGenerator
{
    /*
    public static MathExpression GenerateMathExpressionByValue(int value, int numOperator, params OperatorType[] allowOperatorTypeArray)
    {
        for(int i = 0; i < numOperator; i++)
        {

        }
    }*/

    public static IntRange GetLowerExpectedRange(uint result, OperatorType operatorType, uint expectedNumDigit, uint biasNumDigit=0)
    {
        //  Initialize lower expected range. This range is used for:
        //      1) Pick a randomized second operand (the lower one)
        //      2) Compute a paried higher operand by doing the inverse operation with the result
        IntRange lowerExpectedRange = new IntRange();

        uint resultNumDigit = (uint)(result.ToString().Length);
        switch (operatorType)
        {
            case OperatorType.DIVIDE:
                if (biasNumDigit < resultNumDigit - 1)
                    throw new System.ArithmeticException();
                else if (resultNumDigit < 1 || resultNumDigit > expectedNumDigit)
                    throw new System.ArithmeticException();
                else
                {
                    uint minNumDigit = System.Math.Max( expectedNumDigit - biasNumDigit, 1 );
                    lowerExpectedRange.Min = (uint)System.Math.Pow( 10, minNumDigit-1 );
                    lowerExpectedRange.Max = (uint)((System.Math.Pow(10, expectedNumDigit) - 1) / result);
                    return lowerExpectedRange;
                }
            case OperatorType.MULTIPLY:
                if (biasNumDigit < System.Math.Floor(resultNumDigit / 2.0f))
                    throw new System.ArithmeticException();
                else
                {
                    //IMPLEMENT ME
                    uint minNumDigit = System.Math.Max(expectedNumDigit - biasNumDigit, 1);
                    lowerExpectedRange.Min = (uint)System.Math.Pow(10, minNumDigit - 1);
                    return lowerExpectedRange;
                }
            default:
                throw new System.InvalidOperationException();
        }
    }
}