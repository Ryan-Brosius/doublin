using System;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

[AddComponentMenu("Cinemachine/Custom/InputManager Axis Controller")]
public class InputManagerAxisController : InputAxisControllerBase<InputManagerAxisController.InputManagerReader>
{
    void Update()
    {
        if (Application.isPlaying)
            UpdateControllers();
    }

    [Serializable]
    public class InputManagerReader : IInputAxisReader
    {
        public enum InputSource
        {
            Grimoire,
            Staff,
            Combined
        }

        [Tooltip("Which goblin input source to use for this axis")]
        public InputSource inputSource = InputSource.Combined;
        [Tooltip("The input value is multiplied by this amount prior to processing.")]
        public float Gain = 1.0f;
        [Tooltip("Whether this is the X or Y component of the look vector")]
        public int AxisIndex = 0;
        [Tooltip("Should be enabled on non-deltatime related actions, like moving a mouse")]
        public bool CancelDeltaTime;

        private Vector2 _latestLook;
        private LazyDependency<InputManager> _InputManager;
        private bool gotAction = false;

        public float GetValue(UnityEngine.Object context, IInputAxisOwner.AxisDescriptor.Hints hint)
        {
            if (!_InputManager.IsResolvable())
                return 0f;

            if (!gotAction && _InputManager.IsResolvable())
            {
                _InputManager.Value.OnGrimoireLook -= OnGrimoireLook;
                _InputManager.Value.OnStaffLook -= OnStaffLook;
                _InputManager.Value.OnCombinedLook -= OnCombinedLook;

                _InputManager.Value.OnGrimoireLook += OnGrimoireLook;
                _InputManager.Value.OnStaffLook += OnStaffLook;
                _InputManager.Value.OnCombinedLook += OnCombinedLook;
                gotAction = true;
            }


            var change = AxisIndex == 0 ? _latestLook.x : _latestLook.y;
            change *= Gain;
            if (!CancelDeltaTime)
            {
                change *= Time.deltaTime;
            }

            return change;
        }

        private void OnGrimoireLook(Vector2 v)
        {
            if (inputSource == InputSource.Grimoire) _latestLook = v;
        }

        private void OnStaffLook(Vector2 v)
        {
            if (inputSource == InputSource.Staff) _latestLook = v;
        }

        private void OnCombinedLook(Vector2 v)
        {
            if (inputSource == InputSource.Combined) _latestLook = v;
        }
    }
}
