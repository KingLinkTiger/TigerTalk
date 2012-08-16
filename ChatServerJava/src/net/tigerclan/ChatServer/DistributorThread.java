package net.tigerclan.ChatServer;

import java.util.Vector;
import java.util.concurrent.ConcurrentLinkedQueue;

public class DistributorThread extends Thread {
	ConcurrentLinkedQueue<String> chats;
	Vector<ClientThread> thread_pool;
	boolean running = true;
	public DistributorThread(ConcurrentLinkedQueue<String> chats,Vector<ClientThread> thread_pool){
		this.chats = chats;
		this.thread_pool = thread_pool;
	}
	public void run(){
		while (running){
			if (chats.peek() != null){
				String chat = chats.poll();
				for (ClientThread th : thread_pool){
					th.write(chat);
				}
			}else{// Nothing to do
				Thread.yield();
			}
			
		}
	}
}
