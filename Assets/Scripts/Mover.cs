using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    [SerializeField] Transform Target;
    void Update()
    {
        if (Input.GetMouseButton(1)) // Sað týk
        {
            MoveToCursor();
        }
    }

    private void MoveToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);

        if (hasHit)
        {
            GetComponent<NavMeshAgent>().SetDestination(hit.point);
        }
    }
}