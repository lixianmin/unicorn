
/********************************************************************
created:    2022-08-11
author:     lixianmin

purpose:    extended debug
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Diagnostics;
using System.Threading;
using System.Text;
using Unicorn;

// this class should not be placed into Unicorn namespace, because it will cause compile error 
// when using 'System' and 'Unicorn' simutaniously.

[Flags]
public enum ConsoleFlags : ushort
{
	None = 0x00,
	DetailedMessage = 0x01,
	OpenStandardOutput = 0x02,
	FlushOnWrite = 0x04,
	FlushAtIntervals = 0x08,
}

public static class Console
{
	// We use static ctor, because in editor mode there is no Way to call Init().
	static Console()
	{
		_idMainThread = Thread.CurrentThread.ManagedThreadId;
		Flags = ConsoleFlags.DetailedMessage | ConsoleFlags.FlushOnWrite;
	}

	internal static void Update()
	{
		_time = UnityEngine.Time.realtimeSinceStartup;

		if (null != _sbLogText && _time >= _nextFlushLogTime && _HasFlags(ConsoleFlags.FlushAtIntervals))
		{
			_CheckFlushLogText();
			_nextFlushLogTime = _time + 3.0f;
		}
	}

	public static void WriteLine(string format, params object[] args)
	{
		_WriteLine(_lpfnLog, _FormatMessage(format, args));
	}

	public static void WriteLine(string message)
	{
		_WriteLine(_lpfnLog, message);
	}

	public static void WriteLine(object message)
	{
		_WriteLine(_lpfnLog, message);
	}

	public static class Warning
	{
		public static void WriteLine(string format, params object[] args)
		{
			_WriteLine(_lpfnLogWarning, _FormatMessage(format, args));
		}

		public static void WriteLine(string message)
		{
			_WriteLine(_lpfnLogWarning, message);
		}

		public static void WriteLine(object message)
		{
			_WriteLine(_lpfnLogWarning, message);
		}
	}

	public static class Error
	{
		public static void WriteLine(string format, params object[] args)
		{
			_WriteLine(_lpfnLogError, _FormatMessage(format, args));
		}

		public static void WriteLine(string message)
		{
			_WriteLine(_lpfnLogError, message);
		}

		public static void WriteLine(object message)
		{
			_WriteLine(_lpfnLogError, message);
		}
	}

	private static void _WriteLine(System.Action<object> output, object message)
	{
		var isMainThread = Thread.CurrentThread.ManagedThreadId == _idMainThread;

		if (_HasFlags(ConsoleFlags.DetailedMessage))
		{
			var frameCount = 0;
			var realtime = string.Empty;

			if (isMainThread)
			{
				frameCount = UnityEngine.Time.frameCount;
				realtime = UnityEngine.Time.realtimeSinceStartup.ToString("F3");
			}
			else
			{
				frameCount = os.frameCount;
				realtime = _time.ToString("F3");
			}

			if (_lastFrameCount != frameCount)
			{
				_lastFrameCount = frameCount;
				_messageFormat[1] = frameCount.ToString();
			}

			_messageFormat[3] = realtime;
			_messageFormat[5] = null != message ? message.ToString() : "null mesage (-_-)";
			message = string.Concat(_messageFormat);
		}

		try
		{
			// main thread or in editor and isPlaying= false.
			if (isMainThread || _idMainThread == 0)
			{
				output(message);
			}
			else
			{
				if (os.isEditor)
				{
					var sbText = new StringBuilder(message.ToString());
					sbText.AppendLine();

					var text = _AppendStackTrace(sbText);
					Loom.RunDelayed(() => output(text));
				}
				else
				{
					Loom.RunDelayed(() => output(message));
				}
			}
		}
		catch (MissingMethodException)
		{
			System.Console.WriteLine(message);
		}
	}

	private static string _AppendStackTrace(StringBuilder sbText)
	{
		var trace = new System.Diagnostics.StackTrace(2, true);
		var frames = trace.GetFrames();
		if (frames != null)
		{
			foreach (var frame in frames)
			{
				sbText.AppendFormat("{0} (at {1}:{2})\n"
					, frame.GetMethod()
					, frame.GetFileName()
					, frame.GetFileLineNumber().ToString());
			}
		}
		
		var result = sbText.ToString();
		return result;
	}

	private static string _FormatMessage(string format, params object[] args)
	{
		var message = "null format (-__-)";
		if (null != format)
		{
			_sbFormatter.AppendFormat(null, format, args);
			message = _sbFormatter.ToString();
			_sbFormatter.Length = 0;
		}

		return message;
	}

	private static void _Log(object message)
	{
		if (_HasFlags(ConsoleFlags.FlushOnWrite))
		{
			UnityEngine.Debug.Log(message);
		}
		else
		{
			_sbLogText.AppendLine(message.ToString());
		}

		if (_HasFlags(ConsoleFlags.OpenStandardOutput))
		{
			System.Console.WriteLine(message);
		}
	}

	private static void _LogWarning(object message)
	{
		_CheckFlushLogText();
		UnityEngine.Debug.LogWarning(message);

		if (_HasFlags(ConsoleFlags.OpenStandardOutput))
		{
			System.Console.WriteLine(message);
		}
	}

	private static void _LogError(object message)
	{
		_CheckFlushLogText();
		UnityEngine.Debug.LogError(message);

		if (_HasFlags(ConsoleFlags.OpenStandardOutput))
		{
			System.Console.Error.WriteLine(message);
		}
	}

	private static void _CheckFlushLogText()
	{
		if (_sbLogText is { Length: > 0 })
		{
			UnityEngine.Debug.Log(_sbLogText);
			_sbLogText.Length = 0;
		}
	}

	private static bool _HasFlags(ConsoleFlags flags)
	{
		return (_flags & flags) != 0;
	}

	public static ConsoleFlags Flags
	{
		get => _flags;
		set
		{
			if (_flags == value)
			{
				return;
			}

			_flags = value;

			if (_HasFlags(ConsoleFlags.FlushOnWrite))
			{
				_CheckFlushLogText();
				_sbLogText = null;
			}
			// 只要不是FlushOnWrite，都要加入_sbLogText，否则日志会丢失.
			// else if (_HasFlags(ConsoleFlags.FlushAtIntervals))
			else if (null == _sbLogText)
			{
				_sbLogText = new StringBuilder(1024);
			}
		}
	}

	private static ConsoleFlags _flags;

	private static int _idMainThread;
	private static float _time;

	private static int _lastFrameCount = 0;
	private static StringBuilder _sbLogText;
	private static readonly StringBuilder _sbFormatter = new StringBuilder();
	private static float _nextFlushLogTime;

	private static Action<object> _lpfnLog = _Log;
	private static Action<object> _lpfnLogWarning = _LogWarning;
	private static Action<object> _lpfnLogError = _LogError;

	private static readonly string[] _messageFormat =
	{
		"[@@frame=",	// 加两个@号, 方便使用 adb logcat | grep @@ 过滤日志
		"(-_-)",
		", time=",
		null,
		"] ",
		null
	};
}
