using System;
using System.Windows.Forms;
using FinsUdp;
using PlcControl;

namespace PALMS.Settings.ViewModel.Common
{

    //public class FinsTcp
    //{
    //    private PlcClient PLC = new PlcClient();
    //    public bool connected;
    //    public int maxpoint;
    //    public int Hanginginterval;
    //    private string localip;
    //    private string remoteip;
    //    private int port;
    //    private int PlcHandle;

    //    public FinsTcp(string localip, string remoteip, int port)
    //    {
    //        this.localip = localip;
    //        this.remoteip = remoteip;
    //        this.port = port;
    //        this.connected = false;
    //    }

    //    public int conn(string localip, string remoteip, int port)
    //    {
    //        short num = 0;
    //        if (!this.connected)
    //        {
    //            num = this.PLC.EntLink(localip.Trim(), Convert.ToUInt16(0), remoteip.Trim(), Convert.ToUInt16(port), "LFLQTYTIOWWPEE18040512538FINS/UDP/V34", ref this.PlcHandle, Convert.ToUInt16(1000));
    //            if (num == (short)0)
    //                this.connected = true;
    //        }
    //        return (int)num;
    //    }

    //    public int disconn()
    //    {
    //        short num = 0;
    //        if (this.connected)
    //        {
    //            num = this.PLC.DeLink(this.PlcHandle);
    //            this.connected = false;
    //        }
    //        return (int)num;
    //    }

    //    public void Start()
    //    {
    //        if (this.GetWorkState())
    //            return;
    //        this.writeBit("WR.0.02", true);
    //        this.writeBit("WR.0.02", false);
    //    }

    //    public bool GetWorkState()
    //    {
    //        bool result = false;
    //        this.readBit("WR.0.06", ref result);
    //        return result;
    //    }

    //    public int Stop()
    //    {
    //        int num = this.writeBit("WR.0.03", true);
    //        if (num != 0)
    //            return num;
    //        return this.writeBit("WR.0.03", false);
    //    }

    //    public void Reset()
    //    {
    //        bool model = this.GetModel();
    //        if (this.GetBasePoint() == 0)
    //        {
    //            this.SetModel(0);
    //            this.SetNowPoint(10);
    //            this.GotoPoint();
    //            while (this.GetNowPoint() != 10)
    //                this.Delay(500);
    //        }
    //        this.writeBit("WR.0.0", true);
    //        this.writeBit("WR.0.0", false);
    //        while (!this.GetResetState())
    //            this.Delay(1000);
    //        this.SetModel((int)Convert.ToInt16(model));
    //    }

    //    public int Clear()
    //    {
    //        int num = this.writeBit("WR.0.01", true);
    //        if (num != 0)
    //            return num;
    //        return this.writeBit("WR.0.01", false);
    //    }

    //    public int GetBasePoint()
    //    {
    //        object[] result = new object[1];
    //        this.readWord("DM.550.1", ref result);
    //        return Convert.ToInt32(result[0]);
    //    }

    //    private bool GetResetState()
    //    {
    //        bool result = false;
    //        this.readBit("WR.0.09", ref result);
    //        return result;
    //    }

    //    public bool GetClotheReady()
    //    {
    //        bool result = false;
    //        this.readBit("WR.10.01", ref result);
    //        return result;
    //    }

    //    public int Sorting(int LineNum)
    //    {
    //        if (LineNum == 1)
    //            return this.writeBit("WR.0.00", true);
    //        if (LineNum == 2)
    //            return this.writeBit("WR.0.01", true);
    //        return -23;
    //    }

    //    public int GetWaitHangNum()
    //    {
    //        short num = 0;
    //        object[] result = new object[1];
    //        this.readWord("DM.10.1", ref result);
    //        if (num != (short)0)
    //            return (int)num;
    //        if (Convert.ToInt16(result[0]) == (short)0)
    //            result[0] = (object)this.maxpoint;
    //        return Convert.ToInt32(result[0]);
    //    }

    //    public int ResetWaitHangNum()
    //    {
    //        this.writeWord("DM.10.1", new object[1] { (object)0 });
    //        this.writeBit("WR.10.01", false);
    //        return this.writeBit("WR.12.04", true);
    //    }

    //    public int Packclothes()
    //    {
    //        return this.writeBit("WR.0.02", true);
    //    }

    //    public bool GetClotheInHook()
    //    {
    //        bool result = false;
    //        this.readBit("WR.20.01", ref result);
    //        return result;
    //    }

