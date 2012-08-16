using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Net;									// Endpoint
using System.Net.Sockets;							// Socket namespace
using System.Text;									// Text encoders

// Declare the delegate prototype to send data back to the form
delegate void AddMessage( string sNewMessage );


namespace ChatClient
{
	/// <summary>
	/// This form connects to a Socket server and Streams data to and from it.
	/// Note: The following has been ommitted.
	///		1) Send button need to be grayed when conneciton is 
	///		   not active
	///		2) Send button should gray when no text in the Message box.
	///		3) Line feeds in the recieved data should be parsed into seperate
	///		   lines in the recieved data list
	///		4) Read startup setting from a app.config file
	/// </summary>
	public class FormMain : System.Windows.Forms.Form
	{
		// My Attributes
		private Socket			m_sock;						// Server connection
		private byte []			m_byBuff = new byte[256];	// Recieved data buffer
		private event AddMessage m_AddMessage;				// Add Message Event handler for Form

		// Wizard generated code
		private System.Windows.Forms.Button m_btnConnect;
		private System.Windows.Forms.TextBox m_tbServerAddress;
		private System.Windows.Forms.ListBox m_lbRecievedData;
		private System.Windows.Forms.TextBox m_tbMessage;
		private System.Windows.Forms.Button m_btnSend;
        private TextBox textBox1;
        private Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Add Message Event handler for Form decoupling from input thread
			m_AddMessage = new AddMessage( OnAddMessage );


			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.m_tbServerAddress = new System.Windows.Forms.TextBox();
            this.m_tbMessage = new System.Windows.Forms.TextBox();
            this.m_btnConnect = new System.Windows.Forms.Button();
            this.m_lbRecievedData = new System.Windows.Forms.ListBox();
            this.m_btnSend = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // m_tbServerAddress
            // 
            this.m_tbServerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tbServerAddress.Location = new System.Drawing.Point(8, 8);
            this.m_tbServerAddress.Name = "m_tbServerAddress";
            this.m_tbServerAddress.Size = new System.Drawing.Size(204, 20);
            this.m_tbServerAddress.TabIndex = 1;
            this.m_tbServerAddress.Text = "24.229.41.95";
            // 
            // m_tbMessage
            // 
            this.m_tbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tbMessage.Location = new System.Drawing.Point(8, 37);
            this.m_tbMessage.Name = "m_tbMessage";
            this.m_tbMessage.Size = new System.Drawing.Size(205, 20);
            this.m_tbMessage.TabIndex = 3;
            this.m_tbMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.m_tbMessage_keyDown);
            // 
            // m_btnConnect
            // 
            this.m_btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnConnect.Location = new System.Drawing.Point(228, 8);
            this.m_btnConnect.Name = "m_btnConnect";
            this.m_btnConnect.Size = new System.Drawing.Size(75, 23);
            this.m_btnConnect.TabIndex = 0;
            this.m_btnConnect.Text = "Connect";
            this.m_btnConnect.Click += new System.EventHandler(this.m_btnConnect_Click);
            // 
            // m_lbRecievedData
            // 
            this.m_lbRecievedData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lbRecievedData.IntegralHeight = false;
            this.m_lbRecievedData.Location = new System.Drawing.Point(0, 94);
            this.m_lbRecievedData.Name = "m_lbRecievedData";
            this.m_lbRecievedData.Size = new System.Drawing.Size(311, 290);
            this.m_lbRecievedData.TabIndex = 2;
            // 
            // m_btnSend
            // 
            this.m_btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnSend.Location = new System.Drawing.Point(228, 36);
            this.m_btnSend.Name = "m_btnSend";
            this.m_btnSend.Size = new System.Drawing.Size(75, 23);
            this.m_btnSend.TabIndex = 4;
            this.m_btnSend.Text = "Send";
            this.m_btnSend.Click += new System.EventHandler(this.m_btnSend_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(43, 69);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Nick";
            // 
            // FormMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(312, 385);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.m_btnSend);
            this.Controls.Add(this.m_tbMessage);
            this.Controls.Add(this.m_lbRecievedData);
            this.Controls.Add(this.m_tbServerAddress);
            this.Controls.Add(this.m_btnConnect);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form1";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new FormMain());
		}

		/// <summary>
		/// Connect button pressed. Attempt a connection to the server and 
		/// setup Recieved data callback
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void m_btnConnect_Click(object sender, System.EventArgs e)
		{
			Cursor cursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				// Close the socket if it is still open
				if( m_sock != null && m_sock.Connected )
				{
					m_sock.Shutdown( SocketShutdown.Both );
					System.Threading.Thread.Sleep( 10 );
					m_sock.Close();
				}

				// Create the socket object
				m_sock = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );	

