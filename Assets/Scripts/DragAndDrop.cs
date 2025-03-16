using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    private Camera mainCamera;
    private Rigidbody rb;
    private bool isDragging = false;
    private Vector3 offset;
    private float objectHeight;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        objectHeight = GetComponent<Collider>().bounds.extents.y;
    }

    void OnMouseDown()
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // new position
            Vector3 newPosition = GetMouseWorldPos() + offset;

            // prevent the object from falling below the ground
            newPosition.y = Mathf.Max(newPosition.y, objectHeight + 0.1f);

            rb.MovePosition(newPosition);
        }
    }

    void OnMouseUp()
    {
        // turn off gravity
        isDragging = false;
        rb.useGravity = true;
    }

    private Vector3 GetMouseWorldPos()
    {

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); 

        if (groundPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position; 
    }
}
