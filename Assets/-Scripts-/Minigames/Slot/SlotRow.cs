using System.Collections;
using System.Collections.Generic;
using System.Linq;

//using UnityEditor.Search;
using UnityEngine;
 using UnityEngine.Rendering;

public class SlotRow : MonoBehaviour
{
     int numberOfSlots;
     int numberWinSlots;
     float slotDistance = 0.25f;
     slotType winType;

    //inserire giocatore scelto
     public SlotPlayer selectedPlayer;
   


    private List<GameObject> reorderSlots = new List<GameObject>();

     public bool stopped { get; private set; }
     bool isSlowDown;

    float rotationSpeed;
    float stabilitationTime;

    private Vector3 finalRowRotation;
    private Vector3 startRowRotation;

    private Vector3 targetPosition;

    [SerializeField] private Slot selectedSlotImage;
    private Slotmachine mainMachine;

    [Header("Sounds")]
    [SerializeField] AudioClip stopSlotRowAudio;


    public void Initialize()
    {
        mainMachine = GetComponentInParent<Slotmachine>();
        stopped = true;
        selectedSlotImage = null; //reset della slot presa
        
        if(reorderSlots.Count > 0 )
        {
            for( int i = transform.childCount-1; i >= 0; i-- )
            {
               Destroy(transform.GetChild(i).gameObject);
            }

            reorderSlots.Clear();
            
        }

        InitializeRow();
        
    }

    private void Update()
    {
        RotateRow();
    }

    public void StartSlotMachine() 
    {
        stopped = false;
    }

    public void SetRow(int slotNumber, int winNumber, float distance, slotType winType,float rotationSpeed,float stabilizationSpeed)
    {
        numberOfSlots = slotNumber;
        numberWinSlots = winNumber;
        slotDistance = distance;

        this.winType = winType;
        this.rotationSpeed = rotationSpeed;
        stabilitationTime = stabilizationSpeed;
        
    }

    private void InitializeRow()
    {
        List<GameObject> generatedSlots = new List<GameObject>();
        List<slotType> remainingSlotType = new List<slotType>(mainMachine.allSlotType);

        remainingSlotType.Remove(winType);


        int winGenerate = 0;
        int typeindex = 0; //index per far generare almeno ogni tipo di immagine


        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = new GameObject($"slot #{i}");
            slot.AddComponent<Slot>();
            slot.transform.SetParent(gameObject.transform, true);

           

            if (winGenerate < numberWinSlots)
            {
                slot.GetComponent<Slot>().Type = winType;
                slot.GetComponent<Slot>().Sprite = mainMachine.ChoseSpriteBySlotType(winType);
                

                winGenerate++;
            }
            else if(typeindex<remainingSlotType.Count-1)
            {
                slot.GetComponent<Slot>().Type = remainingSlotType[typeindex];
                slot.GetComponent<Slot>().Sprite = mainMachine.ChoseSpriteBySlotType(remainingSlotType[typeindex]);

                typeindex++;
                          
            }
            else
            {
                slotType randomType = remainingSlotType[Random.Range(0,remainingSlotType.Count)];

                slot.GetComponent<Slot>().Type = randomType;
                slot.GetComponent<Slot>().Sprite = mainMachine.ChoseSpriteBySlotType(randomType);

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

        //set Additional image placeholder

        //add before

        for (int i = 0; i < 2; i++) 
        {
            GameObject slot = Instantiate(reorderSlots[reorderSlots.Count-1-i]);
            slot.transform.SetParent(gameObject.transform, true);
            slot.transform.localPosition = new Vector3(0, + ((slotDistance*i)+slotDistance), 0);

           
        }

        //add after

        for (int i = 0;i < 2; i++)
        {
            GameObject slot = Instantiate(reorderSlots[0+i]);
            slot.transform.SetParent(gameObject.transform, true);
            slot.transform.localPosition = new Vector3(0, reorderSlots[reorderSlots.Count-1].transform.localPosition.y -((slotDistance * i) + slotDistance), 0);
            
        }


    }

    public void RotateRow()
    {
        

        if (!stopped && !isSlowDown)
        {
            transform.localPosition += Vector3.up * Time.deltaTime * rotationSpeed;

            if (transform.localPosition.y >= -finalRowRotation.y)
            {
                transform.localPosition = new Vector3(transform.localPosition.x,startRowRotation.y);
                
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

        selectedSlotImage = reorderSlots[stoppedPosition].GetComponent<Slot>();

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
        return selectedSlotImage;
    }

    public void ResetRow()
    {
        stopped = false;
        isSlowDown = false;
        selectedSlotImage = null;
        
    }

    public void StartSlowDown()
    {
        if(!isSlowDown)
        {
            StopRow();
        }
    }


    private void SetAdditionalImage()
    {
        for (int i = 0; i <= 2; i++)
        {
            GameObject slot = new GameObject($"slot #{i}");
            slot.transform.SetParent(gameObject.transform, true);

            slot.AddComponent<Slot>();
        }
    }

    
   
}
