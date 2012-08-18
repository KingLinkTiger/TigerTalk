package net.tigerclan.ChatServer;

import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Vector;
import java.util.concurrent.ConcurrentLinkedQueue;

public class DistributorThread extends Thread {
	static ConcurrentLinkedQueue<String> chats;
	static Vector<ClientThread> thread_pool;
	public static DateFormat time = new SimpleDateFormat("HH:mm:ss");
	boolean running = true;
	@SuppressWarnings("static-access")
	public DistributorThread(ConcurrentLinkedQueue<String> chats, Vector<ClientThread> thread_pool){
		this.chats = chats;
		this.thread_pool = thread_pool;
	}
	
	public void run(){
		String chat;
		while (running){
			if (chats.peek() != null && thread_pool.size() > 0){
				chat = chats.poll();
				for (ClientThread th : thread_pool){		//<< where disconnect and kick fail
					if (th.running == false){//Needs to be cleaned up
						thread_pool.remove(th);
					}else{
					th.write(chat);
					}
				}
			}else{// Nothing to do
				Thread.yield();
			}
			
		}
	}
	
	public static void sendUsers() {
		String users = "~";
		boolean first = true;
		for (ClientThread th : thread_pool){
			if (!first) {
				users = users + ",";
			}
			users = users + th.nickname;
			first = false;
		}
			chats.add(users);
	}
	
	public static void connectUsers(ClientThread ct){
		String users = "~";
		boolean first = true;
		for (ClientThread th : thread_pool){
			if (!first) {
				users = users + ",";
			}
			users = users + th.nickname;
			first = false;
		}
			ct.write(users);
	}
	
	public static String timeStamp(){
		return "<" + time.format(new Date()) + "> ";
	}

	public static void sendMessage(String[] c, ClientThread sender) {
		
			if (c.length > 2){
				String toUser = c[1];
				
				String start = ">> From " + sender.nickname + ": ";
				String msg = "";
				boolean first = true;
				boolean sent = false;
				for ( int loop = 2; loop < c.length; loop++){
					msg = msg + c[loop] + " ";
				}
				for (ClientThread th : thread_pool){
					if (th.nickname.equals(toUser)){
						th.write(timeStamp() +  start + msg);
						sender.write(timeStamp() + ">> To " + toUser + ": " + msg);
						sent = true;
						break;
					}
				}
					if (!sent){
						sender.write("The nick you entered is invalid");
						ConsolePrinter.write("/msg failed - bad nick");
					} else {
						ConsolePrinter.write(timeStamp() + ">> FROM " + sender.nickname + " TO " + toUser + ": " + msg);
					}
			} else {
				ConsolePrinter.write(time.format(new Date()) + " To few arguments.");
			}
			
		
		
	}

	public static void kicker(String user) {
		
		for (ClientThread th : thread_pool){
			if (th.nickname.equals(user)){
				ConsolePrinter.write("User Kicked");
				th.userDisconnect();
				break;
			}
		}
		
	}
	
	

}
