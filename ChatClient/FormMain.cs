using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Net;									// Endpoint
using System.Net.Sockets;							// Socket namespace
using System.Text;									// Text encoders
using System.Text.RegularExpressions;
using System.Media;

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
		private System.Windows.Forms.TextBox m_tbMessage;
		private System.Windows.Forms.Button m_btnSend;
        private TextBox textBox1;
        private Label label1;
        private Button m_btnConnect;
        private TextBox m_tbServerAddress;
        private ColorDialog colorDialog1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem connectToolStripMenuItem;
        private ToolStripMenuItem tigerClanToolStripMenuItem;
        private ToolStripMenuItem colorToolStripMenuItem;
        private ToolStripMenuItem textColorToolStripMenuItem;
        private ToolStripMenuItem backgroundColorToolStripMenuItem;
        private ListBox listBox1;
        private SplitContainer splitContainer1;
        private RichTextBox textBox2;
        private Button button1;
        private ToolTip toolTip1;
        private Button button2;
        private ToolTip toolTip2;
        private IContainer components;

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
            this.components = new System.ComponentModel.Container();
            this.m_tbMessage = new System.Windows.Forms.TextBox();
            this.m_btnSend = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.m_btnConnect = new System.Windows.Forms.Button();
            this.m_tbServerAddress = new System.Windows.Forms.TextBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tigerClanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backgroundColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBox2 = new System.Windows.Forms.RichTextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_tbMessage
            // 
            this.m_tbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.m_tbMessage.Location = new System.Drawing.Point(12, 557);
            this.m_tbMessage.Name = "m_tbMessage";
            this.m_tbMessage.Size = new System.Drawing.Size(734, 20);
            this.m_tbMessage.TabIndex = 3;
            this.m_tbMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.m_tbMessage_keyDown);
            // 
            // m_btnSend
            // 
            this.m_btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnSend.Location = new System.Drawing.Point(752, 555);
            this.m_btnSend.Name = "m_btnSend";
            this.m_btnSend.Size = new System.Drawing.Size(75, 23);
            this.m_btnSend.TabIndex = 4;
            this.m_btnSend.Text = "Send";
            this.m_btnSend.Click += new System.EventHandler(this.m_btnSend_Click);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(727, 29);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(689, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Nick:";
            // 
            // m_btnConnect
            // 
            this.m_btnConnect.Location = new System.Drawing.Point(644, 27);
            this.m_btnConnect.Name = "m_btnConnect";
            this.m_btnConnect.Size = new System.Drawing.Size(37, 23);
            this.m_btnConnect.TabIndex = 7;
            this.m_btnConnect.Text = "con";
            this.m_btnConnect.UseVisualStyleBackColor = true;
            this.m_btnConnect.Click += new System.EventHandler(this.m_btnConnect_Click);
            // 
            // m_tbServerAddress
            // 
            this.m_tbServerAddress.Location = new System.Drawing.Point(538, 29);
            this.m_tbServerAddress.Name = "m_tbServerAddress";
            this.m_tbServerAddress.Size = new System.Drawing.Size(100, 20);
            this.m_tbServerAddress.TabIndex = 8;
            this.m_tbServerAddress.Text = "192.168.1.1";
            this.m_tbServerAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.m_tbServerAddress_KeyDown);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.colorToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(839, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tigerClanToolStripMenuItem});
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.connectToolStripMenuItem.Text = "Connect";
            // 
            // tigerClanToolStripMenuItem
            // 
            this.tigerClanToolStripMenuItem.Name = "tigerClanToolStripMenuItem";
            this.tigerClanToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.tigerClanToolStripMenuItem.Text = "TigerClan";
            this.tigerClanToolStripMenuItem.Click += new System.EventHandler(this.tigerClanToolStripMenuItem_Click);
            // 
            // colorToolStripMenuItem
            // 
            this.colorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.textColorToolStripMenuItem,
            this.backgroundColorToolStripMenuItem});
            this.colorToolStripMenuItem.Name = "colorToolStripMenuItem";
            this.colorToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.colorToolStripMenuItem.Text = "Color";
            // 
            // textColorToolStripMenuItem
            // 
            this.textColorToolStripMenuItem.Name = "textColorToolStripMenuItem";
            this.textColorToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.textColorToolStripMenuItem.Text = "Text Color";
            this.textColorToolStripMenuItem.Click += new System.EventHandler(this.textColorToolStripMenuItem_Click);
            // 
            // backgroundColorToolStripMenuItem
            // 
            this.backgroundColorToolStripMenuItem.Name = "backgroundColorToolStripMenuItem";
            this.backgroundColorToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.backgroundColorToolStripMenuItem.Text = "Background Color";
            this.backgroundColorToolStripMenuItem.Click += new System.EventHandler(this.backgroundColorToolStripMenuItem_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.BackColor = System.Drawing.Color.Black;
            this.listBox1.ForeColor = System.Drawing.Color.Lime;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(-2, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(201, 498);
            this.listBox1.TabIndex = 12;
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.nickbox_DoubleClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 56);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.textBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listBox1);
            this.splitContainer1.Size = new System.Drawing.Size(838, 495);
            this.splitContainer1.SplitterDistance = 635;
            this.splitContainer1.TabIndex = 13;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.BackColor = System.Drawing.Color.Black;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.ForeColor = System.Drawing.Color.Lime;
            this.textBox2.Location = new System.Drawing.Point(0, -3);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(632, 498);
            this.textBox2.TabIndex = 14;
            this.textBox2.Text = "";
            this.textBox2.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.mRichTextBox_LinkClicked);
            // 
            // button2
            // 
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = global::ChatClient.Properties.Resources.sound;
            this.button2.Location = new System.Drawing.Point(41, 27);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(23, 23);
            this.button2.TabIndex = 14;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.button2.MouseHover += new System.EventHandler(this.button2_MouseHover);
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = global::ChatClient.Properties.Resources.connect;
            this.button1.Location = new System.Drawing.Point(12, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 23);
            this.button1.TabIndex = 14;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.button1.MouseHover += new System.EventHandler(this.button1_MouseHover);
            // 
            // FormMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(839, 579);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.m_tbServerAddress);
            this.Controls.Add(this.m_btnConnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.m_btnSend);
            this.Controls.Add(this.m_tbMessage);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tiger Chat";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        [STAThread]
		static void Main() 
		{
			Application.Run(new FormMain());
		}

        /** FORM FUNCTIONS **/
        private void FormMain_Load(object sender, EventArgs e)
        {
            MenuStrip MainMenu = new MenuStrip();
        }

        private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (m_sock != null && m_sock.Connected)
            {
                m_sock.Shutdown(SocketShutdown.Both);
                m_sock.Close();
            }
        }

        /** VARIABLES **/
            //Strings
            public String connect_text = "Connect";
            public String sound_text = "Play Sounds";
            //Booleans
            public Boolean clicked = false;
            public Boolean playsound = false;
            //ArrayList
            public ArrayList messages = new ArrayList();

        /** FUNCTIONS **/
            public void sendNick()
            {
                if (m_sock == null || !m_sock.Connected)
                {
                    MessageBox.Show(this, "Must be connected");
                    return;
                }
                String message = "~" + textBox1.Text + "\n";
                Byte[] byteDateLine = Encoding.ASCII.GetBytes(message.ToCharArray());
                m_sock.Send(byteDateLine, byteDateLine.Length, 0);
            }

            public void addMessage(String m)
            {
                messages.Add(m);
            }
            private void textBox1_TextChanged(object sender, EventArgs e)
            {

            }

            public void OnAddMessage(string sMessage)
            {
                if (sMessage.StartsWith("~"))
                {
                    listBox1.Items.Clear();
                    string[] nicknames = Regex.Split(sMessage, "[,]");
                    foreach (String nick in nicknames)
                    {
                        listBox1.Items.Add(nick.TrimStart('~'));
                    }
                }
                else
                {
                    textBox2.Text = textBox2.Text + sMessage + "\r\n";

                    if (playsound == true)
                    {
                        SystemSounds.Beep.Play();
                    }

                    textBox2.SelectionStart = textBox2.Text.Length;
                    textBox2.ScrollToCaret();
                }
            }

            public void OnConnect(IAsyncResult ar)
            {
                // Socket was the passed in object
                Socket sock = (Socket)ar.AsyncState;

                // Check if we were sucessfull
                try
                {
                    //sock.EndConnect( ar );
                    if (sock.Connected)
                        SetupRecieveCallback(sock);
                    else
                        MessageBox.Show(this, "Unable to connect to remote machine", "Connect Failed!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Unusual error during Connect!");
                }

                sendNick();
            }

            public void OnRecievedData(IAsyncResult ar)
            {
                // Socket was the passed in object
                Socket sock = (Socket)ar.AsyncState;

                // Check if we got any data
                try
                {
                    int nBytesRec = sock.EndReceive(ar);
                    if (nBytesRec > 0)
                    {
                        // Wrote the data to the List
                        string sRecieved = Encoding.ASCII.GetString(m_byBuff, 0, nBytesRec);

                        // WARNING : The following line is NOT thread safe. Invoke is
                        // m_lbRecievedData.Items.Add( sRecieved );
                        Invoke(m_AddMessage, new string[] { sRecieved });

                        // If the connection is still usable restablish the callback
                        SetupRecieveCallback(sock);
                    }
                    else
                    {
                        // If no data was recieved then the connection is probably dead
                        Console.WriteLine("Client {0}, disconnected", sock.RemoteEndPoint);
                        sock.Shutdown(SocketShutdown.Both);
                        sock.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Unusual error druing Recieve!");
                }
            }

            public void SetupRecieveCallback(Socket sock)
            {
                try
                {
                    AsyncCallback recieveData = new AsyncCallback(OnRecievedData);
                    sock.BeginReceive(m_byBuff, 0, m_byBuff.Length, SocketFlags.None, recieveData, sock);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Setup Recieve Callback failed!");
                }
            }

            /** DOUBLE CLICK FUNCTIONS **/
           private void nickbox_DoubleClick(object sender, MouseEventArgs e)
            {
                if (listBox1.SelectedItem != null && (listBox1.SelectedItem.ToString().Length != 0))
                {
                    m_tbMessage.Text = "/msg " + listBox1.SelectedItem + " ";
                    m_tbMessage.Focus();
                    m_tbMessage.SelectionStart = m_tbMessage.Text.Length + 1;
                }

            }
            /** ON CLICK FUNCTIONS **/

            private void m_btnConnect_Click(object sender, System.EventArgs e)
            {
                Cursor cursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    // Close the socket if it is still open
                    if (m_sock != null && m_sock.Connected)
                    {
                        m_sock.Shutdown(SocketShutdown.Both);
                        System.Threading.Thread.Sleep(10);
                        m_sock.Close();
                    }

                    // Create the socket object
                    m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Define the Server address and port
                    IPEndPoint epServer = new IPEndPoint(IPAddress.Parse(m_tbServerAddress.Text), 399);

                    // Connect to the server blocking method and setup callback for recieved data
                    // m_sock.Connect( epServer );
                    // SetupRecieveCallback( m_sock );

                    // Connect to server non-Blocking method
                    m_sock.Blocking = false;
                    AsyncCallback onconnect = new AsyncCallback(OnConnect);
                    m_sock.BeginConnect(epServer, onconnect, m_sock);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Server Connect failed!");
                }
                Cursor.Current = cursor;
            }

            private void button2_Click(object sender, EventArgs e)
            {
                if (playsound == false)
                {
                    this.button2.Image = ((System.Drawing.Image)(Properties.Resources.sound_mute));
                    sound_text = "Mute Sounds";
                    playsound = true;
                }
                else if (playsound == true)
                {
                    this.button2.Image = ((System.Drawing.Image)(Properties.Resources.sound));
                    sound_text = "Play Sounds";
                    playsound = false;
                }
            }


            public void m_btnSend_Click(object sender, System.EventArgs e)
            {
                // Check we are connected
                if (m_sock == null || !m_sock.Connected)
                {
                    MessageBox.Show(this, "Must be connected to Send a message");
                    return;
                }

                // Read the message from the text box and send it
                try
                {
                    // Convert to byte array and send.
                    String message = m_tbMessage.Text + "\n";
                    addMessage(m_tbMessage.Text);
                    Byte[] byteDateLine = Encoding.ASCII.GetBytes(message.ToCharArray());
                    m_sock.Send(byteDateLine, byteDateLine.Length, 0);
                    m_tbMessage.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Send Message Failed!");
                }
            }

            private void button1_Click(object sender, EventArgs e)
            {
                if (clicked == false)
                {
                    this.button1.Image = ((System.Drawing.Image)(Properties.Resources.disconnect));
                    connect_text = "Disconnect";
                    clicked = true;
                    Cursor cursor = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    try
                    {
                        // Close the socket if it is still open
                        if (m_sock != null && m_sock.Connected)
                        {
                            m_sock.Shutdown(SocketShutdown.Both);
                            System.Threading.Thread.Sleep(10);
                            m_sock.Close();
                        }

                        // Create the socket object
                        m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        // Define the Server address and port
                        IPEndPoint epServer = new IPEndPoint(IPAddress.Parse("24.229.41.95"), 399);

                        // Connect to the server blocking method and setup callback for recieved data
                        // m_sock.Connect( epServer );
                        // SetupRecieveCallback( m_sock );

                        // Connect to server non-Blocking method
                        m_sock.Blocking = false;
                        AsyncCallback onconnect = new AsyncCallback(OnConnect);
                        m_sock.BeginConnect(epServer, onconnect, m_sock);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message, "Server Connect failed!");
                    }
                    Cursor.Current = cursor;
                }
                else if (clicked == true)
                {
                    this.button1.Image = ((System.Drawing.Image)(Properties.Resources.connect));
                    connect_text = "Connect";
                    clicked = false;
                    if (m_sock != null && m_sock.Connected)
                    {
                        m_sock.Shutdown(SocketShutdown.Both);
                    }
                }
                else { }
            }

            private void mRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
            {
                DialogResult result;
                result = MessageBox.Show("Are you sure you trust " + e.LinkText + "?", "WARNING", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                }
                else if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(e.LinkText);
                }
            }

            /** HOVER FUNCTIONS **/
            private void button2_MouseHover(object sender, EventArgs e)
            {
                toolTip2.SetToolTip(button2, sound_text);
            }

            private void button1_MouseHover(object sender, EventArgs e)
            {
                toolTip1.SetToolTip(button1, connect_text);
            }

            /** KEY DOWN FUNCTIONS **/
            private void m_tbServerAddress_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyValue == 13)
                {
                    m_btnConnect.PerformClick();
                }
            }

            private void textBox1_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyValue == 13) //13 = enter
                {
                    sendNick();
                    e.SuppressKeyPress = true;
                }
            }

            protected void m_tbMessage_keyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyValue == 13) //13 = enter
                {
                    m_btnSend.PerformClick();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyValue == 38)
                {
                    int index = (messages.Count - 1);
                    string text = (string)messages[index];
                    m_tbMessage.Text = text;

                }
                else if (e.KeyCode == Keys.Tab)
                {
                    String[] words = m_tbMessage.Text.Split(' ');
                    String word = words[words.Length - 1];
                    foreach (String item in listBox1.Items)
                    {

                        if (item.IndexOf(word) >= 0 )
                        {
                            words[words.Length - 1] = item;
                            m_tbMessage.Text = "";
                            foreach (string word1 in words)
                            {
                                
                                m_tbMessage.AppendText(word1);
                            }
                            m_tbMessage.Focus();

                        }
                    } 
                }
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                if (keyData == Keys.Tab)
                {
                    return false;
                }

                return base.ProcessDialogKey(keyData);
            }
            private void Form1_KeyPress(object sender, KeyPressEventArgs e)
            {
                // Prevent textbox beeping
                if (e.KeyChar == '\t')
                {
                    e.Handled = true;
                }
            }


            /** MENU STRIP CLICK **/
            private void tigerClanToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Cursor cursor = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    // Close the socket if it is still open
                    if (m_sock != null && m_sock.Connected)
                    {
                        m_sock.Shutdown(SocketShutdown.Both);
                        System.Threading.Thread.Sleep(10);
                        m_sock.Close();
                    }

                    // Create the socket object
                    m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Define the Server address and port
                    IPEndPoint epServer = new IPEndPoint(IPAddress.Parse("24.229.41.95"), 399);

                    // Connect to the server blocking method and setup callback for recieved data
                    // m_sock.Connect( epServer );
                    // SetupRecieveCallback( m_sock );

                    // Connect to server non-Blocking method
                    m_sock.Blocking = false;
                    AsyncCallback onconnect = new AsyncCallback(OnConnect);
                    m_sock.BeginConnect(epServer, onconnect, m_sock);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Server Connect failed!");
                }
                Cursor.Current = cursor;
            }

            private void textColorToolStripMenuItem_Click(object sender, EventArgs e)
            {
                ColorDialog MyDialog = new ColorDialog();
                // Keeps the user from selecting a custom color.
                MyDialog.AllowFullOpen = false;
                // Allows the user to get help. (The default is false.)
                MyDialog.ShowHelp = true;
                // Sets the initial color select to the current text color.
                MyDialog.Color = textBox2.ForeColor;

                // Update the text box color if the user clicks OK 
                if (MyDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox2.ForeColor = MyDialog.Color;
                    listBox1.ForeColor = MyDialog.Color;
                }
            }

            private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
            {
                ColorDialog MyDialog = new ColorDialog();
                // Keeps the user from selecting a custom color.
                MyDialog.AllowFullOpen = false;
                // Allows the user to get help. (The default is false.)
                MyDialog.ShowHelp = true;
                // Sets the initial color select to the current text color.
                MyDialog.Color = textBox2.BackColor;

                // Update the text box color if the user clicks OK 
                if (MyDialog.ShowDialog() == DialogResult.OK)
                {
                    textBox2.BackColor = MyDialog.Color;
                    listBox1.BackColor = MyDialog.Color;
                }
            }

        
        /** END FUNCTIONS **/
	}
}
