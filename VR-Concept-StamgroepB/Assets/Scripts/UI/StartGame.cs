
using UnityEngine;
using UnityEngine.XR;

public class StartGame : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private InputDevice leftController;
    private bool wasPressed;

    void Update()
    {
        // Get the left controller
        if (!leftController.isValid)
        {
            leftController = InputDevices.GetDeviceAtXRNode(
                XRNode.LeftHand
            );
        }

        // Read the X button (primary button on left controller)
        if (leftController.TryGetFeatureValue(
            CommonUsages.primaryButton, out bool isPressed))
        {
            // Detect a new button press
            if (isPressed && !wasPressed)
            {
                Debug.Log("X BUTTON PRESSED!");

                if (gameManager != null)
                {
                    gameManager.StartGame();
                }
                else
                {
                    Debug.LogError("GameManager is not assigned!");
                }
            }

            wasPressed = isPressed;
        }
    }
}
