using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Golfer : MonoBehaviour
{
    public GameObject hitMarker;
    public GameObject catapult;
    public float mouseSensitivity;
    public float cameraDistance;
    float cameraRadian;
    float aimRadian;
    bool aiming;
    bool shooting;
    int selectedMove;
    public bool inLevel;
    GolfLevel currentLevel;

    Rigidbody rb;

    public float yawSpeed;
    public float pitchSpeed;
    public float minPitch;
    public float maxPitch;
    private float cameraPitch = 0.0f;

    public LineRenderer shotLineRenderer;
    public LineRenderer catapultLineRenderer;
    public GameObject projectile;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aiming = false;
        shooting = false;
        Cursor.visible = false;
        DrawProjectileArc(new Vector3(0, 3, -3));
    }

    void Update()
    {
        // Gets mouse movement            
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Locks everything in here while you are shooting
        if (!shooting)
        {
            // Toggle whether you are aiming or not 
            if (Input.GetMouseButtonDown(0))
            {
                toggleAim();
            }

            // All this is only active while aiming
            if (aiming)
            {
                // Switch aiming mode
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    selectedMove = 0;
                    catapult.SetActive(false);
                    catapultLineRenderer.enabled = false;
                    hitMarker.SetActive(true);
                    shotLineRenderer.enabled = true;
                }
                if (Input.GetKeyDown(KeyCode.Alpha2) && keepTrackOfPlayer.Instance.unlockedCatapult)
                {
                    selectedMove = 1;
                    hitMarker.SetActive(false);
                    shotLineRenderer.enabled = false;
                    catapult.SetActive(true);
                    catapultLineRenderer.enabled = true;
                }

                // Adds or subtracts mouse movement from where we want the markers
                aimRadian += mouseX;

                // Ensures marker stays within range 0 to 2*pi for easier calculation down the line
                if (aimRadian > 2 * Mathf.PI)
                {
                    aimRadian -= 2 * Mathf.PI;
                }
                else if (aimRadian < 0)
                {
                    aimRadian += 2 * Mathf.PI;
                }

                // Controls the primary shooting marker
                if (selectedMove == 0) 
                {
                    // Rotates the marker around golfball's position on the world Y axis
                    hitMarker.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0, aimRadian * Mathf.Rad2Deg, 0));

                    // If user starts holding space, begin shooting
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        shooting = true;
                        StartCoroutine(shootingCoroutine(aimRadian));
                    }
                }
                // Controls the catapult shooting marker
                else if (selectedMove == 1)
                {
                    // Rotates the catapult around golfball's position on the world Y axis. NOTE: uses parent of parent as catapult is child of a displacer and a rotator, so I can get a good pivot point
                    catapult.transform.parent.parent.SetPositionAndRotation(catapult.transform.parent.parent.position, Quaternion.Euler(0, aimRadian * Mathf.Rad2Deg, 0));

                    // If user starts holding space, begin shooting
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        shooting = true;
                        StartCoroutine(catapultCoroutine(aimRadian));
                    }
                }
            }
        }
        // This runs if user isn't aiming
        if (!aiming)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                keepTrackOfPlayer.Instance.toggleScoreOverlay();
            }

            // Press R to reset your position
            if (Input.GetKeyDown(KeyCode.R))
            {
                resetPosition();
            }

            // Hold down middle mouse button and move mouse around to slide the camera around the world at a fixed y position
            if (Input.GetKey(KeyCode.Mouse2))
            {
                Vector3 movement = new Vector3(mouseX * 1.5f, 0, mouseY * 1.5f);

                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0;
                forward.Normalize();

                Vector3 right = Camera.main.transform.right;
                right.y = 0;
                right.Normalize();

                Vector3 worldMovement = (right * movement.x + forward * movement.z) * Time.deltaTime;

                Camera.main.transform.position += worldMovement * 40;
            }
            // Or don't hold anything down and have full rotational and zoom control over the camera
            else
            {
                // Update camera rotation based on mouse input
                cameraRadian += mouseX * yawSpeed;
                cameraPitch += mouseY * pitchSpeed;
                cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch); // Clamp pitch angle

                // Calculate new camera position based on spherical coordinates
                float cameraNewX = polarToCartesian_X(cameraRadian, cameraDistance) * Mathf.Cos(cameraPitch * Mathf.Deg2Rad);
                float cameraNewY = cameraDistance * Mathf.Sin(cameraPitch * Mathf.Deg2Rad); // Vertical position based on pitch
                float cameraNewZ = polarToCartesian_Z(cameraRadian, cameraDistance) * Mathf.Cos(cameraPitch * Mathf.Deg2Rad);

                // Set camera position to new calculated position
                Camera.main.transform.position = new Vector3(transform.position.x + cameraNewX, transform.position.y + cameraNewY, transform.position.z + cameraNewZ);

                // Make the camera look at the golfball
                Camera.main.transform.LookAt(transform.position);

                // Zoom functionality with mouse scroll
                float scrollInput = Input.GetAxis("Mouse ScrollWheel");
                cameraDistance -= scrollInput;
                cameraDistance = Mathf.Clamp(cameraDistance, 0.3f, 2.0f);

            }
        }

    }
    // Called to enter and exit aiming state
    void toggleAim()
    {
        if (selectedMove < 1)
        {
            if (!aiming)
            {
                aiming = true;
                hitMarker.SetActive(true);
                shotLineRenderer.enabled = true;
            }
            else if (aiming)
            {
                aiming = false;
                hitMarker.SetActive(false);
                shotLineRenderer.enabled = false;
            }
        }
        else if (selectedMove > 0)
        {
            if (!aiming)
            {
                aiming = true;
                catapult.SetActive(true);
                catapultLineRenderer.enabled = true;
            }
            else if (aiming)
            {
                aiming = false;
                catapult.SetActive(false);
                catapultLineRenderer.enabled = false;
            }
        }
    }
    // Helper functions for the rotaty things
    float polarToCartesian_X(float angle, float radius) { return Mathf.Cos(angle) * radius; }
    float polarToCartesian_Z(float angle, float radius) { return Mathf.Sin(angle) * radius; }
    // Adjustable functions for drawing a nice arc
    Vector3 CalculateProjectilePosition(Vector3 velocity, float time)
    {
        return velocity * time + 0.5f * Physics.gravity * time * time;
    }
    void DrawProjectileArc(Vector3 velocity)
    {
        int numberOfPoints = 14;
        float timeBetweenPoints = 0.05f;

        // Set the number of points in the LineRenderer
        catapultLineRenderer.positionCount = numberOfPoints;

        // Loop through each point to calculate its position
        for (int i = 0; i < numberOfPoints; i++)
        {
            // Calculate the time at this point in the arc
            float time = i * timeBetweenPoints;

            // Use the projectile motion formula to find the point's position
            Vector3 pointPosition = CalculateProjectilePosition(velocity, time);

            // Set the position in the LineRenderer
            catapultLineRenderer.SetPosition(i, pointPosition);
        }
    }

    // Primary and secondary fire coroutines
    IEnumerator shootingCoroutine(float radian) 
    {
        float shotPower = 0;
        Renderer hitmarkerRenderer = hitMarker.GetComponent<Renderer>();

        while (Input.GetKey(KeyCode.Space) && shotPower < 2f)
        {
            shotPower += Time.deltaTime;
            float t = shotPower / 2f;
            Color colorShift = Color.Lerp(Color.white, Color.red, t);
            hitmarkerRenderer.material.color = colorShift;
            shotLineRenderer.material.color = colorShift;
            Vector3 newEndPosition = new Vector3(0, 0, -0.3f * (1 + t));
            shotLineRenderer.SetPosition(1, newEndPosition);
            yield return null;
        }

        hitMarker.SetActive(false);
        hitmarkerRenderer.material.color = Color.white;
        shotLineRenderer.enabled = false;
        shotLineRenderer.material.color = Color.white;
        shotLineRenderer.SetPosition(1, new Vector3(0,0,-0.3f));



        float upwardForce = 4 * shotPower * 0.2f; 
        float directionalForceMagnitude = 4 * shotPower * 0.8f;

        // Calculate the directional force based on the markerRadian
        Vector3 shotDirection = new Vector3(Mathf.Cos(radian + (0.5f * Mathf.PI)) * directionalForceMagnitude, upwardForce, - Mathf.Sin(radian + (0.5f * Mathf.PI)) * directionalForceMagnitude);

        rb.AddForce(shotDirection, ForceMode.Impulse);

        if (inLevel) StartCoroutine(currentLevel.takeShot());

        yield return new WaitForSeconds(4);

        StartCoroutine(DampenVelocity());

        resetRotation(); 


        aimRadian = 0;
        aiming = false;
        shooting = false;

    }
    IEnumerator catapultCoroutine(float radian)
    {
        float shotPower = 0;
        Renderer catapultRenderer = catapult.GetComponent<Renderer>();
        catapultLineRenderer.enabled = false;

        // Define the range of the rotation in degrees
        float minRotation = -40f;
        float maxRotation = 40f;


        while (Input.GetKey(KeyCode.Space) && shotPower < 2f)
        {
            shotPower += Time.deltaTime;
            float t = shotPower / 2f;

            // Change the color of the catapult based on the charging progress
            Color colorShift = Color.Lerp(Color.white, Color.red, t);
            catapultRenderer.material.color = colorShift;

            // Rotate the catapult around the x-axis based on the charging progress
            float currentXRotation = Mathf.Lerp(minRotation, maxRotation, t);
            Quaternion targetRotation = Quaternion.Euler(currentXRotation, catapult.transform.rotation.eulerAngles.y, catapult.transform.rotation.eulerAngles.z);
            catapult.transform.rotation = targetRotation;
            
            if (inLevel) StartCoroutine(currentLevel.takeShot());

            yield return null;
        }

        // Snap the catapult back to its default x rotation in a quick shooting motion
        float resetDuration = 0.05f;
        float elapsedTime = 0f;

        Quaternion initialRotation = catapult.transform.rotation;
        Quaternion finalRotation = Quaternion.Euler(minRotation, initialRotation.eulerAngles.y, initialRotation.eulerAngles.z);

        while (elapsedTime < resetDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / resetDuration;

            // Smoothly interpolate the rotation back to the default position
            catapult.transform.rotation = Quaternion.Lerp(initialRotation, finalRotation, progress);

            yield return null;
        }
        catapult.SetActive(false);
        catapultRenderer.material.color = Color.white;
        catapult.transform.rotation = Quaternion.Euler(0, catapult.transform.rotation.eulerAngles.y, catapult.transform.rotation.eulerAngles.z);


        float upwardForce = 2 * shotPower * 0.5f;
        float directionalForceMagnitude = 2 * shotPower * 0.5f;

        // Calculate the directional force based on the markerRadian
        Vector3 shotDirection = new Vector3(Mathf.Cos(radian + (0.5f * Mathf.PI)) * directionalForceMagnitude, upwardForce, -Mathf.Sin(radian + (0.5f * Mathf.PI)) * directionalForceMagnitude);

        GameObject shot = Instantiate(projectile, new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z), Quaternion.identity);

        shot.GetComponent<Rigidbody>().AddForce(shotDirection, ForceMode.Impulse);

        if (inLevel) StartCoroutine(currentLevel.takeShot());

        yield return new WaitForSeconds(2);

        Destroy(shot);

        aiming = false;
        shooting = false;

    }
    // Helper functions to control player velocity better
    public IEnumerator DampenVelocity()
    {
        while (rb.velocity.magnitude > 0.0001f) 
        {
            rb.velocity *= 0.98f; 
            yield return null; 
        }
    }
    void resetRotation()
    {
        transform.rotation = Quaternion.identity;
    }
    public void resetPosition()
    {
        transform.position = keepTrackOfPlayer.Instance.currentRespawnPoint;
    }
    // Collision checks for level logic, killplane, etc
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Level")) 
        {
            currentLevel = other.GetComponent<GolfLevel>();
            currentLevel.startPlaying();
        }
        if (other.CompareTag("Goal"))
        {
            currentLevel.winLevel();
        }
        if (other.CompareTag("Leavebox"))
        {
            if(inLevel)
            {
                currentLevel.stopPlaying();
            }
        }
        if (other.CompareTag("Killplane"))
        {
            if (inLevel)
            {
                currentLevel.startPlaying();
            }
            else
            {
                resetPosition();
            }
        }
    }
}
