using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenItch : MonoBehaviour
{
	private Button thisButton;

    // Start is called before the first frame update
    void Start()
    {
        thisButton = GetComponent<Button>();
		thisButton.onClick.AddListener(OpenWebsite);
    }

    private void OpenWebsite()
	{
		Application.OpenURL("https://glumpis.itch.io/");
	}
}
