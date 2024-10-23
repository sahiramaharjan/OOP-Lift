using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiftDemo_A
{
	public partial class Form1 : Form
	{
		bool isMovingUp = false;
		bool isMovingDown = false;
		int liftSpeed = 5;

		bool isClosing = false;
		bool isOpening = false;
		int doorSpeed = 5;
		int doorMaxOpenWidth;

		string connectString = @"Server = SAHIRA\SAHIRA; Database =sahira; Trusted_Connection = True";
		DataTable dt = new DataTable();
		public Form1()
		{
			InitializeComponent();
			doorMaxOpenWidth = mainElevator.Width / 2 - 30;

			dataGridView1.ColumnCount = 2;
			dataGridView1.Columns[0].Name = "Time";
			dataGridView1.Columns[1].Name = "Events";

			dt.Columns.Add("Time");
			dt.Columns.Add("Events");
		}

		private void logEvents(string message)
		{
			string currentTime = DateTime.Now.ToString("hh:mm:ss");

			dt.Rows.Add(currentTime, message);
			dataGridView1.Rows.Add(currentTime, message);
			InsertLogsIntoDB(dt);

		}

		private void InsertLogsIntoDB(DataTable dt)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(connectString))
				{
					string query = @"Insert into Logs(Logtime, EventDescription) values(@Time, @Logs)";
					using (SqlDataAdapter adapter = new SqlDataAdapter())
					{
                        adapter.InsertCommand = new SqlCommand(query, conn);
						adapter.InsertCommand.Parameters.Add("@Time", SqlDbType.DateTime, 0, "Time");
                        adapter.InsertCommand.Parameters.Add("@Logs", SqlDbType.NVarChar, 255, "Events");

						conn.Open();
						adapter.Update(dt);

                    }
                }
			}
            catch (Exception ex)
            {
                MessageBox.Show("Error Saving logs to DB: " + ex.Message);
            }

        }

        public void btn_1_click(object sender, EventArgs e)
		{

                isMovingUp = true;
                isMovingDown = false;
                liftTimer.Start();
                btn_G.Enabled = false;
                logEvents("Going to First Floor");
        
		}

		public void btn_G_click(object sender, EventArgs e)
		{	
                isMovingUp = false;
                isMovingDown = true;
                liftTimer.Start();
                btn_1.Enabled = false;
                logEvents("Going to Ground Floor");
		}

		public void liftTimer_Tick(object sender, EventArgs e)
		{
			if(isMovingUp)
			{
				btn_G.BackColor = Color.Gray;
				btn_1.BackColor = Color.LightGreen;
				if(mainElevator.Top > 0)
				{
					mainElevator.Top -= liftSpeed;
				}
				else
				{
					mainElevator.Top = 0;
					liftTimer.Stop();
					btn_G.Enabled=true;

					OpenDoor();
				}
			}

			if(isMovingDown)
			{
				btn_1.BackColor = Color.Gray;
				btn_G.BackColor = Color.LightGreen;
				if(mainElevator.Bottom < this.ClientSize.Height)
				{
					mainElevator.Top += liftSpeed;
				}
				else
				{
					liftTimer.Stop();
					btn_1.Enabled=true;
					OpenDoor();
				}
			}
		}
		private void OpenDoor()
		{
			isOpening = true;
			isClosing = false;
			doorTimer.Start();
			btn_Close.Enabled = false;
            logEvents("Lift is opening");

        }

		private void btn_Open_Click(object sender, EventArgs e)
		{
			isOpening = true;
			isClosing = false;
			doorTimer.Start();
			btn_Close.Enabled = false;
			logEvents("Lift is opening");
		}

		private void btn_Close_Click(object sender, EventArgs e)
		{
			isOpening =false;
			isClosing=true;
			doorTimer.Start();
			logEvents("Lift is closing");
			
		}

		private void door_Timer_Tick(object sender, EventArgs e)

		{
			if (mainElevator.Top != 0)
			{
				if (isOpening)
				{
					if (doorLeft_G.Left > doorMaxOpenWidth / 2)
					{
						doorLeft_G.Left -= doorSpeed;
						doorRight_G.Left += doorSpeed;
					}
					else
					{
						doorTimer.Stop();
						btn_Close.Enabled = true;
					}
				}

				if (isClosing)
				{
					if (doorLeft_G.Right < mainElevator.Width + doorMaxOpenWidth / 2 - 5)
					{
						doorLeft_G.Left += doorSpeed;
						doorRight_G.Left -= doorSpeed;
					}
					else
					{
						doorTimer.Stop();

					}
				}
			}
			else 
			{
                if (isOpening)
                {
                    if (doorLeft_1.Left > doorMaxOpenWidth / 2)
                    {
                        doorLeft_1.Left -= doorSpeed;
                        doorRight_1.Left += doorSpeed;
                    }
                    else
                    {
                        doorTimer.Stop();
                        btn_Close.Enabled = true;
                    }
                }
                if (isClosing)
                {
                    if (doorLeft_1.Right < mainElevator.Width + doorMaxOpenWidth / 2 - 5)
                    {
                        doorLeft_1.Left += doorSpeed;
                        doorRight_1.Left -= doorSpeed;
                    }
                    else
                    {
                        doorTimer.Stop();

                    }
                }


            }
        }
	}
}
