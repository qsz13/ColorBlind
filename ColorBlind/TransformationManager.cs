using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorBlind
{
    struct ScreenColorEffect
    {
        public float[,] Matrix { get; private set; }
        public string Description { get; private set; }

        public ScreenColorEffect(float[,] matrix, string description)
            : this()
        {
            this.Matrix = matrix;
            this.Description = description;
        }

    }


    class TransformationManager
    {
        public bool mainLoopPaused = false;
        private bool exiting = false;
        private object invokeColorEffectLock = new object();
        private bool shouldInvokeColorEffect;
        private float[,] currentMatrix = null;
        private ScreenColorEffect invokeColorEffect;

        private float[,] protanopeMatrix =
           new float[,] { 
            { 1F, 5.089494e-01F, 0.6173267F, 0, 0},
            { 0, 4.910539e-01F,-0.6173226F,0,0 },
            { 0, 6.881557e-07F, 1.0000008F, 0, 0 },
            { 0, 0, 0, 1, 0},
            {0, 0, 0, 0, 1}
            };
        private float[,] deuteranopeMatrix =
            new float[,] { 
            { 1F, 2.023248e-01F, 0.5174109F, 0, 0},
            { 0, 7.976745e-01F,-0.5174129F,0,0 },
            { 0, -1.480542e-07F, 0.9999996F, 0, 0 },
            { 0, 0, 0, 1, 0},
            {0, 0, 0, 0, 1}
            };


        private float[,] tritanopeMatrix =
            new float[,] { 
            { 1F, -1.385364e-01F, 3.365585F, 0, 0},
            { 0, 1.138538e+00F,-3.365629F,0,0 },
            { 0, 2.053516e-07F,  0.999995F, 0, 0 },
            { 0, 0, 0, 1, 0},
            {0, 0, 0, 0, 1}
            };


        public void setColorEffect(TransType type)
        {
            
            if(type == TransType.protanope)
            {
                InvokeColorEffect( new ScreenColorEffect(protanopeMatrix, "protanope"));
            }
            else if (type == TransType.deuteranope)
            {
                InvokeColorEffect( new ScreenColorEffect(deuteranopeMatrix, "deuteranope"));
            }
            else if (type == TransType.tritanope)
            {
                InvokeColorEffect(new ScreenColorEffect(tritanopeMatrix, "tritanope"));
            }
        }

        public void init()
        {
            currentMatrix = protanopeMatrix;
            InitializeControlLoop();
        }

        private void InitializeControlLoop()
        {
            System.Threading.Thread t = new System.Threading.Thread(ControlLoop);
            t.SetApartmentState((System.Threading.ApartmentState.STA));
            t.Start();
            
            
        }

        private void InvokeColorEffect(ScreenColorEffect colorEffect)
        {
            lock (invokeColorEffectLock)
            {
                invokeColorEffect = colorEffect;
                //currentMatrix = colorEffect.Matrix;
                shouldInvokeColorEffect = true;
                
            }
        }

        public  void DoMagnifierApiInvoke()
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

          private void PauseLoop()
		{
			while (mainLoopPaused && !exiting)
			{
				System.Threading.Thread.Sleep(100);
                
				DoMagnifierApiInvoke();
			}
		}



          private void ControlLoop()
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
             
          }





        private void SafeChangeColorEffect(float[,] matrix)
        {
       
            if (!mainLoopPaused && !exiting)
            {
                BuiltinMatrices.InterpolateColorEffect(currentMatrix, matrix);
            }
            currentMatrix = matrix;
        }

        private void ToggleColorEffect(bool fromNormal)
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

        public void Toggle()
        {
            this.mainLoopPaused = !mainLoopPaused;
        }
      
    }
}
