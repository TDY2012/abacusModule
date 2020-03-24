using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbacusBead : MonoBehaviour
{
    private AbacusStack abacusStack;
    private int index;
    private bool isTop;

    public void BindToAbacusStack( AbacusStack inputAbacusStack, int inputIndex, bool inputIsTop ){
        abacusStack = inputAbacusStack;
        index = inputIndex;
        isTop = inputIsTop;
    }

    void OnMouseDown(){
        abacusStack.BeginDragAbacusBead( index, isTop );
    }

    void OnMouseDrag(){
        abacusStack.DragAbacusBead();
    }

    void OnMouseUp(){
        abacusStack.EndDragAbacusBead();
    }
}
