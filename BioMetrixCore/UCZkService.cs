using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BioMetrixCore.Model;
using BioMetrixCore.Utilities;

namespace BioMetrixCore
{
    public partial class UCZkService : UserControl
    {

        DeviceManipulator manipulator = new DeviceManipulator();
        public ZkemClient objZkeeper;
        private bool isDeviceConnected = false;
        private Attn_tblDeviceInfo DeviceInfo;
        public int TimerCount { get; set; }
        public bool IsDeviceConnected
        {
            get { return isDeviceConnected; }
            set
            {
                isDeviceConnected = value;
                if (isDeviceConnected)
                {
                    ShowStatusBar("The device is connected !!", true);
                    btnConnect.Text = "Disconnect";
                    ToggleControls(true);
                }
                else
                {
                    ShowStatusBar("The device is diconnected !!", true);
                    objZkeeper.Disconnect();
                    btnConnect.Text = "Connect";
                    ToggleControls(false);
                }
            }
        }
        private void ToggleControls(bool value)
        {
            //btnBeep.Enabled = value;
            //btnDownloadFingerPrint.Enabled = value;
            //btnPullData.Enabled = value;
            //btnPowerOff.Enabled = value;
            //btnRestartDevice.Enabled = value;
            //btnGetDeviceTime.Enabled = value;
            //btnEnableDevice.Enabled = value;
            //btnDisableDevice.Enabled = value;
            //btnGetAllUserID.Enabled = value;
            //btnUploadUserInfo.Enabled = value;
            tbxMachineNumber.Enabled = !value;
            tbxPort.Enabled = !value;
            tbxDeviceIP.Enabled = !value;

        }

        public UCZkService()
        {
            InitializeComponent();
        }

        public UCZkService(Attn_tblDeviceInfo DeviceInfo):this()
        {
            this.DeviceInfo = DeviceInfo;

            if (DeviceInfo != null)
            {
                tbxDeviceIP.Text = DeviceInfo.IP;
                tbxMachineNumber.Text = DeviceInfo.MachineNumber.ToString();
                tbxPort.Text = DeviceInfo.Port.ToString();
                timerGetData.Tag = DeviceInfo.PeekDataEverySec * 1000;
            }
        }

        
        private void DisplayEmpty()
        {
            //ClearGrid();
            //dgvRecords.Controls.Add(new DataEmpty());
        }
        private void UCZkService_Load(object sender, EventArgs e)
        {

        }
        public void ShowStatusBar(string message, bool type)
        {
            if (message.Trim() == string.Empty)
            {
                lblStatus.Visible = false;
                return;
            }

            lblStatus.Visible = true;
            lblStatus.Text = message;
            lblStatus.ForeColor = Color.White;

            if (type)
                lblStatus.BackColor = Color.FromArgb(79, 208, 154);
            else
                lblStatus.BackColor = Color.FromArgb(230, 112, 134);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                ShowStatusBar(string.Empty, true);

                if (IsDeviceConnected)
                {
                    IsDeviceConnected = false;
                    this.Cursor = Cursors.Default;

                    return;
                }

                string ipAddress = tbxDeviceIP.Text.Trim();
                string port = tbxPort.Text.Trim();
                if (ipAddress == string.Empty || port == string.Empty)
                    throw new Exception("The Device IP Address and Port is mandotory !!");

                int portNumber = 4370;
                if (!int.TryParse(port, out portNumber))
                    throw new Exception("Not a valid port number");

                bool isValidIpA = UniversalStatic.ValidateIP(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The Device IP is invalid !!");

                isValidIpA = UniversalStatic.PingTheDevice(ipAddress);
                if (!isValidIpA)
                    throw new Exception("The device at " + ipAddress + ":" + port + " did not respond!!");

                objZkeeper = new ZkemClient(RaiseDeviceEvent);
                IsDeviceConnected = objZkeeper.Connect_Net(ipAddress, portNumber);

                if (IsDeviceConnected)
                {
                    string deviceInfo = manipulator.FetchDeviceInfo(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));
                    lblDeviceInfo.Text = deviceInfo;
                }

            }
            catch (Exception ex)
            {
                ShowStatusBar(ex.Message, false);
            }
            this.Cursor = Cursors.Default;
            btnPullData.Enabled = true;
        }
        private void RaiseDeviceEvent(object sender, string actionType)
        {
            switch (actionType)
            {
                case UniversalStatic.acx_Disconnect:
                    {
                        ShowStatusBar("The device is switched off", true);
                        DisplayEmpty();
                        btnConnect.Text = "Connect";
                        ToggleControls(false);
                        break;
                    }

                default:
                    break;
            }

        }
        private void btnPingDevice_Click(object sender, EventArgs e)
        {

        }

