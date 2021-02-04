// GENERATED AUTOMATICALLY FROM 'Assets/InputControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControls"",
    ""maps"": [
        {
            ""name"": ""KeyboardActions"",
            ""id"": ""c8cf4493-3974-42d9-900d-a6ad5d6243fd"",
            ""actions"": [
                {
                    ""name"": ""RudderLeft"",
                    ""type"": ""Button"",
                    ""id"": ""c980df2f-6308-4d15-9f3c-f4dfcc7a578e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RudderRight"",
                    ""type"": ""Button"",
                    ""id"": ""902e1527-5933-42c3-ae36-ab78db85083b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dca470a6-57df-48cb-817f-a200d3c3107f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RudderLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4df1d967-6d8b-4ceb-a401-f2ba45a15d4d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RudderRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // KeyboardActions
        m_KeyboardActions = asset.FindActionMap("KeyboardActions", throwIfNotFound: true);
        m_KeyboardActions_RudderLeft = m_KeyboardActions.FindAction("RudderLeft", throwIfNotFound: true);
        m_KeyboardActions_RudderRight = m_KeyboardActions.FindAction("RudderRight", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // KeyboardActions
    private readonly InputActionMap m_KeyboardActions;
    private IKeyboardActionsActions m_KeyboardActionsActionsCallbackInterface;
    private readonly InputAction m_KeyboardActions_RudderLeft;
    private readonly InputAction m_KeyboardActions_RudderRight;
    public struct KeyboardActionsActions
    {
        private @InputControls m_Wrapper;
        public KeyboardActionsActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @RudderLeft => m_Wrapper.m_KeyboardActions_RudderLeft;
        public InputAction @RudderRight => m_Wrapper.m_KeyboardActions_RudderRight;
        public InputActionMap Get() { return m_Wrapper.m_KeyboardActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyboardActionsActions set) { return set.Get(); }
        public void SetCallbacks(IKeyboardActionsActions instance)
        {
            if (m_Wrapper.m_KeyboardActionsActionsCallbackInterface != null)
            {
                @RudderLeft.started -= m_Wrapper.m_KeyboardActionsActionsCallbackInterface.OnRudderLeft;
                @RudderLeft.performed -= m_Wrapper.m_KeyboardActionsActionsCallbackInterface.OnRudderLeft;
                @RudderLeft.canceled -= m_Wrapper.m_KeyboardActionsActionsCallbackInterface.OnRudderLeft;
                @RudderRight.started -= m_Wrapper.m_KeyboardActionsActionsCallbackInterface.OnRudderRight;
                @RudderRight.performed -= m_Wrapper.m_KeyboardActionsActionsCallbackInterface.OnRudderRight;
                @RudderRight.canceled -= m_Wrapper.m_KeyboardActionsActionsCallbackInterface.OnRudderRight;
            }
            m_Wrapper.m_KeyboardActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @RudderLeft.started += instance.OnRudderLeft;
                @RudderLeft.performed += instance.OnRudderLeft;
                @RudderLeft.canceled += instance.OnRudderLeft;
                @RudderRight.started += instance.OnRudderRight;
                @RudderRight.performed += instance.OnRudderRight;
                @RudderRight.canceled += instance.OnRudderRight;
            }
        }
    }
    public KeyboardActionsActions @KeyboardActions => new KeyboardActionsActions(this);
    public interface IKeyboardActionsActions
    {
        void OnRudderLeft(InputAction.CallbackContext context);
        void OnRudderRight(InputAction.CallbackContext context);
    }
}
