using System.Collections.Generic;
using System;
using System.Linq;

public enum OperatorType
{
    PLUS, MINUS, MULTIPLY, DIVIDE
}

public static class MathHelper
{
    public static IEnumerable<uint> GenerateUintRange(uint min, uint max)
    {
        uint n = min;
        while (n <= max)
            yield return n++;
    }

    public static Func<uint, uint, uint> GetOperatorFunc( OperatorType operatorType )
    {
        switch (operatorType)
        {
            case OperatorType.PLUS:
                return (x, y) => x + y;
            case OperatorType.MINUS:
                return (x, y) => x - y;
            case OperatorType.MULTIPLY:
                return (x, y) => x * y;
            case OperatorType.DIVIDE:
                return (x, y) => x / y;
            default:
                throw new InvalidOperationException(string.Format("Invalid operator type {0}", operatorType));
        }
    }

    public static uint ApplyOperator( uint firstOperandValue, uint secondOperandValue, OperatorType operatorType )
    {
        switch( operatorType )
        {
            case OperatorType.PLUS:
                return firstOperandValue + secondOperandValue;
            case OperatorType.MINUS:
                return firstOperandValue - secondOperandValue;
            case OperatorType.MULTIPLY:
                return firstOperandValue * secondOperandValue;
            case OperatorType.DIVIDE:
                return firstOperandValue / secondOperandValue;
            default:
                throw new InvalidOperationException( string.Format("Invalid operator type {0}", operatorType ) );
        }
    }

    public static uint[] GetPossibleOperandNumDigitRange( uint value, OperatorType operatorType, uint expectedNumDigit )
    {
        uint valueNumDigit = (uint)value.ToString().Length;
        uint[] numDigitRange = new uint[2];
        switch (operatorType)
        {
            case OperatorType.PLUS:
                numDigitRange[0] = Math.Max(1, valueNumDigit - 1);
                numDigitRange[1] = valueNumDigit;
                break;
            case OperatorType.MINUS:
                numDigitRange[0] = Math.Max(valueNumDigit, expectedNumDigit);
                numDigitRange[1] = Math.Max(valueNumDigit, expectedNumDigit);
                break;
            case OperatorType.MULTIPLY:
                numDigitRange[0] = Math.Max(1, valueNumDigit/2);
                numDigitRange[1] = (valueNumDigit+1)/2;
                break;
            case OperatorType.DIVIDE:
                numDigitRange[0] = Math.Max(1, expectedNumDigit);
                numDigitRange[1] = Math.Max(2*valueNumDigit, expectedNumDigit);
                break;
            default:
                throw new InvalidOperationException(string.Format("Invalid operator type {0}", operatorType));
        }
        return numDigitRange;
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

    public override string ToString()
    {
        return value.ToString();
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

        return MathHelper.ApplyOperator( firstOperandValue, secondOperandValue, operatorType );
    }

    public override string ToString()
    {
        string firstOperandStr = firstOperand.ToString();
        string secondOperandStr = secondOperand.ToString();

        switch( operatorType )
        {
            case OperatorType.PLUS:
                return string.Format( "({0} + {1})", firstOperandStr, secondOperandStr );
            case OperatorType.MINUS:
                return string.Format( "({0} - {1})", firstOperandStr, secondOperandStr );
            case OperatorType.MULTIPLY:
                return string.Format( "({0} * {1})", firstOperandStr, secondOperandStr );
            case OperatorType.DIVIDE:
                return string.Format( "({0} / {1})", firstOperandStr, secondOperandStr );
            default:
                throw new InvalidOperationException( string.Format("Invalid operator type {0}", operatorType ) );
        }
    }
}

public static class MathExpressionGenerator
{
    public static MathExpression GenerateMathExpressionByValue(uint value, uint numOperator, uint expectedNumDigit, params OperatorType[] allowOperatorTypeArray)
    {
        if(allowOperatorTypeArray == null)
            allowOperatorTypeArray = new OperatorType[]{ OperatorType.PLUS, OperatorType.MINUS, OperatorType.MULTIPLY, OperatorType.DIVIDE };

        if(numOperator == 0)
        {
            return new ValueExpression( value );
        }

        Random random = new Random();
        OperatorType operatorType = allowOperatorTypeArray[ random.Next(0, allowOperatorTypeArray.Length) ];

        List<Tuple<uint, uint>> possibleOperandTupleList = GeneratePossibleOperandValueTupleList(value, operatorType, expectedNumDigit);
        Tuple<uint, uint> randomOperandTuple = possibleOperandTupleList[random.Next(0, possibleOperandTupleList.Count)];

        if(numOperator == 1)
        {
            ValueExpression firstOperand = new ValueExpression(randomOperandTuple.Item1);
            ValueExpression secondOperand = new ValueExpression(randomOperandTuple.Item2);
            return new OperationExpression( operatorType, firstOperand, secondOperand );
        }
        else
        {
            uint firstNumOperator = (uint)random.Next(0, (int)numOperator);
            uint secondNumOperator = numOperator - firstNumOperator - 1;
            MathExpression firstOperand = GenerateMathExpressionByValue(randomOperandTuple.Item1, firstNumOperator, expectedNumDigit, allowOperatorTypeArray );
            MathExpression secondOperand = GenerateMathExpressionByValue(randomOperandTuple.Item2, secondNumOperator, expectedNumDigit, allowOperatorTypeArray );
            return new OperationExpression( operatorType, firstOperand, secondOperand );
        }
    }

    public static List<Tuple<uint, uint>> GeneratePossibleOperandValueTupleList(uint value, OperatorType operatorType, uint expectedNumDigit, uint biasNumDigit = 0)
    {
        uint[] expectedNumDigitRange = MathHelper.GetPossibleOperandNumDigitRange(value, operatorType, expectedNumDigit);

        uint minExpectedNum = (uint)Math.Pow(10, Math.Max(expectedNumDigitRange[0] - biasNumDigit, 1) - 1);
        uint maxExpectedNum = (uint)Math.Pow(10, expectedNumDigitRange[1] + biasNumDigit) - 1;

        Func<uint, uint, uint> operatorFunc = MathHelper.GetOperatorFunc(operatorType);
        IEnumerable<Tuple<uint, uint>> resultIEnumberable = from x in MathHelper.GenerateUintRange(minExpectedNum, maxExpectedNum)
                                                            from y in MathHelper.GenerateUintRange(minExpectedNum, maxExpectedNum)
                                                            where operatorFunc( x, y ) == value
                                                            select new Tuple<uint, uint>(x, y);

        if( operatorType == OperatorType.DIVIDE )
        {
            resultIEnumberable = from t in resultIEnumberable
                                 where t.Item1 % t.Item2 == 0
                                 select t;
        }

        List<Tuple<uint, uint>> resultList = resultIEnumberable.ToList();
        if (resultList.Count == 0)
        {
            Console.WriteLine("here");
            return GeneratePossibleOperandValueTupleList(value, operatorType, expectedNumDigit, biasNumDigit + 1);
        }
        return resultList;
    }
}