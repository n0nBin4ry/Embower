using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;

public class HugWriter : MonoBehaviour
{
	private string filedir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
	public string filename = "";
	int hugs = 0;

	public void addHug() {
		hugs++;
	}

	public void writeHug() {
		if (filename == "" || filename == null)
			return;

		StreamWriter file_stream = null;

		if (!File.Exists(filedir + filename))
			file_stream = new StreamWriter(File.Create(filedir + filename));
		else
			file_stream = new StreamWriter(filedir + filename);

		file_stream.Write("Hugs: " + hugs);

		file_stream.Close();
	}
}
