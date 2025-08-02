using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{

    public float cameraDamping;
    public float mseCameraInfluence;
    private Vector3 velocity = Vector3.zero;
    public PlayerManager.PlayerManager playerManager; // For getting active player;

    private InputAction pointAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointAction = InputSystem.actions.FindAction("Point");
    }

    void LateUpdate()
    {
        Vector3 playerPosition = transform.position;

        if (playerManager != null)
        {
            playerPosition = playerManager.currentPlayer.transform.position;    
        }

        Vector2 mousePosition = pointAction.ReadValue<Vector2>();
        Vector3 mousePositionGameWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
        
        Vector3 targetPosition = Vector3.Lerp(playerPosition, mousePositionGameWorld, mseCameraInfluence);

        // No 3D movement! 
        targetPosition.z = transform.position.z;
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, cameraDamping);
    }
}
