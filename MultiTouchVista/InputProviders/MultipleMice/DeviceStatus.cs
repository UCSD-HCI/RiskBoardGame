﻿using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MultipleMice.Native;
using System.Diagnostics;

namespace MultipleMice
{
	class DeviceStatus
	{
		readonly RawDevice device;
		readonly DebugCursor debugCursor;
		Point location;

		public DeviceStatus(RawDevice device)
		{
			this.device = device;

			debugCursor = new DebugCursor();
			debugCursor.Name = "DebugCursor";
			Win32.POINT position = Win32.GetCursorPosition();
			Location = new Point(position.x, position.y);

			Thread t = new Thread(ThreadWorker);
			t.Name = "Cursor for device: " + device.Handle;
			t.SetApartmentState(ApartmentState.STA);
			t.IsBackground = true;
			t.Start();
		}

		void ThreadWorker()
		{
			debugCursor.Show(Location);
			Application.Run(debugCursor);
		}

		public Point Location
		{
			get { return location; }
			set
			{
				if (location != value)
				{
					location = value;
					UpdateLocation();
				}
			}
		}

		public IntPtr Handle
		{
			get { return device.Handle; }
		}

		void UpdateLocation()
		{
            //Trace.WriteLine(location.ToString());
			SynchronizationContext current = SynchronizationContext.Current;
			current.Send(SyncUpdateLocation, null);
		}

		void SyncUpdateLocation(object state)
		{
			debugCursor.Location = new Point(Location.X - debugCursor.Width / 2, Location.Y - debugCursor.Height / 2);
            //Trace.WriteLine(debugCursor.Location);
		}

		public DeviceState ButtonState { get; set; }
	}
}