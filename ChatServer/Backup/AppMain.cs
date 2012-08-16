using System;
using System.Threading;								// Sleeping
using System.Net;									// Used to local machine info
using System.Net.Sockets;							// Socket namespace
using System.Collections;							// Access to the Array list

namespace ChatServer
{
	/// <summary>
	/// Main class from which all objects are created
	/// </summary>
	class AppMain
	{
		// Attributes
		private ArrayList	m_aryClients = new ArrayList();	// List of Client Connections
		/// <summary>
		/// Application starts here. Create an instance of this class and use it
		/// as the main object.
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			AppMain app = new AppMain();
			// Welcome and Start listening
			Console.WriteLine( "*** Chat Server Started {0} *** ", DateTime.Now.ToString( "G" ) );


			/*
			//
			// Method 1
			//
			Socket client;
			const int nPortListen = 399;
			try
			{
				TcpListener listener = new TcpListener( nPortListen );
				Console.WriteLine( "Listening as {0}", listener.LocalEndpoint );
				listener.Start();
				do
				{
					byte [] m_byBuff = new byte[127];
					if( listener.Pending() )
					{
						client = listener.AcceptSocket();
						// Get current date and time.
						DateTime now = DateTime.Now;
						String strDateLine = "Welcome " + now.ToString("G") + "\n\r";

						// Convert to byte array and send.
						Byte[] byteDateLine = System.Text.Encoding.ASCII.GetBytes( strDateLine.ToCharArray() );
						client.Send( byteDateLine, byteDateLine.Length, 0 );
					}
					else
					{
						Thread.Sleep( 100 );
					}
				} while( true );	// Don't use this. 

				//Console.WriteLine ("OK that does it! Screw you guys I'm going home..." );
				//listener.Stop();
			}
			catch( Exception ex )
			{
				Console.WriteLine ( ex.Message );
			}
			*/


			//
			// Method 2 
			//
			const int nPortListen = 399;
			// Determine the IPAddress of this machine
			IPAddress [] aryLocalAddr = null;
			String strHostName = "";
			try
			{
				// NOTE: DNS lookups are nice and all but quite time consuming.
				strHostName = Dns.GetHostName();
				IPHostEntry ipEntry = Dns.GetHostByName( strHostName );
				aryLocalAddr = ipEntry.AddressList;
			}
			catch( Exception ex )
			{
				Console.WriteLine ("Error trying to get local address {0} ", ex.Message );
			}
	
			// Verify we got an IP address. Tell the user if we did
			if( aryLocalAddr == null || aryLocalAddr.Length < 1 )
			{
				Console.WriteLine( "Unable to get local address" );
				return;
			}
			Console.WriteLine( "Listening on : [{0}] {1}:{2}", strHostName, aryLocalAddr[0], nPortListen );

			// Create the listener socket in this machines IP address
			Socket listener = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
			listener.Bind( new IPEndPoint( aryLocalAddr[0], 399 ) );
			//listener.Bind( new IPEndPoint( IPAddress.Loopback, 399 ) );	// For use with localhost 127.0.0.1
			listener.Listen( 10 );

			// Setup a callback to be notified of connection requests
			listener.BeginAccept( new AsyncCallback( app.OnConnectRequest ), listener );

			Console.WriteLine ("Press Enter to exit" );
			Console.ReadLine();
			Console.WriteLine ("OK that does it! Screw you guys I'm going home..." );

			// Clean up before we go home
			listener.Close();
			GC.Collect();
			GC.WaitForPendingFinalizers();		
		}


		/// <summary>
		/// Callback used when a client requests a connection. 
		/// Accpet the connection, adding it to our list and setup to 
		/// accept more connections.
		/// </summary>
		/// <param name="ar"></param>
		public void OnConnectRequest( IAsyncResult ar )
		{
			Socket listener = (Socket)ar.AsyncState;
			NewConnection( listener.EndAccept( ar ) );
			listener.BeginAccept( new AsyncCallback( OnConnectRequest ), listener );
		}

