using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Abacus : MonoBehaviour
{
    public Text numberText;

    [SerializeField]
    private GameObject abacusStackPrefab;

    [SerializeField]
    private int numDigit;
    [SerializeField]
    private float abacusWidth;
    [SerializeField]
    private float abacusHeight;

    private List<GameObject> abacusStackList;

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

    public void ChangeAbacusStack()
    {
        numberText.text = ReadValue().ToString().PadLeft(numDigit,'0');
    }

    public int ReadValue()
    {
        int value = 0;
        for(int i=0; i < numDigit; i++)
        {
            AbacusStack abacusStack = abacusStackList[i].GetComponent<AbacusStack>();
            value += abacusStack.ReadValue() * (int)Mathf.Pow(10, i);
        }
        return value;
    }

    private void Start()
    {
        PopulateAbacus();
        ChangeAbacusStack();
    }
}
