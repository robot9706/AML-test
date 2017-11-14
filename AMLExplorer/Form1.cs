using ACPILib.Parser;
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
			using (Stream s = File.OpenRead(@"test.aml"))
			{
				s.Seek(36, SeekOrigin.Begin); //Skip header

				AMLOp root = new Parser(s).Parse();

				TreeNode rootNode = new TreeNode("Root");
				treeView1.Nodes.Add(rootNode);

				PopulateNode(root, rootNode);
			}
		}

		private void PopulateNode(AMLOp op, TreeNode root)
		{
			if (!string.IsNullOrEmpty(op.Name))
				root.Text += " (" + op.Name + ")";

			if (op.OpCode != null)
			{
				root.Nodes.Add("OpCode = " + op.OpCode.ToString());
				root.Nodes.Add("Start = " + op.Start.ToString());
				root.Nodes.Add("Length = " + op.Length.ToString());
				root.Nodes.Add("End = " + op.End.ToString());
				root.Nodes.Add("Value = " + ValueToString(op.Value));
			}

			if (op.Nodes.Count > 0)
			{
				TreeNode nodes = new TreeNode("Nodes");
				root.Nodes.Add(nodes);

				foreach (AMLOp ch in op.Nodes)
				{
					TreeNode newNode = new TreeNode((ch.OpCode != null && !string.IsNullOrEmpty(ch.OpCode.Name)) ? ch.OpCode.Name : "Node");
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

			return val.ToString();
		}
	}
}
