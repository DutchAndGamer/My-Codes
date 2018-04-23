using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))] //This makes sure that this script requires Controller2D to function
public class Player : MonoBehaviour
{
    public float jumpHeight = 4; //Height the player can jump
    public float timeToJumpApex = .4f; //The time it takes for the player to get to the jumpHeight
    float accelerationTimeAirborne = .2f; //how fast you accelerate in the air
    float accelerationTimeGrounded = .1f; //how fast you accelerate on the ground
    float moveSpeed = 6; //The movement speed

    float gravity; //How fast the player falls down
    float jumpVelocity; //How fast the player moves in jumps
    Vector3 velocity; //The general velocity of the player
    float velocityXSmoothing; //The smooth movement between the begin and end

    Controller2D controller; //Variable of the controller

    void Start()
    {
        controller = GetComponent<Controller2D>(); //Connects the variable to the script

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); //Calculate what the gravity should be
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex; //Calculate what the jumpVelocity should be by multiplying the gravity times the timeToJumpApex
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity); //Print in the console to see what the gravity and jumpVelocity is
    }

    void Update()
    {
        //Check if the raycast hits above or below the player and send that infomation to the Controller2D script which checks if its true or not
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0; //Change the velocity to 0
        }

        //Get the Horizontal and Vertical raw input and put it into Vector2 input.
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //Check if KeyCode Space is pressed and if there is any collision below the player
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity; //cchange the velocity.y to the set jumpVelocity
        }

        float targetVelocityX = input.x * moveSpeed; //What the acual speed is when you press A or D * the movement speed
        //Makes sure you dont bounce around when you are grounded and want to move
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime; //How fast the gravity should increase
        controller.Move(velocity * Time.deltaTime); //The time to move
    }
}