        private void timerGetData_Tick(object sender, EventArgs e)
        {
            TimerCount+=1;//in sec
            var info = $"Next data fetch in {timerGetData.Tag.ToString().ToInt()/1000- TimerCount } Seconds.";
            ShowStatusBar(info, true);
            if (TimerCount >= timerGetData.Tag.ToString().ToInt() / 1000)
            {
                TimerCount = 0;
                info = $"Data fetching from Device IP: {DeviceInfo.IP}";
                ShowStatusBar(info, true);
                btnPullData_Click(null,null);
            }
            ShowStatusBar( info, true);
        }

        private void btnPullData_Click(object sender, EventArgs e)
        {

            try
            {
                // DBAccess.Sql.ExecuteDataSet("");
               
                ShowStatusBar(string.Empty, true);

                ICollection<MachineInfo> lstMachineInfo = manipulator.GetLogData(objZkeeper, int.Parse(tbxMachineNumber.Text.Trim()));

                if (lstMachineInfo != null && lstMachineInfo.Count > 0)
                {
                    if (manipulator.PreviousCount == lstMachineInfo.Count)// >0 and same as previous, so deleteting is safe
                    {
                        //ckeck off peak hour
                        var tfrom = DeviceInfo.OffPeakHourFrom;
                        var tto = DeviceInfo.OffPeakHourTo;
                        
                        if (tfrom.HasValue)
                        {
                            var now = DateTime.Now;
                            var starttime = new DateTime(now.Year, now.Month, now.Day, tfrom.Value.Hours, tfrom.Value.Minutes, tfrom.Value.Seconds);

                            if (DeviceInfo.IsNextDayEndHour.GetValueOrDefault(false))
                            {
                                now = now.AddDays(1);
                            }

                            var endtime = new DateTime(now.Year, now.Month, now.Day, tto.Value.Hours, tto.Value.Minutes, tto.Value.Seconds);
                            
                            if (( DateTime.Now>= starttime) && (DateTime.Now <= endtime))
                            {
                                objZkeeper.ClearGLog(tbxMachineNumber.Text.ToInt());
                                manipulator.PreviousCount = 0;
                            }
                        }
                        else
                        {
                            objZkeeper.ClearGLog(tbxMachineNumber.Text.ToInt());
                            manipulator.PreviousCount = 0;
                        }
                    }
                    else//insert into database
                    {


                        foreach (MachineInfo m in lstMachineInfo)
                        {
                            Attn_tblZKMaster objAttn_tblZKMaster = new Attn_tblZKMaster();
                            objAttn_tblZKMaster.DeviceID = m.MachineNumber.ToString();
                            objAttn_tblZKMaster.UserID = m.IndRegID.ToString();
                            var zx = DateTime.Parse(m.DateTimeRecord);
                            objAttn_tblZKMaster.LogTime = DateTime.Parse(m.DateTimeRecord);
                            objAttn_tblZKMaster.SerialNumber = m.SerialNumber;
                            //var xx =DateTime.ParseExact( m.DateTimeRecord,"MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);//6/6/2018 9:47:31 PM
                            //check already inserted
                            var q = $"select count(*) cnt from Attn_tblZKMaster where DeviceID='{objAttn_tblZKMaster.DeviceID}' and UserID='{objAttn_tblZKMaster.UserID}' and LogTime='{objAttn_tblZKMaster.LogTime}'";
                            var ds = DBAccess.Sql.ExecuteDataSet(q, "tt");
                            if (ds.Tables[0].Rows[0][0].ToString().ToInt() == 0)
                                DBAccess.Sql.InsertObject(objAttn_tblZKMaster);
                        }
                        manipulator.PreviousCount = lstMachineInfo.Count;
                    }

                    /////////////////////////
                    //BindToGridView(lstMachineInfo);
                    ShowStatusBar(lstMachineInfo.Count + " records found !!", true);
                    //dd.SaveInformation(lstMachineInfo);
                }
                else
                    DisplayListOutput("No records found");
            }
            catch (Exception ex)
            {
                DisplayListOutput(ex.Message);
            }
            timerGetData.Enabled = true;
        }
        private void DisplayListOutput(string message)
        {
            //if (dgvRecords.Controls.Count > 2)
            //{ dgvRecords.Controls.RemoveAt(2); }

            ShowStatusBar(message, false);
        }
        #region SendToMainDB
        private void btnSendData_Click(object sender, EventArgs e)
        {
            btnSendData.Enabled = false;
            timerSendData.Enabled = true;

            
        }

