using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Web.Http;


namespace DvizhSeller_Console
{
    class Program
    {

        public static services.Fiscal fiscal;
        public static repositories.Cart cart;
        public static WebServer server;

        public const string BASE_URI = "http://localhost:8911/dvizh/cashier/api/1.0/";

        static void Main()
        {
            Console.WriteLine("Для запуска сервера введите команду start.");
            Console.WriteLine("Для остановки сервера введите команду stop.");
            Console.WriteLine("Для выхода из программы введите команду exit.");

            cart = new repositories.Cart();
            fiscal = new services.Fiscal(new drivers.FiscalAbstractFabric(), cart);
            server = new WebServer(SendResponse, BASE_URI);
            fiscal.IsSessionOpen();
            string command;
            while (true)
            {
                command = Console.ReadLine();
                switch (command)
                {
                    case "start":
                        server.Run();
                        break;
                    case "stop":
                        server.Stop();
                        break;
                    case "exit":
                        return;
                        break;
                    default:
                        Console.WriteLine("Введена неверная команда");
                        break;
                }
            }
        }

        static string SendResponse(HttpListenerRequest request)
        {
            if (request.RawUrl == "/dvizh/cashier/api/1.0/")
            {
                System.IO.Stream body = request.InputStream;
                System.IO.StreamReader reader = new System.IO.StreamReader(body, request.ContentEncoding);
                string json = reader.ReadToEnd();
                services.actions.RequestAction requestAction = JsonConvert.DeserializeObject<services.actions.RequestAction>(json);
                switch (requestAction.method)
                {
                    case "purchase":
                        if (fiscal.IsSessionOpen())
                        {
                            services.actions.PurchaseAction product = JsonConvert.DeserializeObject<services.actions.PurchaseAction>(json);
                            for (int i = 0; i < Properties.Settings.Default.indentSize / 5; i++)
                            fiscal.ScrollPaper();
                            product.purchase();
                        }
                        else return "{\n \"status\": " + fiscal.getStatus().ToString() + ",\n \"message\": \"Смена не открыта\"\n}";
                        break;
                    case "annulate":
                        services.actions.AnnulateAction annulate = JsonConvert.DeserializeObject<services.actions.AnnulateAction>(json);
                        foreach (var ann in annulate._params.elements)
                        {
                            ann.SetCancelAt(DateTime.Now.ToShortDateString());
                            fiscal.Annulate(ann);
                        }
                        break;
                    case "storning":
                        services.actions.AnnulateAction storno = JsonConvert.DeserializeObject<services.actions.AnnulateAction>(json);
                        foreach (var st in storno._params.elements)
                        {
                            st.SetCancelAt(DateTime.Now.ToShortDateString());
                            fiscal.Storning(st);
                        }
                        break;
                    case "start":
                        fiscal.OpenSession();
                        return "{\n \"status\": " + fiscal.getStatus().ToString() + ",\n \"message\": \"Смена открыта\"\n}";
                        break;
                    case "stop":
                        fiscal.CloseSession();
                        return "{\n \"status\": " + fiscal.getStatus().ToString() + ",\n \"message\": \"Смена закрыта\"\n}";
                        break;
                    case "setCashier":
                        services.actions.SetCashierAction setCashier = JsonConvert.DeserializeObject<services.actions.SetCashierAction>(json);
                        fiscal.SetCashier(setCashier._params.name);
                        break;
                    case "settings":
                        FprnM1C.IFprnM45 cmd = new FprnM1C.FprnM45();
                        cmd.ShowProperties();
                        break;
                    case "test":
                        fiscal.TestPrint();
                        break;
                    case "getStatus":
                        if (fiscal.getStatus() < 0)
                        {
                            return "{\n \"status\": " + fiscal.getStatus().ToString() + ",\n \"message\": \"Произошел сбой в работе ККМ\"\n}";
                        }
                        else return "{\n \"status\": " + fiscal.getStatus().ToString() + ",\n \"message\": \"Оборудование готово к работе\"\n}"; ;
                        break;
                    case "advSettings":
                        string resp = "";
                        services.actions.Setting setting = JsonConvert.DeserializeObject<services.actions.Setting>(json);
                        resp = fiscal.AdvSetting(setting._params.com_port, setting._params.com_speed, setting._params.use_remote, setting._params.ip + ":" + setting._params.port, setting._params.model);
                        if (resp[0] != 'П')
                        {
                            Properties.Settings.Default.cashierSign = setting._params.purchase_Settings.cashierSign;
                            Properties.Settings.Default.buyerSign = setting._params.purchase_Settings.buyerSign;
                            Properties.Settings.Default.comment = setting._params.purchase_Settings.comment;
                            Properties.Settings.Default.indentSize = setting._params.purchase_Settings.indent;
                            resp += "\"purchase settings\": {\n \"cashierSign\": \"OK\",\n \"buyerSing\": \"OK\",\n \"comment\" = \"" + Properties.Settings.Default.comment + "\",\n \"indent\": \"" + Properties.Settings.Default.indentSize + "\",\n}\n}";
                        }
                        return resp;
                        break;
                    case "getInfo":
                        return fiscal.Get_info();
                        break;
                    default:
                        break;
                }
            }
            return "{\n \"result\": \"Операция успешно выполнена\"\n}";
        }
    }
}
