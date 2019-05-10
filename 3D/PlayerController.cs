using UnityEngine;

// Include the namespace required to use Unity UI
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {
	
	// Create public variables for player speed, and for the Text UI game objects
	public Text countText;
	public Text winText;
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public int pickupQuota = 10;
    public bool isGrounded;

    // Create private references to the rigidbody component on the player, and the count of pick up objects picked up so far
    private Rigidbody rb;
	private int count;

    // For shooting projectiles    
    public GameObject shot;
    public Transform shotSpawn;

    public float fireRate;
    private float nextFire;
    private GameObject dimScreen;

   

    // At the start of the game..
    void Start ()
	{
		// Assign the Rigidbody component to our private rb variable
		rb = GetComponent<Rigidbody>();
        dimScreen = GameObject.Find("Dimmer");
        dimScreen.SetActive(false);

        // Set the count to zero 
        count = 0;
		// Run the SetCountText function to update the UI (see below)
		SetCountText ();
		// Set the text property of our Win Text UI to an empty string, making the 'You Win' (game over message) blank
		winText.text = "";
    }


    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            rb.AddForce(new Vector3(0.0f, 80.0f, 0.0f) * speed, ForceMode.Impulse);
            print("Jump!");
        }
        if (Input.GetKeyDown("q") || Input.GetKeyDown("escape"))
        {
            winText.text = "Quitting...";
            StartCoroutine(quitToMenu());
        }
        if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
        {
            print("Fire!");
            nextFire = Time.time + fireRate;
            Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        }
    }

    // Each physics step..
    void FixedUpdate ()
	{
		// Set some local float variables equal to the value of our Horizontal and Vertical Inputs
		float rotation = Input.GetAxis ("Horizontal") * rotationSpeed;
		float translation = Input.GetAxis ("Vertical") * speed;
		
        // Create a Vector3 variable, and assign X and Z to feature our horizontal and vertical float variables above
        //Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Add a physical force to our Player rigidbody using our 'movement' Vector3 above, 
        // multiplying it by 'speed' - our public player speed that appears in the inspector
        //rb.AddForce (movement * speed);        
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);      

    }

    void OnCollisionStay()
    {
        isGrounded = true;
    }

    // When this game object intersects a collider with 'is trigger' checked, 
    // store a reference to that collider in a variable named 'other'..
    void OnTriggerEnter(Collider other) 
	{
		// ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
		if (other.gameObject.CompareTag ("Pick Up"))
		{
			// Make the other game object (the pick up) inactive, to make it disappear
			other.gameObject.SetActive (false);

			// Add one to the score variable 'count'
			count = count + 1;

			// Run the 'SetCountText()' function (see below)
			SetCountText ();
		} else if (other.gameObject.CompareTag("Enemy"))
        {
            // Make the other game object (the pick up) inactive, to make it disappear
            other.gameObject.SetActive(false);
            Debug.Log("You Died!");
            // Add one to the score variable 'count'
            count = 0;
            // Run the 'SetCountText()' function (see below)
            SetCountText();
            winText.text = "You Died!";

            // Exit to main menu
            StartCoroutine(quitToMenu());                                  
        }

    }

    IEnumerator quitToMenu()
    {
        dimScreen.SetActive(true);
        //Wait for 4 seconds
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("StartMenu");
    }

    public void countUpdate()
    {
        // Add one to the score variable 'count'
        count = count + 1;
        // Run the 'SetCountText()' function (see below)
        SetCountText();
    }

	// Create a standalone function that can update the 'countText' UI and check if the required amount to win has been achieved
	void SetCountText()
	{
		// Update the text field of our 'countText' variable
		countText.text = "Count: " + count.ToString ();

		// Check if our 'count' is equal to or exceeded 12
		if (count >= pickupQuota) 
		{
			// Set the text value of our 'winText'
			winText.text = "You Win!";
            StartCoroutine(quitToMenu());
        }
	}


    public void setQuota(string input)
    {
        pickupQuota = int.Parse(input);
        Debug.Log("The quota has been set to " + input) ;
    }
}