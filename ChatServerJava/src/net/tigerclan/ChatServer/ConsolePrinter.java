package net.tigerclan.ChatServer;

import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;

public class ConsolePrinter {
	
	public ConsolePrinter(){

	}
	
	public static void main(String[] args){
		write(args[1]);
	}
	
	public static void write(String t){
		  // Create file 
		
		  try {
			FileWriter fstream;			
			fstream = new FileWriter("Console.log", true);
			BufferedWriter out = new BufferedWriter(fstream);
			out.write(t);
			out.newLine();
			out.flush();
			out.close();
		} catch (IOException e) {
			  System.out.println("Cant connect to log");
			e.printStackTrace();
		}  
		  
		System.out.println(t);
		  
		
	}

}
