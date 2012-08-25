package net.tigerclan.ChatServer;

import java.io.IOException;
import java.net.*;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Vector;
import java.util.concurrent.ConcurrentLinkedQueue;


public class ChatServer {
	static int id = 0;
	static int socket = 399;
	public static boolean running = false;
	public static Vector<ClientThread> thread_pool = new Vector<ClientThread>();
	/**
	 * @param args
	 */
	public static void main(String[] args){
		ServerSocket ss;
		DateFormat dateFormat = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
		ConcurrentLinkedQueue<String> chats = new ConcurrentLinkedQueue<String>();
		DistributorThread ds = new DistributorThread(chats, thread_pool);
		ConsolePrinter.write("Started! --- Server Start Time: " + dateFormat.format(new Date()));
		ds.start();
		try {
			ss = new ServerSocket(socket);
			running = true;
		} catch (IOException e) {
			ConsolePrinter.write("Could not bind to Socket!");
			ConsolePrinter.write("Startup failed.");
			System.exit(1);
			return;
		}
		if(running){
			ConsolePrinter.write("Server Running");			
		}

		while (running){
			Socket s;
			try {
				s = ss.accept();
			} catch (IOException e) {
				return;
			}
			thread_pool.add(new ClientThread(s, id, chats));
			thread_pool.get(thread_pool.size()-1).start();
			id++;
		}
		try {
			ss.close();
		} catch (IOException e) {
			e.printStackTrace();
		}

	}
	
	public static void loadSettings(){
		
	}
	
	public static void closeConnection(ClientThread th){
		thread_pool.remove(th);
	}

	public static void kicker(String user) {
		for (ClientThread th : thread_pool){
			if (th.nickname.equals(user)){
				ConsolePrinter.write("User Kicked");
				th.write("You were kicked!");
				th.userDisconnect();
				break;
			}
		}
		
	}

	public static Vector<ClientThread> getPool() {
		return thread_pool;
	}

}
