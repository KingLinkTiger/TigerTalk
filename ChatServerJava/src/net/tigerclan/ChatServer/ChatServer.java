package net.tigerclan.ChatServer;

import java.io.IOException;
import java.net.*;
import java.util.Vector;
import java.util.concurrent.ConcurrentLinkedQueue;


public class ChatServer {
	static int id = 0;
	public static ConsolePrinter console = new ConsolePrinter();
	/**
	 * @param args
	 */
	public static void main(String[] args){
		ServerSocket ss;

		boolean running = true;
		Vector<ClientThread> thread_pool = new Vector<ClientThread>();
		ConcurrentLinkedQueue<String> chats = new ConcurrentLinkedQueue<String>();
		DistributorThread ds = new DistributorThread(chats, thread_pool, console);
		System.out.println("Started!");
		ds.start();
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
			thread_pool.add(new ClientThread(s, id++, chats, console));
			thread_pool.get(thread_pool.size()-1).start();
		}
		
	}

}
