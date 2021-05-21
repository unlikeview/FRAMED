using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveCharacterHead : MonoBehaviour
{
    public string draggingTag;

    public Text switchText;

    public Camera cam;

    private Vector3 dis;

    private float posX;

    private float posY;

    private bool touched = false;

    private bool dragging = false;

    private bool isTurn = false;

    private Transform toDrag;
    private Rigidbody toDragRigidbody;
    private Vector3 previousePosition;

    [SerializeField]
    private Button OnOffBtn;

  //  public GameObject[] playerTags;

   // private GameObject basket; 

    void Awake()
    {
        OnOffBtn.onClick.AddListener(Switch);
        
    }

    private void Switch()
    {
        isTurn = !isTurn;
        switchText.GetComponentInChildren<Text>().text = isTurn ? "Now Control Body" : "-";
        if (isTurn == true)
        {
            GameObject.Find("Sports").transform.Find("Move_target").gameObject.SetActive(true);
     /*       foreach (GameObject playerTag in playerTags)
            {
                playerTag.SetActive(true);
            }
            */
        }
        else
        {
            GameObject.Find("Sports").transform.Find("Move_target").gameObject.SetActive(false);
          
          /*  foreach (GameObject playerTag in playerTags)
            {
                playerTag.SetActive(false);
            }
            */
        }

    }


    void FixedUpdate()
    {
        if (Input.touchCount != 1)

        {
            dragging = false;
            touched = false;
            return;
        }

        Touch touch = Input.touches[0];
        Vector3 pos = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit) && hit.collider.tag == draggingTag)
            {
                toDrag = hit.transform;
                previousePosition = toDrag.position;
                toDragRigidbody = toDrag.GetComponent<Rigidbody>();

                Debug.Log("리지드바디 체크");

                dis = cam.WorldToScreenPoint(previousePosition);
                posX = Input.GetTouch(0).position.x - dis.x;
                posY = Input.GetTouch(0).position.y - dis.y;

                SetDraggingProperties(toDragRigidbody);

                touched = true;

            }

        }
        if (touched && touch.phase == TouchPhase.Moved)
        {

            dragging = true;

            float posXNow = Input.GetTouch(0).position.x - posX;
            float posYNow = Input.GetTouch(0).position.y - posY;
            Vector3 curPos = new Vector3(posXNow, posYNow, dis.z);

            Vector3 worldPos = cam.ScreenToWorldPoint(curPos) - previousePosition;
            worldPos = new Vector3(worldPos.x, worldPos.y, 0.0f);

            toDragRigidbody.velocity = worldPos / (Time.deltaTime * 10);

            previousePosition = toDrag.position;

        }
        if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            dragging = false;
            // touched = false;
            previousePosition = new Vector3(0.0f, 0.0f, 0.0f);
            SetFreeProperties(toDragRigidbody);
        }
    }


    private void SetDraggingProperties(Rigidbody rb)

    {
        rb.isKinematic = false;
        rb.drag = 20;

    }
    private void SetFreeProperties(Rigidbody rb)

    {
        rb.isKinematic = true;
        rb.drag = 5;

    }
}
