using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 20f;

    // GameObject, auf das die Kamera beschränkt werden soll
    private List<GameObject> limitObjects = new List<GameObject>();

    // Update is called once per frame
    private void Update()
    {
        // Kamera Bewegung mit Rechtsklick
        if (Input.GetMouseButton(1))
        {
            float horizontal = Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            float vertical = Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;

            // Überprüfe, ob die neue Position innerhalb der Grenzen des limitierten GameObjects liegt
            Vector3 newPosition = transform.position - new Vector3(horizontal, vertical, 0);
            if (IsPositionWithinLimits(newPosition))
            {
                transform.Translate(-horizontal, -vertical, 0);
            }
        }

        // Zoomen der Kamera mit dem Mausrad
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f) {
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - (scroll * zoomSpeed), minZoom, maxZoom);
        }

        if(limitObjects.Count == 0 && FindObjectsOfType<Tilemap>().Length > 0) {
            foreach(Tilemap t in FindObjectsOfType<Tilemap>()) {
                limitObjects.Add(t.gameObject);
            }
        }
    }

    // Überprüfe, ob die Position innerhalb der Grenzen des limitierten GameObjects liegt
    private bool IsPositionWithinLimits(Vector3 position)
    {
        foreach(GameObject limitObject in limitObjects) {
            if (limitObject == null)
            {
                return true; // Wenn kein limitiertes GameObject angegeben ist, erlaube die Bewegung überall
            }
            
            // Überprüfe, ob die Position innerhalb der Grenzen des limitierten GameObjects liegt
            Renderer renderer = limitObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                Bounds bounds = renderer.bounds;
                return bounds.Contains(position);
            }
        }
        return false;
    }
}