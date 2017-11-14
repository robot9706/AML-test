using ASLTest.AML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASLTest
{
	class Program
	{
		static FileStream _sdt;
		static BinaryReader _reader;

		static uint _sdtLength;

		static void Main(string[] args)
		{
			//_sdt = File.OpenRead("DSDT.bin");
			_sdt = File.OpenRead(@"test.aml");
			_reader = new BinaryReader(_sdt);

			//STUFF
			{
				ReadHeader();

				ACPILib.Parser.AMLOp root = new ACPILib.Parser.Parser(_sdt).Parse();
			}

			_sdt.Close();
		}

		static void ReadHeader()
		{
			Console.WriteLine("SDT header:");

			//Signature
			Console.WriteLine("\tSignature: " + Encoding.ASCII.GetString(_reader.ReadBytes(4)));

			//Length
			_sdtLength = _reader.ReadUInt32();
			Console.WriteLine("\tLendth: " + _sdtLength.ToString() + " / " + _sdtLength.ToString("X2"));

			//Revision
			Console.WriteLine("\tRevision: " + _reader.ReadByte().ToString());

			//Checksum
			Console.WriteLine("\tChecksum: " + _reader.ReadByte().ToString());

			//OEM ID
			Console.WriteLine("\tOEM ID: " + Encoding.ASCII.GetString(_reader.ReadBytes(6)));

			//OEMTableID
			Console.WriteLine("\tOEMTableID: " + Encoding.ASCII.GetString(_reader.ReadBytes(8)));

			//OEMRevision
			Console.WriteLine("\tOEMRevision: " + _reader.ReadUInt32().ToString());

			//OEMRevision
			Console.WriteLine("\tCreatorID: " + _reader.ReadUInt32().ToString());

			//OEMRevision
			Console.WriteLine("\tCreatorRevision: " + _reader.ReadUInt32().ToString());
		}
	}
}