        private void timerSendData_Tick(object sender, EventArgs e)
        {
            timerSendData.Enabled = false;
            backgroundWorkerSendToMainDB.RunWorkerAsync();


        }

        private void backgroundWorkerSendToMainDB_DoWork(object sender, DoWorkEventArgs e)
        {

            var dt = "2018-06-29";// DateTime.Now.ToString("yyyy-MM-dd");
            var q1 = $"SELECT [Id],[DeviceID],[SerialNumber],[UserID],[LogTime],[IsSent] FROM [Attn_tblZKMaster] where IsSent=0 and  cast(LogTime as date)='{dt}' order by id";
            var Attn_tblZKMasterList= DBAccess.Sql.GetObjectCollection<Attn_tblZKMaster>(q1,true);
            foreach (Attn_tblZKMaster Attn_tblZKMaster in Attn_tblZKMasterList)
            {
                var zm = Attn_tblZKMaster;
                q1 = $"SELECT [ID],[UserID],[AttnDate],[InTime],[OutTime],[DeviceID],[Device_UserID],[IsFromDevice] FROM [Attn_tblAttendance]  where DeviceID='{Attn_tblZKMaster.DeviceID}' and Device_UserID={Attn_tblZKMaster.UserID} and AttnDate='{dt}'";
                var ds = DBAccess.SqlMainDB.ExecuteDataSet(q1, "tab1");
                if (ds.Tables[0].Rows.Count > 0)//already inserted for today
                {//row exist, update out time only
                    //DBAccess.SqlMainDB use this for main db. main db=school's db
                    q1 = $"UPDATE [Attn_tblAttendance]  SET [OutTime] = '{Attn_tblZKMaster.LogTime}' WHERE ID = {ds.Tables[0].Rows[0]["ID"].ToString()}";
                    DBAccess.SqlMainDB.Execute(q1);

                }
                else//no data,so, insert
                {

                    
                    q1 = $"INSERT INTO [Attn_tblAttendance]([UserID],[IsPresent],[IsAbsent],[AttnDate],     [InTime],     [OutTime],[DeviceID] ,[Device_UserID],[IsFromDevice] ,[IsSMSSent] ,[AddBy] ,[AddTime])"+
                                                    $"VALUES('{zm.UserID}',1,0,'{zm.LogTime.Date}','{zm.LogTime}',NULL,'{zm.DeviceID}','{zm.UserID}'                ,1,               0,   'device', '{DateTime.Now.ToString()}')";
                    DBAccess.SqlMainDB.Execute(q1);

                }
            }
        }

        private void backgroundWorkerSendToMainDB_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorkerSendToMainDB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timerSendData.Enabled = true;
        }
        #endregion
    }
}
