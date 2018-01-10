﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvizhSeller_Console.services.actions
{

    public class PurchaseAction
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }
        public PurchaseParams _params { get; set; }

        services.Fiscal fiscal;

        public void purchase()
        {
            repositories.Cart cart = new repositories.Cart();
            foreach (var product in this._params.elements)
            {
                cart.Put(product,product.GetCartCount());
            }
            fiscal = new services.Fiscal(new drivers.FiscalAbstractFabric(), cart);
            fiscal.Register(this._params.payment_type, Properties.Settings.Default.comment);
        }
    }

    public class PurchaseParams
    {
        public byte payment_type { get; set; }
        public int discount { get; set; }
        public string[] discounts { get; set; }

        public entities.Product[] elements { get; set; }

        public int GetPayment()
        {
            return payment_type;
        }

        public int GetDiscount()
        {
            return discount;
        }
    }

}   


