package net.tigerclan.ChatServer;

import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;

public class ConsolePrinter {
	
	public FileWriter fstream;
	public static BufferedWriter out;
	
	public ConsolePrinter(){
			  // Create file 
			  try {
				fstream = new FileWriter("Console.log");
				out = new BufferedWriter(fstream);
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

	}
	
	public void write(String t){
		  try {
			out.write(t);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		  System.out.println(t);
	}

}
