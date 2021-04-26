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
                            break;
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



