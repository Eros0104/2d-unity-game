using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal") * speed;
        bool isRunning = horizontalInput != 0;
        float y = body.velocity.y;       

        // Flip character when moving right - left
        if (horizontalInput > 0.01f)
        {
            transform.localScale = new Vector2(4, 4);
        } 
        else if(horizontalInput < -0.01f)
        {
            transform.localScale = new Vector2(-4, 4);
        }

        // Set animator parameters
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isGrounded", isGrounded());
        animator.SetBool("isOnWall", isOnWall());

        // Wall Jump
        if (wallJumpCooldown > 0.2f)
        {
            body.velocity = new Vector2(horizontalInput, y);

            if(isOnWall() && !isGrounded())
            {
                body.gravityScale = 2;
                body.velocity = Vector2.zero;
            } 
            else
            {
                body.gravityScale = 7;
            }

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                Jump();
            }
        } 
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (isGrounded()) {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");
        } 
        else if (!isGrounded() && isOnWall())
        {
            float playerOppositeDiretion = -Mathf.Sign(transform.localScale.x);
            if (horizontalInput == 0)
            {
                body.velocity = new Vector2(playerOppositeDiretion * 10, 0);
                transform.localScale = new Vector2(playerOppositeDiretion, transform.localScale.y);
            }
            else
            {               
                body.velocity = new Vector2(playerOppositeDiretion * 4, jumpPower / 2.5f);
            }
            wallJumpCooldown = 0;                      
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool isOnWall()
    {
        Vector2 direction = new Vector2(transform.localScale.x, 0);
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, direction, 0.1f, wallLayer);
        return raycastHit.collider != null;
    }

    public bool isAbleToAttack()
    {
        return horizontalInput == 0 && isGrounded() && !isOnWall();
    }
}
