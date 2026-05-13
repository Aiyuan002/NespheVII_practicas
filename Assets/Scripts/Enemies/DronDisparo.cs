using UnityEngine;

public class DronDisparo : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private float speed = 10f;
    private Vector2 moveDirection;

    //Estados de Unity
    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
    }
    void Start(){
        rb.linearVelocity = moveDirection * speed;
    }
    void Update(){
    }
    //Métodos
    public void Initialize(Vector2 direction){
        moveDirection = direction.normalized;
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0,0, angle + 180f);
    }
}
