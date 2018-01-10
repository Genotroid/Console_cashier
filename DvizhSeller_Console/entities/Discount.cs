﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DvizhSeller_Console.entities
{
    public class Discount
    {
        int id;
        int discount;
        string code;
        byte type;

        public Discount(int setId, string setCode, int setDiscount)
        {
            id = setId;
            code = setCode;
            discount = setDiscount;
        }

        public string Code
        {
            get { return code; }
            set
            {
                code = value;
            }
        }

        public int DiscountVal
        {
            get { return discount; }
            set { discount = value; }
        }

        public string GetName()
        {
            return code;
        }

        public string GetCode()
        {
            return code;
        }

        public int GetDiscount()
        {
            return discount;
        }

        public int GetId()
        {
            return id;
        }

        public void SetCode(string setCode)
        {
            code = setCode;
        }

        public void SetType(byte setType)
        {
            type = setType;
        }

        public void SetDiscount(int setDiscount)
        {
            discount = setDiscount;
        }

        public void SetId(int setId)
        {
            id = setId;
        }

        public byte GetType()
        {
            return type;
        }
    }
}
