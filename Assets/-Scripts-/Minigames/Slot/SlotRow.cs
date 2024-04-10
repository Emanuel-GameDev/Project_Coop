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

    //inserire giocatore scelto
     public SlotPlayer selectedPlayer;

     private Sprite playerSprite;
     private List<Sprite> losingSpriteList;


    private List<GameObject> reorderSlots;

     public bool stopped { get; private set; }
     bool isSlowDown;

    float rotationSpeed;
    float stabilitationTime;

    private Vector3 finalRowRotation;
    private Vector3 startRowRotation;

    private Vector3 targetPosition;

    [SerializeField] private Slot selectedSlotImage;
    private Slotmachine mainMachine;



    private void Start()
    {
        losingSpriteList= new List<Sprite>();
    }

    public void Initialize()
    {
        mainMachine = GetComponentInParent<Slotmachine>();
        stopped = true;
        selectedSlotImage = null; //reset della slot presa
        reorderSlots = new List<GameObject>();
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

    public void SetRow(int slotNumber, int winNumber, float distance, Sprite playerSprite,List<Sprite>  losingSpriteList,float rotationSpeed,float stabilizationSpeed)
    {
        numberOfSlots = slotNumber;
        numberWinSlots = winNumber;
        slotDistance = distance;
        this.playerSprite = playerSprite;
        this.losingSpriteList = losingSpriteList;
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
                int index=Random.Range(0,losingSpriteList.Count);

                if (losingSpriteList[index] == null)
                {
                    Debug.LogError("Inserisci almeno uno sprite dentro la lista delle immagini perdenti");
                }

                slot.GetComponent<Slot>().Sprite = losingSpriteList[index];
                slot.GetComponent<Slot>().Type = slotType.OtherCharacter;
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
