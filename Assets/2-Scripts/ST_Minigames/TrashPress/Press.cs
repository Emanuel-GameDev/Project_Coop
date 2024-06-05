using System.Collections;
using UnityEngine;

public class Press : MonoBehaviour
{
    [Header("LeftPress")]
    [SerializeField] GameObject leftPress;
    [SerializeField] GameObject leftStartingPos;
    [SerializeField] GameObject leftEndingPos;
    [SerializeField] GameObject leftEndingPreviewPos;
    [Header("RightPress")]
    [SerializeField] GameObject rightPress;
    [SerializeField] GameObject rightStartingPos;
    [SerializeField] GameObject rightEndingPos;
    [SerializeField] GameObject rightEndingPreviewPos;


    private float speed;
    private float previewTimer;
    private bool startedPreviw;
    private bool startedMove;
    private float tempTimer = 0;

    public IEnumerator Activate(float speed, float previewTimer)
    {
        this.speed = speed;
        this.previewTimer = previewTimer;
        startedPreviw = true;
        startedMove = false;

        leftPress.GetComponent<SpriteRenderer>().color = Color.red;
        rightPress.GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(previewTimer);

        leftPress.GetComponent<SpriteRenderer>().color = Color.green;
        rightPress.GetComponent<SpriteRenderer>().color = Color.green;
        startedPreviw = false;
        startedMove = true;
    }

    private void Update()
    {
        if (startedPreviw)
        {
           
            float distanceRight = Vector3.Distance(rightStartingPos.transform.position, rightEndingPreviewPos.transform.position);
            float distanceLeft = Vector3.Distance(leftStartingPos.transform.position, leftEndingPreviewPos.transform.position);

            leftPress.transform.position = leftStartingPos.transform.position;

            if (tempTimer <= previewTimer)
            {
                //left          
                float pingPong1 = Mathf.PingPong(Time.time * speed, distanceLeft) / distanceLeft;
                leftPress.transform.position = Vector3.Lerp(leftStartingPos.transform.position, leftEndingPreviewPos.transform.position, pingPong1);


                //right
                float pingPong2 = Mathf.PingPong(Time.time * speed, distanceRight) / distanceRight;
                rightPress.transform.position = Vector3.Lerp(rightStartingPos.transform.position, rightEndingPreviewPos.transform.position, pingPong2);

               

            }
            else
            {
                tempTimer += Time.deltaTime;
            }


        }
        else if(startedMove)
        {           
            rightPress.transform.position = Vector3.Lerp(rightPress.transform.position, rightEndingPos.transform.position, (speed * Time.deltaTime));
            leftPress.transform.position = Vector3.Lerp(leftPress.transform.position, leftEndingPos.transform.position, (speed * Time.deltaTime));

            if((Vector3.Distance(rightPress.transform.position,rightEndingPos.transform.position) <0.1f) && 
                    (Vector3.Distance(leftPress.transform.position, leftEndingPos.transform.position) < 0.1f))
            {
                Debug.Log("ARRIVATOOOO");
            }
        }
    }


}
