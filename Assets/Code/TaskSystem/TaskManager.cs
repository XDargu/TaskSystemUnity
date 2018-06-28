using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskManager : MonoBehaviour
{
	enum TaskFlags
	{
		Destroyed = 1,
		Paused = 2
	}

	class TaskData
	{
		public Task task;
		TaskFlags flags;
		public int ID;

		public void SetFlag (TaskFlags flag, bool value = true)
		{
			// Add Flag value
			if (value) { flags = flags | flag; }
			else { flags = flags & ~flag; }
		}

		public bool IsFlagSet (TaskFlags flag)
		{
			return (flags & flag) == flag;
		}

		public bool isValid()
		{
			return !IsFlagSet(TaskFlags.Destroyed);
		}
	}

	List<TaskData> tasks;
	TaskData currentTask;

	TaskManagerDebugger taskDebugger;

	int lastID;

	void Awake ()
	{
		tasks = new List<TaskData>();
		taskDebugger = new TaskManagerDebugger(gameObject);

		currentTask = null;
		lastID = 0;
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		taskDebugger.Update();

		// Just execute the task with highest priority
		// List is sorted, with task with highest priority at the beggining
		TaskData nextCurrentTask = null;

		for (int i = 0; i < tasks.Count; i++)
		{
			if (tasks [i].isValid ())
			{
				nextCurrentTask = tasks [i];
				break;
			}
		}

		// Manage paused tasks
		if (currentTask != null && currentTask != nextCurrentTask)
		{
			if (currentTask.isValid())
			{
				currentTask.task.Pause();
				currentTask.SetFlag (TaskFlags.Paused);
			}

			if (nextCurrentTask.IsFlagSet(TaskFlags.Paused))
			{
				nextCurrentTask.task.Resume ();
				nextCurrentTask.SetFlag (TaskFlags.Paused, false);
			}
		}

		if (nextCurrentTask != null)
			Debug.Assert (nextCurrentTask.isValid());

		// Update tasks
		currentTask = nextCurrentTask;
		if (currentTask != null)
		{
			currentTask.task.Update ();
		}

		// Clean up
		for (var i = tasks.Count - 1; i > -1; i--)
		{
			if (tasks [i].IsFlagSet (TaskFlags.Destroyed))
			{
				tasks.RemoveAt (i);
			}
		}
	}

	public void TriggerTask (Task triggerTask, string origin)
	{
		// Remove existing tasks with same priority
		int indexToInsert = 0;
		int taskToRemoveIndex = -1;
		for (int i = 0; i < tasks.Count; i++)
		{
			if (tasks [i].task.priority == triggerTask.priority)
			{
				indexToInsert = i;
				taskToRemoveIndex = i;
				break;
			}

			if (tasks [i].task.priority > triggerTask.priority)
			{
				indexToInsert = i + 1;
				break;
			}
		}

		UnregisterTask(taskToRemoveIndex, "TaskManager::TriggerTask");
        RegisterTask(triggerTask, indexToInsert, origin);
	}

    public void RegisterTask (Task taskToRegister, int indexToInsert, string origin)
	{
		// Prepare task
		taskToRegister.gameObject = gameObject;

		// Add new priority
		TaskData taskData = new TaskData();
		taskData.task = taskToRegister;
		taskData.ID = lastID++;

		tasks.Insert(indexToInsert, taskData);
		taskToRegister.Construct();

		// Debugging data
		taskDebugger.RegisterTask(taskData.ID, taskToRegister.GetType().ToString(), taskData.task.priority, origin);
	}


    public void UnregisterTask(Task task, string origin)
	{
		// Remove existing tasks with same priority
		int taskToRemoveIndex = -1;
		for (int i = 0; i < tasks.Count; i++)
		{
			if (tasks [i].task == task)
			{
				taskToRemoveIndex = i;
				break;
			}
		}

		UnregisterTask(taskToRemoveIndex, origin);
	}

    public void UnregisterTask (int taskToRemoveIndex, string origin)
	{
		if (taskToRemoveIndex >= 0 && taskToRemoveIndex < tasks.Count)
		{
			// Debugging data
			taskDebugger.UnregisterTask(tasks [taskToRemoveIndex].ID, origin);

			tasks[taskToRemoveIndex].task.Destruct ();
			tasks[taskToRemoveIndex].SetFlag(TaskFlags.Destroyed);
		}
	}

    public void OnSignal (Signal signal, string origin)
	{
		foreach (TaskData taskData in tasks)
		{
			if (taskData.isValid())
			{
                taskData.task.OnSignal (signal, origin);
			}
		}
	}

	public void OnGUI ()
	{
		taskDebugger.OnGUI();
	}
}
