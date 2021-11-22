using System;
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

            GraphicConsole gc = new GraphicConsole(mf.GetGraphicConsole());
            
            gc.PutLine("COGNAV Version 1.0");

            ControlHandler con = new ControlHandler(gc);

            CommunicationHandler ch = new CommunicationHandler(gc, con.GetRegisterAdapter());

            Application.Run(mf);

            gc.Terminate();
            ch.Terminate();
            con.Terminate();


        }
        
        
    }
}