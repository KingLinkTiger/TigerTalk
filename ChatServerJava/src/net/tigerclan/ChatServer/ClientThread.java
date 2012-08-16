package net.tigerclan.ChatServer;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.Socket;
import java.util.concurrent.ConcurrentLinkedQueue;

public class ClientThread extends Thread {
	Socket s;
	int id;
	boolean running;
	String name;
	OutputStream os;
	boolean setupDone = false;
	ConcurrentLinkedQueue<String> chats;
	public ClientThread(Socket s,int id,ConcurrentLinkedQueue<String> chats) {
		this.s = s;
		running = true;
		this.id = id;
		this.chats = chats;
	}
	
	public void run(){
		BufferedReader is;
		try {
			os = s.getOutputStream();
			is =  new BufferedReader(new InputStreamReader(s.getInputStream()));
			setupDone = true;
			while (running){
					if (!s.isConnected()){//Disconnect
						running = false;
						break;
					}
					String line = is.readLine();
					chats.add(line);
					System.out.println(line);
			}
		} catch (IOException e) {
			e.printStackTrace();
			System.out.println("FAILURE ON THREAD "+id+"!!");
			running = false;
		}
		System.out.println("Client closed: "+id);
	}
	public void write(String input){
		try {
			if (setupDone) os.write(input.getBytes());
		} catch (IOException e) {
			System.out.println("FAILURE ON THREAD "+id+"!!");
		}
	}
}
