using System;
using System.Collections.Generic;
using System.Threading;
using SharpDX.DirectInput;

namespace COGNAV.Interface {
    public class GamepadHandler {

        public volatile float LeftStick = 0;
        public volatile float RightStick = 0;
        
        private volatile bool _run;

        private GraphicConsole _graphicConsole;
        private System.Windows.Forms.Label _leftStickLabel;
        private System.Windows.Forms.Label _rightStickLabel;
        private System.Windows.Forms.ComboBox _menu;
        
        public GamepadHandler(GraphicConsole graphicConsole, System.Windows.Forms.Label leftStickLabel, System.Windows.Forms.Label rightStickLabel, System.Windows.Forms.ComboBox menu) {
            
            // Your usual default value settings
            _run = true;
            _graphicConsole = graphicConsole;
            _leftStickLabel = leftStickLabel;
            _rightStickLabel = rightStickLabel;
            _menu = menu;
            
            // Create the thread
            Thread gamepadThread = new Thread(new ThreadStart(this.HandleGamepad));

            // Start the thread
            gamepadThread.Start();
        }

        /**
         * Tells the handler thread to end its loop and quit
         */
        public void Terminate() {
            _run = false;
        }

        /**
         * Thread safe handling for setting menu text
         */
        private void SetMenuText(String str) {
            
            void DoSafe() {
                _menu.Text = str;
            }

            _menu.Invoke((Action) DoSafe);

        }
        
        /**
         * Thread safe getting of menu text
         */
        private String GetMenuText() {
            
            String str = String.Empty;
            
            void DoSafe() {
                str = _menu.Text;
            }

            _menu.Invoke((Action) DoSafe);

            return str;
        }

        /**
         * Thread safe adding of objects to menu
         */
        private void AddToMenu(String str) {
            
            void DoSafe() {
                _menu.Items.Add(str);
            }

            _menu.Invoke((Action) DoSafe);
        }

        /**
         * Handler variables
         */
        
        private const string NoneString = @"          --- none ---";

        private string _currentSelection;

        /**
         * Gamepad handler thread
         */
        private void HandleGamepad() {
            
            // Set the default menu selection
            SetMenuText(NoneString);
            AddToMenu(NoneString);

            // Make this the current selection too
            _currentSelection = GetMenuText();

            // Indicate Gamepad thread is starting up
            _graphicConsole.PutStartup("Starting Gamepad Thread...");
            
            // Make a DirectInput object for later use
            DirectInput directInput = new DirectInput();

            // Grab from gamepads
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)) {
                SetMenuText(deviceInstance.InstanceName);
            }

