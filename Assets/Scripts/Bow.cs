using UnityEngine;

public class Bow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public float coolDown = 2.5f;
    private float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = coolDown;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <=0){
             Instantiate(arrowPrefab, transform.position, Quaternion.identity);
             timer = coolDown;

        }
       
        
    }
}
