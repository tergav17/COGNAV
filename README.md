# COGNAV Communication Register Map
	
Draft Version 1.0, Outlined by Gavin Tersteeg
	
## Overview
	
The [CO]mbination [G]ui and [NAV]iation system (COGNAV) interfaces with
the CyBOT playform using the ARAP protocol. It serves to extend CyBOT
functionality in a number of facets. These include:

	- Improved graphical feedback to the user
	- Simplified robot state switching
	- Teleop robot control through keyboard or gamepad
	- Enhanced navigation using map creation and pathfinding
	
Communication will be done through the ARAP protocol, with COGNAV acting
as the server, and the CyBOT acting as the client. The CyBOT will poll
the COGNAV registers for instructions to execute, and will update these
registers with relevant information about the robot. The contents and
functionality of these registers are as follows:

### Register 0:
  
  Data Format: ?
  
  ARAP special function register, outlined in ARAP.txt
  
  Read:
    Resend last packet
	
  Write:
    Send ping packet
	
### Register 1:

  Data Format: Unsigned Byte (8 bits)

  Command register; Used to send opcode commands to the client, or for
  the client to confirm that an opcode has finished execution. The
  definitions for each opcode can be found in the OPCODES section
  
  Read:
    Will send at maximum one arg back, this being the current opcode
	to execute. This opcode will continue to be send until the client
	confirms that this opcode command has finshed execution. If there
	is no operation to execute, no arguments will be sent.
	
  Write:
    Writing anything (even with no arguments) will tell COGNAV that
	the current opcode has finished executing. COGNAV will place
	the next opcode in the buffer to be read
	
### Register 2:

  Data Format: Float (32 bits)
  
  Rotational Position register; Used to obtain the requested rotational
  position, or to indicate that the rotational position has changed.
  
  Read:
	Will send exactly one argument back. If a ROT instruction has been
	sent, then this register will return the desired angle. Otherwise it
	will return the angle that COGNAV thinks the robot is at the moment.
	
  Write:
    Used to update COGNAV with an exact rotational position after a
	rotational event has occured.
	
### Register 3:

  Data Format: Float (32 bits)
	
  Linear Displacement register; Used to obtain the requested linear
  displacement, or to indicate that the linear position has changed.
  
  Read:
    Will send exactly one argument back. If a MOVE instruction has been
	sent, then this register will return the desired displacement.
	Otherwise it will return "0".
	
  Write:
    Used to update COGNAV with an exact displacement after a linear
	displacemnt event has occured.
	
### Register 4:

  Data Format: Float (32 bits)
  
  Object Angle Offset register; Used to define the offset angle that an
  object is at when transferring object data to COGNAV.
  
  Write:
    Any value written will be used to set the object's angluar offset.
	This angle starts at the front of the robot, and wraps clockwise
	around it.
	
### Register 5:

  Data Format: Float (32 bits)
	
  Object Distance register; Used to define how far away that an object
  is when transferring object data to COGNAV.
  
  Write:
    Any value written will be used to set the object distance. This is
	measured in centimeters, and begins at the center of the robot.
	
### Register 6:

  Data Format: Float (32 bits)
  
  Object Width register; Used to define how wide an object is when
  transferring object data to COGNAV.
  
  Write:
    Any value written will be used to set the object width. This is
	measured in centimeters.
	
### Register 7:

  Data Format: Unsigned Byte (8 bit)
  
  Object Type register; Used to give addition metadata about an object
  being sent to COGNAV. When this register is written to, it will
  cause COGNAV to register a new object using data found in registers
  4 through 6.
  
  Write:
    Only accepts on argument at time, any more will be ignored. The
	written value is used to define the object type
	
	0x00 - Temporary Unidentified Visual Object
	0x01 - Unidentified Visual Object
	0x02 - Confirmed Wide Visual Object
	0x03 - Confirmed Thin Visual Object
	0x04 - Bumper Collision Object
	0x05 - White Tape Object
	0x06 - Hole
	
### Register 8:

  Data Format: Float (32 bit)
  
  Left Analog Stick register; Returns the current position of the left
  analog stick current attached to the COGNAV software. Used for direct
  robot control in manual mode.
  
  Read:
    A value ranging from -1.0 to 1.0
	
### Register 9:

  Data Format: Float (32 bit)
  
  Right Analog Stick register; Returns the current position of the right
  analog stick current attached to the COGNAV software. Used for direct
  robot control in manual mode.
  
  Read:
    A value ranging from -1.0 to 1.0
	
## Opcodes
	
 -- work in progress --
