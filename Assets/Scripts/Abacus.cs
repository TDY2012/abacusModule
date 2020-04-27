using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Abacus : MonoBehaviour
{
    [SerializeField]
    private GameObject abacusStackPrefab;
    [SerializeField]
    private uint numDigit;
    public uint NumDigit {
        get {
            return numDigit;
        }
    }
    [SerializeField]
    private float abacusWidth;
    [SerializeField]
    private float abacusHeight;
    private GameManager gameManager;
    private List<GameObject> abacusStackList;

    public void BindToGameManager(GameManager inputGameManager)
    {
        gameManager = inputGameManager;
    }

    public void PopulateAbacus()
    {
        abacusStackList = new List<GameObject>();

        float abacusStackSpace = abacusWidth/numDigit;
        for (int i = 0; i < numDigit; i++)
        {
            GameObject abacusStackObject = Instantiate(abacusStackPrefab, this.transform);
            AbacusStack abacusStack = abacusStackObject.GetComponent<AbacusStack>();
            abacusStack.PopulateAbacusStack();
            abacusStack.BindToAbacus(this, i);
            abacusStack.transform.localPosition = Vector3.left * ((abacusStackSpace*(i+1)) - ((abacusWidth + abacusStackSpace)/2));
            abacusStackList.Add(abacusStackObject);
        }
    }

    public void ResetAbacus()
    {
        foreach( GameObject abacusStackObject in abacusStackList)
        {
            AbacusStack abacusStack = abacusStackObject.GetComponent<AbacusStack>();
            abacusStack.ResetAbacusStack();
        }
    }

    public void ChangeAbacusStack()
    {
        gameManager.UpdateQuestionText();
    }

    public uint ReadValue()
    {
        uint value = 0;
        for(int i=0; i < numDigit; i++)
        {
            AbacusStack abacusStack = abacusStackList[i].GetComponent<AbacusStack>();
            value += abacusStack.ReadValue() * (uint)Mathf.Pow(10, i);
        }
        return value;
    }

    public string ReadValueAsString()
    {
        string value = "";
        for (int i = 0; i < numDigit; i++)
        {
            AbacusStack abacusStack = abacusStackList[i].GetComponent<AbacusStack>();
            value = abacusStack.ReadValueAsString() + value;
        }
        return value;
    }
}
