using System.Collections;
using System.Collections.Generic;
//using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Rendering;

public class SlotRow : MonoBehaviour
{
     int numberOfSlots;
     int numberWinSlots;
     float slotDistance = 0.25f;

    //inserire giocatore scelto ggggggg
     private Sprite playerSprite;
     private Sprite mouseSprite;

     private List<GameObject> reorderSlots;

     public bool stopped { get; private set; }
     bool isSlowDown;

    float rotationSpeed;
    float stabilitationTime;

    private Vector3 finalRowRotation;
    private Vector3 startRowRotation;

    private Vector3 targetPosition;

    [SerializeField] private Slot selectedSlot;
    private Slotmachine mainMachine;

    private void Start()
    {
        mainMachine = GetComponentInParent<Slotmachine>();
        stopped = true;
        selectedSlot = null; //reset della slot presa
        reorderSlots = new List<GameObject>();
        InitializeRow();

        //momentaneo
        stopped = false;
    }

    private void Update()
    {
        RotateRow();
    }

    public void SetRow(int slotNumber, int winNumber, float distance, Sprite playerSprite,Sprite enemySprite,float rotationSpeed,float stabilizationSpeed)
    {
        numberOfSlots = slotNumber;
        numberWinSlots = winNumber;
        slotDistance = distance;
        this.playerSprite = playerSprite;
        mouseSprite = enemySprite;
        this.rotationSpeed = rotationSpeed;
        stabilitationTime = stabilizationSpeed;
        
    }

    private void InitializeRow()
    {
        List<GameObject> generatedSlots = new List<GameObject>();
        int winGenerate = 0;

        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = new GameObject($"slot #{i}");
            slot.transform.SetParent(gameObject.transform, true);

            slot.AddComponent<Slot>();

            if (winGenerate < numberWinSlots)
            {
                slot.GetComponent<Slot>().Sprite = playerSprite;
                slot.GetComponent<Slot>().Type = slotType.Player;

                winGenerate++;
            }
            else
            {
                slot.GetComponent<Slot>().Sprite = mouseSprite;
                slot.GetComponent<Slot>().Type = slotType.Mouse;
            }

            generatedSlots.Add(slot);

            

            
        }

        for(int i=0; i< numberOfSlots; i++)
        {
            int random= Random.Range(0, generatedSlots.Count);

            GameObject slot = generatedSlots[random];
            generatedSlots.Remove(slot);

            if (reorderSlots.Count == 0)
            {
                slot.transform.position = gameObject.transform.position;
                reorderSlots.Add(slot);
                //inserire varianti
            }
            else
            {
                GameObject previousSlot = reorderSlots[reorderSlots.Count - 1];
                slot.transform.position = new Vector2(previousSlot.transform.position.x, previousSlot.transform.position.y - slotDistance);
                reorderSlots.Add(slot);
                //inserire varianti
            }
        }


        Vector3 firstSlot = reorderSlots[0].transform.localPosition;
        startRowRotation = new Vector3(firstSlot.x, firstSlot.y-(slotDistance/2) + firstSlot.z);

        Debug.Log(startRowRotation.y);

        Vector3 lastSlot = reorderSlots[reorderSlots.Count - 1].transform.localPosition;
        finalRowRotation=new Vector3(lastSlot.x, lastSlot.y-(slotDistance/2), lastSlot.z);

        Debug.Log(finalRowRotation.y);
    }

    public void RotateRow()
    {
        

        if (!stopped && !isSlowDown)
        {
            transform.localPosition += Vector3.up * Time.deltaTime * rotationSpeed;

            if (transform.localPosition.y >= -finalRowRotation.y)
            {
                transform.localPosition = new Vector3(transform.position.x,startRowRotation.y);
                
            }
        }
        else if (isSlowDown)
        {
            isSlowDown = false;
            
        }


        //if (Input.GetMouseButtonDown(0) && !isSlowDown)
        //{
        //    StopRow();
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    ResetRow();
        //}
        
    }

    public void StopRow()
    {
        

        Debug.Log(transform.localPosition.y % slotDistance);

        Vector3 targetDistance=Vector3.zero;
        isSlowDown = true;

        if(transform.localPosition.y % slotDistance > (slotDistance/2))
        {
            targetDistance = new Vector3(transform.localPosition.x, transform.localPosition.y + (slotDistance- (transform.localPosition.y % slotDistance)));  //ok
        }

        if(transform.localPosition.y % slotDistance <= (slotDistance/2))
        {
            targetDistance = new Vector3(transform.localPosition.x, transform.localPosition.y - (transform.localPosition.y % slotDistance)); //ok
        }

        targetPosition = targetDistance;

        Debug.LogFormat("Target Position : {0} , Posizione iniziale : {1}",targetPosition,transform.localPosition);

        StartCoroutine(StabilizateRow(targetPosition));
    }

    IEnumerator StabilizateRow(Vector3 target)
    {
        float elapsedTime = 0;

        while (elapsedTime < stabilitationTime)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, (elapsedTime/stabilitationTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;

        int stoppedPosition = (int)(transform.localPosition.y / slotDistance);

        selectedSlot = reorderSlots[stoppedPosition].GetComponent<Slot>();

        stopped = true;


        //if (GameObject.ReferenceEquals(mainMachine.GetLastRow(), gameObject))
        //{
        //    Debug.Log(gameObject);
        //    mainMachine.CheckForWin();  //non gli va bene quando si fermano tutte e 4 contemporeamente
        //}

        mainMachine.CheckAllRowStopped();
    }

    
    public Slot GetSelectedSlot()
    {
        return selectedSlot;
    }

    public void ResetRow()
    {
        stopped = false;
        isSlowDown = false;
        selectedSlot = null;
    }

    public void StartSlowDown()
    {
        if(!isSlowDown)
        {
            StopRow();
        }
    }

   
}
