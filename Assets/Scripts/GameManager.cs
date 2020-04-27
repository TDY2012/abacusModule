using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject abacusPrefab;
    [SerializeField]
    private Transform abacusTransform;
    [SerializeField]
    private Text questionText;
    [SerializeField]
    private Text statusText;
    [SerializeField]
    private string defaultStatusText;
    [SerializeField]
    private float statusDisplayTime_sec;

    private Abacus abacus;

    private IEnumerator setStatusTextCoroutine;
    private bool isEnableInput = true;

    public uint resultNumDigit;
    public uint numOperator;
    public uint expectedNumDigit;
    public OperatorType[] allowOperatorTypeArray;

    uint result;
    MathExpression mathExpression;

    public void CreateAbacus()
    {
        GameObject abacusObject = Instantiate(abacusPrefab, abacusTransform);
        abacus = abacusObject.GetComponent<Abacus>();
        abacus.PopulateAbacus();
        abacus.BindToGameManager(this);
    }

    public void GenerateQuestion()
    {
        //  Generate random result within given number of result digit
        System.Random random = new System.Random();
        result = (uint)random.Next((int)Math.Pow(10, resultNumDigit-1), (int)Math.Pow(10, resultNumDigit));

        //  Generate random math expression which evaluates equal to the result.
        //  Each operand number of digit in the expression should be around expected one.
        //  Only allowed operators be chose to generate this expression
        mathExpression = MathExpressionGenerator.GenerateMathExpressionByValue(result, numOperator, expectedNumDigit, allowOperatorTypeArray);

        //  Call update question text
        UpdateQuestionText();
    }

    public void UpdateQuestionText()
    {
        //  Generate question text and update to ui
        questionText.text = mathExpression.ToString();
        questionText.text += " = ";
        questionText.text += abacus.ReadValueAsString();
    }

    private IEnumerator ProceedToNextQuestion( float time )
    {
        yield return SetStatusText("Correct!", time);
        abacus.ResetAbacus();
        GenerateQuestion();
    }

    private IEnumerator SetStatusText( string text, float time )
    {
        statusText.text = text;
        yield return new WaitForSeconds(time);
        statusText.text = defaultStatusText;
    }

    public void VerifyAnswer()
    {
        if(setStatusTextCoroutine!=null)
            StopCoroutine(setStatusTextCoroutine);
        if (abacus.ReadValueAsString() == mathExpression.GetValue().ToString().PadLeft((int)abacus.NumDigit, '0'))
        {
            setStatusTextCoroutine = ProceedToNextQuestion(statusDisplayTime_sec);
        }
        else
        {
            setStatusTextCoroutine = SetStatusText("Wrong!", statusDisplayTime_sec);
        }
        StartCoroutine(setStatusTextCoroutine);
    }

    private void Start()
    {
        CreateAbacus();
        GenerateQuestion();
    }

    private void Update()
    {
        if (!isEnableInput)
            return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            VerifyAnswer();
        }
    }
}
