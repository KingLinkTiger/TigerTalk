package net.tigerclan.ChatServer;

import java.net.Socket;

public class ClientThread extends Thread {
	Socket s;
	public ClientThread(Socket s) {
		// Do something
		this.s = s;
	}

}
