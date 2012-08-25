package net.tigerclan.ChatServer;

import java.io.*;
import java.net.InetAddress;
import java.net.Socket;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Vector;
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
	
	public ClientThread(Socket s,int id,ConcurrentLinkedQueue<String> chats) {
		this.s = s;
		running = true;
		this.chats = chats;
		this.id = id;
	}
	
	public void run(){
		try {
			String commandline;
			String[] commandarray;
			String line;
			String send = "";
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
							runCommand(commandarray);
						} else {
							send = timeStamp() + nickname+ ": " + line;
							chats.add(send);
							ConsolePrinter.write(send);
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
		
		userDisconnect();

	}
	
	public void userConnect() {
		
		String line = "";
		try {
			os = s.getOutputStream();
			os.write(new String("\r\n*******************************************************\r\n").getBytes());
			os.write(new String("Welcome! Server time is " + dateFormat.format(new Date()) + "\r\n").getBytes());
			os.write(new String("*******************************************************\r\n").getBytes());
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
		chats.add(nickname + " has joined the server!");
		DistributorThread.sendUsers();
	}

	
	public void userDisconnect() {
		running = false;
		ChatServer.closeConnection(this);
		chats.add(nickname + " has left the server!");
		ConsolePrinter.write(nickname + " has disconnected. (IP: " + ip.toString().substring(1) + ")");
	}
	public void MOTD(){
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
			if (setupDone){
				os.write((input + "\r\n").getBytes());
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
	
	private void setNick(String n, boolean a) {
		if (a){
			ConsolePrinter.write(nickname + " changed their nick to " + n + ".");
			chats.add(nickname + " is now know as " + n + ".");
		}
			nickname = n;
	}
	
	public void reloadUsers(){
		DistributorThread.sendUsers();
	}
	
	public void runCommand(String[] c){
		String command = c[0];
		ConsolePrinter.write(nickname + " tried command " + command);
		if(command.equals("server") || command.equals("version"))
		{
			write("This server is running Tiger Chat v1.0");
		}else if(command.equals("me")){
			//write("you tried command me!");
			String message = "";
			int loop = 0;
			for(String arg : c){
				if (loop == 0) {
					
				} else {
					message = message + arg + " ";
				}
				loop++;
			}
			chats.add("**" + nickname + " " + message);
		}else if(command.equalsIgnoreCase("nick")){			
			setNick(c[1], true);
		}else if(command.equalsIgnoreCase("kick")){
			ChatServer.kicker(c[1]);
			//write("Sorry that would crash the server!");
		}else if (command.equalsIgnoreCase("help") || command.equalsIgnoreCase("?") || command.equalsIgnoreCase("h")){
			write("Commands are: version, me, nick, message, disconnect, and help.");
		}else if (command.equalsIgnoreCase("msg") || command.equalsIgnoreCase("message") || command.equalsIgnoreCase("m")){
			DistributorThread.sendMessage(c, this);
		}else if (command.equalsIgnoreCase("users") || command.equalsIgnoreCase("user") || command.equalsIgnoreCase("u")){
			reloadUsers();
		}else if (command.equalsIgnoreCase("test")){
			write("Testing!");
		}else if (command.equalsIgnoreCase("discon") || command.equalsIgnoreCase("dc") || command.equalsIgnoreCase("disconnect")){
			userDisconnect();
			//write("Sorry that would crash the server!");
		}else if (command.equalsIgnoreCase("ping")){
			if(c.length >= 2)
			{
				write("Attempting Your Ping...");
				ping(c[1]);
			} else {
				write("No target specified for ping!");
			}
		}else{
			write("/" + command + " is not a valid command.");
		}
	}

	private void ping(String target) {
		Vector<ClientThread> thread_pool = ChatServer.getPool();
		boolean sent = false;
		for(ClientThread ct : thread_pool){
			if (ct.nickname.equals(target)){
				Process p;
				String[] result = new String[3];
				int i = 0;
				try {
					p = Runtime.getRuntime().exec("cmd /c ping " + ct.ip.toString().substring(1) + " -n 3");
				try {
					p.waitFor();
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} 
				BufferedReader reader=new BufferedReader(new InputStreamReader(p.getInputStream())); 
				String line=reader.readLine(); 
				while(line!=null) 
				{ 
					if(line.length() > 0){
						if(line.substring(0, 3) == "Reply" && i < 3){
							result[i] = line;
							i++;
						}
					}
				line=reader.readLine(); 
				} 
				} catch (IOException e) {
					write("Can't Run ping command");
					e.printStackTrace();
				} 
				write(result.length + "");
				if (i == 2){
					int num = 0;
					int[] pings = new int[3];
					for (String s : result){
						write(s);
						s = s.substring(s.indexOf("time"));	
						pings[num] = Integer.parseInt(s);
					}
					int average = (pings[1] + pings[2] + pings[3]) / 3;
						write("ping: " + average);
				} else {
					write("Your ping failed!");
					return;
				}
				sent = true;
			}
			if (!sent){
				write("Can't find target!");
				return;
			}
		}
	}

}