using System;
using UnityEngine;
using UnityEngine.Events;

namespace CodeStage.AntiCheat.Detectors
{
	[AddComponentMenu("")]
	public abstract class ActDetectorBase : MonoBehaviour
	{
		protected const string ContainerName = "Anti-Cheat Toolkit Detectors";

		protected const string MenuPath = "Code Stage/Anti-Cheat Toolkit/";

		protected const string GameObjectMenuPath = "GameObject/Create Other/Code Stage/Anti-Cheat Toolkit/";

		protected static GameObject detectorsContainer;

		[Tooltip("Automatically start detector. Detection Event will be called on detection.")]
		public bool autoStart = true;

		[Tooltip("Detector will survive new level (scene) load if checked.")]
		public bool keepAlive = true;

		[Tooltip("Automatically dispose Detector after firing callback.")]
		public bool autoDispose = true;

		[SerializeField]
		protected UnityEvent detectionEvent;

		[SerializeField]
		protected bool detectionEventHasListener;

		protected bool started;

		protected bool isRunning;

		public bool IsRunning => isRunning;

		public event Action CheatDetected;

		private void Start()
		{
			if (detectorsContainer == null && base.gameObject.name == "Anti-Cheat Toolkit Detectors")
			{
				detectorsContainer = base.gameObject;
			}
			if (autoStart && !started)
			{
				StartDetectionAutomatically();
			}
		}

		private void OnEnable()
		{
			ResumeDetector();
		}

		private void OnDisable()
		{
			PauseDetector();
		}

		private void OnApplicationQuit()
		{
			DisposeInternal();
		}

		protected virtual void OnDestroy()
		{
			StopDetectionInternal();
			if (base.transform.childCount == 0 && GetComponentsInChildren<Component>().Length <= 2)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (base.name == "Anti-Cheat Toolkit Detectors" && GetComponentsInChildren<ActDetectorBase>().Length <= 1)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		internal virtual void OnCheatingDetected()
		{
			if (this.CheatDetected != null)
			{
				this.CheatDetected();
			}
			if (detectionEventHasListener)
			{
				detectionEvent.Invoke();
			}
			if (autoDispose)
			{
				DisposeInternal();
			}
			else
			{
				StopDetectionInternal();
			}
		}

		protected virtual bool Init(ActDetectorBase instance, string detectorName)
		{
			if (instance != null && instance != this && instance.keepAlive)
			{
				UnityEngine.Object.Destroy(this);
				return false;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			return true;
		}

		protected virtual void DisposeInternal()
		{
			UnityEngine.Object.Destroy(this);
		}

		protected virtual bool DetectorHasCallbacks()
		{
			return this.CheatDetected != null || detectionEventHasListener;
		}

		protected virtual void StopDetectionInternal()
		{
			this.CheatDetected = null;
			started = false;
			isRunning = false;
		}

		protected virtual void PauseDetector()
		{
			if (started)
			{
				isRunning = false;
			}
		}

		protected virtual bool ResumeDetector()
		{
			if (!started || !DetectorHasCallbacks())
			{
				return false;
			}
			isRunning = true;
			return true;
		}

		protected abstract void StartDetectionAutomatically();
	}
}
