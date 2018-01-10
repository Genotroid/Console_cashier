﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DvizhSeller_Console.drivers
{
    public class Atol : FiscalInterface
    {
        private string cashierName = "Unknow";
        private int docNumber = 1;
        private byte numDepart = 1;
        private List<int> statuses;
        private byte taxNumber = 1;
        bool driverExists = true;

        FprnM1C.IFprnM45 cmd;

        dynamic driver = false;

        public const int DOC_TYPE_SERVICE = 0; //For print texts
        public const int DOC_TYPE_REGISTER = 1; //For fiscal registration
        public const int DOC_TYPE_ANNULATE = 2;

        public Atol()
        {
            statuses = new List<int>();

            try
            {
                driver = Type.GetTypeFromProgID("AddIn.FPrnM45");
                if (driver != null)
                    cmd = Activator.CreateInstance(driver);
                else
                {
                    driverExists = false;
                    return;
                }

                driverExists = true;

                if (cmd.CheckState != 0)
                    cmd.CancelCheck();
                
                cmd.AttrPrint = 1;

                cmd.DeviceEnabled = true;
                cmd.Password = "30";

                if (cmd.GetStatus() < 0)
                    Console.WriteLine("Ошибка ККТ: " + cmd.GetStatus().ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Не удалось загрузить драйвер Атол. " + e.ToString());
                driverExists = false;
            }
        }

        public bool Ready()
        {
            return driverExists;
        }

        public void OpenDocument(byte type)
        {
            if (!driverExists)
                return;
            if (type > 2)
                type = DOC_TYPE_ANNULATE;
            else
                type = DOC_TYPE_REGISTER;

            cmd.OperatorName = cashierName;
            
            cmd.Mode = 1;
            cmd.SetMode();

            cmd.TestMode = Properties.Settings.Default.testMode;
            cmd.CheckType = type;

            cmd.OpenCheck();

            if (cmd.Fiscal)
                cmd.BeginFiscDocument();
            else
                cmd.BeginDocument();
        }

        public void CloseDocument()
        {
            if (!driverExists)
                return;

            cmd.CloseCheck();

            if (cmd.Fiscal)
                cmd.EndFiscDocument();
            else
                cmd.EndDocument();
        }

        public List<int> GetStatuses()
        {
            return statuses;
        }

        public void SetTaxNumber(byte number)
        {
            taxNumber = number;
        }

        public void SetCashierName(string name)
        {
            cashierName = name;
            cmd.OperatorName = name;
            
        }

        public void SetNumDepart(byte number)
        {
            numDepart = number;
        }

        public void SetDocNumber(int number)
        {
            docNumber = number;
        }

        public void ScrollPaper()
        {
            cmd.Caption = "";
            cmd.PrintString();
        }

        public void PrintString(string text)
        {
            if (!driverExists)
                return;
            
            cmd.Mode = 1;
            cmd.SetMode();
            cmd.Alignment = 1;
            cmd.Caption = text;
            cmd.PrintString();
        }

        public void RegisterProduct(string name, string barcode, double quantity, double price, int numPos = 1)
        {
            if (!driverExists)
                return;

            if (cmd.GetStatus() < 0)
            {
                Console.WriteLine("Ошибка ККМ: " + cmd.GetStatus().ToString());
                return;
            }
            cmd.Alignment = 0;
            //cmd.TaxTypeNumber = Properties.Settings.Default.taxType;
            cmd.SummTax();

            cmd.Name = name;
            cmd.Price = price;
            cmd.Quantity = quantity;
            cmd.Department = numPos;

            cmd.Registration();
        }

        public void AnnulateProduct(string name, double quantity, double price)
        {
            if (!driverExists)
                return;
            if (cmd.GetStatus() < 0) {
                Console.WriteLine("Ошибка ККМ: " + cmd.GetStatus().ToString());
                return;
            }
                
            cmd.Alignment = 1;
            cmd.Caption = "Отмена операции";
            cmd.PrintString();

            cmd.Name = name;
            cmd.Price = price;
            cmd.Quantity = quantity;

            cmd.Caption = name + " - отменен";
            cmd.PrintString();

            //cmd.BuyReturn();
            
            cmd.Annulate();
        }

        public void Storning(string name, double quantity, double price)
        {
            if (!driverExists)
                return;

            if (cmd.GetStatus() < 0)
            {
                Console.WriteLine("Ошибка ККМ: " + cmd.GetStatus().ToString());
                return;
            }

            cmd.Alignment = 1;
            cmd.Caption = "Сторнирование";
            cmd.PrintString();

            cmd.Name = name;
            cmd.Price = price;
            cmd.Quantity = quantity;

            cmd.Caption = name + " - " + price + "х" + quantity + " = " +quantity*price;
            cmd.PrintString();

            cmd.BuyReturn();

            cmd.Storno();
        }

        public void RegisterPayment(double sum, byte type = 0)
        {
            if (!driverExists)
                return;

            cmd.Mode = 1;
            cmd.SetMode();
            cmd.TypeClose = type;
            cmd.Summ = sum;
            cmd.Payment();
        }
        
        public void PrintTotal()
        {
            if (!driverExists)
                return;
            
            cmd.CashIncome();
        }

        public void RegisterDiscount(byte type, string nameDiscount, int sum)
        {
            cmd.DiscountType = type;
            cmd.DiscountValue = sum;
        }

        public void PrintServiceData()
        {
            if (!driverExists)
                return;
            
            cmd.Alignment = 1;
            cmd.Caption = "Тестирование печати.";
            cmd.PrintString();
            
            cmd.Caption = "Все ОК.";
            cmd.PrintString();
            
            cmd.Caption = "Номер чека: " + cmd.CheckNumber.ToString();
            cmd.PrintString();
            
            cmd.Caption = cmd.DeviceSettings;
            cmd.PrintString();
            
            if (cmd.Fiscal)
                cmd.Caption = "Фискальный";
            else
                cmd.Caption = "Нефискальный";
            cmd.PrintString();
            
            cmd.Caption = "ИНН" + cmd.INN;
            cmd.PrintString();
        }

        public void OpenSession()
        {
            if (!driverExists)
                return;

            if (cmd.GetStatus() < 0)
                return;

            cmd.Mode = 1;
            cmd.SetMode();
            cmd.OpenSession();
            cmd.Beep();
        }

        public void CloseSession()
        {
            if (!driverExists)
                return;

            if (cmd.CheckState != 0)
                cmd.CancelCheck();

            if (cmd.GetStatus() < 0)
                return;

            cmd.Mode = 3;
            cmd.SetMode();
            cmd.ReportType = 1;
            cmd.Report();
            cmd.Beep();
        }

        public bool IsSessionOpen()
        {
            if(cmd.SessionExceedLimit)
            {
                Console.WriteLine("Смена превысила 24 часа, нужно перезапустить ее.");
            }

            if (cmd.GetStatus() < 0)
                return false;

            if (!driverExists)
                return false;
             
            cmd.Mode = 1;
            cmd.SetMode();

            bool opened = cmd.SessionOpened;
            
            return opened;
        }

        ~Atol()
        {
            cmd.DeviceEnabled = false;
        }

        public void cashierSign()
        {
            cmd.Caption = "";
            cmd.PrintString();
            cmd.Caption = "_________________________________";
            cmd.PrintString();
            cmd.Caption = "        (подпись кассира)";
            cmd.PrintString();
            cmd.Caption = "";
            cmd.PrintString();
        }

        public void buyerSign()
        {
            cmd.Caption = "";
            cmd.PrintString();
            cmd.Caption = "==================================";
            cmd.PrintString();
            cmd.Caption = "        (подпись клиента)";
            cmd.PrintString();
        }

        public int getStatus()//Возвращает состояние устройства, надо новое имя
        {
            return cmd.GetStatus();
        }

        public string AdvSetting(int com_port, int com_speed, bool use_remote, string ip, int model)//Возвращает состояние устройства, надо новое имя
        {
            int _portNumber = cmd.PortNumber;
            string _hostAddress = cmd.HostAddress;
            int _com_speed = cmd.BaudRate;
            int _model = cmd.Model;
            int rate = 1;
            if (com_speed > 9000) rate++;
            if (com_speed > 14000) rate++;

            string resp = "{\n";
            cmd.DeviceEnabled = false;
            if(use_remote)
            {
                cmd.PortNumber = 99;
                cmd.HostAddress = ip;
                resp += "\"use_remote\": \"OK\",\n \"ip\":\""+ip+"\",\n";

            }
            else cmd.PortNumber = com_port + 1000;
            while (com_speed > 300)
            {
                com_speed /= 2;
                rate++;
            }
            cmd.BaudRate = rate; // 3 - 1200 4 - 2400 .. 18 - 115200
            cmd.Model = model;
            resp += "\"com_port\": \"OK\",\n \"com_speed\": \"OK\",\n \"model\": \"OK\",\n"+rate+" - BaudRate \n";
            try { cmd.DeviceEnabled = true; }
            catch(Exception e)
            {
                return "Неккоректные параметры настройки ККМ. " + e.ToString();
            }
            /*if(!cmd.DeviceEnabled)
            {
                resp = "При настройке аппарата произошла ошибка, проверьте настройки";
                cmd.PortNumber = _portNumber;
                cmd.HostAddress = _hostAddress;
                cmd.BaudRate = _com_speed;
                cmd.Model = _model;
                cmd.DeviceEnabled = true;
            }*/

            return resp;
        }

        public string Get_info()
        {
            string resp = "{\n \"info\":{\n";
            resp += "\"Model\": " + cmd.Model.ToString() + ",\n";
            resp += "\"Version\": \"" + cmd.Version.ToString() + "\",\n";
            resp += "\"fiscal\": " + cmd.Fiscal.ToString() + ",\n";
            resp += "\"kkt\":{\n";
            resp += "\"status\": " + cmd.GetStatus().ToString() + ",\n";
            resp += "\"com_port\": " + (cmd.PortNumber - 1000).ToString() + ",\n";
            int _baudRate = 300;
            for(int i=0; i<cmd.BaudRate-2;i++)
            {
                _baudRate *= 2;
            }
            resp += "\"com_speed\": " + _baudRate.ToString() + "\n";
            resp += "},\n \"server\": {\n";
            resp += "\"server\": \"" + cmd.HostAddress + "\"\n}\n}";

            return resp;
        }

        public void setTax_id(int tax_id)
        {
            switch(tax_id)
            {
                case 0:
                        tax_id = 1;
                        break;
                case 10:
                        tax_id = 2;
                        break;
                case 18:
                        tax_id = 3;
                        break;
                default:
                    tax_id = 0;
                    break;
            }
            cmd.TaxTypeNumber = tax_id;
        }
    }
}
