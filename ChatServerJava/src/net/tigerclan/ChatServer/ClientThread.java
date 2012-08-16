package net.tigerclan.ChatServer;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.Socket;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.concurrent.ConcurrentLinkedQueue;

public class ClientThread extends Thread {
	public DateFormat dateFormat = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
	public Date date = new Date();
	Socket s;
	int id;
	public boolean running;
	//String name;
	//String message;
	OutputStream os;
	public String nickname = "";
	boolean setupDone = false;
	ConcurrentLinkedQueue<String> chats;
	public ClientThread(Socket s,int id,ConcurrentLinkedQueue<String> chats) {
		this.s = s;
		running = true;
		this.chats = chats;
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
			System.out.println("User Connected! Thread id: " + id + ". " + dateFormat.format(date));
			while (running){
					if (!s.isConnected()){//Disconnect
						running = false;
						break;
					}
					String line = is.readLine();
					String[] nick = line.split("[:]");
					nickname = nick[0];
					line = nick[1].trim();
					//name = line.substring(0, line.indexOf(':'));
					//line = line.substring(line.indexOf(':') + 1);

					//message = line.substring(line.indexOf(':') + 1);
					
					
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
					System.out.println(nickname+ ": " + line);
					}
			}
		} catch (IOException e) {
			e.printStackTrace();
			System.out.println("FAILURE ON READ: THREAD "+id+"!!");
			running = false;
		}
		//System.out.println("Client closed: "+id);
		write(nickname + " has left the server.");
	}
	public void write(String input){
		try {
			if (setupDone){
				os.write(input.getBytes());
				//os.write('\n'); //Nevermind
			}
		} catch (IOException e) {
			System.out.println("FAILURE ON WRITE: THREAD "+ id +"!!");
		}
	}
	
	public boolean isCommand(String line){
		if(line.indexOf('/') == 0){
			return true;
		}
		return false;
	}
	
	public void runCommand(String[] c){
		String command = c[0];
		System.out.println(nickname + " tried command " + command);
		if(command.equals("server") || command.equals("version"))
		{
			write("This server is running Tiger Chat v1.0");
		}else if(command.equals("me")){
			write("you tried command me!");
		
		}else if(command.equals("nick")){
			write("You can't change your nick yet");
		}else{
			write("invalid command " + command);
		}
	}
}
