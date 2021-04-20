using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentBehaviour : MonoBehaviour
{
    public int id;
    [SerializeField] public GameObject tailSegment;
    // Start is called before the first frame update
    public void UpdateMovement(Vector3 pos) {
        Vector3 posDelta = transform.position;
        transform.position = pos;
        if (tailSegment != null) tailSegment.GetComponent<SegmentBehaviour>().UpdateMovement(posDelta);
    }
}
