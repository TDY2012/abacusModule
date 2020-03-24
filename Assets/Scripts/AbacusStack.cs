using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbacusStack : MonoBehaviour
{
    public const int numTopAbacusBead = 1;
    public const int numBottomAbacusBead = 4;

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
    private int state;

    private GameObject draggingAbacusBeadRoot;

    public bool IsValidInputSize(){

        if( abacusBeadHeight*numTopAbacusBead >= topAbacusStackHeight )
            return false;
        if( abacusBeadHeight*numBottomAbacusBead >= bottomAbacusStackHeight )
            return false;
        return true;
    }

    public bool IsValidAbacusIndex( int index, bool isTop ){
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
            abacusBeadObject.transform.localPosition += Vector3.up * ( topAbacusStackHeight + halfAbacusStackSpace - ( abacusBeadHeight/2 + i*abacusBeadHeight ) );
            AbacusBead abacusBead = abacusBeadObject.GetComponent<AbacusBead>();
            abacusBead.BindToAbacusStack( this, i, true );
            topAbacusBeadList.Add( abacusBeadObject );
        }

        for(int i=0; i<numBottomAbacusBead; i++){
            GameObject abacusBeadObject = Instantiate( abacusBeadPrefab, this.transform );
            abacusBeadObject.transform.localPosition += Vector3.down * ( bottomAbacusStackHeight + halfAbacusStackSpace - ( abacusBeadHeight/2 + i*abacusBeadHeight ) );
            AbacusBead abacusBead = abacusBeadObject.GetComponent<AbacusBead>();
            abacusBead.BindToAbacusStack( this, i, false );
            bottomAbacusBeadList.Add( abacusBeadObject );
        }
    }

    public List<GameObject> GetDraggingAbacusBeadList( int index, bool isTop ){
        if( !IsValidAbacusIndex( index, isTop ) )
            return null;
        
        List<GameObject> draggingAbacusBeadList = new List<GameObject>();
        int numAbacusBead = isTop ? numTopAbacusBead : numBottomAbacusBead;
        for(int i=index; i<numAbacusBead; i++){
            if(isTop)
                draggingAbacusBeadList.Add( topAbacusBeadList[i] );
            else
                draggingAbacusBeadList.Add( bottomAbacusBeadList[i] );
        }

        return draggingAbacusBeadList;
    }

    public void BeginDragAbacusBead( int index, bool isTop ){
        Debug.Log("Begin Drag");
        List<GameObject> draggingAbacusBeadList = GetDraggingAbacusBeadList( index, isTop );

        if( draggingAbacusBeadList is null )
            return;

        draggingAbacusBeadRoot = new GameObject();
        draggingAbacusBeadRoot.transform.parent = transform;
        foreach( GameObject obj in draggingAbacusBeadList ){
            obj.transform.parent = draggingAbacusBeadRoot.transform;
        }
    }

    public void DragAbacusBead(){
    }

    public void EndDragAbacusBead(){
        Debug.Log("End Drag");
        for(int i=0; i<draggingAbacusBeadRoot.transform.childCount; i++){
            draggingAbacusBeadRoot.transform.GetChild(i).parent = transform;
        }
    }

    void Start(){

        PopulateAbacusStack();
    }

    void Update(){
        
    }
}
