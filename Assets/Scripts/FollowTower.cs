using UnityEngine;

public class FollowTower : MonoBehaviour
{
    public Transform tower;
    public Vector3 offset = new Vector3(0, 3f, 0); // sesuaikan jarak atas

    void Update()
    {
        if (tower != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(tower.position + offset);
        }
    }
}