    //    public bool GetModel()
    //    {
    //        bool result = false;
    //        this.readBit("HR.1.00", ref result);
    //        return result;
    //    }

    //    public int SetModel(int num)
    //    {
    //        if (num == 0)
    //            return this.writeBit("HR.1.00", false);
    //        return this.writeBit("HR.1.00", true);
    //    }

    //    public int HangUpToPoint(int num)
    //    {
    //        if (this.GetModel())
    //            return -30;
    //        if (this.SetNowPoint(num) != 0)
    //            return -1;
    //        if (this.GetNowPoint() != num)
    //        {
    //            this.GotoPoint();
    //            this.Delay(500);
    //            while (this.DialState())
    //                this.Delay(500);
    //        }
    //        this.Hang_In();
    //        this.Delay(100);
    //        while (!this.Hang_In_State())
    //            this.Delay(500);
    //        return 0;
    //    }

    //    public int SetNowPoint(int num)
    //    {
    //        if (this.GetModel())
    //            return -30;
    //        if (num >= this.Hanginginterval)
    //            num -= this.Hanginginterval;
    //        else
    //            num = this.maxpoint - this.Hanginginterval + num;
    //        return this.writeWord("DM.552.1", new object[1]
    //        {
    //    (object) num
    //        });
    //    }

    //    public int GetNowPoint()
    //    {
    //        short num = 0;
    //        object[] result = new object[1];
    //        this.readWord("DM.0.1", ref result);
    //        if (num != (short)0)
    //            return (int)num;
    //        if (Convert.ToInt16(result[0]) == (short)0)
    //            result[0] = (object)this.maxpoint;
    //        return Convert.ToInt32(result[0]);
    //    }

    //    public int GotoPoint()
    //    {
    //        if (this.GetModel())
    //            return -30;
    //        int num = 0;
    //        num = this.writeBit("WR.24.05", true);
    //        num = this.writeBit("WR.24.06", true);
    //        return this.writeBit("WR.10.13", true);
    //    }

    //    public int Hang_In()
    //    {
    //        if (this.GetModel())
    //            return -30;
    //        this.writeBit("WR.10.12", true);
    //        return this.writeBit("WR.10.12", false);
    //    }

    //    public bool Hang_In_State()
    //    {
    //        bool result = false;
    //        this.readBit("WR.10.11", ref result);
    //        return result;
    //    }

    //    /// <summary>
    //    /// Turntable status, 1 when going, stop 0
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool DialState()
    //    {
    //        bool result = false;
    //        this.readBit("WR.24.02", ref result);
    //        return result;
    //    }

    //    public int TakeOutClothes(string FrameList)
    //    {
    //        object[] sendWord = new object[10];
    //        string[] strArray = FrameList.Split(',');
    //        for (int index = 0; index < 10; ++index)
    //            sendWord[index] = index >= strArray.Length ? (object)0 : (object)((int)Convert.ToInt16(strArray[index]) - 1);
    //        this.writeWord("DM.20.10", sendWord);
    //        this.writeWord("DM.40.2", new object[2]
    //        {
    //    (object) strArray.Length,
    //    (object) 1
    //        });
    //        return 0;
    //    }

    //    public bool GetTakeOutClothesState()
    //    {
    //        bool flag = false;
    //        object[] result = new object[1];
    //        this.readWord("DM.4.1", ref result);
    //        if (Convert.ToInt32(result[0]) == 1)
    //            flag = true;
    //        return flag;
    //    }

    //    public int GetMaxPoint()
    //    {
    //        object[] result = new object[1];
    //        this.readWord("DM.510.1", ref result);
    //        this.maxpoint = Convert.ToInt32(result[0]);
    //        return this.maxpoint;
    //    }

    //    public int SetMaxPoint(int num)
    //    {
    //        int num1 = this.writeWord("DM.510.1", new object[1]
    //        {
    //    (object) num
    //        });
    //        if (num1 != 0)
    //            return num1;
    //        this.maxpoint = num;
    //        return 0;
    //    }

    //    public int GetHanginpoint()
    //    {
    //        object[] result = new object[1];
    //        this.readWord("DM.512.1", ref result);
    //        return this.Hanginginterval = Convert.ToInt32(result[0]);
    //    }

