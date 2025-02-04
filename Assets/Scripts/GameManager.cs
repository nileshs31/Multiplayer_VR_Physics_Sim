using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance
    public GameObject XROrigin, spectatorOnlyCanvas;
    public bool isSpec, _canPlay;
    [SerializeField]
    Transform[] spectatorPostions;
    int currentIndex = 0;
    public bool canPlay
    {
        get => _canPlay;
        set
        {
            if (_canPlay != value)
            {
                _canPlay = value;
                OnCanPlayChanged();
            }
        }
    }


    public void AngleChange(bool isNext)
    {
        if (isNext)
        {
            currentIndex = (currentIndex + 1) % spectatorPostions.Length;
        }
        else
        {
            currentIndex = (currentIndex - 1 + spectatorPostions.Length) % spectatorPostions.Length;
        }

        XROrigin.transform.position = spectatorPostions[currentIndex].position;
        Vector3 directionToZero = Vector3.zero - XROrigin.transform.position;

        directionToZero.y = 0;

        if (directionToZero != Vector3.zero)
        {
            XROrigin.transform.rotation = Quaternion.LookRotation(directionToZero);
        }
    }

    public void PlayerAngle(int i)
    {
        if (i == 1)
        {
            var go = FindObjectsOfType<NetworkPlayer>();
            foreach (var item in go)
            {
                if (item.OwnerClientId == 0) // Change this to 0 later
                {

                    XROrigin.transform.position = item.root.position;
                    Vector3 midpoint = (item.leftHand.position + item.rightHand.position) / 2;
                    Vector3 direction = (midpoint - XROrigin.transform.position).normalized;
                    direction.y = 0;
                    direction.Normalize();
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    targetRotation *= Quaternion.Euler(0, 45, 0);
                    XROrigin.transform.rotation = targetRotation;

                }
            }
        }
        else if (i == 2)
        {
            var go = FindObjectsOfType<NetworkPlayer>();
            foreach (var item in go)
            {
                if (item.OwnerClientId == 1) // Change this to 1 later
                {
                    XROrigin.transform.position = item.root.position;
                    Vector3 midpoint = (item.leftHand.position + item.rightHand.position) / 2;
                    Vector3 direction = (midpoint - XROrigin.transform.position).normalized;
                    direction.y = 0;
                    direction.Normalize();
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    targetRotation *= Quaternion.Euler(0, 45, 0);
                    XROrigin.transform.rotation = targetRotation;
                }
            }
        }
    }


    // Global references for the components
    private XRDirectInteractor leftDirectInteractor;
    private XRDirectInteractor rightDirectInteractor;
    public XRRayInteractor rightRayInteractor;
    private DynamicMoveProvider locomotion;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        InitializeComponents();
    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Check for Android Back button
        if (Application.platform == RuntimePlatform.Android && Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        // Check for Oculus Options button (on the left controller)
        if (Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            Application.Quit();
        }
    }
    private void InitializeComponents()
    {
        // Get the XR Rig components once in Awake
        var leftController = XROrigin.transform.Find("Camera Offset/Left Controller");
        var rightController = XROrigin.transform.Find("Camera Offset/Right Controller");

        if (leftController != null)
        {
            leftDirectInteractor = leftController.GetComponentInChildren<XRDirectInteractor>();
        }

        if (rightController != null)
        {
            rightDirectInteractor = rightController.GetComponentInChildren<XRDirectInteractor>();
        }

        locomotion = XROrigin.GetComponent<DynamicMoveProvider>();
    }


    public void OnSpectatorJoined()
    {
        isSpec = true;
        canPlay = false;
        spectatorOnlyCanvas.SetActive(true);
        DisableLocomotionAndInteractions();
    }

    
    public void OnCanPlayChanged()
    {

        // Move the XR Rig to the spectator position
        if (!canPlay)
        {
            // Disable mobility and interactions
            DisableLocomotionAndInteractions();
        }
        else
        {
            // Re-enable mobility and interactions for a player
            EnableLocomotionAndInteractions();
        }
    }

    private void DisableLocomotionAndInteractions()
    {
        currentIndex = 0;
        XROrigin.transform.position = spectatorPostions[currentIndex].position;
        if (leftDirectInteractor != null)
        {
            leftDirectInteractor.gameObject.SetActive(false);
        }

        if (rightDirectInteractor != null)
        {
            rightDirectInteractor.gameObject.SetActive(false);
        }

        if (rightRayInteractor != null)
        {
            rightRayInteractor.enabled = false;
        }

        // Disable locomotion
        if (locomotion != null)
        {
            locomotion.enabled = false;
        }
    }

    private void EnableLocomotionAndInteractions()
    {
        if (leftDirectInteractor != null)
        {
            leftDirectInteractor.gameObject.SetActive(true);
        }

        if (rightDirectInteractor != null)
        {
            rightDirectInteractor.gameObject.SetActive(true);
        }

        if (rightRayInteractor != null)
        {
            rightRayInteractor.enabled = true;
        }

        // Enable locomotion
        if (locomotion != null)
        {
            locomotion.enabled = true;
        }
    }

}
