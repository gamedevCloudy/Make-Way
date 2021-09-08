using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System; 

public class aaPathCreator : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public List<Vector3> points = new List<Vector3>();
    public Action<IEnumerable<Vector3>> OnNewPathCreated = delegate { };
    [SerializeField] LayerMask mask;
   

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
       
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
            points.Clear();
        if (Input.GetButton("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, mask))
            {
                if (DistanceToLastPoint(hitInfo.point) > 0.3f)
                {
                    points.Add(hitInfo.point);
                    // new array
                    Vector3[] linePoints = points.ToArray();
                    for (int i = 0; i < points.Count; i++)
                    {
                        linePoints[i] += new Vector3(0f, 0.1f, 0f); 
                    }
                    lineRenderer.positionCount = points.Count;
                    //lineRenderer.SetPositions(points.ToArray());
                    lineRenderer.SetPositions(linePoints); 
                }
               
            }
            
        }
        else if (Input.GetButtonUp("Fire1"))
            OnNewPathCreated(points); 
    }
    private float DistanceToLastPoint(Vector3 point)
    {
        if (!points.Any()) return Mathf.Infinity;
        return Vector3.Distance(points.Last(), point); 
    }
}