    //    public int SetHanginpoint(int num)
    //    {
    //        int num1 = this.writeWord("DM.512.1", new object[1]
    //        {
    //    (object) num
    //        });
    //        if (num1 != 0)
    //            return num1;
    //        this.Hanginginterval = num;
    //        return 0;
    //    }

    //    public int SetTakeOutClothesPoint(int num)
    //    {
    //        return this.writeWord("DM.514.1", new object[1]
    //        {
    //    (object) num
    //        });
    //    }

    //    public int GetTakeOutClothesPoint()
    //    {
    //        object[] result = new object[1];
    //        this.readWord("DM.514.1", ref result);
    //        return Convert.ToInt32(result[0]);
    //    }

    //    private int readBit(string memory, ref bool result)
    //    {
    //        object[] objArray = this.Getmemory(memory);
    //        return (int)this.PLC.Bit_Test(this.PlcHandle, (PlcClient.PlcMemory)objArray[0], Convert.ToUInt16(objArray[1]), Convert.ToUInt16(objArray[2]), ref result);
    //    }

    //    private int writeBit(string memory, bool sendBit)
    //    {
    //        object[] objArray = this.Getmemory(memory);
    //        if (sendBit)
    //            return (int)this.PLC.Bit_Set(this.PlcHandle, (PlcClient.PlcMemory)objArray[0], Convert.ToUInt16(objArray[1]), Convert.ToUInt16(objArray[2]));
    //        return (int)this.PLC.Bit_Reset(this.PlcHandle, (PlcClient.PlcMemory)objArray[0], Convert.ToUInt16(objArray[1]), Convert.ToUInt16(objArray[2]));
    //    }

    //    private int readWord(string memory, ref object[] result)
    //    {
    //        object[] objArray = this.Getmemory(memory);
    //        return (int)this.PLC.CmdRead(this.PlcHandle, (PlcClient.PlcMemory)objArray[0], (PlcClient.DataType)1, Convert.ToUInt16(objArray[1]), Convert.ToUInt16(objArray[2]), ref result);
    //    }

    //    private int readWord(string memory, int DataType, ref object[] result)
    //    {
    //        object[] objArray = this.Getmemory(memory);
    //        return (int)this.PLC.CmdRead(this.PlcHandle, (PlcClient.PlcMemory)objArray[0], (PlcClient.DataType)DataType, Convert.ToUInt16(objArray[1]), Convert.ToUInt16(objArray[2]), ref result);
    //    }

    //    private int writeWord(string memory, object[] sendWord)
    //    {
    //        object[] objArray = this.Getmemory(memory);
    //        return (int)this.PLC.CmdWrite(this.PlcHandle, (PlcClient.PlcMemory)objArray[0], (PlcClient.DataType)1, Convert.ToUInt16(objArray[1]), Convert.ToUInt16(objArray[2]), ref sendWord);
    //    }

    //    private object[] Getmemory(string memory)
    //    {
    //        object[] objArray = new object[3];
    //        string[] strArray = memory.Split('.');
    //        switch (strArray[0])
    //        {
    //            case "CIO":
    //                objArray[0] = (object)(PlcClient.PlcMemory)1;
    //                break;
    //            case "WR":
    //                objArray[0] = (object)(PlcClient.PlcMemory)2;
    //                break;
    //            case "DM":
    //                objArray[0] = (object)(PlcClient.PlcMemory)3;
    //                break;
    //            case "HR":
    //                objArray[0] = (object)(PlcClient.PlcMemory)7;
    //                break;
    //            case "TIM":
    //                objArray[0] = (object)(PlcClient.PlcMemory)5;
    //                break;
    //            case "CNT":
    //                objArray[0] = (object)(PlcClient.PlcMemory)6;
    //                break;
    //            default:
    //                objArray[0] = (object)(PlcClient.PlcMemory)4;
    //                break;
    //        }
    //        objArray[1] = (object)strArray[1];
    //        objArray[2] = (object)strArray[2];
    //        return objArray;
    //    }

    //    public void Delay(int milliSecond)
    //    {
    //        int tickCount = Environment.TickCount;
    //        while (Math.Abs(Environment.TickCount - tickCount) < milliSecond)
    //            Application.DoEvents();
    //    }

    //    private string byteToHexStr(byte[] bytes)
    //    {
    //        string str = "";
    //        if (bytes != null)
    //        {
    //            for (int index = 0; index < bytes.Length; ++index)
    //                str += bytes[index].ToString("X2");
    //        }
    //        return str;
    //    }
    //}
}
