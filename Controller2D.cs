using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))] //This makes sure that this script requires Player to function
[RequireComponent(typeof(BoxCollider2D))] //This makes sure that this script requires a BoxCollider2D to function
public class Controller2D : MonoBehaviour
{
    public LayerMask collisionMask; //The layer which the player can collide with

    const float skinWidth = .015f; //This is how far the rays begin inside the box to make sure the space between the boxcollider and the layermask doesnt have any room between it
    public int horizontalRayCount = 4; //How many horizontal rays are cast, this can be edited to make sure it covers enough space to be effective
    public int verticalRayCount = 4; //How many vertical rays are cast, this can be edited to make sure it covers enough space to be effective

    float horizontalRaySpacing; //The value so that the space between each horizontal ray is equal
    float verticalRaySpacing;//The value so that the space between each vertical ray is equal

    BoxCollider2D col; //Reference to the BoxCollider2D
    RaycastOrigins raycastOrigins; //References to the raycastOrigins
    public CollisionInfo collisions; //information to what it collides with

    void Start()
    {
        col = GetComponent<BoxCollider2D>(); //Connect the box collider tot he reference
        CalculateRaySpacing(); //Calculates the spacing at the beginning of the game once
    }

    //The function to acually get the collisions
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins(); //Keeps updating the raycast when you move
        collisions.Reset(); //Delete the old information with every update of the raycast

        if (velocity.x != 0)//If moving horizontal
        {
            HorizontalCollisions(ref velocity);//Calculate the horizontal collisions
        }
        if (velocity.y != 0)//If moving vertical
        {
            VerticalCollisions(ref velocity);//Calculate the horizontal collisions
        }

        transform.Translate(velocity);//Transfansform the player with the velocity
    }

    //This function is for all the horizontal collisions with as reference the horizontal velocity
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x); //Calculate how fast you are moving
        float rayLength = Mathf.Abs(velocity.x) + skinWidth; //Calculates how long the rays should be

        for (int i = 0; i < horizontalRayCount; i++) //Do this for every ray count there is
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; //Find to where the raycast send to
            rayOrigin += Vector2.up * (horizontalRaySpacing * i); //Make sure the orgins stays correct
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask); //Get what the raycast acually hits

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red); //Draw the raycast with a red line

            if (hit) //If the raycast hits
            {
                velocity.x = (hit.distance - skinWidth) * directionX; //Change the raycast to the distance between the orgin and hit collision
                rayLength = hit.distance; //Make the distance between it

                collisions.left = directionX == -1; //Make sure it doesnt continue on the left
                collisions.right = directionX == 1; //Make sure it doesnt continue on the right
            }
        }
    }

    //This function is the same as the horizontal collision function but calculates everything vertical
    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y); //Calculate how fast you are moving
        float rayLength = Mathf.Abs(velocity.y) + skinWidth; //Calculates how long the rays should be

        for (int i = 0; i < verticalRayCount; i++) //Do this for every ray count there is
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;//Find to where the raycast send to
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);//Make sure the orgins stays correct
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);//Get what the raycast acually hits

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);//Draw the raycast with a red line

            if (hit)//If the raycast hits
            {
                velocity.y = (hit.distance - skinWidth) * directionY; //Change the raycast to the distance between the orgin and hit collision
                rayLength = hit.distance; //Make the distance between it

                collisions.below = directionY == -1; //Make sure it doesnt continue on the below
                collisions.above = directionY == 1; //Make sure it doesnt continue on the above
            }
        }
    }

    //This function updates the raycast orgins
    void UpdateRaycastOrigins()
    {
        Bounds bounds = col.bounds; //Find the bounds
        bounds.Expand(skinWidth * -2); //Make sure its a bit thicker

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y); //Put the ray cast bottomLeft on the bottom left
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y); //Put the ray cast bottomRight on the bottom right
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y); //Put the ray cast topLeft on the top left
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y); //Put the ray cast topRight on the top right
    } 

    //This calculates the spacing between all the ray cast so tis all equal
    void CalculateRaySpacing()
    {
        Bounds bounds = col.bounds; //Looks for the bounds 
        bounds.Expand(skinWidth * -2); //And makes the bounds a little bit thicker

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue); //Calculates the horizontal ray counts
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue); //Calculates the vertical ray counts

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1); //Acual spacing is giving to those ray counts for horizontal
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1); //Acual spacing is giving to those ray counts for veritcal
    }

    //The orgins of where the first raycast comes from
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    //The information for the raycast in all directions that come from the box collider
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }

}