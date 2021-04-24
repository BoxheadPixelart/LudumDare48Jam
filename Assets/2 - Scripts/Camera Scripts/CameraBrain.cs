using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class CameraBrain : MonoBehaviour
{
    public Transform lookAt;
    public Transform followTransform; 
    public bool followTarget;
    Vector3 startPos;
    CinemachineBrain cBrain;
    CinemachineVirtualCamera vCam;
    float zoomTimer;
    float zoomTime;
    Vector3 lastFramePos;
    float zoomValue;

    float defaultFOV = 60;
    float minZoomValue = 20;
    public DG.Tweening.Core.TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> zoomInTween; 
    // Start is called before the first frame update
    void Start()
    {
        followTarget = false;
        startPos = lookAt.position;
        zoomTime = 3;
        cBrain = GetComponent<CinemachineBrain>();
        vCam = cBrain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        zoomValue = defaultFOV;
        SetZoomOut(); 
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(lookAt, transform.up);
      //  transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z); 

        if (followTarget)
        {
            lookAt.position = followTransform.position;
            if (IsLookAtStill())
            {
                
                zoomTimer += Time.deltaTime; 

                if (zoomTimer >= zoomTime)
                {
                    SetZoomIn(); 
                }
            } else
            {
                zoomTimer = 0;
                SetZoomOut(); 
            }
        }

        vCam.m_Lens.FieldOfView = zoomValue; 
        lastFramePos = lookAt.position;
    }
    bool IsLookAtStill()
    {
        Vector3 dist = lookAt.position - lastFramePos;
        if (dist == Vector3.zero)
        {
            return (true);
        } else
        {
            return (false);
        }
    }
    public void SelectTarget(Transform target)
    {
        followTransform = target;
        followTarget = true; 
    }
    public void DeselectTarget()
    {
        followTarget = false;
        followTransform = null;
        ReturnToStart();
        SetZoomOut(); 
    }

    public void ToggleTarget(Transform target)
    {
        if (followTarget)
        {
            DeselectTarget(); 
        } else
        {
            SelectTarget(target); 
        }
    }
    void ReturnToStart()
    {
        lookAt.DOMove(startPos, 1); 
    }

    void SetZoomIn()
    {
        if (zoomInTween == null)
        {
            zoomInTween = DOTween.To(() => zoomValue, x => zoomValue = x, minZoomValue, 3f).OnComplete(() => { zoomInTween = null;}) ; 
        }
        
    }

    void SetZoomOut()
    {
            if (zoomValue != defaultFOV)
            {
                  if (zoomInTween != null)
                  {
                        print("Has Killed Zoom In"); 
                      zoomInTween.Kill();
                     zoomInTween = null; 
                  }
            
                DOTween.To(() => zoomValue, x => zoomValue = x, defaultFOV, .5f);
            }
    }
}
