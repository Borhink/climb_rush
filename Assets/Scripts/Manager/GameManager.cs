using UnityEngine;
using ClimbRush.Gameplay.Player;
using ClimbRush.Input;

namespace ClimbRush.Manager
{
    /// <summary>
    /// Main entry point - initializes the game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private InputHandler inputHandler;

        private void Awake()
        {
            if (inputHandler != null)
                inputHandler.SetInputEnabled(true);
            
            Debug.Log("[GameManager] Initialized");
        }
    }
}
