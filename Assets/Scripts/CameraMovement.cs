using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // ... (Existing Variables)
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public Vector2 cameraBoundsMin; 
    public Vector2 cameraBoundsMax;

    // --- NEW VARIABLE ---
    public Vector2 deadZoneSize = new Vector2(2f, 1f); // Width and Height of the dead zone

    void Update()
    {
        if (player == null) return;

        // The camera's ideal center is the player's position (ignoring Z for now)
        Vector3 targetPosition = player.position + offset;
        
        // --- DEAD ZONE LOGIC ---
        
        // 1. Calculate the player's position relative to the camera's center
        // We project the camera's current Z to the player's Z plane for the calculation
        Vector3 cameraCenter = transform.position - offset; 

        // 2. Calculate the distance between the player and the camera center
        float deltaX = player.position.x - cameraCenter.x;
        float deltaY = player.position.y - cameraCenter.y;
        
        // Variables to store how much the camera needs to be pushed
        float pushX = 0f;
        float pushY = 0f;

        // 3. Check if the player has exceeded the horizontal dead zone
        if (Mathf.Abs(deltaX) > deadZoneSize.x)
        {
            // Calculate how much the camera needs to move to re-center the player at the dead zone edge
            pushX = deltaX - deadZoneSize.x * Mathf.Sign(deltaX);
        }

        // 4. Check if the player has exceeded the vertical dead zone
        if (Mathf.Abs(deltaY) > deadZoneSize.y)
        {
            // Calculate how much the camera needs to move to re-center the player at the dead zone edge
            pushY = deltaY - deadZoneSize.y * Mathf.Sign(deltaY);
        }

        // 5. Update the target position only if the player moved outside the zone
        if (pushX != 0f || pushY != 0f)
        {
            // The camera's new target center is moved by pushX/pushY
            targetPosition.x = cameraCenter.x + pushX + offset.x;
            targetPosition.y = cameraCenter.y + pushY + offset.y;
        } 
        else
        {
            // If the player is inside the dead zone, the target is the current camera position.
            // This prevents the camera from moving/smoothing at all!
            targetPosition = transform.position;
        }

        // --- CLAMPING AND SMOOTHING (Slightly modified from our previous chat) ---

        // 6. Smoothly move the camera towards the *new* (or current) target position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);

        // 7. Apply camera bounds to the final smoothed position
        smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, cameraBoundsMin.x, cameraBoundsMax.x);
        smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, cameraBoundsMin.y, cameraBoundsMax.y);

        // 8. Set the final position, preserving the fixed Z depth from the offset
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, offset.z);
    }
}