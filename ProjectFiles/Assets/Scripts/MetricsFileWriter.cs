using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;

public class MetricsFileWriter : MonoBehaviour
{
	private string filedir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
	public string filename = "";

	private List<List<int>> deaths = new List<List<int>>();

	// adds a floor to deaths
	private void addFloor() {
		deaths.Add(new List<int>());
	}

	// adds a room to given floor to deaths (inputs should be 1-indexed)
	private void addRoom(int floor) {
		if (floor < 1)
			return;
		deaths[floor - 1].Add(0);
	}

	// adds a death to the currect room of the floor (inputs should be 1-indexed)
	public void addDeath(int floor, int room) {
		if (floor < 1 || room < 1)
			return;
		// add rooms until there are enough to meet targetted rrom
		while (floor > deaths.Count)
			addFloor();
		while (room > deaths[floor - 1].Count)
			addRoom(floor);
		// add death to room
		deaths[floor - 1][room - 1]++;
		writeDeath();
	}

	// when object is destroyed write deaths dynamic array to file
	private void writeDeath() {
		if (filename == "" || filename == null)
			return;
		StreamWriter file_stream = null;

		if (!File.Exists(filedir + filename))
			file_stream = new StreamWriter(File.Create(filedir + filename));
		else
			file_stream = new StreamWriter(filedir + filename);

		for (int i = 0; i < deaths.Count; i++) {
			for (int j = 0; j < deaths[i].Count; j++) {
				file_stream.WriteLine("Floor " + (i + 1) + ", Room: " + (j + 1) + " : ");
				//file_stream.WriteLine("\tDeaths: " + (deaths[i][j] / 2) + "\n");
				file_stream.WriteLine("\tDeaths: " + deaths[i][j] + "\n"); // DIFFERENT LINES BECAUSE OF THE DYING-TWICE BUG
			}
		}

		file_stream.Close();
	}
}
