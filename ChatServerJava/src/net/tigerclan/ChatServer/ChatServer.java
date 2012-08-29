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
	public static Vector<ChannelThread> chan_pool = new Vector<ChannelThread>();
	public static int cid = 0;
	static InetAddress thisIp;
	/**
	 * @param args
	 */
	public static void main(String[] args){
		try {
			thisIp = InetAddress.getLocalHost();
		} catch (UnknownHostException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}
		ServerSocket ss;
		DateFormat dateFormat = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
		ConcurrentLinkedQueue<String> chats = new ConcurrentLinkedQueue<String>();
		DistributorThread ds = new DistributorThread(chats, thread_pool);
		ConsolePrinter.write("Started! --- Server Start Time: " + dateFormat.format(new Date()) + ". Server IP: " + thisIp.toString());
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
		addChan("Lobby");
		ChannelThread lobby = chan_pool.get(chan_pool.size()-1);
		ClientThread user;
		while (running){
			Socket s;
			try {
				s = ss.accept();
			} catch (IOException e) {
				return;
			}
			user = new ClientThread(s, id);
			user.start();
//			lobby.addUser(user);
			thread_pool.add(user);
//			thread_pool.get(thread_pool.size()-1).start();
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
				ConsolePrinter.write(user +" was Kicked");
				th.write("You were kicked!");
				th.userDisconnect();
				break;
			}
		}
		
	}

	public static Vector<ClientThread> getPool() {
		return thread_pool;
	}
	
	public static void addChan (String cName){
		chan_pool.add(new ChannelThread(cid, cName));
		cid++;
	}

	public static void changeChannel(ClientThread user, String[] c) {
		boolean moved = false;
		for (ChannelThread Chan : chan_pool){
			if(Chan.name.equalsIgnoreCase(c[1])){
				if(user.channel != null){
					user.channel.dropUser(user);
				}
				Chan.addUser(user);
				moved = true;
			}
		}
		if(!moved){
			addChan(c[1]);
			ChannelThread Chan = chan_pool.get(chan_pool.size()-1);
			if(user.channel != null){
				user.channel.dropUser(user);
			}
			Chan.addUser(user);
			moved = true;
		}
		
	}
}