				// Define the Server address and port
				IPEndPoint epServer = new IPEndPoint(  IPAddress.Parse( m_tbServerAddress.Text ), 399 );

				// Connect to the server blocking method and setup callback for recieved data
				// m_sock.Connect( epServer );
				// SetupRecieveCallback( m_sock );
				
				// Connect to server non-Blocking method
				m_sock.Blocking = false;
				AsyncCallback onconnect = new AsyncCallback( OnConnect );
				m_sock.BeginConnect( epServer, onconnect, m_sock );
			}
			catch( Exception ex )
			{
				MessageBox.Show( this, ex.Message, "Server Connect failed!" );
			}
			Cursor.Current = cursor;
		}

		public void OnConnect( IAsyncResult ar )
		{
			// Socket was the passed in object
			Socket sock = (Socket)ar.AsyncState;

			// Check if we were sucessfull
			try
			{
				//sock.EndConnect( ar );
				if( sock.Connected )
					SetupRecieveCallback( sock );
				else
					MessageBox.Show( this, "Unable to connect to remote machine", "Connect Failed!" );
			}
			catch( Exception ex )
			{
				MessageBox.Show( this, ex.Message, "Unusual error during Connect!" );
			}
		}

		/// <summary>
		/// Get the new data and send it out to all other connections. 
		/// Note: If not data was recieved the connection has probably 
		/// died.
		/// </summary>
		/// <param name="ar"></param>
		public void OnRecievedData( IAsyncResult ar )
		{
			// Socket was the passed in object
			Socket sock = (Socket)ar.AsyncState;

			// Check if we got any data
			try
			{
				int nBytesRec = sock.EndReceive( ar );
				if( nBytesRec > 0 )
				{
					// Wrote the data to the List
					string sRecieved = Encoding.ASCII.GetString( m_byBuff, 0, nBytesRec );

					// WARNING : The following line is NOT thread safe. Invoke is
					// m_lbRecievedData.Items.Add( sRecieved );
					Invoke( m_AddMessage, new string [] { sRecieved } );

					// If the connection is still usable restablish the callback
					SetupRecieveCallback( sock );
				}
				else
				{
					// If no data was recieved then the connection is probably dead
					Console.WriteLine( "Client {0}, disconnected", sock.RemoteEndPoint );
					sock.Shutdown( SocketShutdown.Both );
					sock.Close();
				}
			}
			catch( Exception ex )
			{
				MessageBox.Show( this, ex.Message, "Unusual error druing Recieve!" );
			}
		}

		public void OnAddMessage( string sMessage )
		{
			m_lbRecievedData.Items.Add( sMessage );
            m_lbRecievedData.SelectedIndex = m_lbRecievedData.Items.Count - 1;
            m_lbRecievedData.SelectedIndex = -1;
		}
		


		/// <summary>
		/// Setup the callback for recieved data and loss of conneciton
		/// </summary>
		public void SetupRecieveCallback( Socket sock )
		{
			try
			{
				AsyncCallback recieveData = new AsyncCallback( OnRecievedData );
				sock.BeginReceive( m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, sock );
			}
			catch( Exception ex )
			{
				MessageBox.Show( this, ex.Message, "Setup Recieve Callback failed!" );
			}
		}

		/// <summary>
		/// Close the Socket connection bofore going home
		/// </summary>
		private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if( m_sock != null && m_sock.Connected )
			{
				m_sock.Shutdown( SocketShutdown.Both );
				m_sock.Close();
			}
		}

        private void m_tbMessage_keyDown(object sender, KeyEventArgs e)
        {
            
            if (e.KeyValue == 13) //13 = enter
            {
                m_btnSend.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
		/// <summary>
		/// Send the Message in the Message area. Only do this if we are connected
		/// </summary>
		private void m_btnSend_Click(object sender, System.EventArgs e)
		{
			// Check we are connected
			if( m_sock == null || !m_sock.Connected )
			{
				MessageBox.Show( this, "Must be connected to Send a message" );
				return;
			}

			// Read the message from the text box and send it
			try
			{		
				// Convert to byte array and send.
                String message = textBox1.Text + ": " + m_tbMessage.Text;
                Byte[] byteDateLine = Encoding.ASCII.GetBytes(message.ToCharArray());
				m_sock.Send( byteDateLine, byteDateLine.Length, 0 );
                m_tbMessage.Clear();
			}
			catch( Exception ex )
			{
				MessageBox.Show( this, ex.Message, "Send Message Failed!" );
			}
		}
	}
}
