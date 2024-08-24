using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public PlayerController playerController;
    public TMP_Text speed, height, distance, endSpeed, endHeight, endDistance;
    public float highest, fastest;

    public GameObject endScreen;
    private void Update()
    {
        // Keep track of best speed and height
        if(playerController.speed > fastest)
            fastest = playerController.speed;

        if (playerController.transform.position.y > highest)
            highest = playerController.transform.position.y;

        speed.text = $"{playerController.speed:0.0}m/s";
        height.text = $"{highest:0.0}m";
        
        // Only start counting distance once first jump has started
        if(playerController.firstJumpStarted)
            distance.text = $"{playerController.transform.position.x - playerController.jumpStartX:0.0}m";

        // Reset scene if any key is pressed after the end screen is shown
        if(endScreen.activeSelf)
        {
            if (Input.anyKeyDown)
                SceneManager.LoadScene(0);
        }
    }

    public void ShowEndScreen()
    {
        StartCoroutine(OpenEndScreen());
    }

    private IEnumerator OpenEndScreen()
    {
        yield return new WaitForSeconds(2f);

        endSpeed.text = $"Speed: {fastest:0.0m/s}";
        endHeight.text = $"Height: {height.text}";
        endDistance.text = $"Distance: {distance.text}";
        endScreen.SetActive(true);
    }
}
