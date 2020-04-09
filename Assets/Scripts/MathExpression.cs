using System.Collections.Generic;
using System;

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
                uint multiplier = (uint)Math.Pow(primeFactorToNumFactorTuple.Key, i);
                foreach (uint factor in allFactorList)
                    multipliedList.Add(multiplier * factor);
            }
            allFactorList.AddRange(multipliedList);
        }

        return allFactorList;
    }

    public static uint[] GenerateUintRange( uint min, uint max, uint step=1 )
    {
        if( min>max )
            throw new ArgumentException("Minimum bound must not be greater than maximum bound.");
        uint rangeLength = (max-min+step)/step;
        uint[] range = new uint[ rangeLength ];
        for(uint i=0;i<rangeLength;i++)
        {
            range[i] = min + i*step;
        }
        return range;
    }

    public static OperatorType GetInverseOperatorType( OperatorType operatorType )
    {
        switch(operatorType){
            case OperatorType.PLUS:
                return OperatorType.MINUS;
            case OperatorType.MINUS:
                return OperatorType.PLUS;
            case OperatorType.MULTIPLY:
                return OperatorType.DIVIDE;
            case OperatorType.DIVIDE:
                return OperatorType.MULTIPLY;
            default:
                throw new InvalidOperationException( string.Format("Invalid operator type {0}", operatorType ) );
        }
    }

    public static uint ApplyOperator( uint firstOperand, uint secondOperand, OperatorType operatorType )
    {
        switch( operatorType )
        {
            case OperatorType.PLUS:
                return firstOperand + secondOperand;
            case OperatorType.MINUS:
                if (secondOperand > firstOperand)
                    throw new ArithmeticException( string.Format( "{0} - {1} will produce negative result.", firstOperand, secondOperand ) );
                return firstOperand - secondOperand;
            case OperatorType.MULTIPLY:
                return firstOperand * secondOperand;
            case OperatorType.DIVIDE:
                if (secondOperand == 0)
                    throw new DivideByZeroException( "The denominator cannot be zero." );
                else if (firstOperand % secondOperand != 0)
                    throw new ArithmeticException( string.Format( "The {0} is not divisible by {1}.", firstOperand, secondOperand ) );
                else
                    return firstOperand / secondOperand;
            default:
                throw new InvalidOperationException( string.Format("Invalid operator type {0}", operatorType ) );
        }
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
        uint biasNumDigit = 0;
        uint[] lowerExpectedRange;
        while(true)
        {
            try
            {
                lowerExpectedRange = GetLowerExpectedRange( value, operatorType, expectedNumDigit, biasNumDigit );
                break;
            }
            catch( ArgumentException e )
            {
                biasNumDigit++;
            }
            catch( Exception e )
            {
                throw e;
            }
        }

        uint secondOperandValue = lowerExpectedRange[ random.Next(0, lowerExpectedRange.Length) ];
        uint firstOperandValue = MathHelper.ApplyOperator( value, secondOperandValue, MathHelper.GetInverseOperatorType( operatorType ) );

        if(numOperator == 1)
        {
            ValueExpression firstOperand = new ValueExpression( firstOperandValue );
            ValueExpression secondOperand = new ValueExpression( secondOperandValue );
            return new OperationExpression( operatorType, firstOperand, secondOperand );
        }
        else
        {
            uint firstNumOperator = (uint)random.Next(0, (int)numOperator);
            uint secondNumOperator = numOperator - firstNumOperator - 1;
            MathExpression firstOperand = GenerateMathExpressionByValue( firstOperandValue, firstNumOperator, expectedNumDigit, allowOperatorTypeArray );
            MathExpression secondOperand = GenerateMathExpressionByValue( secondOperandValue, secondNumOperator, expectedNumDigit, allowOperatorTypeArray );
            return new OperationExpression( operatorType, firstOperand, secondOperand );
        }
    }

    public static uint[] GetLowerExpectedRange(uint result, OperatorType operatorType, uint expectedNumDigit, uint biasNumDigit=0)
    {
        uint resultNumDigit = (uint)(result.ToString().Length);
        if(resultNumDigit < 1)
            throw new ArithmeticException( "The number of result digits cannot be less than 1." );
        switch (operatorType)
        {
            case OperatorType.DIVIDE:
                if (biasNumDigit < resultNumDigit - 1)
                    throw new ArgumentException( "Too few number of bias digits." );
                else if (resultNumDigit > expectedNumDigit)
                    throw new ArithmeticException( "The number of expected digits cannot be less than of result." );
                else
                {
                    uint minExpectedNum = (uint)Math.Pow( 10, Math.Max( expectedNumDigit - biasNumDigit, 1 ) - 1);
                    uint maxExpectedNum = (uint)((Math.Pow(10, expectedNumDigit) - 1) / result);
                    return MathHelper.GenerateUintRange( minExpectedNum, maxExpectedNum );
                }
            case OperatorType.MULTIPLY:
                if (biasNumDigit < resultNumDigit/2)
                    throw new ArgumentException( "Too few number of bias digits." );
                else
                {
                    List<uint> primeFactorList = MathHelper.Factorize( result );
                    List<uint> allFactorList = MathHelper.GenerateAllFactorList( primeFactorList );
                    allFactorList = allFactorList.FindAll( x => x >= Math.Pow( 10, Math.Max( expectedNumDigit - biasNumDigit, 1 ) - 1)
                                                                && x <= Math.Pow( 10, biasNumDigit + 1 ) );
                    return allFactorList.ToArray();
                }
            case OperatorType.MINUS:
                {
                    uint minExpectedNum = (uint)Math.Pow( 10, Math.Max( expectedNumDigit - biasNumDigit, 1 ) - 1);
                    uint maxExpectedNum = (uint)((Math.Pow(10, expectedNumDigit) - 1) - result);
                    return MathHelper.GenerateUintRange( minExpectedNum, maxExpectedNum );
                }
            case OperatorType.PLUS:
                if (expectedNumDigit > resultNumDigit)
                    throw new ArithmeticException( "The number of expected digits cannot be greater than of result." );
                else if( 2*(Math.Pow( 10, expectedNumDigit ) - 1) < result )
                    throw new ArithmeticException( "Given number of expected digits will produce an empty range." );
                else if( 2*(Math.Pow( 10, Math.Max( expectedNumDigit - biasNumDigit, 1 ) - 1 )) > result )
                    throw new ArgumentException( "Given number of expected digits and bias digits will produce an empty range." );
                else
                {
                    uint minExpectedNum = (uint)Math.Max( result - Math.Pow( 10, expectedNumDigit ) - 1, Math.Pow( 10, Math.Max( expectedNumDigit - biasNumDigit, 1 ) - 1));
                    uint maxExpectedNum = (uint)Math.Min( result - minExpectedNum, Math.Pow( 10, expectedNumDigit ) - 1 );
                    return MathHelper.GenerateUintRange( minExpectedNum, maxExpectedNum );
                }
            default:
                throw new InvalidOperationException();
        }
    }
}