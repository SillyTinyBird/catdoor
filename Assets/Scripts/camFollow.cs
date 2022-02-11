using UnityEngine;
public class camFollow : MonoBehaviour
{
    [Range(0.05f, 10.0f)]
    public float mouseSensitivity;
    public Transform anchor;
    public float magnitude;
    public LayerMask GroundLayers;
    private Vector3 previousUp;
    // Start is called before the first frame update
    void Start()
    {
        previousUp = anchor.up;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        inputToPosition();
    }
    void inputToPosition()
    {

        Vector3 point = anchor.position + anchor.up;
        Vector3 direction = Quaternion.FromToRotation(previousUp,anchor.up) * Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * mouseSensitivity,
        Vector3.Cross(transform.position - point, anchor.up))
        * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * mouseSensitivity, anchor.up)
        * Quaternion.LookRotation(transform.position - point, anchor.up) * Vector3.forward;
        previousUp = anchor.up;
        if (Physics.Raycast(point, direction, out RaycastHit Hit, magnitude + 0.1f, GroundLayers))
        {
            transform.position = point + (direction * (Vector3.Distance(point, Hit.point) - 0.3f));
        }
        else
        {
            transform.position = point + (direction * magnitude);
        }
        transform.LookAt(anchor, anchor.up);
    }
}
