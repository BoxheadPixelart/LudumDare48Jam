using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerUIController : MonoBehaviour
{
    GraphicRaycaster gr;
    public List<RaycastResult> results = new List<RaycastResult>();
    public List<GameObject> obj; 
    PointerEventData ped = new PointerEventData(null);
    public string output;
    public RectTransform cursor;
    public LayerMask buttonLayer;
    public LayerMask panelLayer;
    public BotUIController panel;
    BotUIController cachedPanel; 
    // Start is called before the first frame update
    void Start()
    {
      gr = GetComponent<GraphicRaycaster>();
      
    }

    // Update is called once per frame
    void Update()
    {
        ped.position = Input.mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            print("Player UI Detecting Click");
            gr.Raycast(ped, results);
            print("Player UI Raycasting");
            if (results.Count > 0)
            {
                print("Player UI Raycaster has result: " + results[results.Count - 1].gameObject.name);
                foreach (RaycastResult hit in results)
                {
                    Button button = hit.gameObject.GetComponent<Button>();
                    if (button)
                    {
                        button.onClick.Invoke();
                    }
                }
              
         
             //cursor.anchoredPosition = new Vector3(results[results.Count - 1].screenPosition.x, results[results.Count - 1].screenPosition.y);
            }
        } 
        //if (Input.GetMouseButtonUp(0))
       // {
          //  results.Clear(); 
       // }
        //on  hover

        gr.Raycast(ped, results);
        if (results.Count > 0)
        {
              if (Input.GetMouseButtonDown(0))
              {
                  foreach (RaycastResult hit in results)
                  {
                      Button button = hit.gameObject.GetComponent<Button>();
                      if (button)
                      {
                          button.onClick.Invoke();
                      }
                  }
              }
             foreach (RaycastResult hit in results)
             {

                  panel = hit.gameObject.transform.parent.GetComponent<BotUIController>();
                     if (panel)
                     {
                         panel.LookAtCamera();
                        break; 
                     }
                 
       
             }
          
        //cursor.anchoredPosition = new Vector3(results[results.Count - 1].screenPosition.x, results[results.Count - 1].screenPosition.y);
        }
        else
        {
            if (panel)
            {
                panel.ResetLook();
                panel = null;
            }
            
        }

        if (panel != cachedPanel)
        {
            cachedPanel?.ResetLook(); 
        }


        cachedPanel = panel;
    }
    private void LateUpdate()
    {
        results.Clear();
        
    }
}
    //Code to be place in a MonoBehaviour with a GraphicRaycaster component

    //Create the PointerEventData with null for the EventSystem
  
    //Set required parameters, in this case, mouse position
 
//Create list to receive all results


