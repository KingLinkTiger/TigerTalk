package net.tigerclan.ChatServer;

import java.io.*;
import java.net.Socket;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.concurrent.ConcurrentLinkedQueue;

public class ClientThread extends Thread {
	public DateFormat dateFormat = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
	public Date date = new Date();
	public ConsolePrinter console;
	Socket s;
	int id;
	public boolean running;
	//String name;
	//String message;
	OutputStream os;
	public String nickname = "";
	boolean setupDone = false;
	ConcurrentLinkedQueue<String> chats;
	public ClientThread(Socket s,int id,ConcurrentLinkedQueue<String> chats, ConsolePrinter console) {
		this.s = s;
		running = true;
		this.chats = chats;
		this.console = console;
	}
	
	public void run(){
		BufferedReader is;

		try {
			os = s.getOutputStream();
			os.write(new String("Welcome! Server time is " + dateFormat.format(date)).getBytes());
			is =  new BufferedReader(new InputStreamReader(s.getInputStream()));
			setupDone = true;
			String commandline;
			String[] commandarray;
			chats.add(nickname + " has joined the server!");
			console.write("User Connected! Thread id: " + id + ". " + dateFormat.format(date));
			
			String line = is.readLine();
			if (isSeverCommand(line)){
				commandline = line.substring(1);
				commandarray = commandline.split(" ");
				ServerCommand(commandarray);
			}
			chats.add(nickname + " has joined the server!\r\n");//<<<<<Righthere
			DistributorThread.sendUsers(this);
			while (running){
					if (!s.isConnected()){//Disconnect
						running = false;
						break;
					}
<<<<<<< HEAD
					line = is.readLine();
					
=======
					String line = is.readLine();

>>>>>>> 56881f90656a0db50f3a07b2ef641ae5e8aebd76
					if (isSeverCommand(line)){
						commandline = line.substring(1);
						commandarray = commandline.split(" ");
						ServerCommand(commandarray);
<<<<<<< HEAD
					}else{					
=======
						
					}else{					
					
>>>>>>> 56881f90656a0db50f3a07b2ef641ae5e8aebd76
						if (isCommand(line)){
							commandline = line.substring(line.indexOf('/') + 1);
							commandarray = commandline.split(" ");
							runCommand(commandarray);
						} else {
							if (line != null){
								chats.add(nickname+ ": " + line);
							}else{// This means someone disconnected.
								running = false;
								break;
							}
							console.write(nickname+ ": " + line);
					}
					}
			}
		} catch (IOException e) {
			e.printStackTrace();
			console.write("FAILURE ON READ: THREAD "+id+"!!");
			running = false;
		}
		//System.out.println("Client closed: "+id);
		chats.add(nickname + " has left the server!");
	}

	public void write(String input){
		try {
			if (setupDone){
				os.write(input.getBytes());
				//os.write('\n'); //Nevermind
			}
		} catch (IOException e) {
			console.write("FAILURE ON WRITE: THREAD "+ id +"!!");
		}
	}
	
	public boolean isCommand(String line){
		if(line.indexOf('/') == 0){
			return true;
		}
		return false;
	}
	
	public boolean isSeverCommand(String line){
		if(line.indexOf('~') == 0){
			return true;
		}
		return false;
	}
	
	private void ServerCommand(String[] c) {
		console.write(nickname + " user changed nick to " + c[0]);
		nickname = c[0];
<<<<<<< HEAD
		write("nick changed to " + nickname +"\r\n");
=======
		write("nick changed to " + nickname);
>>>>>>> 56881f90656a0db50f3a07b2ef641ae5e8aebd76

	}
	
	public void runCommand(String[] c){
		String command = c[0];
		console.write(nickname + " tried command " + command);
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
		}else if(command.equals("nick")){			
			console.write(nickname + " user changed nick to " + c[1]);
			nickname = c[1];
			write("nick changed to " + nickname + "\r\n");
		}else if (command.equals("help")){
			write("Commands are: version, me, nick, and help.");
		}else{
			write("/" + command + " is not a valid command.");
		}
	}

}
