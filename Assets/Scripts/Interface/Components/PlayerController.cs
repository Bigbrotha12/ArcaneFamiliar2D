using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer playerSprite;
    public List<Sprite> idleSprites;
    public List<Sprite> runningSprites;
    public bool running;
    // Movement speed of the player
    public float speed = 5f;

    void Start()
    {
        StartCoroutine("Animate");
    }

    IEnumerator Animate()
    {
        while(true)
        {
            if(running)
            {
                foreach (Sprite frame in runningSprites)
                {
                    playerSprite.sprite = frame;
                    yield return new WaitForSeconds(0.15f);
                }
    
            } else
            {
                
                foreach (Sprite frame in idleSprites)
                {
                    playerSprite.sprite = frame;
                    yield return new WaitForSeconds(0.25f);
                }
                idleSprites.Reverse();
                foreach (Sprite frame in idleSprites)
                {
                    playerSprite.sprite = frame;
                    yield return new WaitForSeconds(0.25f);
                }
                idleSprites.Reverse();
            }
            
        }
    }

    void Update()
    {
        // Get the horizontal and vertical input axes
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate the movement vector based on the input axes and speed
        Vector3 vertical = Vector3.up * verticalInput * speed * Time.deltaTime;
        Vector3 horizontal = Vector3.right * horizontalInput * speed * Time.deltaTime;

        transform.position = transform.position + vertical + horizontal;

    }
}