using UnityEngine;

namespace Primordia.Core.Debug
{
    public class ImguiDebug : MonoBehaviour
    {
        private bool _showDebug;
        private Rect _windowRect = new(20, 20, Screen.width * .3f, Screen.width * .3f);


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0)) _showDebug = !_showDebug;
        }

        private void OnGUI()
        {
            if (!_showDebug) return;

            _windowRect = GUI.ModalWindow(0, _windowRect, Window, "Debug");
        }

        public void Window(int windowID)
        {
            Texture texture = Shader.GetGlobalTexture("_CameraDepth");
            GUILayout.Label("Showing Camera Depth");
            GUILayout.Box(texture, GUILayout.Height(Screen.height * .2f), GUILayout.Width(Screen.width * .2f));
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }
    }
}