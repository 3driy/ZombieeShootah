using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapturePointTracker : MonoBehaviour
{
    [SerializeField]
    GameObject trackerPointPrefab;
    [SerializeField]
    GameObject trackerPanel;
    GameObject[] capturePoints;
    GameObject[] trackerPoints;
    GameObject mainCamera;
    Vector3 origTrackerPos;
    Transform playerCamYaw;
    float halfPanelWidth;
    void Start()
    {
        capturePoints = GameObject.FindGameObjectsWithTag("Capturepoint");
        trackerPoints = new GameObject[capturePoints.Length];
        for (int i = 0; i < capturePoints.Length; i++)
        {
            trackerPoints[i] = Instantiate(trackerPointPrefab,trackerPanel.transform);
            origTrackerPos = trackerPoints[i].GetComponent<RectTransform>().position;
        }
        mainCamera = GameObject.Find("Main Camera");
        RectTransform panel = trackerPanel.transform as RectTransform;
        halfPanelWidth =  panel.rect.width * 0.5f;
        playerCamYaw = Camera.main.gameObject.transform;
    }

    void Update()
    {
        for (int i = 0; i < trackerPoints.Length; i++)
        {
            TrackerPoint script = trackerPoints[i].GetComponent<TrackerPoint>();
            script.captureSpotPoints = Mathf.FloorToInt(capturePoints[i].GetComponent<CapturePoint>().spotCapture);
            RectTransform trackerTransform = trackerPoints[i].GetComponent<RectTransform>();

            Vector3 fwd = playerCamYaw.TransformDirection(Vector3.forward);
            Vector2 forwardPlayer = new Vector2(fwd.x, fwd.z);
            Vector3 toPointVector = (capturePoints[i].transform.position - playerCamYaw.position);
            Vector2 toPointNorm = new Vector2(toPointVector.x, toPointVector.z).normalized;
            float dotForward = Vector2.Dot(forwardPlayer, toPointNorm);

            if (dotForward < 0.77f)
            {
                script.inFront = false;
                Vector3 rt = playerCamYaw.TransformDirection(Vector3.right);
                Vector2 rightPlayer = new Vector2(rt.x, rt.z);
                float dotRight = Vector2.Dot(rightPlayer, toPointNorm);
                if (dotRight > 0)
                {
                    trackerTransform.position = new Vector2( origTrackerPos.x + halfPanelWidth, origTrackerPos.y);
                    script.onRight = true;
                }
                else
                {
                    trackerTransform.position = new Vector2(origTrackerPos.x - halfPanelWidth, origTrackerPos.y);
                    script.onRight = false;
                }

            }
            else { 
                Vector3 trackerPos = mainCamera.GetComponent<Camera>().WorldToScreenPoint(capturePoints[i].transform.position);
               trackerTransform.position = new Vector2(trackerPos.x, origTrackerPos.y);
                script.inFront = true;
            }
        }
    }
}
