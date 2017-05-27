using System;
using System.Collections;
using TheNextFlow.UnityPlugins;
using UnityEngine;

public class TestScene : MonoBehaviour
{
    private float xOffset = Screen.width/8.0f;
    private float yOffset = Screen.height/6.0f;

	private void OnGUI()
    {
		if (GUI.Button(new Rect(xOffset, yOffset, 150, 50), "Open alert dialog"))
		{
			MobileNativePopups.OpenAlertDialog(
				"Hello!", "Welcome to Mobile Native Popups",
				"Cancel", () => { Debug.Log("Cancel was pressed"); });
		}

	    if (GUI.Button(new Rect(xOffset, yOffset * 2, 150, 50), "Open alert dialog with 2 buttons"))
        {
            MobileNativePopups.OpenAlertDialog(
                "Hello!", "Welcome to Mobile Native Popups",
                "Accept", "Cancel",
                () => { Debug.Log("Accept was pressed"); }, () => { Debug.Log("Cancel was pressed"); });
        }

		if (GUI.Button(new Rect(xOffset, yOffset * 3, 150, 50), "Open alert dialog with 3 buttons"))
		{
			MobileNativePopups.OpenAlertDialog(
				"Hello!", "Welcome to Mobile Native Popups",
				"Accept", "Neutral", "Cancel",
				() => { Debug.Log("Accept was pressed"); },
				() => { Debug.Log("Neutral was pressed"); },
				() => { Debug.Log("Cancel was pressed"); });
		}

#if UNITY_IOS
		if (GUI.Button(new Rect(xOffset, yOffset * 4, 150, 50), "Open alert dialog with many buttons"))
		{
			IosNativePopups.OpenAlertDialog(
				"Hello!", "Welcome to Mobile Native Popups",
				"Cancel", new String[] { "First Button", "Second Button", "Third Button"},
			(buttonIndex) => {
				switch(buttonIndex) {
				case 0:
					Debug.Log("Cancel was pressed");
					break;
				case 1:
					Debug.Log("First button was pressed");
					break;
				case 2:
					Debug.Log("Second button was pressed");
					break;
				default:
					Debug.Log("Third button was pressed");
					break;
				} 
			});
		}
		
#elif UNITY_ANDROID
	    if (GUI.Button(new Rect(xOffset, yOffset * 4, 150, 50), "Open date picker dialog"))
        {
            AndroidNativePopups.OpenDatePickerDialog(
                1986, 10, 14,
                (year, month, day) => { Debug.Log("Date set: " + year + "/" + month + "/" + day); });
        }

	    if (GUI.Button(new Rect(xOffset * 4, yOffset, 150, 50), "Open time picker dialog"))
        {
            AndroidNativePopups.OpenTimePickerDialog(
                10, 9, false,
                (hour, minute) => { Debug.Log("Time set: " + hour + ":" + minute); });
        }

	    if (GUI.Button(new Rect(xOffset * 4, yOffset * 2, 150, 50), "Open progress dialog"))
        {
            StartCoroutine(OpenFakeProgressDialog());
        }


	    if (GUI.Button(new Rect(xOffset * 4, yOffset * 3, 150, 50), "Open toast"))
        {
            AndroidNativePopups.OpenToast("Welcome to Mobile Native Popups", AndroidNativePopups.ToastDuration.Short);
        }
#endif
    }

#if UNITY_ANDROID
    private IEnumerator OpenFakeProgressDialog()
    {
        AndroidNativePopups.OpenProgressDialog("Loading", "Loading...");
        yield return new WaitForSeconds(3);
        AndroidNativePopups.CloseProgressDialog();
    }
#endif
}

