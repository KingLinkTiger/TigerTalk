package net.tigerclan.ChatServer;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.net.*;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.Vector;
import java.util.concurrent.ConcurrentLinkedQueue;


public class ChatServer {
	static int id = 0;
	static int socket = 399;
	public boolean running = false;
	public Vector<ClientThread> thread_pool = new Vector<ClientThread>();
	public static Vector<ChannelThread> chan_pool = new Vector<ChannelThread>();
	public static int cid = 0;
	static InetAddress thisIp;
	public ConsoleReader cReader;
	public static ServerSocket ss;
	public ArrayList<String> GlobalOps = new ArrayList<String>();
	public ArrayList<String> GlobalVoices = new ArrayList<String>();
	public DatabaseConnect db;
	public ArrayList<Setting> settingsList = new ArrayList<Setting>();
	
	public static BufferedReader opsReader;
	public static BufferedReader voicesReader;
	public BufferedWriter opsPrinter;
	public BufferedWriter voicesPrinter;
	private boolean mysql;

	public ChatServer(){
		try {
			opsReader = new BufferedReader(new FileReader("OPS"));
		} catch (FileNotFoundException e) {
			ConsolePrinter.write("Can't Find ops file attempting to creat a new one...");
			File new1 = new File("OPS");
			try {
				new1.createNewFile();
				opsReader = new BufferedReader(new FileReader("OPS"));
			} catch (IOException e1) {
				ConsolePrinter.write("Error creating empty ops file!");
			}
		}
		try {
			voicesReader = new BufferedReader(new FileReader("VOICES"));
		} catch (FileNotFoundException e) {
			ConsolePrinter.write("Can't Find voices file attempting to creat a new one...");
			File new2 = new File("VOICES");
			try {
				new2.createNewFile();
				voicesReader = new BufferedReader(new FileReader("VOICES"));
			} catch (IOException e1) {
				ConsolePrinter.write("Error creating empty voices file");
			}
		}
		try {
			opsPrinter = new BufferedWriter(new FileWriter("OPS", true));
			voicesPrinter = new BufferedWriter(new FileWriter("Voices", true));
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

		loadAdmins();

	}

	public static void main(String[] args){
		ChatServer server = new ChatServer();
		server.runServer();
	}
	
	public void loadAdmins() {
		String str = "";		
		try {
			while ((str = opsReader.readLine()) != null)   {
				GlobalOps.add(str);
			}
			while ((str = voicesReader.readLine()) != null)   {
				GlobalVoices.add(str);
			}
		} catch (IOException e) {
			ConsolePrinter.write("Error loading saved voiced and opped users!");
		}
		
	}
	
	private void runServer() {
		loadSettings();
		mysql = getSetting("mysql").equalsIgnoreCase("TRUE");
		if(mysql){
			db = new DatabaseConnect(getSetting("mysqlserver"), getSetting("mysqlbd"), getSetting("mysqluser"), getSetting("mysqlpass"), this);
		}
		try {
			thisIp = InetAddress.getLocalHost();
		} catch (UnknownHostException e1) {
			// TODO Auto-generated catch block
			e1.printStackTrace();
		}
		DateFormat dateFormat = new SimpleDateFormat("dd/MM/yy HH:mm:ss");
		ConcurrentLinkedQueue<String> chats = new ConcurrentLinkedQueue<String>();
		DistributorThread ds = new DistributorThread(chats, thread_pool);
		ConsolePrinter.write("Started! --- Server Start Time: " + dateFormat.format(new Date()) + ". Server IP: " + thisIp.toString());
		ds.start();
		cReader = new ConsoleReader(this);
		cReader.start();
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
		
		if (mysql){
			try {
				loadSaved();
			} catch (SQLException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		
		sendVoices();
		sendOps();
		
		ClientThread user;
		while (running){
			Socket s;
			try {
				s = ss.accept();
			} catch (IOException e) {
				return;
			}
			user = new ClientThread(s, id, this);
			user.start();
			thread_pool.add(user);
			id++;
		}
	}
	
	private void loadSaved() throws SQLException {
		
		ResultSet result = db.results("SELECT * FROM `channels`");
		//int numCols = result.getMetaData().getColumnCount();
		//int i = 1;
		//int row = 1;
		int id;
		String name;
		String owner;
		String password;
		Boolean secure;
		String chanMotd;
		
		while (result.next()){
			id = result.getInt(1); 
			name = result.getString(2);
			owner = result.getString(3);
			secure = result.getBoolean(5);
			password = result.getString(6);
			chanMotd = result.getString(4);
			
			chan_pool.add(new ChannelThread(id, name, this, owner, secure, password));
			//row++;
			chan_pool.get(chan_pool.size() - 1).MOTD = chanMotd;
			
			if (id>cid) cid = id;
		}
		
		
	}

	public void changeChannel(ClientThread user, String[] c) {
		boolean moved = false;
		boolean found = false;
		for (ChannelThread Chan : chan_pool){
			if(Chan.name.equalsIgnoreCase(c[1])){
				
				found = true;
				if (Chan.secured){
					if (c.length > 2 && c[2].equalsIgnoreCase(Chan.password)){
						if(user.channel != null){
							user.channel.dropUser(user);
						}
						Chan.addUser(user);
						moved = true;
					}
					else{
						user.write("Sorry the Channel requires a password!");
					}
				}else{
					if(user.channel != null){
						user.channel.dropUser(user);
					}
					Chan.addUser(user);
					moved = true;
				}
				

			}
		}
		if(!moved && !found){
			addChan(c[1]);
			ChannelThread Chan = chan_pool.get(chan_pool.size()-1);
			if(user.channel != null){
				user.channel.dropUser(user);
			}
			Chan.addUser(user);
			moved = true;
		}
		
	}
	
	public void closeConnection(ClientThread th){
		thread_pool.remove(th);
	}
	
	private void disconAllUser() {
		for(ChannelThread ct : chan_pool){
			ct.running = false;
		}
	}

	public void kicker(String user) {
		for (ClientThread th : thread_pool){
			if (th.nickname.equals(user)){
				ConsolePrinter.write(user +" was Kicked");
				th.write("You were kicked!");
				th.userDisconnect();
				break;
			}
		}
		
	}

	public void loadSettings(){
		try {
			BufferedReader settingsRead = new BufferedReader(new FileReader("server.config"));
			String str;
			Setting s;
		    while ((str = settingsRead.readLine()) != null) {
		    	if(str.indexOf("=") > 0){
		    		 s = new Setting(str.substring(0,str.indexOf("=")), str.substring(str.indexOf("=") + 1));
		    		 settingsList.add(s);
		    	}
		    }
			settingsRead.close();
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	
	public String getSetting(String setting){
		String result = null;
		for(Setting st : settingsList){
			if(st.name.equalsIgnoreCase(setting)){
				result = st.value; 
			}
		}
		if(result != null){
			return result;
		} else {
			return "default";
		}
	}

	public void saveGlobalAdmins(){
		
		try {
			for (String admin : GlobalOps){
			
			opsPrinter.write(admin);
			opsPrinter.newLine();
			}
			
			opsPrinter.flush();
			opsPrinter.close();
			
			for (String admin : GlobalVoices){
				
			voicesPrinter.write(admin);
			voicesPrinter.newLine();
			}
			
			voicesPrinter.flush();
			voicesPrinter.close();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	public void addChan (String cName){
		chan_pool.add(new ChannelThread(cid, cName, this));
		cid++;
		sendOpsChannel(chan_pool.get(chan_pool.size() - 1));
	}

	public void sendOps() {
		for(ChannelThread ct : chan_pool){
			for(String op : GlobalOps){
				ct.addOp(op);
			}
		}
		
	}
	
	public void sendOpsChannel(ChannelThread chan){
		for(String op : GlobalOps){
			chan.addOp(op);
		}
		for(String v : GlobalVoices){
			chan.addVoice(v);
		}
	}

	public void sendVoices() {
		for(ChannelThread ct : chan_pool){
			for(String v : GlobalVoices){
				ct.addVoice(v);
			}
		}
		
	}
	
	public void stop() {
		ConsolePrinter.write("Stopping Server...");
		ConsolePrinter.write("Closing Connections...");
		disconAllUser();

		ConsolePrinter.write("Closing Channels...");
		stopChannels();
		ConsolePrinter.write("Saving Admins...");
		saveGlobalAdmins();
		
		try {
			ss.close();
		} catch (IOException e) {
			e.printStackTrace();
		}
		ConsolePrinter.write("Server Stopped");
		System.exit(0);
	}
	
	public void stopChannels(){
		for(ChannelThread ct : chan_pool){
			ct.close();
		}
		
	}

	public void nickChange(String newName, String oldName) {
		boolean opped = false;
		boolean voiced = false;
		for(String o : GlobalOps){
			if (o == oldName){
				GlobalOps.remove(o);
				GlobalOps.add(newName);
				opped = true;
			}
		}
		for(String o : GlobalOps){
			if (o == oldName){
				GlobalVoices.remove(o);
				GlobalVoices.add(newName);
				voiced = true;
			}
		}
		if(opped){
			for(ChannelThread ct : chan_pool){
				if(ct.isOpped(oldName))ct.deOp(oldName);
				ct.addOp(newName);
			}
		}
		if(voiced){
			sendVoices();
			for(ChannelThread ct : chan_pool){
				if(ct.isVoiced(oldName))ct.deVoice(oldName);
				ct.addVoice(newName);
			}
		}
	}

	public boolean isReg(String n) {
		if (mysql){
			ResultSet result = db.results("SELECT * FROM  `Users` WHERE  `user` LIKE  '" + n +"'");
			int num = 0;
			try {
				num = result.getMetaData().getColumnCount();
			} catch (SQLException e) {
				ConsolePrinter.write("** WARNIGN ** Cannot connect to sql to complete Authentication query! Bypassing authentication!!");
			}
			if( num == 1){
				return true;
			}else if (num > 1){
				return true;
			}else{
				return false;
			}
		}else{
			return false;
		}

	}

	public boolean checkPass(String string, String p) {
		if(mysql){
				ResultSet result = db.results("SELECT * FROM  `Users` WHERE  `user` LIKE  '" + string +"'");
				int num = 0;
				try {
					num = result.getMetaData().getColumnCount();
				} catch (SQLException e) {
					ConsolePrinter.write("** WARNIGN ** Cannot connect to sql to complete Authentication query! Bypassing authentication!!");
				}
				String password;
				try {
					while(result.next()){
						
						System.out.println(result.getString(2));
						password = result.getString(4);
			
						if(password.equals(DemoMD5.MD5(p))){
							return true;
						}
					}
				} catch (SQLException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (NoSuchAlgorithmException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				} catch (UnsupportedEncodingException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
		return false;

		}else{
			return false;
		}
	}
	
	public String listChans(){
		
		String list = "";
		
		for (ChannelThread ct : chan_pool){
			
			list = list + ct.name + ": " + ct.MOTD + "\r\n";
			
		}
		
		return list;
	}
}
