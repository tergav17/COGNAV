using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using COGNAV.ARAP;
using COGNAV.Control;
using COGNAV.Interface;

namespace COGNAV {
    static class Entry {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mf = new MainForm();

            mf.GetGraphicConsole().Text = @"COGNAV Version 1.0" + '\n';

            void RunApplication() {
                Application.Run(mf);
            }

            Thread t = new Thread(RunApplication);
            t.Start();

            GraphicConsole gc = new GraphicConsole(mf.GetGraphicConsole());

            GamepadHandler gh = new GamepadHandler(gc, mf.GetLeftStickLabel(), mf.GetRightStickLabel(), mf.GetGamepadMenu());

            ControlHandler con = new ControlHandler(gc, mf.GetEnvironmentRedrawHandler(), mf.GetIsrLabel(), gh);

            mf.Control = con;
            
            CommunicationHandler ch = new CommunicationHandler(gc, con.GetRegisterAdapter());
        }
        
        
    }
}