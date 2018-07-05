using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TaskManagerDebugger
{
    public static readonly string[] SignalNames =
    {
        "RifleFound",
        "RifleLost",
        "RiflePickedUp",

        "TargetFound",
        "TargetLost",

        "TargetInRange",
        "TargetOutOfRange",

        "TargetDeath",

        "Hit"
    };

	static float taskHeight = 30.0f;
	public static float pixelPerSecond = 30.0f;
	static float horizontalOffset = 20.0f;

	public class TaskHistoryData
	{
		public int ID;
		public string trigger = "";
		public string name = "";
		public float startTime = 0;
		public float endTime = 0;
		public int priority = 0;
        public string origin = "";
        public string end = "";
        public TaskType taskType = TaskType.Default;
	}

    public class SignalHistoryData
    {
        public Signal signal;
        public string origin = "";
        public float time = 0;
    }

    GameObject mGameObject;

	public TaskManagerDebugger (GameObject gameObject)
	{
		historyData = new List<TaskHistoryData>();
        signalData = new List<SignalHistoryData>();

        mGameObject = gameObject;
	}

	public void Update ()
	{
		if (Input.GetAxis ("Mouse ScrollWheel") != 0f) // forward
		{
			pixelPerSecond += Input.GetAxis ("Mouse ScrollWheel") * 10.0f;
		}
	}

    public void RegisterSignal(Signal signal, string origin)
    {
        SignalHistoryData data = new SignalHistoryData();
        data.signal = signal;
        data.origin = origin;
        data.time = Time.timeSinceLevelLoad;

        signalData.Add(data);
    }

    public void RegisterTask(int ID, string name, int priority, string origin, TaskType type)
	{
		// Debugging data
		TaskHistoryData historyTaskData = new TaskHistoryData();
		historyTaskData.ID = ID;
		historyTaskData.name = name;
		historyTaskData.startTime = Time.timeSinceLevelLoad;
		historyTaskData.priority = priority;
        historyTaskData.origin = origin;
        historyTaskData.taskType = type;
		historyData.Add(historyTaskData);
	}

	public void UnregisterTask (int ID, string origin)
	{
		foreach (TaskHistoryData data in historyData)
		{
			if (data.ID == ID)
			{
				data.endTime = Time.timeSinceLevelLoad;
                data.end = origin;
			}
		}
	}

	public TaskHistoryData GetActiveTaskAtTime(float time)
	{
		int highestPriorityTask = -1;
		TaskHistoryData activeTask = null;

		foreach (TaskHistoryData data in historyData)
		{
			if (time > data.startTime && (time < data.endTime || data.endTime == 0))
			{
				if (data.priority > highestPriorityTask)
				{
					highestPriorityTask = data.priority;
					activeTask = data;
				}
			}
		}

		return activeTask;
	}

	public void RenderTask (TaskHistoryData data, float startingTime)
	{
		if (data == null)
		{
			return;
		}

		float currentTime = Time.timeSinceLevelLoad;
	
		float startTimeSnapToWindow = Mathf.Max (data.startTime, startingTime);
		float startingPixel = (startTimeSnapToWindow - startingTime) * pixelPerSecond;

		float elpasedTime = data.endTime - startTimeSnapToWindow;
		if (data.endTime == 0)
		{
			elpasedTime = currentTime - startTimeSnapToWindow;
		}

		float width = elpasedTime * pixelPerSecond;

        GUI.contentColor = Color.white;
        GUI.Box (new Rect (
			horizontalOffset + startingPixel, 
			10 + taskHeight * data.priority,
			width,
			taskHeight
		), data.name);
	}

    public void RenderSignal(SignalHistoryData data, float startingTime, float verticalOffset)
    {
        if (data == null)
        {
            return;
        }

        float currentTime = Time.timeSinceLevelLoad;

        float startTimeSnapToWindow = Mathf.Max(data.time, startingTime);
        float startingPixel = (startTimeSnapToWindow - startingTime) * pixelPerSecond;

        GUI.DrawTexture(new Rect(horizontalOffset + startingPixel, 10, 1, taskHeight * 5 + verticalOffset), Texture2D.whiteTexture, ScaleMode.StretchToFill);

        GUI.contentColor = Color.black;
        
        GUI.Label(new Rect(horizontalOffset + startingPixel, taskHeight * 5 + 10 + verticalOffset, 600, 100), SignalNames[(int)data.signal]);
    }

	public void OnGUI ()
	{
		// Task debugger
		if (UnityEditor.Selection.activeGameObject != mGameObject)
		{
			return;
		}

		float currentTime = Time.timeSinceLevelLoad;
		float startingTime = currentTime;

		foreach (TaskHistoryData data in historyData)
		{
			if (data.startTime < startingTime)
			{
				startingTime = data.startTime;
			}
		}


		float timeFitsOnScreen = (Screen.width - horizontalOffset * 2) / pixelPerSecond;
		float maxStartingTime = currentTime - timeFitsOnScreen;

		startingTime = Mathf.Max (maxStartingTime, startingTime);

		int maxPriority = 0;

		foreach (TaskHistoryData data in historyData)
		{
			if (data.endTime > 0 && data.endTime < startingTime)
			{
				continue;
			}

			if (data.priority > maxPriority)
			{
				maxPriority = data.priority;
			}

			RenderTask (data, startingTime);
		}

        float verticalOffset = 0.0f;
        foreach (SignalHistoryData data in signalData)
        {
            if (data.time < startingTime)
            {
                continue;
            }

            RenderSignal(data, startingTime, verticalOffset);

            verticalOffset += 60.0f;
            if (verticalOffset >= 300.0f)
            {
                verticalOffset = 0.0f;
            }
        }

        // Draw basic lines
        GUI.DrawTexture (new Rect (horizontalOffset, 10, timeFitsOnScreen * pixelPerSecond, 3), Texture2D.whiteTexture, ScaleMode.StretchToFill);
		GUI.DrawTexture (new Rect (horizontalOffset, 10, 3, taskHeight * 5), Texture2D.whiteTexture, ScaleMode.StretchToFill);

		GUI.DrawTexture (new Rect (horizontalOffset + (currentTime - startingTime) * pixelPerSecond, 10, 1, taskHeight * 5), Texture2D.whiteTexture, ScaleMode.StretchToFill);

		// Draw mouse line
		Vector2 screenPos = Event.current.mousePosition;
		Vector2 convertedGUIPos = GUIUtility.ScreenToGUIPoint (screenPos);
		convertedGUIPos.x = Mathf.Clamp (convertedGUIPos.x, horizontalOffset, Screen.width - horizontalOffset * 2);

		GUI.DrawTexture (new Rect (convertedGUIPos.x, 10, 1, taskHeight * 8), Texture2D.whiteTexture, ScaleMode.StretchToFill);

		// Draw active task at mouse time
		float timeAtMouse = startingTime + (convertedGUIPos.x - horizontalOffset) / pixelPerSecond;
		TaskHistoryData taskAtMouse = GetActiveTaskAtTime (timeAtMouse);
		RenderTask (taskAtMouse, startingTime);

		// Draw timestamp
		TimeSpan timeSpan = TimeSpan.FromSeconds (timeAtMouse);
		string timeAsText = String.Format ("{0:00}:{1:00}:{2:000}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		if (taskAtMouse != null)
		{
            timeAsText += "\n" + taskAtMouse.name + "\nOrigin: " + taskAtMouse.origin + "\nEnd: " + taskAtMouse.end;;
		}

		GUI.contentColor = Color.black;
		GUI.Label(new Rect(convertedGUIPos.x, taskHeight * 8 + 10, 600, 100 ), timeAsText);
	}

	List<TaskHistoryData> historyData;
    List<SignalHistoryData> signalData;
}