            // Grab from joysticks
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)) {
                SetMenuText(deviceInstance.InstanceName);
            }

            // Create a null input object, will be acquired later
            Joystick input = null;
            
            // Update timer, we want an update to only happen once a second, but instantly when the thread first starts
            int updateTimer = 100;
            
            // Run till ya drop
            while (_run) {

                // Detect if a change in selection has occured
                if (!GetMenuText().Equals(_currentSelection)) {

                    // If input isn't null...
                    if (input != null) {
                        // Dispose of the object and make it null
                        input.Dispose();
                        input = null;
                    }
                    
                    // Update current selection
                    _currentSelection = GetMenuText();
                    
                    // If it isn't the NoneString, attempt to acquire the gamepad
                    if (!_currentSelection.Equals(NoneString)) {

                        _graphicConsole.PutLine("Attempting To Acquire " + _currentSelection);
                        input = AcquireJoystick(directInput, _currentSelection);
                    }
                }

                // Every 100 ticks (~1 second), update the menu options and reset the timer
                if (++updateTimer > 100) {
                    UpdateMenuOptions(directInput);
                    updateTimer = 0;
                }
                
                // If the gamepad isn't null, attempt to update the gamepad values
                if (input != null) if (!UpdateGamepadValues(input)) {
                    // If an error occurs while trying to update, do the following:
                    
                    // Tell the user an error has occured
                    _graphicConsole.PutError("Gamepad Connection Lost!");
                    
                    // Dispose and nullify the current object
                    input.Dispose();
                    input = null;
                    
                    // Set the current selection to the NoneString
                    SetMenuText(NoneString);
                    _currentSelection = NoneString;
                }

                
                // If there are no joysticks selected, set both sticks to 0 to avoid foul behavior 
                if (_currentSelection.Equals(NoneString)) {
                    RightStick = 0;
                    LeftStick = 0;
                    
                    // Update the status label too, just for good measure
                    _leftStickLabel.Text = @"Left Stick: " + LeftStick.ToString("0.00");
                    _rightStickLabel.Text = @"Right Stick: " + RightStick.ToString("0.00");
                }
                
                // Waiter so that this thread runs ~100 a second, don't want to eat up *all* the CPU time do we?
                Thread.Sleep(10);
            }
        }

        /**
         * Given the name of a selection, attempt to make a new joystick object and Acquire it
         */
        private Joystick AcquireJoystick(DirectInput directInput, string selection) {
            
            // Default objects lmao
            Joystick stick = null;
            Guid joystickGuid = Guid.Empty;

            // Menu options again, but this time we are using it to generate names only
            List<string> menuOptions = new List<string>();
            
            // Grab from gamepads
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)) {
                // Get the desired name
                string currentName = (deviceInstance.InstanceName ?? string.Empty);

                // If the name already exists, start adding counting markers to make it different
                int copyCounter = 2;
                while (menuOptions.Contains(currentName)) currentName = (deviceInstance.InstanceName ?? string.Empty) + " (" + copyCounter++ + ")";
                
                // Add this to the menu options
                menuOptions.Add(currentName);
                
                // If this equals the selection, set the joystickGuid
                if (currentName.Equals(selection)) joystickGuid = deviceInstance.InstanceGuid;
            }

            // Grab from joysticks
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)) {
                // Get the desired name
                string currentName = (deviceInstance.InstanceName ?? string.Empty);

                // If the name already exists, start adding counting markers to make it different
                int copyCounter = 2;
                while (menuOptions.Contains(currentName)) currentName = (deviceInstance.InstanceName ?? string.Empty) + " (" + copyCounter++ + ")";
                
                // Add this to the menu options
                menuOptions.Add(currentName);
                
                // If this equals the selection, set the joystickGuid
                if (currentName.Equals(selection)) joystickGuid = deviceInstance.InstanceGuid;
            }

            if (joystickGuid != Guid.Empty) {
                stick = new Joystick(directInput, joystickGuid);
                stick.Properties.BufferSize = 128;
                stick.Acquire();
                _graphicConsole.PutSuccess("Acquired Joystick " + selection);
            }

            return stick;
        }

        /**
         * Grabs the most recent list of gamepads connected to the computer
         * Changes the dropdown menu list to match these changes
         * This method is kinda slow, so only run it sparingly
         * Again, more terrible code here but I don't care
         */
        private void UpdateMenuOptions(DirectInput directInput) {
            // List to store the most recent menu options
            // Will be used to compare against the current menu elements
            List<string> menuOptions = new List<string>();
            
            // Grab from gamepads
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)) {
                // Get the desired name
                string currentName = (deviceInstance.InstanceName ?? string.Empty);

                // If the name already exists, start adding counting markers to make it different
                int copyCounter = 2;
                while (menuOptions.Contains(currentName)) currentName = (deviceInstance.InstanceName ?? string.Empty) + " (" + copyCounter++ + ")";
                
                // Add this to the menu options
                menuOptions.Add(currentName);
            }

            // Grab from joysticks
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)) {
                // Get the desired name
                string currentName = (deviceInstance.InstanceName ?? string.Empty);

                // If the name already exists, start adding counting markers to make it different
                int copyCounter = 2;
                while (menuOptions.Contains(currentName)) currentName = (deviceInstance.InstanceName ?? string.Empty) + " (" + copyCounter++ + ")";
                
                // Add this to the menu options
                menuOptions.Add(currentName);
            }

            // Deadpool of elements to remove
            List<string> deadpool = new List<string>();
            
            // Scan through, and find any elements that need removing
            for (int i = 0; i < _menu.Items.Count; i++) {
                if (!menuOptions.Contains((string) _menu.Items[i])) deadpool.Add((string) _menu.Items[i]);
            }
            
            // Remove those elements
            foreach (var item in deadpool)
                
                // If it is the NoneString, don't remove it
                if (!item.Equals(NoneString)) {
                    
                    // Reset the current selection to the NoneString
                    if (GetMenuText().Equals(item)) SetMenuText(NoneString);
                    _menu.Items.Remove(item);
                }

            // If the menu doesn't have an option, add it
            foreach (var t in menuOptions) {
                if (!_menu.Items.Contains(t)) AddToMenu(t);
            }
        }

        /**
         * Takes a joystick object, and updates the left and right stick values for it.
         * Return true if successful, false if there was an error (I.E.) unexpected disconnection
         */
        private bool UpdateGamepadValues(Joystick input) {
            // Big ass try catch lmao
            // I feel like yandev with this shitty ass code
            try {

                // Poll for joystick data
                input.Poll();
                
                // Grab the data from the buffer
                var data = input.GetBufferedData();
                
                // Scan through each point of data
                foreach (var state in data) {
                    
                    // Get the values of the Y and RotZ axis, correct to float
                    if (state.Offset == JoystickOffset.Y)
                        LeftStick = -Convert.ToSingle((state.Value - 32768) / 32768.0);
                    if (state.Offset == JoystickOffset.RotationZ)
                        RightStick = -Convert.ToSingle((state.Value - 32768) / 32768.0);

                    // Basic float capping for small values
                    if (Math.Abs(LeftStick) < 0.05) LeftStick = 0;
                    if (Math.Abs(RightStick) < 0.05) RightStick = 0;

                    // Basic float capping for large values
                    if (LeftStick > 0.99) LeftStick = 1;
                    if (LeftStick < -0.99) LeftStick = -1;
                        
                    if (RightStick > 0.99) RightStick = 1;
                    if (RightStick < -0.99) RightStick = -1;

                    // Update the status label too, just for good measure
                    _leftStickLabel.Text = @"Left Stick: " + LeftStick.ToString("0.00");
                    _rightStickLabel.Text = @"Right Stick: " + RightStick.ToString("0.00");
                }

            } catch (Exception e) {
                // Uh oh, error!
                // Put it on the console whatever it is
                _graphicConsole.PutError("Caught Error: " + e.Message);
                
                // And return false to indicate that an error has occured
                // The main thread should drop this joystick NOW!
                return false;
            }
            
            // Successful return
            return true;
        }

    }
}