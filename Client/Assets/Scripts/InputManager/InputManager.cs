using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	private static InputManager _singleton;
	public static InputManager Singleton
	{
		get => _singleton;
		private set
		{
			if (_singleton == null)
				_singleton = value;
			else if (_singleton != value)
			{
				Debug.Log($"{nameof(InputManager)} instance already exists, destroying duplicate!");
				Destroy(value);
			}
		}
	}

	public float smooth = 0.5f;
	public static KeyCode forward { get; set; }
	public static KeyCode backward { get; set; }
	public static KeyCode left { get; set; }
	public static KeyCode right { get; set; }
	public static KeyCode jump { get; set; }
	public static KeyCode run { get; set; }
	public static KeyCode crouch { get; set; }
	public static KeyCode fire { get; set; }
	public static KeyCode reload { get; set; }
	public static KeyCode scoreboard { get; set; }
	public static KeyCode cancell { get; set; }

	public static string[] names = { "Forward", "Backward", "Left", "Right", "Jump", "Run", "Crouch" , "Fire", "Reload", "Scoreboard", "Cancel"};
	public static string[] playerPrefs = { "forwardKey", "backwardKey", "leftKey", "rightKey", "jumpKey", "runKey","crouchKey", "fireKey", "reloadKey", "scoreboardKey", "cancelKey" };
	public static KeyCode[] keys = { forward, backward, left, right, jump, run, crouch, fire, reload, scoreboard, cancell };
	public static float horizontal, vertical;
	public static bool getAnyAxes;



	public KeyCode[] keys2Show = { forward, backward, left, right, jump, fire, reload, scoreboard, cancell };
	public bool[] key2PressShow;
	public float[] showAxes;
	public float[] realAxes;
	public bool updated;
	public void Awake()
	{
		Singleton = this;
		   updated = false;
		#region keys
		forward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("forwardKey", "W"));
		backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backwardKey", "S"));
		left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "A"));
		right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "D"));
		jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpKey", "Space"));
		run = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("runKey", "LeftShift"));
		crouch = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("crouchKey", "LeftControl"));
		fire = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fireKey", "Mouse0"));
		reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reloadKey", "R"));
		scoreboard = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("scoreboardKey", "Tab"));
		cancell = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("cancelKey", "Escape"));
		#endregion
		updated = true;
	}
	public void Update()
	{
		#region keys
		forward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("forwardKey", "W"));
		backward = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("backwardKey", "S"));
		left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "A"));
		right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "D"));
		jump = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpKey", "Space"));
		fire = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("fireKey", "Mouse0"));
		reload = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("reloadKey", "R"));
		scoreboard = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("scoreboardKey", "Tab"));
		cancell = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("cancelKey", "Escape"));
		#endregion

		KeyCode[] Keys = { forward, backward, left, right, jump, run, crouch, fire, reload, scoreboard, cancell };
		keys = Keys;

		keys2Show = Keys;

		key2PressShow = new bool[keys.Length];
		int i = 0;
		foreach (var item in Keys)
		{
			if (Input.GetKey(item))
			{
				key2PressShow.SetValue(true, i);
			}
			else
			{
				key2PressShow.SetValue(false, i);
			}
			i++;
		}
		showAxes = new float[2];
		showAxes[0] = horizontal;
		showAxes[1] = vertical;

		realAxes = new float[2];
		realAxes[0] = Input.GetAxis("Horizontal");
		realAxes[1] = Input.GetAxis("Vertical");

		vertical = axes(vertical, forward, backward, smooth);
		horizontal = axes(horizontal, right, left, smooth);

		getAnyAxes = horizontal == 0 && vertical == 0;
	}
	float axes(float axe, KeyCode Plus, KeyCode Minus, float time = 0.5f)
	{
		float axew = 0;

		if (thisKey(Plus) && thisKey(Minus)) axew = 0;
		else if (thisKey(Plus)) axew = 1;
		else if (thisKey(Minus)) axew = -1;

		axe = Mathf.Lerp(axe, axew, time);
		if (Mathf.Abs(axe) < 0.009) axe = 0;
		return axe;
	}

	bool thisKey(KeyCode Key)
	{
		if (Input.GetKeyDown(Key) || Input.GetKey(Key))
			return true;
		return false;
	}
}
