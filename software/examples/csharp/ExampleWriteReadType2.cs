using System;
using Tinkerforge;

class Example
{
	private static string HOST = "localhost";
	private static int PORT = 4223;
	private static string UID = "XYZ"; // Change XYZ to the UID of your NFC/RFID Bricklet

	// Callback function for state changed callback
	static void StateChangedCB(BrickletNFCRFID sender, byte state, bool idle)
	{
		if(state == BrickletNFCRFID.STATE_REQUEST_TAG_ID_READY)
		{
			Console.WriteLine("Tag found");

			// Write 16 byte to pages 5-8
			byte[] dataWrite = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};

			sender.WritePage(5, dataWrite);
			Console.WriteLine("Writing data...");
		}
		else if(state == BrickletNFCRFID.STATE_WRITE_PAGE_READY)
		{
			// Request pages 5-8
			sender.RequestPage(5);
			Console.WriteLine("Requesting data...");
		}
		else if(state == BrickletNFCRFID.STATE_REQUEST_PAGE_READY)
		{
			// Get and print pages
			byte[] data = sender.GetPage();
			string str = "" + data[0];

			for(int i = 1; i < data.Length; i++)
			{
				str += " " + data[i];
			}

			Console.Write("Read data: [" + str + "]");
		}
		else if((state & (1 << 6)) == (1 << 6))
		{
			// All errors have bit 6 set
			Console.WriteLine("Error: " + state);
		}
	}

	static void Main()
	{
		IPConnection ipcon = new IPConnection(); // Create IP connection
		BrickletNFCRFID nr = new BrickletNFCRFID(UID, ipcon); // Create device object

		ipcon.Connect(HOST, PORT); // Connect to brickd
		// Don't use device before ipcon is connected

		// Register state changed callback to function StateChangedCB
		nr.StateChangedCallback += StateChangedCB;

		// Select NFC Forum Type 2 tag
		nr.RequestTagID(BrickletNFCRFID.TAG_TYPE_TYPE2);

		Console.WriteLine("Press enter to exit");
		Console.ReadLine();
		ipcon.Disconnect();
	}
}
