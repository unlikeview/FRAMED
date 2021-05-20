using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ClassificationPlane : MonoBehaviour
{

    public ARPlane _ARPlane;

    public MeshRenderer _PlaneMeshRenderer;   // 색변경을 위해 

    public TextMesh _TextMesh;

    public GameObject _TextObj;

    GameObject _mainCam;   //카메라에 엑세스하기위함 
    // Start is called before the first frame update

    void Start()
    {
        _mainCam = FindObjectOfType<Camera>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLabel();
        UpdatePlaneColor();
    }

    void UpdateLabel()
    {
      //  _TextMesh.text = _ARPlane.classification.ToString(); //글씨 지우기
        _TextObj.transform.position = _ARPlane.center;
        _TextObj.transform.LookAt(_mainCam.transform);
        _TextObj.transform.Rotate(new Vector3(0, 180, 0));
    }

    void UpdatePlaneColor()
    {
        Color planeMatColor = Color.white;

        switch (_ARPlane.classification)
        {
            case PlaneClassification.None:
                planeMatColor = Color.white;
                break;
            case PlaneClassification.Floor:
                planeMatColor = Color.green;
                break;
            case PlaneClassification.Wall:
                planeMatColor = Color.white;
                break;
            case PlaneClassification.Table:
                planeMatColor = Color.yellow;
                break;
        }

        planeMatColor.a = 0.05f;  // 알파값 변경 
        _PlaneMeshRenderer.material.color = planeMatColor;

    }
}
