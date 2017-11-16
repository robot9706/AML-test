using ACPILib.AML;
using ACPILib.Parser2;
using System;
using System.IO;
using System.Windows.Forms;

namespace AMLExplorer
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//using (Stream s = File.OpenRead(@"DSDTs\iASL_example.dsdt"))
			//using (Stream s = File.OpenRead(@"DSDTs\Test_pc.dsdt"))
			using (Stream s = File.OpenRead(@"DSDTs\QEMU.dsdt"))
			{
				s.Seek(36, SeekOrigin.Begin); //Skip header

				//AMLOp root = new Parser(s).Parse();
				ParseNode root = new Parser(s).Parse();

				TreeNode rootNode = new TreeNode("Root");
				treeView1.Nodes.Add(rootNode);

				PopulateNode(root, rootNode);
			}
		}

		private void PopulateNode(ParseNode op, TreeNode root)
		{
			if (!string.IsNullOrEmpty(op.Name))
				root.Text += " (" + op.Name + ")";

			if (op.Op != null)
			{
				root.Nodes.Add("OpCode = " + op.Op.ToString());
				root.Nodes.Add("Start = " + op.Start.ToString());
				root.Nodes.Add("Length = " + op.Length.ToString());
				root.Nodes.Add("End = " + op.End.ToString());
				if (op.ConstantValue != null)
				{
					root.Nodes.Add("Value = " + ValueToString(op.ConstantValue));
				}
			}

			if (op.Arguments.Count > 0)
			{
				TreeNode nodes = new TreeNode("Arguments");
				root.Nodes.Add(nodes);

				for (int x = 0; x < op.Op.ParseArgs.Length; x++)
				{
					if (op.Op.ParseArgs[x] == ParseArgFlags.DataObjectList || op.Op.ParseArgs[x] == ParseArgFlags.TermList || op.Op.ParseArgs[x] == ParseArgFlags.ObjectList)
						continue;

					TreeNode newNode = new TreeNode(ValueToString(op.Arguments[x]) + " (" + op.Op.ParseArgs[x].ToString() + ")");
					nodes.Nodes.Add(newNode);
				}
			}

			if (op.Nodes.Count > 0)
			{
				TreeNode nodes = new TreeNode("Nodes");
				root.Nodes.Add(nodes);

				foreach (ParseNode ch in op.Nodes)
				{
					TreeNode newNode = new TreeNode((ch.Op != null && !string.IsNullOrEmpty(ch.Op.Name)) ? ch.Op.Name : "Node");
					nodes.Nodes.Add(newNode);

					PopulateNode(ch, newNode);
				}
			}
		}

		private string ValueToString(object val)
		{
			if (val == null)
				return "null";

			if (val is string)
				return "\"" + val.ToString() + "\"";

			if (val is byte)
				return "0x" + ((byte)val).ToString("X2");

			if (val.GetType().IsArray)
			{
				Array ar = (Array)val;

				string rt = "";

				for (int x = 0; x < ar.Length; x++)
					rt += ValueToString(ar.GetValue(x)) + (x < ar.Length - 1 ? ", " : string.Empty);

				return rt;
			}

			if (val is ParseNode)
			{
				ParseNode node = (ParseNode)val;

				if (node.ConstantValue != null)
					return ValueToString(node.ConstantValue);
			}

			return val.ToString();
		}
	}
}