		/// <summary>
		/// Add the given connection to our list of clients
		/// Note we have a new friend
		/// Send a welcome to the new client
		/// Setup a callback to recieve data
		/// </summary>
		/// <param name="sockClient">Connection to keep</param>
		//public void NewConnection( TcpListener listener )
		public void NewConnection( Socket sockClient )
		{
			// Program blocks on Accept() until a client connects.
			//SocketChatClient client = new SocketChatClient( listener.AcceptSocket() );
			SocketChatClient client = new SocketChatClient( sockClient );
			m_aryClients.Add( client );
			Console.WriteLine( "Client {0}, joined", client.Sock.RemoteEndPoint );
 
			// Get current date and time.
			DateTime now = DateTime.Now;
			String strDateLine = "Welcome " + now.ToString("G") + "\n\r";

			// Convert to byte array and send.
			Byte[] byteDateLine = System.Text.Encoding.ASCII.GetBytes( strDateLine.ToCharArray() );
			client.Sock.Send( byteDateLine, byteDateLine.Length, 0 );

			client.SetupRecieveCallback( this );
		}

		/// <summary>
		/// Get the new data and send it out to all other connections. 
		/// Note: If not data was recieved the connection has probably 
		/// died.
		/// </summary>
		/// <param name="ar"></param>
		public void OnRecievedData( IAsyncResult ar )
		{
			SocketChatClient client = (SocketChatClient)ar.AsyncState;
			byte [] aryRet = client.GetRecievedData( ar );

			// If no data was recieved then the connection is probably dead
			if( aryRet.Length < 1 )
			{
				Console.WriteLine( "Client {0}, disconnected", client.Sock.RemoteEndPoint );
				client.Sock.Close();
				m_aryClients.Remove( client );      				
				return;
			}

			// Send the recieved data to all clients (including sender for echo)
			foreach( SocketChatClient clientSend in m_aryClients )
			{
				try
				{
					clientSend.Sock.Send( aryRet );
				}
				catch
				{
					// If the send fails the close the connection
					Console.WriteLine( "Send to client {0} failed", client.Sock.RemoteEndPoint );
					clientSend.Sock.Close();
					m_aryClients.Remove( client );
					return;
				}
			}
			client.SetupRecieveCallback( this );
		}
	}

	/// <summary>
	/// Class holding information and buffers for the Client socket connection
	/// </summary>
	internal class SocketChatClient
	{
		private Socket m_sock;						// Connection to the client
		private byte[] m_byBuff = new byte[50];		// Receive data buffer
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="sock">client socket conneciton this object represents</param>
		public SocketChatClient( Socket sock )
		{
			m_sock = sock;
		}

		// Readonly access
		public Socket Sock
		{
			get{ return m_sock; }
		}

		/// <summary>
		/// Setup the callback for recieved data and loss of conneciton
		/// </summary>
		/// <param name="app"></param>
		public void SetupRecieveCallback( AppMain app )
		{
			try
			{
				AsyncCallback recieveData = new AsyncCallback(app.OnRecievedData);
				m_sock.BeginReceive( m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, this );
			}
			catch( Exception ex )
			{
				Console.WriteLine( "Recieve callback setup failed! {0}", ex.Message );
			}
		}

		/// <summary>
		/// Data has been recieved so we shall put it in an array and
		/// return it.
		/// </summary>
		/// <param name="ar"></param>
		/// <returns>Array of bytes containing the received data</returns>
		public byte [] GetRecievedData( IAsyncResult ar )
		{
            int nBytesRec = 0;
			try
			{
				nBytesRec = m_sock.EndReceive( ar );
			}
			catch{}
			byte [] byReturn = new byte[nBytesRec];
			Array.Copy( m_byBuff, byReturn, nBytesRec );
			
			/*
			// Check for any remaining data and display it
			// This will improve performance for large packets 
			// but adds nothing to readability and is not essential
			int nToBeRead = m_sock.Available;
			if( nToBeRead > 0 )
			{
				byte [] byData = new byte[nToBeRead];
				m_sock.Receive( byData );
				// Append byData to byReturn here
			}
			*/
			return byReturn;
		}
	}
}
