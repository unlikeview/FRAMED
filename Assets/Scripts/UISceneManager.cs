using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;




public class UISceneManager : MonoBehaviour
{

   public void OnClickStartBtn()
   {
       SceneManager.LoadScene("SceneLoader");
   }
}
