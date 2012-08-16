package net.tigerclan.ChatServer;

import java.io.IOException;
import java.net.*;
import java.util.Vector;


public class ChatServer {
	/**
	 * @param args
	 */
	public static void main(String[] args){
		ServerSocket ss;
		boolean running = true;
		Vector<ClientThread> thread_pool;
		System.out.println("Start!");
		try {
			ss = new ServerSocket(399);
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			System.err.println("FAIL!");
			return;
		}
		while (running){
			Socket s;
			try {
				s = ss.accept();
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
				return;
			}
			thread_pool.add(new ClientThread(s));
		}
	}

}
