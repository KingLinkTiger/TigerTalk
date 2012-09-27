package net.tigerclan.ChatServer;

import java.io.*;
import java.net.InetAddress;
import java.net.Socket;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
//import java.util.Vector;
import java.util.concurrent.ConcurrentLinkedQueue;

public class ClientThread extends Thread {
	public DateFormat dateFormat = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
	public DateFormat time = new SimpleDateFormat("HH:mm:ss");
	Socket s;
	int id;
	public boolean running;
	OutputStream os;
	BufferedReader is;
	InetAddress ip;
	public String nickname = "";
	boolean setupDone = false;
	ConcurrentLinkedQueue<String> chats;
	ChannelThread channel = null;
	boolean inRoom = false;
	public ChatServer server;
	public Command command;
	
	public ClientThread(Socket s,int id, ChatServer chatServer) {
		this.s = s;
		running = true;
		this.id = id;
		server = chatServer;
		command = server.cReader.sCommand;
	}
	
	public void run(){
		try {
			String commandline;
			String[] commandarray;
			String line;
			String send = "";
			String logger = "";
			userConnect();
			while (running){
					if (!s.isConnected()){//Disconnect
						running = false;
						break;
					}else{
					line = is.readLine();
					
					if (line != null){
						if (isCommand(line)){
							commandline = line.substring(line.indexOf('/') + 1);
							commandarray = commandline.split(" ");
							command.userRun(commandarray, this);
						} else {
							if (inRoom){
								send = timeStamp() + nickname+ ": " + line;
								logger = "(" + channel.name + ")" + send;
								chats.add(send);
								ConsolePrinter.write(logger);
							} else {
								write("Sorry you are not in a channel. Join a channel to talk.");
							}
						}
					}else{// This means someone disconnected.
						running = false;
						break;
					}
				}
			}
		} catch (IOException e) {
			e.printStackTrace();
			ConsolePrinter.write("FAILURE ON READ: THREAD "+id+"!!");
			running = false;
		}
		
		try {
			s.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		userDisconnect();

	}
	
	public void userConnect() {
		
		String line = "";
		try {
			os = s.getOutputStream();
			is =  new BufferedReader(new InputStreamReader(s.getInputStream()));
			setupDone = true;
			line = is.readLine();
			ip = s.getInetAddress();
		} catch (IOException e) {
			e.printStackTrace();
		}
		
		if (isNickSend(line)){
			line = line.substring(1);
			setNick(line, false);
		}
		ConsolePrinter.write(nickname + " connected! Thread id: " + id + ". " + dateFormat.format(new Date()) + " with an ip of " + ip.toString().substring(1) + ".");
		MOTD();
	}

	
	public void userDisconnect() {
		running = false;
		channel.dropUser(this);
		server.closeConnection(this);
		chats.add(nickname + " has left the server!");
		ConsolePrinter.write(nickname + " has disconnected. (IP: " + ip.toString().substring(1) + ")");
	}
	
	public void MOTD(){
		write("*******************************************************");
		write("Welcome! Server time is " + dateFormat.format(new Date()));
		write("*******************************************************");
		try {
			    BufferedReader motd = new BufferedReader(new FileReader("MOTD"));
			    String str;
			    while ((str = motd.readLine()) != null) {
			    	write(str);
			    }
			    motd.close();
		} catch (IOException e) {
			ConsolePrinter.write("MOTD Read Failed. Make sure you have an MOTD file if you want one.");
		}
	}
	
	public void write(String input){
		try {
			if (setupDone && input != null){
				os.write(new String(input + "\r\n").getBytes());
			}
		} catch (IOException e) {
			ConsolePrinter.write("FAILURE ON WRITE: THREAD "+ id +"!!");
			running = false;
		}
	}
	
	public boolean isCommand(String line){
		if(line.indexOf('/') == 0){
			return true;
		}
		return false;
	}
	
	public boolean isNickSend(String line){
		if(line.contains("~") && line.indexOf('~') == 0){
				return true;
		}
		return false;
	}
	
	public String timeStamp(){
		return "<" + time.format(new Date()) + "> ";
	}
	
	public void setNick(String n, boolean a) {

		if (a){
			ConsolePrinter.write(nickname + " changed their nick to " + n + ".");
			if (inRoom){
				chats.add(nickname + " is now known as " + n + ".");
				//if(channel.isOpped(nickname)){
				//	channel.deOp(nickname);
				//	channel.addOp(n);
				//}
			}
		}
	
		nickname = n;
	}
	
	public void reloadUsers(){
		channel.sendUsers();
	}
	
	public void giveChats(ConcurrentLinkedQueue<String> given){
		chats = given;
	}
}