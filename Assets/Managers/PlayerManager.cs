using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    private InputAction moveAction;

    [SerializeField]
    private Character playerPrefab;
    private Character player;

    public Character Player { get { return player; } }

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<Character>();
    }

    public void InternalUpdate()
    {
        player.Move(moveAction.ReadValue<Vector2>());
    }
}