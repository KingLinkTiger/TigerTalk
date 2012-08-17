package net.tigerclan.ChatServer;

import java.util.Vector;
import java.util.concurrent.ConcurrentLinkedQueue;

public class DistributorThread extends Thread {
	ConcurrentLinkedQueue<String> chats;
	Vector<ClientThread> thread_pool;
	public ConsolePrinter console;
	boolean running = true;
	public DistributorThread(ConcurrentLinkedQueue<String> chats,Vector<ClientThread> thread_pool, ConsolePrinter console){
		this.chats = chats;
		this.thread_pool = thread_pool;
		this.console = console;
	}
	public void run(){
		while (running){
			if (chats.peek() != null){
				String chat = chats.poll();
				for (ClientThread th : thread_pool){
					if (th.running == false){//Needs to be cleaned up
						thread_pool.remove(th);
					}
				}
				for (ClientThread th : thread_pool){
					th.write(chat);
				}
			}else{// Nothing to do
				Thread.yield();
			}
			
		}
	}
}
