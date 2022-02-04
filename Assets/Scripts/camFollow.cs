using UnityEngine;

public class camFollow : MonoBehaviour
{
    [Range(0.05f, 10.0f)]
    public float mouseSensitivity;
    public Transform anchor;
    [Range(0.05f, 1.0f)]
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    public float magnitude;
    public float rayDistance = 1f;
    public LayerMask GroundLayers;
    private Vector3[] positions = {
        new Vector3(0f, 0f, 1f),
        new Vector3(0f, 0f, -1f),
        new Vector3(0f, 1f, 0f),
        new Vector3(0f, -1f, 0f),
        new Vector3(1f, 0f, 0f),
        new Vector3(-1f, 0f, 0f),

        new Vector3(1f, 1f, 1f),
        new Vector3(1f, -1f, 1f),
        new Vector3(-1f, 1f, 1f),
        new Vector3(-1f, -1f, 1f),
        new Vector3(1f, 1f, -1f),
        new Vector3(1f, -1f, -1f),
        new Vector3(-1f, 1f, -1f),
        new Vector3(-1f, -1f, -1f)};



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        inputToPosition();
    }
    void inputToPosition()
    {
        Quaternion qu = Quaternion.LookRotation(transform.position - anchor.position, anchor.up);
        //Debug.DrawLine(anchor.position, anchor.position + qu * Vector3.forward, Color.blue, 0.0f, false);
        /*transform.position = Vector3.SmoothDamp(transform.position, anchor.position
            + Vector3.Normalize(Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * mouseSensitivity,
            Vector3.Cross(transform.position - anchor.position, anchor.up))
            * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * mouseSensitivity, anchor.up)
            * qu * Vector3.forward) * magnitude, ref velocity,smoothTime);*/
        Vector3 nextPos = anchor.position
        + Vector3.Normalize(Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * mouseSensitivity,
        Vector3.Cross(transform.position - anchor.position, anchor.up))
        * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * mouseSensitivity, anchor.up)
        * qu * Vector3.forward) * magnitude;
        Vector3 dir = nextPosCollider(nextPos);
        if (dir == Vector3.zero)
        {
            transform.position = nextPos;
            transform.LookAt(anchor, anchor.up);
        }
        /*else
        {
            transform.position = Vector3.ProjectOnPlane(nextPos, dir);
            transform.LookAt(anchor, anchor.up);
        }*/

        
        //Debug.Log(nextPosCollider(nextPos));
    }
    Vector3 nextPosCollider(Vector3 pos)
    {
        Vector3 HitDir = Vector3.zero;
        //bool result = false;
        RaycastHit Hit;
        foreach (Vector3 item in positions)
        {
            if(Physics.Raycast(pos, item + pos, out Hit, rayDistance, GroundLayers))
            {
                HitDir += Hit.normal;
                //result = true; 
                break;
            };
            Debug.DrawLine(pos, item + pos);
        }
        return HitDir;
    }
}
