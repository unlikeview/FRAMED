
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaceMentControllerWithMultiple : MonoBehaviour
{
    public GameObject placedPrefab;

    private bool isModelExist = false;     // 모델 한 프리셋씩만 생성

    private ARRaycastManager arRaycastManager;

    // 프리셋 모델들 버튼
    [SerializeField]
    private Button presetNumb1;
    [SerializeField]
    private Button presetNumb2;
    [SerializeField]
    private Button presetNumb3;
    [SerializeField]
    private Button presetNumb4;
    [SerializeField]
    private Button presetNumb5;
    [SerializeField]
    private Text SelectionText;  // 프리셋 종류를 표기

    [SerializeField]
    private Button dismissButton; // WelcomePanel을 종료하기 위한 버튼
    [SerializeField]
    private GameObject welcomePanel;



    [SerializeField]
    private Slider scaleSlider;
    [SerializeField]
    private Text scaleTextValue; // 스케일 값 달아주기 
    [SerializeField]
    private Slider rotationSlider;
    private PlacementObject lastSelectedObject;

    [SerializeField]
    private Camera arCamera;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    GameObject createdModel;

    public GameObject selectionEffect;

    [SerializeField]
    private Button lockButton;

    private bool isLocked = false;

    [SerializeField]
    private Button gridButton;   // 그리드 버튼 추가 (락 상태에서만 실행 가능)

    private bool isGrid = false;

    public GameObject gridPNG;


    public GameObject PreventUISelection;    // 터치 영역 제한 

    private RectTransform rectTrans;

    private bool isTouchPosition = false;

    void Awake()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return; // 중복 클릭 못하게 만들기 (유아이랑 겹쳐지는 부분 클릭못하게하기)
        arRaycastManager = GetComponent<ARRaycastManager>();

        //디폴트로 떠있을것 미리 선언해주기 ex.  ChangePrefabTo("ARBlue"); 
        //-----.onclick.AddListener(() => ChangePrefabTo("GuideLine");

        presetNumb1.onClick.AddListener(() => ChangePrefabTo("Romance"));
        presetNumb2.onClick.AddListener(() => ChangePrefabTo("Adventure"));
        presetNumb3.onClick.AddListener(() => ChangePrefabTo("Action"));
        presetNumb4.onClick.AddListener(() => ChangePrefabTo("Sports"));
        presetNumb5.onClick.AddListener(() => ChangePrefabTo("Extreme"));
        dismissButton.onClick.AddListener(Dismiss);
        scaleSlider.onValueChanged.AddListener((val) => OnScaleChanged(val));
        rotationSlider.onValueChanged.AddListener((val) => OnRotationChanged(val));


        if (lockButton != null && gridButton != null)      // Lock버튼이 존재하는지 체크 
        {
            lockButton.onClick.AddListener(Lock);
            gridButton.onClick.AddListener(Grid);
        }

        if (PreventUISelection != null)
        {
            rectTrans = PreventUISelection.GetComponent<RectTransform>();
        }

    }


    private void Lock()
    {
        isLocked = !isLocked;
        lockButton.GetComponentInChildren<Text>().text = isLocked ? "Now Locked" : "Now UnLocked";
        if (createdModel != null)
        {
            createdModel.GetComponent<PlacementObject>().SetOverlayText("GuideGrid MoveModels");
            Debug.Log($"{isLocked} 확인");  // 여기까진 확인 완료 찍힘. 
        }
    }

    private void Grid()
    {
        if (isLocked == true)
        {
            isGrid = !isGrid;
            gridButton.GetComponentInChildren<Text>().text = isGrid ? "<color=#9BBFEA>Activate</color>" : "<color=#FFFFFF>Activate</color>";

            if (isGrid == true)
            {
                gridPNG.SetActive(true);
            }
            else if (isGrid == false)
            {
                gridPNG.SetActive(false);
            }
        }
    }

    //Dismiss 누르는 순간 없애주기 

    private void Dismiss()
    {
        welcomePanel.SetActive(false);
    }

    void ChangePrefabTo(string prefabName)
    {
        isModelExist = false;

        Destroy(createdModel);   // 다른 프리셋 누를시 이전 프리셋을 삭제하기

        placedPrefab = Resources.Load<GameObject>($"Prefabs/{prefabName}"); //리소스 폴더 필요

        if (placedPrefab == null)
        {
            Debug.LogError($"프리팹{prefabName}이 없습니다. ");
        }

        switch (prefabName)
        {
            case "Action":
                SelectionText.text = $"Selected : <color=#ff0000>{prefabName}</color>";
                break;
            case "Adventure":
                SelectionText.text = $"Selected : <color=#ff0000>{prefabName}</color>";
                break;
            case "Romance":
                SelectionText.text = $"Selected : <color=#ff0000>{prefabName}</color>";
                break;
            case "Sports":
                SelectionText.text = $"Selected : <color=#ff0000>{prefabName}</color>";
                break;
            case "Extreme":
                SelectionText.text = $"Selected : <color=#ff0000>{prefabName}</color>";
                break;
        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        else
        {
            touchPosition = default;

            return false;
        }
    }


    public void OnScaleChanged(float newValue)   //On 앞에 붙으면 이벤트를 불러온다는뜻 
    {

        if (lastSelectedObject != null && lastSelectedObject.Selected)
        {
            Debug.Log("선택된 오브젝트 사이즈" + lastSelectedObject.Selected);
            lastSelectedObject.transform.localScale = Vector3.one * newValue;

            scaleTextValue.GetComponentInChildren<Text>().text = $"Scale : {(int)newValue} x";
            scaleTextValue.text = $"Scale : {(int)newValue} x";
        }
    }

    public void OnRotationChanged(float newValue)
    {
        if (lastSelectedObject != null && lastSelectedObject.Selected)
        {
            Debug.Log("선택된 오브젝트 회전" + $"{lastSelectedObject.Selected}");
            lastSelectedObject.transform.eulerAngles = Vector3.up * newValue;     // 에러? 
        }
    }

    void Update()
    {

        if (!TryGetTouchPosition(out Vector2 touchPosition)) return;  //  터치가 없으면 리턴 


        if (placedPrefab == null || welcomePanel.gameObject.activeSelf)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);
        touchPosition = touch.position;
        // 디버그 text.text = "x " + touchPosition.x + "y" + touchPosition.y; 
        if (touchPosition.x < Screen.width * 0.84f && touchPosition.y > Screen.height * 0.3f)
        {
            isTouchPosition = true;
        }
        else
        {
            isTouchPosition = false;
        }

        if (Input.touchCount > 0)
        {
            if (touch.phase == TouchPhase.Began && isTouchPosition == true)
            {

                Ray ray = arCamera.ScreenPointToRay(touchPosition);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.gameObject.layer == 6)
                    {
                        Debug.Log("레이발사");
                        Debug.Log($"hitObejct : {hitObject}");

                        lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();
                        Debug.Log($"lastSelectedObject.Selected 바깥: {lastSelectedObject.Selected}");

                        if (lastSelectedObject != null)
                        {
                            lastSelectedObject.Selected = true;

                            Debug.Log($"lastSelectedObject.Selected 선택된 모델 : {lastSelectedObject.Selected}");
                        }

                    }

                }
            }

        }



        if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon) && isTouchPosition == true)
        {
            // if (touch.phase == TouchPhase.Began)
            var hitPose = hits[0].pose;
            if (!isModelExist && lastSelectedObject == null)
            {
                createdModel = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);  // 오브젝트 생성 
                isModelExist = true;   // 모델 하나 존재
            }
            if (lastSelectedObject.Selected && Input.touchCount > 0 && isLocked == false)
            {  
                lastSelectedObject.transform.position = hitPose.position;
                lastSelectedObject.ChangeSelectedObject();
                Debug.Log($"바뀐 색은 = {lastSelectedObject}");

                /*     SkinnedMeshRenderer[] mss = lastSelectedObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                     foreach(var ms in mss)
                     {
                             ms.material = newMaterial;
                     }
                     */
                //Debug.Log($"ms: {ms.material}");
                //ms.material = newMaterial;
                //Debug.Log($"ms: {ms.material}");
                // lastSelectedObject.ChangeSelectedObject(ms);
                //lastSelectedObject.transform.rotation = hitPose.rotation;  켜있을시 자꾸 원점으로 회전됨. 

            }
        }
        if (touch.phase == TouchPhase.Ended)
        {
            Debug.Log("터치 종료");
            lastSelectedObject.BackToBeginColor();
        }
    }
}
