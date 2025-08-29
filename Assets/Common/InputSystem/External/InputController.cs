using Common.InputSystem.Runtime;
using UnityEngine;

namespace Common.InputSystem.External
{
    public class InputController
    {
        public InputResult GetInput()
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.LogError("W");
                return InputResult.Up;
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.LogError("S");
                return InputResult.Down;
            }

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.LogError("A");
                return InputResult.Left;
            }

            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.LogError("D");
                return InputResult.Right;
            }

            return InputResult.None;
        }
    }
}