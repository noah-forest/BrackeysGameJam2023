using UnityEngine;
using UnityEngine.Events;

public class Settings : MonoBehaviour
{
	public int startingGold = 12;
	public int battleReward = 8;

	[Space(10)]

	[HideInInspector] public static UnityEvent postProcessingToggled = new();

	private readonly static string s_SimplifiedStats = "simplifiedStats";
	public static bool SimplifiedStats
	{
		get
		{
			return PlayerPrefs.GetInt(s_SimplifiedStats, 1) != 0;
		}

		set
		{
			PlayerPrefs.SetInt(s_SimplifiedStats, value ? 1 : 0);
		}
	}

	private readonly static string s_RarityBorders = "RarityBorders";
	public static bool RarityBorders
	{
		get
		{
			return PlayerPrefs.GetInt(s_RarityBorders, 1) != 0;
		}

		set
		{
			PlayerPrefs.SetInt(s_RarityBorders, value ? 1 : 0);
		}
	}

	private readonly static string s_PostProcessing = "PostProcessing";
	public static bool PostProcessing
	{
		get
		{
			return PlayerPrefs.GetInt(s_PostProcessing, 1) != 0;
		}

		set
		{
			PlayerPrefs.SetInt(s_PostProcessing, value ? 1 : 0);
			postProcessingToggled?.Invoke();
		}
	}
}
