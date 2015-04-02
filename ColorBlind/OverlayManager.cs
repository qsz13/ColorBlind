//Copyright 2011-2014 Melvyn Laily
//http://arcanesanctum.net

//This file is part of NegativeScreen.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Concurrent;

namespace ColorBlind
{






    struct ScreenColorEffect
    {
        public float[,] Matrix { get; public set; }
        public string Description { get; public set; }

        public ScreenColorEffect(float[,] matrix, string description)
            : this()
        {
            this.Matrix = matrix;
            this.Description = description;
        }

    }
	/// <summary>
	/// inherits from Form so that hot keys can be bound to its message loop
	/// </summary>
	public partial class OverlayManager 
	{

		/// <summary>
		/// control whether the main loop is paused or not.
		/// </summary>
		public bool mainLoopPaused = false;

		/// <summary>
		/// allow to exit the main loop
		/// </summary>
		public bool exiting = false;

		/// <summary>
		/// store the current color matrix.
		/// </summary>
		public float[,] currentMatrix = null;

		// /!\ The full screen magnifier seems not to be thread-safe on Windows 8 at least,
		// so every call after initialization must be done on the same thread.
		#region Inter-thread color effect calls

		/// <summary>
		/// allow to execute magnifer api calls on the correct thread.
		/// </summary>
		public ScreenColorEffect invokeColorEffect;
		public bool shouldInvokeColorEffect;
		public object invokeColorEffectLock = new object();

		/// <summary>
		/// Ask for a color effect change to be executed on the correct thread.
		/// </summary>
		/// <param name="colorEffect"></param>
		public void InvokeColorEffect(ScreenColorEffect colorEffect)
		{
			lock (invokeColorEffectLock)
			{
				invokeColorEffect = colorEffect;
				//SynchronizeMenuItemCheckboxesWithEffect(colorEffect);
				shouldInvokeColorEffect = true;
			}
		}

		/// <summary>
		/// Execute the specified color effect change, on the right thread.
		/// </summary>
		public void DoMagnifierApiInvoke()
		{
			lock (invokeColorEffectLock)
			{
				if (shouldInvokeColorEffect)
				{
					SafeChangeColorEffect(invokeColorEffect.Matrix);
				}
				shouldInvokeColorEffect = false;
			}
		}

		#endregion

		public static OverlayManager _Instance;
		public static OverlayManager Instance
		{
			get
			{
				Initialize();
				return _Instance;
			}
		}

		public static void Initialize()
		{
			if (_Instance == null)
			{
				_Instance = new OverlayManager();
			}
		}

		public OverlayManager()
		{
			

			//InitializeContextMenu();

			//currentMatrix = Configuration.Current.InitialColorEffect.Matrix;
//			SynchronizeMenuItemCheckboxesWithEffect(Configuration.Current.InitialColorEffect); //requires the context menu to be initialized

			InitializeControlLoop();
		}




		public void InitializeControlLoop()
		{
			System.Threading.Thread t = new System.Threading.Thread(ControlLoop);
			t.SetApartmentState((System.Threading.ApartmentState.STA));
			t.Start();
		}

		/// <summary>
		/// Main loop, in charge of controlling the magnification api.
		/// </summary>
		public void ControlLoop()
		{
			
				mainLoopPaused = true;
				PauseLoop();

			while (!exiting)
			{
				if (!NativeMethods.MagInitialize())
				{
					throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
				ToggleColorEffect(fromNormal: true);
				while (!exiting)
				{
                    System.Threading.Thread.Sleep(100);
					DoMagnifierApiInvoke();
					if (mainLoopPaused)
					{
						ToggleColorEffect(fromNormal: false);
						if (!NativeMethods.MagUninitialize())
						{
							throw new Exception("MagUninitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
						}
						PauseLoop();
						//we need to reinitialize
						break;
					}
				}
			}
			/*this.Invoke((Action)(() =>
				{
					//this.Dispose();
					Application.Exit();
				}));*/
            Application.Exit();
		}

		public void PauseLoop()
		{
			while (mainLoopPaused && !exiting)
			{
				System.Threading.Thread.Sleep(100);
				DoMagnifierApiInvoke();
			}
		}



		/// <summary>
		/// Can be called from any thread.
		/// </summary>
		public void Exit()
		{
			if (!mainLoopPaused)
			{
				mainLoopPaused = true;
			}
			this.exiting = true;
		}

		/// <summary>
		/// Can be called from any thread.
		/// </summary>
		public void Toggle()
		{
			this.mainLoopPaused = !mainLoopPaused;
		}

		public void ToggleColorEffect(bool fromNormal)
		{
			if (fromNormal)
			{
					BuiltinMatrices.InterpolateColorEffect(BuiltinMatrices.Identity, currentMatrix);
			}
			else
			{
				
					BuiltinMatrices.InterpolateColorEffect(currentMatrix, BuiltinMatrices.Identity);
			}
		}

		/// <summary>
		/// Check if the magnification api is in a state where a color effect can be applied, then proceed.
		/// </summary>
		/// <param name="matrix"></param>
		public void SafeChangeColorEffect(float[,] matrix)
		{
			if (!mainLoopPaused && !exiting)
			{
				
					BuiltinMatrices.InterpolateColorEffect(currentMatrix, matrix);
				
			}
			currentMatrix = matrix;
		}

		

		/*public void SynchronizeMenuItemCheckboxesWithEffect(ScreenColorEffect effect)
		{
			ToolStripMenuItem currentItem = null;
			foreach (ToolStripMenuItem effectItem in this.changeModeToolStripMenuItem.DropDownItems)
			{
				effectItem.Checked = false; //reset all the check boxes
				var castItem = (ScreenColorEffect)effectItem.Tag;
				if (castItem.Matrix == effect.Matrix) currentItem = effectItem; //TODO: should implement equality comparison...
			}
			if (currentItem != null)
			{
				currentItem.Checked = true;
			}
		}*/

		#region Event Handlers

		public void OverlayManager_FormClosed(object sender, FormClosedEventArgs e)
		{
			Exit();
		}

		public void toggleInversionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Toggle();
		}

		public void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Exit();
		}

		
	

		#endregion

	}
}
