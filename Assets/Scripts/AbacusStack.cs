using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbacusStack : MonoBehaviour
{
    public const uint numTopAbacusBead = 1;
    public const uint numBottomAbacusBead = 4;

    private Abacus abacus;
    private int index;

    [SerializeField]
    private GameObject abacusBeadPrefab;
    [SerializeField]
    private float abacusBeadWidth;
    [SerializeField]
    private float abacusBeadHeight;
    [SerializeField]
    private float topAbacusStackHeight;
    [SerializeField]
    private float bottomAbacusStackHeight;
    [SerializeField]
    private float abacusStackHeight;

    private List<GameObject> topAbacusBeadList;
    private List<GameObject> bottomAbacusBeadList;
    private List<bool> topAbacusBeadStateList;
    private List<bool> bottomAbacusBeadStateList;

    public void BindToAbacus(Abacus inputAbacus, int inputIndex)
    {
        abacus = inputAbacus;
        index = inputIndex;
    }

    public bool IsValidInputSize(){

        if( abacusBeadHeight*numTopAbacusBead >= topAbacusStackHeight )
            return false;
        if( abacusBeadHeight*numBottomAbacusBead >= bottomAbacusStackHeight )
            return false;
        return true;
    }

    public bool IsValidAbacusIndex( uint index, bool isTop ){
        if( isTop )
            return ( index >= 0 ) && ( index < numTopAbacusBead );
        else
            return ( index >= 0 ) && ( index < numBottomAbacusBead );
    }

    public void PopulateAbacusStack(){

        if( !IsValidInputSize() ){
            Debug.Log("PopulateAbacusStack() - Invalid input size.");
            return;
        }

        topAbacusBeadList = new List<GameObject>();
        bottomAbacusBeadList = new List<GameObject>();

        float halfAbacusStackSpace = (abacusStackHeight - (topAbacusStackHeight + bottomAbacusStackHeight))/2;

        for(int i=0; i<numTopAbacusBead; i++){
            GameObject abacusBeadObject = Instantiate( abacusBeadPrefab, this.transform );
            AbacusBead abacusBead = abacusBeadObject.GetComponent<AbacusBead>();
            abacusBead.BindToAbacusStack( this, i, true );
            topAbacusBeadList.Add( abacusBeadObject );
        }

        for(int i=0; i<numBottomAbacusBead; i++){
            GameObject abacusBeadObject = Instantiate( abacusBeadPrefab, this.transform );
            AbacusBead abacusBead = abacusBeadObject.GetComponent<AbacusBead>();
            abacusBead.BindToAbacusStack( this, i, false );
            bottomAbacusBeadList.Add( abacusBeadObject );
        }

        ResetAbacusStack();
    }

    public List<GameObject> GetDraggingAbacusBeadList( uint index, bool isTop ){
        if( !IsValidAbacusIndex( index, isTop ) )
            return null;
        
        List<GameObject> draggingAbacusBeadList = new List<GameObject>();
        uint numAbacusBead = isTop ? numTopAbacusBead : numBottomAbacusBead;
        for(int i=(int)index; i<numAbacusBead; i++){
            if(isTop)
                draggingAbacusBeadList.Add( topAbacusBeadList[i] );
            else
                draggingAbacusBeadList.Add( bottomAbacusBeadList[i] );
        }

        return draggingAbacusBeadList;
    }

    public void ResetAbacusStack()
    {
        topAbacusBeadStateList = new List<bool>();
        bottomAbacusBeadStateList = new List<bool>();
        for (int i = 0; i < numTopAbacusBead; i++)
        {
            topAbacusBeadStateList.Add(false);
        }
        for (int i = 0; i < numBottomAbacusBead; i++)
        {
            bottomAbacusBeadStateList.Add(false);
        }
        UpdateAbacusBeadPosition();
    }

    public void UpdateAbacusBeadPosition()
    {
        float halfAbacusStackSpace = (abacusStackHeight - (topAbacusStackHeight + bottomAbacusStackHeight)) / 2;

        for (int i = 0; i < numTopAbacusBead; i++)
        {
            if (!topAbacusBeadStateList[i])
            {
                topAbacusBeadList[i].transform.localPosition = Vector3.up * (topAbacusStackHeight + halfAbacusStackSpace - (abacusBeadHeight / 2 + i * abacusBeadHeight));
            }
            else
            {
                topAbacusBeadList[i].transform.localPosition = Vector3.up * (halfAbacusStackSpace + (abacusBeadHeight / 2) + ((numTopAbacusBead - i - 1) * abacusBeadHeight));
            }
        }
        for (int i = 0; i < numBottomAbacusBead; i++)
        {
            if (!bottomAbacusBeadStateList[i])
            {
                bottomAbacusBeadList[i].transform.localPosition = Vector3.down * (bottomAbacusStackHeight + halfAbacusStackSpace - (abacusBeadHeight / 2 + i * abacusBeadHeight));
            }
            else
            {
                bottomAbacusBeadList[i].transform.localPosition = Vector3.down * (halfAbacusStackSpace + (abacusBeadHeight / 2) + ((numBottomAbacusBead - i - 1) * abacusBeadHeight));
            }
        }
    }

    public void MoveAbacusBead( int index, bool isTop ){
        
        if (isTop)
        {
            bool toggledState = !topAbacusBeadStateList[index];
            if(toggledState)
            {
                for(int i=index; i<numTopAbacusBead; i++)
                {
                    topAbacusBeadStateList[i] = toggledState;
                }
            }
            else
            {
                for (int i = index; i >= 0; i--)
                {
                    topAbacusBeadStateList[i] = toggledState;
                }
            }
        }
        else
        {
            bool toggledState = !bottomAbacusBeadStateList[index];
            if (toggledState)
            {
                for (int i = index; i < numBottomAbacusBead; i++)
                {
                    bottomAbacusBeadStateList[i] = toggledState;
                }
            }
            else
            {
                for (int i = index; i >= 0; i--)
                {
                    bottomAbacusBeadStateList[i] = toggledState;
                }
            }
        }

        UpdateAbacusBeadPosition();
        abacus.ChangeAbacusStack();
    }

    public uint ReadValue()
    {
        uint value = 0;

        for (int i = 0; i < numTopAbacusBead; i++)
        {
            value += (uint)(topAbacusBeadStateList[i] ? 5 * (i + 1) : 0);
        }
        for (int i = 0; i < numBottomAbacusBead; i++)
        {
            value += (uint)(bottomAbacusBeadStateList[i] ? 1 : 0);
        }

        return value;
    }

    public string ReadValueAsString()
    {
        return ReadValue().ToString();
    }

    public override string ToString()
    {
        string result = "";
        foreach (bool bottomAbacusBeadState in bottomAbacusBeadStateList)
        {
            result += bottomAbacusBeadState ? "O" : "X";
        }
        result += "|";
        foreach ( bool topAbacusBeadState in topAbacusBeadStateList)
        {
            result += topAbacusBeadState ? "O" : "X";
        }
        return result;
    }
}
