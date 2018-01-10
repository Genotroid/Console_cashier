using System;
using System.Collections.Generic;

namespace DvizhSeller_Console.services
{
    public class Fiscal
    {
        repositories.Cart cart;
        drivers.FiscalInterface driver;
        byte tax_id;

        public const int DOC_TYPE_SERVICE = 1; //For print texts
        public const int DOC_TYPE_REGISTER = 2; //For fiscal registration
        public const int DOC_TYPE_RETURN = 3;
        public const int DOC_TYPE_INCOME = 4;
        public const int DOC_TYPE_OUTCOME = 5;
        public const int DOC_TYPE_BUY = 6;
        public const int DOC_TYPE_ANNULATE = 7;

        public Fiscal(drivers.FiscalAbstractFabricInterface fiscalFabric, repositories.Cart setCart = null)
        {
            driver = fiscalFabric.Build();
            cart = setCart;
            //tax_id = 0;
        }

        public bool Ready()
        {
            return driver.Ready();
        }

        public void PrintString(string str)
        {
            driver.OpenDocument(DOC_TYPE_SERVICE);
            driver.PrintString(str);
            driver.CloseDocument();
        }

        public void TestPrint()
        {
            driver.PrintServiceData();
        }

        public void March()
        {
            //))))
        }

        public void Storning(entities.OrderElement orderElement)
        {
            driver.OpenDocument(DOC_TYPE_OUTCOME);
            driver.Storning(orderElement.GetProductName(), orderElement.GetCount(), orderElement.GetPrice());
            driver.PrintTotal();
            driver.CloseDocument();
        }

        public void Annulate(entities.OrderElement orderElement)
        {
            driver.OpenDocument(DOC_TYPE_ANNULATE);
            driver.AnnulateProduct(orderElement.GetProductName(), orderElement.GetCount(), orderElement.GetPrice());
            driver.PrintTotal();
            driver.RegisterPayment(orderElement.GetPrice(), 0);
            driver.CloseDocument();
        }

        public void SetCashier(string cashierName)
        {
            driver.SetCashierName(cashierName);
        }
    
        public void Register(byte paymentType = 0, string comment ="")
        {
            if (cart.GetCount() <= 0)
                driver.OpenDocument(DOC_TYPE_BUY);
            int i = 1;
            double cartSum = cart.GetTotal();
            double sum = 0;
            //if (cart.GetDiscount() >= 1) driver.RegisterDiscount(1, "Sale", cart.GetDiscount());
            foreach (entities.Product element in cart.GetElements())
            {
                driver.setTax_id(element.Tax_id);
                driver.RegisterProduct(element.GetName(), element.GetSku(), element.GetCartCount(), element.GetPrice(), i);
                driver.RegisterDiscount(1, "sale", element.Discount);
                sum += (element.GetCartCount()*element.GetPrice());
                i++;
            }
            double paymentSum;
            if (cartSum < sum)
            {
                int discount = Convert.ToInt32(sum - cartSum);
                if (discount <= 1)
                    discount = 1;

                paymentSum = sum-discount;
            }
            else
            {
                paymentSum = sum;
            }
            //driver.SetTaxNumber(tax_id);
            //driver.SetCashierName(Properties.Settings.Default.cashierName);
            driver.PrintTotal();
            driver.RegisterPayment(cartSum, paymentType);
            if (comment != "") driver.PrintString(comment);
            if (Properties.Settings.Default.cashierSign) driver.cashierSign();
            if (Properties.Settings.Default.buyerSign) driver.buyerSign();
            for (i = 0; i < Properties.Settings.Default.indentSize / 5; i++) driver.ScrollPaper(); 
            driver.CloseDocument();
        }

        public void OpenSession()
        {
            driver.OpenSession();
        }

        public void CloseSession()
        {
            driver.CloseSession();
        }
        
        public bool IsSessionOpen()
        {
            return driver.IsSessionOpen();
        }
        
        public List<int> GetStatus()
        {
            return driver.GetStatuses();
        }

        public void ScrollPaper()
        {
            driver.ScrollPaper();
            driver.ScrollPaper();
        }

        public int getStatus()// надо новое имя
        {
           return driver.getStatus();
        }

        public string AdvSetting(int com_port, int com_speed, bool use_remote, string ip, int model)
        {
            return driver.AdvSetting(com_port, com_speed, use_remote, ip, model); 
        }

        public string Get_info()
        {
            return driver.Get_info();
        }
    }
}
